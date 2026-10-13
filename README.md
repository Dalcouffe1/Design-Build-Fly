# Design–Build–Fly: Lift + Cruise VTOL (student capstone)

A hybrid VTOL with four vertical lift rotors for takeoff and landing, and a pusher prop and fixed wing for cruise. The lift rotors stop in cruise. The aircraft is built and flight-tested on a **fixed $2,000 budget** (hardware, spares, test gear, tax and shipping) by **2027-04-30**.

**Open the design review:** double-click [`VTOL-Review.html`](VTOL-Review.html). It is a single self-contained page with 13 sections, interactive charts, sliders and a 3D viewer.

## Current design (v0.3.1, 2026-10-13)

The numbers below come from `analysis/results.json`, produced by `analysis/sizing.mjs`.

| | |
|---|---|
| Layout | C0: 4-lift twin-boom quadplane, H-tail on the booms, pusher on the pod |
| Span / wing | 2.4 m, 0.573 m², NACA 2415, XPS foam core with packing-tape skin |
| MTOW | 6.18 kg (hard cap 25 kg; below the 8–15 kg working target, pending sign-off, see review §1) |
| Lift / pusher | 5 × SunnySky X4110S 400KV; 17 in lift props; APC 14×8.5EP pusher |
| Hover / cruise | 832 W / 365 W at 20 m/s; hover T/W 2.03 (maker data; confirm on the thrust stand) |
| Energy | 2 × Ovonic 6S 6500 mAh LiPo; 11.9 min cruise available after 2 min hover + 20 % reserve |
| Structure | 6061-T6 booms, spar sockets and 0.76 m joiner; joiner reserve factor 1.85 on yield at 4.5 g (hand calc governs; FEA confirms) |
| Avionics | CoreWing F405 Wing V2 + ArduPilot QuadPlane (beta target; go/no-go 2027-02-22), M10 GPS, MS5525 airspeed, ELRS, BeeID Remote ID |
| Budget | **$1,897.92 committed** (goods $1,630.77 + 10.25 % tax + $100 shipping estimate); **$102.08 reserve** (thin) |

## Repository layout

| Path | What it is |
|---|---|
| `VTOL-Review.html` | The design review (standalone; rebuilt by `review/build.mjs`) |
| `docs/DESIGN-BASIS.md` | Requirements R1–R20 (Given vs Assumed), decisions log |
| `docs/research/` | Live-priced parts research, every price with URL and date (v0.2 and `budget-*` v0.3) |
| `docs/purchasing/C0_BOM.csv` | Purchasing list grouped by vendor, with order-tracking columns |
| `analysis/model.js` | Sizing model (formulas F1–F12). The page sliders run this same file |
| `analysis/sizing.mjs` | Concept sizing, geometry, CG, structures, BOM → `analysis/results.json` |
| `analysis/vlm.mjs` | Vortex-lattice aero check (lift slope, neutral point, span efficiency) → `aero.json` |
| `analysis/verify.py` | Independent re-derivation of every formula, the BOM, FEA vs hand and VLM vs sizing |
| `cad/` | SOLIDWORKS model generator, simulations and outputs. See [`cad/README.md`](cad/README.md) |
| `review/` | Page sources (`src/`) and build script |

## Regenerate everything

```bash
node analysis/sizing.mjs        # sizing + BOM  -> analysis/results.json
node analysis/vlm.mjs           # aero check    -> analysis/aero.json
node analysis/export_bom.mjs    # purchasing    -> docs/purchasing/C0_BOM.csv
py -3 analysis/verify.py        # independent checks (expects "FAILED CHECKS: none")
node review/build.mjs           # review page   -> VTOL-Review.html, review/vtol-review.html
```

The CAD and simulations need SOLIDWORKS 2025 with Simulation on Windows:

```bash
powershell -ExecutionPolicy Bypass -File cad/build.ps1   # params -> parts, assembly, STEP, eDrawings
cad/Snap.exe <full path>/cad/model/VTOL_Concept.SLDASM <full path>/cad/images
cad/Sim.exe cad/params.txt cad/sim                      # FEA + modal + thermal -> cad/sim/results.txt
```

## Working rules (from CLAUDE.md)

- Every number cites its source: a repo file, a formula shown in the review, or a URL with the date checked.
- Estimates are labelled `est`. Unknowns are shown as "—" and never invented.
- The review follows `docs/reference/REVIEW-STYLE.md`.

## Open items for the team

These come from review §1, and each technical section ends with "What the team must measure".

1. **Flying site and legal path.** Ask UCPD/Risk Services by 2026-11-06 whether the capstone qualifies, and request Richmond Field Station.
2. **Flight controller firmware.** The CoreWing ArduPilot target is beta-only today; the Matek F405-WMO is the stable fallback.
3. **Thrust stand first.** Qualify every motor/ESC (tests T1–T6) before any flight; C0 has no hover motor-out tolerance.
4. **Cart totals.** Price shipping at checkout. If the reserve drops under $100: borrow a hot-wire cutter, then drop the spare motor, then the airspeed sensor.
5. **Flight build-up.** Follow review §13 (SITL → bench → tethered → free hover → partial → full transition → mission).

v0.3.1 incorporates a six-reviewer project audit (79 confirmed findings; review §12).
