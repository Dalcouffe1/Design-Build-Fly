# Aerodynamics: section polars, whole-aircraft VLM, drag build-up and 2-D RANS CFD (C0, v0.3.1 analysis, updated for v0.4)

Baseline: concept C0, 4-lift twin-boom quadplane, NACA 2415 XPS wing (span 2.4 m, chord
0.2387 m, S 0.5728 m², AR 10.06), NACA 0009 H-tail and fins on the boom tops, 0.85 × 0.16 m
pod, stopped 17 in lift rotors, 14 in pusher. MTOW 6.182 kg, W/S 105.8 N/m² (analysis/results.json
v0.3.1). All runs are reproducible from `analysis/aero/run_all.py`; every number below is in
`analysis/aero/aero_results.json` or the `analysis/aero/out/*.json` it is built from.

Verdict first:

| Sizing assumption (sizing.mjs / model.js) | This package | Status |
|---|---|---|
| CL_max 1.2 → Vs 12.0 m/s (R11 ≤ 12) | 2-D CL_max 1.30 (exp. Re 2e5; XFOIL 1.36, NeuralFoil 1.28); 3-D trimmed **1.13** → **Vs 12.4 m/s** | **R11 at risk** (3 % over) |
| CD0 total 0.059 (clean 0.030 + rotors/booms) | **0.081** (clean airframe 0.059, of which round gear legs 0.036; rotors + booms 0.021) | **fails** |
| L/D cruise 6.5, P_cruise 365 W, 11.9 min cruise | **L/D 5.0, 479 W, 9.1 min** at 20 m/s | **R10 (≥ 10 min) fails** |
| same, with faired gear legs (EST Cd 0.1) | L/D 8.0, 299 W, 14.6 min | R10 met |
| Oswald e 0.80 | 0.835 (fit; inviscid 0.927 AVL) | holds |
| Neutral point 46.7 % MAC, SM 18.7 % at the 28 % CG | NP **41.1 %** with the pod (47.3 % without); SM **13.1 %** at 28 %, **5.2 %** at the no-payload CG (36 %) | holds at 28 %; **no-payload case at risk** |
| Cruise trim | α 3.2°, elevator −3.8° (AVL); at 12 m/s: α 11.7°, elevator −9.4° | holds |

## Design v0.4 update (2026-10-23)

Sections 3 to 9 record the analysis of the v0.3.1 geometry (chord 239 mm, bare round landing legs), which drove the redesign: printed teardrop fairings on the legs, the wing sized on the trimmed CL_max, the pod's neutral-point shift added to the sizing, and a ballast rule when flying without payload. Every script was then re-run on the v0.4 geometry; `analysis/aero/aero_results.json` holds the current values, and the review reads them from there:

| Quantity | v0.4 value |
|---|---|
| Wing | S 0.632 m², chord 263 mm, AR 9.12 |
| Cruise Reynolds number | 3.60×10⁵ |
| CL_max, aircraft trimmed | 1.122 (section 1.30 adopted from wind-tunnel data) |
| Stall speed at MTOW | 12.05 m/s (R11: ≤ 12, at the limit) |
| CD0, with faired legs | 0.0427 |
| Cruise L/D at 20 m/s | 8.32 |
| Cruise electrical power | 297 W (sizing design value 323 W) |
| Cruise available | 14.2 min (sizing 13.1 min); bare round legs 8.8 min |
| Neutral point, AVL with pod | 40.4 % MAC; static margin 12.4 % at the 28 % CG, 5.3 % with the bay empty |
| CFD | §6 below (final status) |

## 1. Tools and versions (all checked 2026-10-21)

| Tool | Version | Source | Used for |
|---|---|---|---|
| XFOIL | 6.99 Windows binary | https://web.mit.edu/drela/Public/web/xfoil/XFOIL6.99.zip | section polars, Cp (transitional e^N, Ncrit 9 and 5) |
| NeuralFoil / AeroSandbox | 0.3.3 / 4.2.10 (PyPI) | https://github.com/peterdsharpe/NeuralFoil, https://github.com/peterdsharpe/AeroSandbox | polar cross-check through stall; VLM and AeroBuildup cross-check |
| AVL | 3.52 Windows binary | https://web.mit.edu/drela/Public/web/avl/avl352.exe | whole-aircraft VLM + slender body: lift curve, NP, derivatives, trim, span loading |
| SU2 | 8.5.0 win64-omp | https://github.com/su2code/SU2/releases/download/v8.5.0/SU2-v8.5.0-win64-omp.zip | 2-D incompressible RANS (SA; SST + LM transition) |
| gmsh | 4.15.2 (PyPI) | https://pypi.org/project/gmsh/ | unstructured hybrid meshes (quad boundary layer + triangles) |
| numpy / scipy / matplotlib | 2.5.4 / 1.18.1 / 3.11.2 (PyPI) | | post-processing, plots |

