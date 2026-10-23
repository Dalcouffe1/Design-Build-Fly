"""SU2 8.5.0 2-D incompressible RANS of the NACA 2415 section at the cruise Reynolds number.

Solver: INC_RANS (pressure-based, FDS, implicit Euler), turbulence SA (fully turbulent, Spalart-Allmaras) and,
as a transition-sensitivity set, SST with the Langtry-Menter LM model.  Chord = 1 in the mesh; the Reynolds number
is set through the constant viscosity MU = rho V c / Re with rho 1.225, V 20 m/s (common.CONDITIONS["cruise"]).
Angle of attack: the free-stream velocity vector is rotated (INC_VELOCITY_INIT) and AOA is set so SU2 reports
lift/drag in wind axes.

Study matrix (see MATRIX below):
  mesh study : L1, L2, L3 at alpha 2 and 10 deg (SA)        -> GCI (Roache 1994 / Celik et al. 2008)
  alpha sweep: L2, SA,  alpha -4 .. 20 deg                  -> lift curve, drag polar, CL_max
  transition : L2, SST-LM, alpha 0 .. 16                    -> effect of laminar-turbulent transition
Runs are executed in parallel (N_JOBS concurrent SU2 processes x THREADS OpenMP threads each).
Outputs: out/su2/<case>/history.csv, surface_flow.csv; out/su2_2d.json (forces, convergence, timings).
Usage: py su2_2d.py [--quick]   (--quick: mesh study + reduced sweep, for smoke testing)
"""
from __future__ import annotations
import os, sys, re, time, subprocess, csv, math, shutil
from concurrent.futures import ThreadPoolExecutor
import numpy as np
from common import *

