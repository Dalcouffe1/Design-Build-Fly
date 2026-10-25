# Flight test card FTC-F08 — Airspeed calibration and cruise performance

DBF-TP-001 rev B · Test F08 · Review step: F6 follow-on; review 10 M1, 4 M2 · Requirements: R10, R11

**Prerequisites (signed in test-matrix.csv):** F07 passed twice

**Configuration:** As F07. Flight 1: ARSPD_AUTOCAL 1 (set to 0 after landing). Flight 2: ARSPD_AUTOCAL 0 with the new ARSPD_RATIO.

**Weather limits:** Steady <= 5 m/s, gusts <= 8 m/s; calm air preferred for the power legs (still-air drag back-out, review 4 M2).

**Predicted:** P_cruise 325 W / 14.6 A (results.json sized v0.4) or 298 W / 13.4 A (aero_results mission); CD0 0.0457 / 0.0422; L/D 7.6 / 8.3; drag 8.3 / 7.6 N. Cruise available 12.9 min (sizing) / 14.1 min (aero).

**Crew:** TD, SO, PIC, GCS · **Duration:** 1 session (2 flights), 2 packs

## Pre-flight (every flight)
- [ ] Flight request approved for today and site (R19); Remote ID powered, broadcast seen (E12); registration mark on the pod; PIC TRUST/AMA documents on site (R18).
- [ ] Weather recorded: wind steady ____ m/s, gusts ____ m/s (limit on this card), temp ____ degC, visibility >= 3 sm, no precipitation.
- [ ] Weight and CG per G01: take-off mass ____ kg (<= 7.42 kg), CG ____ % MAC (28 +-2, never aft of 32.3), 0.5 kg ballast fitted (mandatory without payload; GA sheet note 5): Y / N; leg fairings on: Y / N.
- [ ] Props: correct rotation, screws witness-marked, no nicks; pusher spin check.
- [ ] Witness lines on boom clamps, gear clamps, wing bolts unmoved.
- [ ] Packs >= 4.15 V/cell; pack strapped (J18); plugged in LAST through the XT90-S.
- [ ] GCS: firmware hash ____; parameters checked with check_params.py; FENCE_ALT_MAX = site ceiling; FENCE_RADIUS = ____ m; EKF/GPS ok (sats >= 10, HDOP <= 1.5); compass consistent; airspeed 0 +-1 m/s, blow test >= 6 m/s.
- [ ] RC: arm switch CH5 off, mode CH6 in slot for this card, E-stop CH7 off, Q_ASSIST CH8 auto, RTL CH9 off; LQ >= 90 %.
- [ ] Range: crew line 30 m upwind, fire bag + extinguisher + first aid at the line, spectators behind, no people downrange, sky scan.
- [ ] Crew brief: TD states the build-up points, abort word "ABORT", who calls what. Timer ready.

## Hover limits on this aircraft (R14 accepted limitation, DESIGN-BASIS.md)
Hover-only flights: <= 5 m AGL and <= 60 s per hover; cool motors < 45 degC between hovers. C0 cannot survive a lift-motor failure in hover.

## Transition rule (firmware/README.md 9.6; SITL finding)
Start every transition at the upwind field edge and turn as soon as 'Transition done' appears: a straight transition reached the 300 m fence circle ~6 s after transition end in SITL. Telemetry: CRSF on the handset (or the ELRS MAVLink overlay); no separate radio.

## Build-up points (in order; stop and review the log between points when the card says so)
1. [ ] Flight 1: transition, climb to 60 m, fly repeated circuits/LOITER for ~5 min with ARSPD_AUTOCAL 1 (ArduPilot: values save every 2 min when changed > 5 %). Land; set ARSPD_AUTOCAL 0; read ARSPD_RATIO.
2. [ ] Flight 2: transition; two reciprocal 60 s legs at 20 m/s indicated at 60 m AGL (CRUISE); then two reciprocal legs at 18 m/s; then two at 22 m/s. Record current and throttle on each leg.
3. [ ] Back-transition, QLAND.

## Abort criteria (card-specific items first, then the universal list)
- ARSPD_RATIO outside 1.5-3.0 during flight 1 (GCS reads it): land and check for leaks (ArduPilot airspeed calibration page).
- Cruise current > 22 A sustained EST at 20 m/s (pack endurance at risk; pass is 18.9 A).
- Aircraft within 50 m of the 300 m fence in fixed-wing flight.
Universal (TEST-PLAN.md s5): VIBE > 30 m/s2 or rising clipping; attitude error > 15 deg; any oscillation; sag-compensated voltage < 21.0 V or < 1,950 mAh remaining (firmware low-battery RTL point; critical 19.8 V / 1,300 mAh -> QLAND); motor can > 80 degC EST after landing; wind above the card limit; LQ < 70 % EST or any failsafe message; EKF/GPS/compass warning; people or animals inside the range; manned aircraft converging.

## Data to record
ARSP.Airspeed, ARSP.Ratio; GPS.Spd per leg (mean of reciprocal legs removes wind); BAT.Curr and Volt mean per leg; CTUN.ThrOut; reduce with log_summary.py.

## Pass criteria
- [ ] ARSPD_RATIO 1.5-3.0 (ardupilot.org calibrating-an-airspeed-sensor, checked 2026-10-21) and airspeed vs reciprocal-leg GPS mean error <= 10 % (review 10 M1).
- [ ] Cruise at 20 m/s: electrical power <= 420 W = 18.9 A at 22.2 V (calc in TEST-PLAN.md R10 line; aerodynamics.md M1 threshold 420 W) so R10 holds on the selected pack; implied CD0 <= 0.046 x 1.2 EST (sizing CD0 0.0457, aero 0.0422).
- [ ] Trimmed level flight achievable at 18, 20 and 22 m/s (R11 band).

## Run log

| Run | Time | Pack V/cell start | Wind | Point reached | Result | Notes |
|---|---|---|---|---|---|---|
| 1 | | | | | | |
| 2 | | | | | | |
| 3 | | | | | | |

## Post-flight (every flight)
- [ ] IR temperatures within 30 s: M1 ____ M2 ____ M3 ____ M4 ____ pusher ____ ESC max ____ pack ____ degC (any unit 15 degC hotter than its neighbours or pack > 60 degC EST grounds the aircraft).
- [ ] Pack: resting V/cell ____; mAh returned on charge ____.
- [ ] Airframe walk-round: witness lines, prop nicks, foam skin, gear.
- [ ] Log copied from the SD card and named `YYYY-MM-DD_F08_r<n>_C0.bin`; run `docs/test/analysis/log_summary.py` on it.
- [ ] Post-flight report filed within 3 business days for on-campus flights (R19).
- [ ] Result: PASS / PASS WITH LIMITS / FAIL. TD ______ SO ______ PIC ______ Date ______