Tools live in `C:\Users\dillo\tools\` (outside the repo). SOLIDWORKS Flow Simulation was not used
(add-in does not load on this licence). No WSL/OpenFOAM on this machine (`wsl -l -v`: not installed).

Scripts (`analysis/aero/`): `common.py` (geometry and conditions read from results.json and
cad/params.txt), `xfoil_polars.py`, `neuralfoil_polars.py`, `avl_run.py`, `asb_model.py`,
`mesh_2d.py`, `su2_2d.py`, `drag_buildup.py`, `plots.py`, `assemble_results.py`, `run_all.py`.
Meshes and solver volume files are git-ignored (`analysis/aero/.gitignore`); `mesh_2d.py` regenerates
them in about a minute.

## 2. Flight conditions

ISA sea level, ρ 1.225 kg/m³ (results.json base.rho), μ 1.789e-5 Pa s. Re = ρ V c / μ.

| Case | V (m/s) | Wing Re (c 0.2387 m) | Mach | Tail Re (c 0.0756 m) |
|---|---|---|---|---|
| stall (R11) | 12 | 1.96e5 | 0.035 | 6.2e4 |
| transition (AIRSPEED_MIN) | 16 | 2.61e5 | 0.047 | — |
| cruise (R11 mid) | 20 | 3.27e5 | 0.059 | 1.04e5 |

Cruise CL = 2W/(ρ V² S) = 0.432 (model.js F5).

## 3. Section polars (XFOIL, NeuralFoil) and validation

`analysis/aero/out/xfoil_polars.json`, `neuralfoil_polars.json`; plot `plots/section_polars.png`,
`plots/tail_section_polars.png`. XFOIL: 240 panels, 200 iterations, ±sweeps from α 0 in 0.5° steps.

| Case | Re | Ncrit | CL_max (α) XFOIL | CL_max (α) NeuralFoil | CLα /rad | α0L | Cm c/4 | Cd min |
|---|---|---|---|---|---|---|---|---|
| NACA 2415 stall | 1.96e5 | 9 | 1.362 (18.0°) | 1.282 (17.0°) | 6.89 | −2.11° | −0.057 | 0.0109 |
| NACA 2415 transition | 2.61e5 | 9 | 1.361 (18.5°) | 1.274 (18.0°) | 6.87 | −2.05° | −0.055 | 0.0094 |
| NACA 2415 cruise | 3.27e5 | 9 | 1.360 (18.5°) | 1.275 (17.5°) | 6.84 | −2.02° | −0.054 | 0.0086 |
| NACA 2415 cruise, dirty | 3.27e5 | 5 | 1.360 (15.0°) | 1.278 (16.0°) | 6.45 | −2.04° | −0.049 | 0.0093 |
| NACA 2415, high Re check | 3.0e6 | 9 | 1.739 (18.5°) | — | 6.36 | −2.20° | −0.051 | 0.0058 |
| NACA 0009 tail stall | 6.2e4 | 9 | 0.779 (8.0°) | 0.765 (8.0°) | 5.96 | −0.2° | −0.002 | 0.0148 |
| NACA 0009 tail cruise | 1.04e5 | 9 | 0.827 (8.5°) | 0.802 (8.0°) | 6.48 | −0.1° | −0.002 | 0.0119 |

Published data at the relevant Re: Genç, Lock and Kaynak (AIAA ATIO 2008, abstract at
https://avesis.erciyes.edu.tr/yayin/0e967406-3720-4460-b125-1dec2b27685c, checked 2026-10-21)
measured a NACA 2415 at Re 2e5 with **CL_max 1.3 at a 12° stall**; Genç, Karasu and Açıkel
(Exp. Thermal Fluid Sci. 39 (2012) 252–264, DOI 10.1016/j.expthermflusci.2012.01.029, abstract
only read) report mild stall at the upper end of Re 0.5–3e5 and abrupt stall at the lower end.
XFOIL's 1.36 is 5 % above the experiment and NeuralFoil's 1.28 is 1.5 % below; both over-predict
the stall angle (XFOIL 18° vs 12°), which is the usual e^N behaviour on a thick cambered section
with a separation bubble. **Adopted 2-D CL_max = 1.30** (experiment). The NACA Report 824 values at
Re 3e6 were not retrieved online on 2026-10-21 (—); the Re 3e6 XFOIL case is only a self-check
that the panelling and Ncrit are sane (XFOIL is known to over-predict CL_max at high Re).

Ncrit 5 (taped XPS skin, outdoor turbulence) does not change CL_max but brings the stall angle
down to 15° and raises the drag at cruise CL by 1 % (0.00880 vs 0.00875 on S_ref). The low-Re tail
(Re 6e4–1e5) stalls at only 8–8.5° with CL_max 0.78–0.83: the elevator authority margin at 12 m/s
(see §5) must respect this.

## 4. Whole-aircraft VLM (AVL) and comparison with the repo VLM

`analysis/aero/out/avl_results.json`, model file `out/avl/c0_pod.avl`. Wing 16 × 30 panels per
half, tail 10 × 16, fins 8 × 10, pod as an AVL BODY from the CAD revolve profile; section CDCL
polars from XFOIL (wing at Re 3.27e5, tail at 1.04e5). Reference point: CG at 28 % MAC
(x = −0.0668 m), Z_ref 0 (CG height is not in results.json; EST).

| Quantity | AVL with pod | AVL no pod | repo vlm.mjs (aero.json) | sizing.mjs | AeroSandbox VLM (no pod) |
|---|---|---|---|---|---|
| CLα aircraft /rad | 5.48 | 5.23 | 5.33 | 5.16 (Helmbold wing) | 5.39 |
| Cmα /rad about 28 % | −0.72 | −1.00 | — | — | −1.15 |
| Neutral point % MAC | **41.1** | 47.3 | 47.6 | 46.7 | 49.4 |
| Static margin at 28 % CG | 13.1 % | 19.3 % | 19.6 % | 18.7 % | 21.4 % |
| Span efficiency e (inviscid) | 0.927 | 0.940 | 0.974 (wing alone) | 0.80 (Oswald, EST) | — |
| Trim at CL 0.432 | α 3.16°, elevator −3.75° | α 3.14°, −4.11° | — | — | — |
| Cm at CL 0.432, elevator 0 | −0.069 | | | | |

Without the pod AVL reproduces the repo VLM and sizing neutral point within 1 % MAC, so the repo
model is consistent. **The pod moves the neutral point forward by 6.2 % MAC** (slender-body
destabilising moment, Munk; the 0.85 m pod with its nose 0.45 m ahead of the LE sits mostly ahead
of the CG), which the sizing and vlm.mjs both note they omit. Static margin by CG case
(results.json cg): 28 % target 13.1 %; battery +30 mm forward 14.8 %; −30 mm aft 11.5 %;
**no-payload CG 36.0 % → 5.2 %**. Raymer (5th ed. §16.4) suggests 5–10 % for a conventional
aircraft; ArduPilot will fly 5 %, but gust response and the tail stall at 8° make the no-payload
case marginal. Tail volumes V_h 0.35, V_v 0.030 (results.json) are at the low end of Raymer
table 6.4 (homebuilt 0.50/0.04, sailplane 0.50/0.02).

Stability derivatives at cruise trim (AVL, per rad, body axes, Xref 28 % MAC):
CLα 5.49, Cmα −0.72, CLq 8.17, Cmq −11.6, CYβ −0.198, Clβ −0.0097, Cnβ 0.0385, Clp −0.579,
Clr 0.120, Cnp −0.033, Cnr −0.048; spiral parameter Clβ Cnr/(Clr Cnβ) = 0.10 (< 1: spiral mode
divergent, slow; normal for a low-dihedral, twin-fin layout; the autopilot holds it). Cnβ 0.0385
is adequate (Raymer typical 0.04–0.08 per rad is not quoted here; Nelson, Flight Stability and
Automatic Control, table 2.1 lists 0.05–0.1 for light aircraft; EST that 0.0385 is on the low side
and the fins should not be reduced). Elevator and aileron derivatives are not extracted by the
parser (AVL names them CLd2 etc. only when control deflections are variables; "—").

Span loading (`plots/span_loading.png`): the rectangular wing peaks at 2y/b ≈ 0.2 with
max cl / CL = 1.148 at trim, so the critical section (first to reach the 2-D CL_max) is inboard,
which gives benign root-first stall but costs 13 % of CL_max compared with an elliptic wing.

## 5. CL_max, stall speed and elevator at stall

3-D CL_max (trimmed, tail download included): critical-section method on the AVL loading,
CL_max,3D = 1.30 / 1.148 = **1.13**; Raymer eq. 12.15 (0.9 × 2-D for an unswept high-AR wing) gives
1.17; AeroSandbox AeroBuildup (NeuralFoil sections, untrimmed, elevator 0) gives 1.33 at 17°. Adopted
**1.13** (the lower of the two trimmed estimates).

Stall speed at the sized wing loading: Vs = sqrt(2 W / (ρ S CL_max)) = **12.36 m/s** vs the R11
limit of 12.0 (sizing used 1.2 → 12.0 exactly). 1.3 Vs = 16.1 m/s, i.e. the transition AIRSPEED_MIN
of 16 m/s (DESIGN-BASIS R11) no longer carries any margin. AVL trim at CL 1.20 (12 m/s): α 11.7°,
elevator −9.4°; the tail section at Re 6e4 stalls at about 8° of local incidence, so the elevator
is within authority but with modest margin (the trim at CL 1.13 is ~−8.5°, EST by interpolation).

## 6. 2-D RANS CFD (SU2) — mesh study and validation

See §6 details below (filled from `out/su2_2d.json`; `plots/su2_mesh_study.png`,
`plots/su2_convergence.png`, `plots/cp_distributions.png`).

**Status 2026-10-23, design v0.4 (chord 263 mm, Re 3.60×10⁵): complete, with the medium mesh unresolved.** Set-up as before: incompressible RANS (SU2 INC_RANS, FDS + MUSCL, Venkatakrishnan limiter, adaptive CFL), SA fully turbulent, SST + Langtry–Menter transition as a sensitivity set, gmsh hybrid meshes with a quad boundary layer (y+ below 1): L1 55,384 cells, L2 130,792, L3 267,902. The v0.3.1 matrix (Re 3.27×10⁵) is archived in `out/archive_v031/`.

| Case | CL | CD | Status |
|---|---|---|---|
| L1 SA α 10° | 1.044 | 0.0443 | converged |
| L2 SA α 10° | — | — | did not converge |
| L3 SA α 10° | 1.168 | 0.0281 | converged |
| L1 SA α 2° | 0.403 | 0.0165 | converged |
| L2 SA α 2° | — | — | did not converge |
| L3 SA α 2° | 0.413 | 0.0149 | converged |
| XFOIL (Ncrit 9), α 10° / 2° | 1.204 / 0.452 | 0.0185 / 0.0091 | transitional |

From the coarse to the fine mesh lift changes 10.6 % at 10° and 2.3 % at 2°. A three-mesh GCI is undefined because the medium mesh did not converge, so the fine mesh is taken as the reference without an extrapolated value. On it, SA lift sits 2.9 % below XFOIL at 10° and 8.7 % below at 2°, and SA drag is higher at 2° (0.0149 against 0.0091): the expected signature of a fully turbulent model at a Reynolds number where the real boundary layer stays laminar over much of the chord.

Of the 29 cases, 6 produced no usable result and 7 of the remaining 23 settled to a steady force history. The medium-mesh sweep converged only at -2°, 8°, 15° (CL -0.003, 0.995, 1.166); elsewhere it diverged or oscillated, as did most transition-model runs. Two driver faults found during the run are fixed in `su2_2d.py`: the retry rewrote every `ITER` key (including the linear-solver limit, which stopped those runs within seconds), and the surface-pressure reader expected a coefficient column the incompressible solver does not write (Cp = 2 p with INC_NONDIM INITIAL_VALUES). **Conclusion for the design:** the section CL_max and drag used in the sizing rest on XFOIL, NeuralFoil and the Genç, Lock and Kaynak (2008) wind-tunnel data; the converged fine-mesh CFD agrees with them within about 10 % in lift and is a cross-check, not a source. A converged medium mesh (lower CFL, longer limiter freeze) would complete a formal GCI; it does not change any design number.

Wall-clock: L1 ≈ 11–13 min, L2 ≈
12–22 min, L3 ≈ 38–43 min per case at 4 threads with 5 cases sharing 20 logical CPUs.

## 7. Drag build-up at cruise

`analysis/aero/out/drag_buildup.json`; `plots/drag_breakdown.png`, `plots/drag_pie.png`,
`plots/aircraft_polar.png`. q = 245 Pa, S_ref 0.5728 m². Method: Raymer 5th ed. ch. 12.5 (Cf
eq. 12.27, wing FF eq. 12.30, body FF eq. 12.31, wetted areas eq. 7.12) for streamlined parts;
Hoerner, *Fluid-Dynamic Drag* (1965) ch. III (cylinders), V (protuberances), VIII (junctions) for
bluff parts; XFOIL section drag for the wing and tail. Items tagged EST are engineering estimates
of frontal areas of printed parts that are not dimensioned in cad/params.txt.

| Component | ΔCD0 | Basis |
|---|---|---|
| **Landing legs (4), round 25.4 mm × 205 mm tubes in cross-flow** | **0.0364** | Hoerner ch. III: 2-D Cd 1.2 at subcritical Re 3.5e4 × 0.8 finite-length (L/D 8) = 1.0 |
| Wing profile (XFOIL, Ncrit 9, exposed span, section cl = 0.43) | 0.0088 | out/xfoil_n2415_cruise_ncrit9.txt (Raymer fully-turbulent FF method: 0.0116) |
| Lift motors (4), stopped, 48 × 32 mm cylinders in cross-flow | 0.0068 | Hoerner ch. III short cylinder Cd 0.63 |
| Lift props (4), stopped at random azimuth | 0.0054 | EST Cd 0.06 on blade planform (pitch angle 7.8° at 0.75 R for 17×5.5) |
| Boom and gear clamps (8) | 0.0040 | EST Cd 0.8 on 30 × 12 mm each beyond the boom |
| Leakage, hinges, horns, wires | 0.0038 | Raymer §12.5.6: 5 % of the rest (EST, taped foam build) |
| Pod (body of revolution, Swet 0.37 m², f 5.3) | 0.0038 | Raymer FF 1.41, Cf at Re 1.16e6 |
| Motor mounts (4) | 0.0034 | EST Cd 0.8 on 50 × 12 mm |
| Feet (4) | 0.0025 | EST Cd 0.8 on 30 × 15 mm |
| Booms (2), 1.47 m axial cylinders | 0.0018 | Raymer Cf × FF 1.145 × Swet 0.117 m² each |
| H-tail (XFOIL NACA 0009, Re 1.0e5) | 0.0014 | out/xfoil_n0009_tail_cruise_ncrit9.txt |
| Fins (2) | 0.0012 | Raymer FF, Q 1.05 |
| Canopy blister, wing-pod junctions, pitot | 0.0015 | Hoerner (EST) |
| **Total CD0** | **0.0806** | sizing: 0.0590 |

By group: wing 0.0088, tail 0.0026, pod 0.0051, booms 0.0058, rotors 0.0155, gear 0.0389,
misc 0.0040. Against the sizing inputs: clean airframe (wing + tail + pod + gear + misc)
**0.059 vs 0.030 assumed** — the whole excess is the bare round gear legs; the clean airframe
without gear is 0.020 (better than assumed). Per boom incl. clamps 1.7e-3 m² vs 1.5e-3 assumed
(holds); per stopped rotor unit (motor + mount + prop) 2.2e-3 m² vs 3.4e-3 assumed (conservative in
the sizing, but the prop term is a rough EST — see M3).

Trimmed polar: CD = CD0,fixed + Cd,wing(CL) (XFOIL) + CL²/(π AR e_inv) with e_inv 0.927 from AVL;
a parabola fit over CL 0.2–0.9 returns **Oswald e = 0.835** (sizing 0.80 holds) and K 0.0379. At
cruise CL 0.432: **CD 0.0870, L/D 4.96**, D 12.2 N, P_elec = D V/(0.60 × 0.85) = **479 W** (sizing
365 W, +31 %). L/D_max 8.7 at CL 1.15, i.e. at 12.3 m/s — essentially the stall speed, which means
the aircraft is flown far above its best L/D speed and the drag is dominated by parasite drag.
With streamlined gear legs (Hoerner ch. VI strut fairing, Cd ≈ 0.1 on frontal, EST): CD 0.0515, L/D 7.96, 299 W.

Wing profile drag, three ways (ΔCD on S_ref at section cl 0.43): XFOIL transitional 0.0088,
Raymer fully-turbulent 0.0116, SU2 SA fully-turbulent see §6. The transitional value is used
because the taped XPS skin is smooth; if the skin is rough (tape ridges, dents) the turbulent
value applies (+0.003 CD, +4 % cruise power).

## 8. Mission implication (model.js F7–F9 with the new drag)

Hover power and the pack are unchanged (832 W, Ovonic 6S 6500, 144.3 Wh; results.json). Cruise
time available after 2 min hover + 40 s transitions and the 20 % reserve:

| Case | P_cruise (W) | I_cruise (A) | Cruise available (min) | R10 ≥ 10 min | Sizing margin ≥ 11.5 min |
|---|---|---|---|---|---|
| sizing.mjs (CD0 0.059, e 0.80) | 365 | 16.5 | 11.9 | met | met |
| this build-up, round gear legs | **479** | 21.6 | **9.1** | **fails** | fails |
| this build-up, faired gear legs (EST) | 299 | 13.5 | 14.6 | met | met |

Flying at 18 m/s instead of 20 (R11 lower bound) with round legs: CL 0.533, CD 0.0908,
P = 0.5 ρ V³ S CD /(0.60 × 0.85) = 364 W → 12.0 min (calc, same formula): R10 met, but only
0.5 min above the 11.5 min design margin and the drag-count is itself ±20 % (EST items). At 22 m/s:
620 W, 7.0 min. Fairing the legs is the cheapest fix (balsa or printed PETG teardrop sleeves; no
BOM line yet, EST < $10). The 10 min floor corresponds to ≤ 436 W electrical cruise power.

## 9. Implications for the design

1. **CD0 does not hold.** 0.081 vs 0.059. The bare 1 in round gear legs are 45 % of the
   total drag (ΔCD 0.036); the sizing's "clean airframe incl. gear 0.030" implicitly assumed
   faired or absent gear. Action: add teardrop fairings to the four legs (or retractable/
   removable legs) and carry CD0 ≈ 0.052–0.055 in sizing.mjs; re-check the pusher at 20 m/s
   (APC 14×8.5 at 479 W electrical is near the X4110S continuous rating, —, not on the page read).
2. **R10 endurance fails with round legs** (9.1 min < 10); met with fairings (14.6 min).
3. **Stall speed 12.4 m/s vs R11 ≤ 12.** CL_max,3D 1.13 (sizing 1.2). Options: accept 12.4 (R11 is
   an assumed value), add 5 % wing area (chord 0.251 m → Vs 12.0 at the same CL_max), or allow 2°
   of washout-free aileron droop (not modelled). Transition AIRSPEED_MIN 16 m/s equals 1.3 Vs
   exactly; set AIRSPEED_MIN to 17 m/s if R11 is kept.
4. **Static margin holds at the 28 % CG (13 %) but the pod cuts it from the 18.7 % the sizing
   expected; the no-payload CG (36 % MAC) leaves 5 %.** Action: keep the payload allowance mass
   or a ballast at the bay when no payload flies; do not move the battery aft of the sized station;
   update sizing.mjs `hn` with a pod term (ΔNP −6 % MAC) so the CG target can move to ~25 % if desired.
5. **Oswald e 0.835 holds** (sizing 0.80 slightly conservative).
6. **Cruise power** 479 W → I 21.6 A continuous from the 6S pack (sizing 16.5 A); within the 60 A
   assumed pack limit but changes the cruise motor thermal case (pusher at 479/0.85 ≈ 560 W shaft-in,
   X4110S 400 KV; maker max 615 W at 3130 g static, budget-lift.md A3) — the pusher is near its
   limit with round legs.
7. The CFD confirms the XFOIL lift curve and Cp to within the limits in §6; it does not change the
   above conclusions, which are driven by the gear and rotor bluff-body drag, not by the wing.

## 10. What the team must measure

- M1 Glide/power test: at 20 m/s level flight read the watt-meter; > 420 W electrical confirms the
  high-drag case and fails R10 (threshold from §8: 10 min requires ≤ 436 W).
- M2 Stall test at altitude at MTOW: airspeed at the pitch break; > 12.5 m/s fails R11 even with
  the 3 % relaxation; also confirms the elevator reserve (expect −9° at 12 m/s).
- M3 Stopped-rotor drag: fly the same speed with props removed vs fitted; ΔP > 60 W means the
  prop EST (Cd 0.06) is low and a prop-parking (azimuth) scheme is needed.
- M4 CG and static margin: weigh at the two legs, confirm 28 % ± 2 % MAC with and without the
  payload allowance; fly a trim-speed sweep to check dδe/dCL is negative (positive = unstable).
- M5 Gear fairing A/B: time-to-climb or watt-meter at 20 m/s with and without the fairings.
