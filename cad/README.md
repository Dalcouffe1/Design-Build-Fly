# CAD and simulation

Everything here is generated from `analysis/results.json`. After changing the sizing, re-run the build and the CAD follows. Nothing is modelled by hand.

## Files

| File | Purpose |
|---|---|
| `params.mjs` | Converts the sizing results into build parameters and component placements → `params.txt` |
| `BuildVtol.cs` | SOLIDWORKS API builder: 25 parts + 46-component assembly; exports SLDASM, STEP, EASM |
| `build.ps1` | Runs `params.mjs`, compiles and runs the builder (needs SOLIDWORKS 2025 + .NET Framework 4 `csc`) |
| `Snap.cs` | Saves iso/top/side/front PNGs of the assembly → `images/` |
| `Sim.cs` | SOLIDWORKS Simulation studies → `sim/results.txt`, plots, and `sim/*.SLDPRT` with the studies saved inside |
| `ListDocs.cs` | Lists open SOLIDWORKS documents; `ListDocs.exe close` closes only files under this folder |
| `TlbImp.cs`, `Probe.cs` | Tools used to inspect the Flow Simulation API and add-in availability |
| `model/` | Generated parts, `VTOL_Concept.SLDASM`, `VTOL_Concept.STEP`, `VTOL_Concept.EASM` |

## Model content (v0.3.1)

- **Wing:** NACA 2415 panels, chord from the sizing. A 26 % chord aileron (2 % hinge gap) runs along the outer 55 % of span. The 25.4 mm spar socket is bored 0.8 m from the root at 30 % chord, leaving about 4.6 mm of foam cover.
- **Tail:** NACA 0009 stabiliser seated on the booms, with a separate 30 % chord elevator fitted between the fins. Fins are NACA 0009, lofted to a 65 % tip chord with a swept leading edge.
- **Booms:** 6061-T6 1.000 × 0.035 in front booms, split at the rear motor mount, with aft stubs carrying the tail.
- **Joiner and sockets:** 0.76 m 6061-T6 joiner (0.30 m engagement per side), with 0.23 mm diametral clearance.
- **Propulsion:** turned outrunner envelopes (base flange, stator neck, bell, prop adapter), tapered two-blade props shown parked fore-aft (notional: the BLHeli_S ESCs cannot park them), a pusher spinner, and PA6-CF motor mounts.
- **Clamps:** front and rear boom clamps, with tops sized to meet the wing lower surface at each station, plus gear clamps under the booms.
- **Disc clearance:** booms sit outboard so the rear lift discs clear the pusher disc by 61 mm. `params.mjs` throws if the clearance drops below 20 mm.
- **Pod and canopy:** streamlined body of revolution (elliptic nose, parallel section, smooth-step boat-tail) with an avionics canopy blister.
- **Gear:** four 0.205 m legs cut from the socket-tube offcut (2 × 0.8 + 4 × 0.205 m fits the 96 in tube), with dome feet. The pusher prop clears the ground by 36 mm.
- **Labelled estimates:** motor, pack and pod proportions are marked EST in `params.mjs` because maker drawings were not available.

## Simulations (`sim/results.txt`)

| Study | Model | Result vs hand calc |
|---|---|---|
| Boom static | Arm cantilevered at the spar clamp, tip block = lift-unit mass 0.242 kg, full thrust | 34.2 MPa (1.5 mm mesh; 31.3 at 3 mm) vs 32.6 MPa; 2.18 vs 2.30 mm |
| Boom modal | Same, first bending mode | 38.5 vs 36.0 Hz (hand includes 0.236 m_boom; rigid root) |
| Spar + joiner static | Half-span, fixed at the centreline, joiner idealised with OD = socket ID, load scaled to the elliptic root moment at 4.5 g | 146 MPa (2 mm mesh; 138 at 3 mm) vs 148.6 MPa; the hand calc on the true joiner governs (RF 1.85) |
| Motor thermal | Steady state, 41.6 W heat (est.), h = 50 W/m²K (est.), 35 °C ambient | 133.9 vs 133.5 °C bulk; 90 s hover ≈ 73 °C (lumped, c = 470 J/kgK est.) |

**Not run:**
- **Transient thermal through the API.** It returned no saved time steps, so the 90 s value is a lumped-capacitance estimate (shown in review §12).
- **Flow Simulation (CFD).** The add-in would not load on this licence (only the base SOLIDWORKS serial is listed).

Material for all structural studies: SOLIDWORKS library `6061-T6 (SS)` (E 69 GPa, yield 275 MPa). Results are from the SOLIDWORKS Educational edition, and the plots carry its watermark.