SU2_DIR = OUT / "su2"; SU2_DIR.mkdir(exist_ok=True)
MESH_DIR = HERE / "mesh"
NCPU = os.cpu_count() or 4
THREADS = 4
N_JOBS = max(1, NCPU // THREADS)
RE = CONDITIONS["cruise"]["Re"]
V = CONDITIONS["cruise"]["V"]
MU_C = RHO * V * 1.0 / RE     # chord 1 in the mesh

CFG = """% SU2 8.5.0 incompressible RANS, NACA 2415, chord 1, Re {Re:.0f} (analysis/aero/su2_2d.py)
SOLVER= INC_RANS
KIND_TURB_MODEL= {turb}
{trans}
MATH_PROBLEM= DIRECT
RESTART_SOL= NO
INC_DENSITY_MODEL= CONSTANT
INC_DENSITY_INIT= {rho}
INC_VELOCITY_INIT= ( {vx:.6f}, {vy:.6f}, 0.0 )
INC_NONDIM= INITIAL_VALUES
VISCOSITY_MODEL= CONSTANT_VISCOSITY
MU_CONSTANT= {mu:.8e}
FREESTREAM_TURBULENCEINTENSITY= {ti}
FREESTREAM_TURB2LAMVISCRATIO= {tlr}
AOA= {alpha}
REF_ORIGIN_MOMENT_X= 0.25
REF_ORIGIN_MOMENT_Y= 0.0
REF_ORIGIN_MOMENT_Z= 0.0
REF_LENGTH= 1.0
REF_AREA= 1.0
MARKER_HEATFLUX= ( airfoil, 0.0 )
MARKER_FAR= ( farfield )
MARKER_PLOTTING= ( airfoil )
MARKER_MONITORING= ( airfoil )
NUM_METHOD_GRAD= WEIGHTED_LEAST_SQUARES
CFL_NUMBER= {cfl}
CFL_ADAPT= YES
CFL_ADAPT_PARAM= ( 0.5, 1.2, 1.0, {cflmax} )
ITER= {iters}
LINEAR_SOLVER= FGMRES
LINEAR_SOLVER_PREC= LU_SGS
LINEAR_SOLVER_ERROR= 1E-5
LINEAR_SOLVER_ITER= 10
CONV_NUM_METHOD_FLOW= FDS
MUSCL_FLOW= YES
SLOPE_LIMITER_FLOW= VENKATAKRISHNAN
VENKAT_LIMITER_COEFF= 0.05
LIMITER_ITER= {limfreeze}
TIME_DISCRE_FLOW= EULER_IMPLICIT
CONV_NUM_METHOD_TURB= SCALAR_UPWIND
MUSCL_TURB= NO
TIME_DISCRE_TURB= EULER_IMPLICIT
CONV_FIELD= ( RMS_PRESSURE, LIFT, DRAG )
CONV_RESIDUAL_MINVAL= -9
CONV_STARTITER= 200
CONV_CAUCHY_ELEMS= 200
CONV_CAUCHY_EPS= 2E-6
MESH_FILENAME= {mesh}
MESH_FORMAT= SU2
TABULAR_FORMAT= CSV
CONV_FILENAME= history
SURFACE_FILENAME= surface_flow
OUTPUT_FILES= ( SURFACE_CSV )
OUTPUT_WRT_FREQ= 100000
HISTORY_OUTPUT= ( ITER, RMS_RES, AERO_COEFF, CAUCHY, LINSOL )
SCREEN_OUTPUT= ( INNER_ITER, RMS_PRESSURE, RMS_VELOCITY-X, LIFT, DRAG, CAUCHY_DRAG )
SCREEN_WRT_FREQ_INNER= 100
WRT_RESTART_OVERWRITE= YES
"""

def case_name(level, alpha, model):
    return f"L{level}_{model}_a{alpha:+05.1f}".replace("+", "p").replace("-", "m")

def write_case(level, alpha, model, iters=4000, cfl=2.0, cflmax=50.0, limfreeze=300):
    name = case_name(level, alpha, model)
    d = SU2_DIR / name; d.mkdir(exist_ok=True)
    a = math.radians(alpha)
    if model == "SA":
        turb, trans, ti, tlr = "SA", "", 0.001, 10.0
    elif model == "SSTLM":
        turb, trans, ti, tlr = "SST", "KIND_TRANS_MODEL= LM", 0.0007, 1.0   # Ncrit 9 ~ Tu 0.07 % (Mack 1977: N = -8.43 - 2.4 ln Tu)
    else:
        raise ValueError(model)
    mesh = MESH_DIR / f"naca2415_L{level}.su2"
    if not mesh.exists():
        raise FileNotFoundError(f"{mesh} missing: run mesh_2d.py {level}")
    shutil.copy(mesh, d / mesh.name)
    (d / "case.cfg").write_text(CFG.format(Re=RE, turb=turb, trans=trans, rho=RHO, vx=V * math.cos(a), vy=V * math.sin(a), mu=MU_C,
                                           ti=ti, tlr=tlr, alpha=alpha, cfl=cfl, cflmax=cflmax, iters=iters, mesh=mesh.name, limfreeze=limfreeze))
    return d

SAFE = dict(cfl=0.5, cflmax=10.0, limfreeze=1500)   # retry settings if a case diverges

def _finite(r):
    return all(math.isfinite(r[k]) and abs(r[k]) < 50 for k in ("CL", "CD", "CM"))

def run_case(d: Path, threads=THREADS, force=False):
    hist = d / "history.csv"
    if hist.exists() and (d / "done.txt").exists() and not force:
        r = parse_case(d)
        if _finite(r) and "rc=0" in (d / "done.txt").read_text():
            return r
        force = True   # previous attempt diverged: fall through to the retry below
    attempts = [None, SAFE]
    for k, settings in enumerate(attempts):
        if settings is not None:   # rewrite the config with the safer numerics
            cfg = (d / "case.cfg").read_text()
            cfg = re.sub(r"CFL_NUMBER= [0-9.]+", f"CFL_NUMBER= {settings['cfl']}", cfg)
            cfg = re.sub(r"CFL_ADAPT_PARAM= \(.*\)", f"CFL_ADAPT_PARAM= ( 0.5, 1.1, 0.1, {settings['cflmax']} )", cfg)
            cfg = re.sub(r"LIMITER_ITER= \d+", f"LIMITER_ITER= {settings['limfreeze']}", cfg)
            cfg = re.sub(r"(?m)^ITER= \d+", "ITER= 8000", cfg)   # only the main count, never LINEAR_SOLVER_ITER / LIMITER_ITER / CONV_STARTITER
            (d / "case.cfg").write_text(cfg)
        mesh = MESH_DIR / (re.search(r"MESH_FILENAME= (\S+)", (d / "case.cfg").read_text()).group(1))
        if not (d / mesh.name).exists():
            shutil.copy(mesh, d / mesh.name)
        t0 = time.time()
        with open(d / ("su2.log" if k == 0 else "su2_retry.log"), "w") as log:
            p = subprocess.run([str(SU2_CFD), "-t", str(threads), "case.cfg"], cwd=str(d), stdout=log, stderr=subprocess.STDOUT, text=True)
        wall = time.time() - t0
        (d / "done.txt").write_text(f"rc={p.returncode} wall_s={wall:.1f} attempt={k+1}\n")
        for f in d.glob("*.su2"):
            f.unlink()          # mesh copy, keep the folder small
        for f in d.glob("restart*"):
            f.unlink()
        r = parse_case(d)
        if p.returncode == 0 and _finite(r):
            break
        print(f"  {d.name}: diverged on attempt {k+1}" + (", retrying with CFL 0.5" if k == 0 else ""))
    print(f"  {d.name}: CL {r['CL']:.4f} CD {r['CD']:.5f} CM {r['CM']:.4f}  iters {r['iters']}  conv {r['converged']}  {wall:.0f} s")
    return r

NAN_RESULT = {"CL": float("nan"), "CD": float("nan"), "CM": float("nan"), "CL_final": float("nan"), "CD_final": float("nan"),
              "CL_std_last300": None, "CD_std_last300": None, "rmsP_final": None, "rmsP_drop": None, "iters": 0, "converged": False, "wall_s": None}

def parse_case(d: Path):
    # a crashed run can leave no history, or only a header: report it as not converged (the caller retries it)
    hist = d / "history.csv"
    if not hist.exists():
        return {**NAN_RESULT, "dir": d.name}
    with open(hist) as f:
        rows = list(csv.reader(f))
    if len(rows) < 3:
        return {**NAN_RESULT, "dir": d.name}
    hdr = [h.strip().strip('"') for h in rows[0]]
    data = np.array([[float(v) for v in r] for r in rows[1:] if len(r) == len(hdr)])
    if data.ndim != 2 or len(data) < 2:
        return {**NAN_RESULT, "dir": d.name}
    col = {h: i for i, h in enumerate(hdr)}
    cl, cd, cm = data[:, col["CL"]], data[:, col["CD"]], data[:, col["CMz"]]
    rp = data[:, col["rms[P]"]]
    n = len(cl); tail = slice(max(0, n - 300), n)
    done = (d / "done.txt").read_text() if (d / "done.txt").exists() else ""
    wall = float(done.split("wall_s=")[1].split()[0]) if "wall_s=" in done else None
    # converged = residual dropped and the force Cauchy criterion was met before ITER, or forces are steady
    cl_std, cd_std = float(cl[tail].std()), float(cd[tail].std())
    steady = cl_std < 2e-3 and cd_std < 2e-4
    return {"CL": float(cl[tail].mean()), "CD": float(cd[tail].mean()), "CM": float(cm[tail].mean()),
            "CL_final": float(cl[-1]), "CD_final": float(cd[-1]),
            "CL_std_last300": cl_std, "CD_std_last300": cd_std, "rmsP_final": float(rp[-1]), "rmsP_drop": float(rp[0] - rp[-1]),
            "iters": int(n), "converged": bool(steady), "wall_s": wall, "dir": d.name}

def surface_cp(d: Path):
    f = d / "surface_flow.csv"
    if not f.exists():
        return None
    with open(f) as fh:
        rows = list(csv.reader(fh))
    hdr = [h.strip().strip('"') for h in rows[0]]
    data = np.array([[float(v) for v in r] for r in rows[1:] if len(r) == len(hdr)])
    ix, iy = hdr.index("x"), hdr.index("y")
    if "Pressure_Coefficient" in hdr:
        cp = data[:, hdr.index("Pressure_Coefficient")]
    elif "Pressure" in hdr:
        # INC_NONDIM= INITIAL_VALUES: gauge pressure scaled by rho V^2 (freestream 0), so Cp = p / (0.5) = 2 p;
        # check: the stagnation point reads 0.50, i.e. Cp = 1.0
        cp = 2.0 * data[:, hdr.index("Pressure")]
    else:
        return None
    icf = hdr.index("Skin_Friction_Coefficient_x") if "Skin_Friction_Coefficient_x" in hdr else None
    out = {"x": data[:, ix].tolist(), "y": data[:, iy].tolist(), "Cp": cp.tolist()}
    if icf is not None:
        out["Cfx"] = data[:, icf].tolist()
    return out

def gci(f1, f2, f3, r=math.sqrt(2.0), Fs=1.25):
    """Celik et al., J. Fluids Eng. 130 (2008) 078001: f1 fine, f2 medium, f3 coarse, constant ratio r."""
    e32, e21 = f3 - f2, f2 - f1
    if abs(e21) < 1e-12 or abs(e32) < 1e-12 or e32 / e21 <= 0:
        return {"p": None, "GCI_fine_pct": None, "f_ext": None, "note": "oscillatory or converged to round-off; GCI undefined"}
    p = abs(math.log(abs(e32 / e21))) / math.log(r)
    f_ext = (r**p * f1 - f2) / (r**p - 1)
    ea = abs((f1 - f2) / f1) if f1 else float("nan")
    return {"p": p, "GCI_fine_pct": 100 * Fs * ea / (r**p - 1), "f_ext": f_ext, "e21_pct": 100 * ea}

def main():
    quick = "--quick" in sys.argv
    t0 = time.time()
    sweep = [-4, -2, 0, 2, 4, 6, 8, 10, 12, 13, 14, 15, 16, 17, 18, 20]
    if quick:
        sweep = [0, 2, 8, 14]
    matrix = []
    for L in (1, 2, 3):
        for a in (2.0, 10.0):
            matrix.append((L, a, "SA"))
    for a in sweep:
        if (2, float(a), "SA") not in matrix:
            matrix.append((2, float(a), "SA"))
    for a in ([2, 10] if quick else [0, 2, 4, 6, 8, 10, 12, 14, 16]):
        matrix.append((2, float(a), "SSTLM"))
    dirs = []
    for L, a, m in matrix:
        high = a >= 13
        dirs.append(write_case(L, a, m, iters=6000 if high else 4000, cfl=1.0 if high else 2.0, cflmax=20.0 if high else 50.0, limfreeze=800 if high else 300))
    print(f"SU2 2-D: {len(dirs)} cases, {N_JOBS} jobs x {THREADS} threads on {NCPU} CPUs")
    results = {}
    def safe_run(d):   # one broken case must never stop the batch
        try:
            return run_case(d)
        except Exception as e:
            print(f"  {d.name}: failed ({e})")
            return {**NAN_RESULT, "dir": d.name, "error": str(e)}
    failed = []
    with ThreadPoolExecutor(max_workers=N_JOBS) as ex:
        for d, r in zip(dirs, ex.map(safe_run, dirs)):
            if _finite(r):
                results[d.name] = r
            else:
                failed.append(d.name)   # kept out of the polars and the mesh study (NaN is not valid JSON either)
    if failed:
        print(f"  {len(failed)} cases without a usable result: {', '.join(failed)}")
    # assemble
    res = {"tool": "SU2 8.5.0 win64-omp (https://github.com/su2code/SU2/releases/tag/v8.5.0, checked 2026-10-21)",
           "solver": "INC_RANS, FDS + MUSCL, implicit Euler; SA fully turbulent; SST + Langtry-Menter LM transition",
           "Re": RE, "V_mps": V, "mu_model": MU_C, "threads_per_case": THREADS, "jobs": N_JOBS, "cases": results, "failed_cases": failed}
    mesh_info = json.loads((OUT / "mesh_2d.json").read_text()) if (OUT / "mesh_2d.json").exists() else {}
    res["mesh_study"] = {}
    for a in (2.0, 10.0):
        f = {L: results.get(case_name(L, a, "SA")) for L in (1, 2, 3)}
        if all(f.values()):
            res["mesh_study"][f"alpha_{a:g}"] = {
                "cells": {f"L{L}": mesh_info.get(f"L{L}", {}).get("cells") for L in (1, 2, 3)},
                "CL": {f"L{L}": f[L]["CL"] for L in (1, 2, 3)}, "CD": {f"L{L}": f[L]["CD"] for L in (1, 2, 3)},
                "GCI_CL": gci(f[3]["CL"], f[2]["CL"], f[1]["CL"]), "GCI_CD": gci(f[3]["CD"], f[2]["CD"], f[1]["CD"]),
                "pct_change_CL_L2_L3": 100 * (f[3]["CL"] - f[2]["CL"]) / f[3]["CL"],
                "pct_change_CD_L2_L3": 100 * (f[3]["CD"] - f[2]["CD"]) / f[3]["CD"]}
    for m in ("SA", "SSTLM"):
        pol = sorted([(a, r) for a, r in ((float(k.split("_a")[1].replace("p", "+").replace("m", "-")), v) for k, v in results.items()
                      if k.startswith(f"L2_{m}_"))])
        res[f"polar_L2_{m}"] = [{"alpha_deg": a, "CL": r["CL"], "CD": r["CD"], "CM": r["CM"], "converged": r["converged"],
                                 "CL_std": r["CL_std_last300"]} for a, r in pol]
    res["cp"] = {}
    for a in (2.0, 8.0, 12.0):
        cp = surface_cp(SU2_DIR / case_name(2, a, "SA"))
        if cp: res["cp"][f"L2_SA_a{a:g}"] = cp
    for a in (2.0,):
        cp = surface_cp(SU2_DIR / case_name(2, a, "SSTLM"))
        if cp: res["cp"][f"L2_SSTLM_a{a:g}"] = cp
    res["wall_s_total"] = time.time() - t0
    dump_json(OUT / "su2_2d.json", res)
    print(f"SU2 2-D total {res['wall_s_total']/60:.1f} min")

if __name__ == "__main__":
    main()
