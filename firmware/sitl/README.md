# SITL rehearsal — C0 parameter set on ArduPlane 4.7.2

**Executed 2026-10-21 on Windows. 7/7 scenarios passed.** Results are in `results/`: `summary.json`, one CSV per
scenario, and `console.log` with the autopilot's status text.

## What was run

| Item | Value |
|---|---|
| SITL binary | ArduPlane **V4.7.2** (fbd0a316), the Windows/cygwin build Mission Planner uses: https://firmware.ardupilot.org/Tools/MissionPlanner/sitl/PlaneStable/ (`ArduPlane.elf` + `cyg*.dll`, run as `ArduPlane.exe`) |
| Model | `quadplane`: ArduPilot's generic QuadPlane. Lift motors on outputs 5–8; aileron/elevator/throttle/rudder on 1–4 (`libraries/SITL/SIM_QuadPlane.cpp`, `SIM_Plane.cpp`) |
| Defaults chain | `Tools/autotest/default_params/quadplane.parm` (ArduPilot's SITL model tune and sensor cal) → `../vtol_c0.param` → `sitl_overrides.param` |
| Overrides | output remap to the SITL model, PWM instead of DShot, SITL airspeed (type 100), SITL battery pins, simulated 6S 6500 mAh pack, SITL serial ports |
| Driver | `mission_test.py` (pymavlink 2.4.50), speed-up 5×, home 37.9156, −122.3366 (placeholder, not the real field) |
| Parameter read-back | 133 parameters from the files read back from the running firmware: all equal. 3 absent in SITL: `SERVO_BLH_AUTO/MASK/OTYPE` (no BLHeli in SITL) |

WSL is not installed on this PC (`wsl -l -v` → "not installed"), so the Linux `sim_vehicle.py` route was not used.
The Mission Planner SITL binary gives the same firmware without WSL.

## Scenarios and results

| # | Scenario | Expected | Result |
|---|---|---|---|
| A | QLOITER take-off to 35 m → AUTO: 6 waypoints at 60 m → NAV_VTOL_LAND at home | transition, loop, back-transition, vertical landing, disarm | PASS. "Transition airspeed reached 16.0" 5 s after the transition started; "Transition done" 10–11 s after AUTO; landed 0.1 m from home |
| B | FBWA transition → fixed-wing LOITER → RC lost (`SIM_RC_FAIL=1`) | RTL → QRTL → land at home, no fence involvement | PASS ("RC Long Failsafe On: switched to RTL") |
| C | QLOITER hover at 15 m → RC lost | QLAND in place | PASS ("RC Short Failsafe: switched to QLand") |
| D | FBWA selected with the throttle low (no pusher thrust) | `Q_TRANS_FAIL` 15 s → QRTL → land | PASS ("Transition failed, exceeded time limit" 15 s after the transition started) |
| E | fixed-wing LOITER → `BATT_LOW_MAH` raised to 6400 | low-battery RTL → QRTL → land at home | PASS ("Battery 1 is low") |
| F | QLOITER full-throttle climb | "Max Alt fence breached" at 110 m → RTL → QRTL; peak < 121.9 m | PASS, peak 113.1 m |
| G | receiver RC removed; RC only as `RC_CHANNELS_OVERRIDE` (how an ELRS RX in MAVLink mode sends it); overrides stop in hover | RC failsafe → QLAND | PASS, failsafe 1–2 s after the overrides stopped (`RC_FS_TIMEOUT` 1 s) |

Log-derived (dataflash log of an earlier full run): attitude-tracking error RMS 0.67° roll / 0.62° pitch in VTOL
modes, 4.5° / 1.7° in fixed-wing modes.

## What SITL does and does not prove

- **Proves** (for firmware 4.7.2): every parameter name and value is accepted. Modes, transition thresholds,
  `Q_TRANS_FAIL`, `Q_RTL_MODE` landing, RC/battery/fence failsafes and the AUTO VTOL mission all behave as intended.
- **Does not prove**: the C0 gains, hover throttle, power or endurance. The SITL model is a generic QuadPlane
  (learned `Q_M_THST_HOVER` 0.15, against about 0.49 est. for C0). Board items — DShot, BLHeli passthrough, the
  CoreWing UART/ADC map, the airspeed I²C address — are replaced by SITL equivalents.
- **Finding from the first run:** a straight FBWA transition left the 300 m fence circle about 6 s after
  "Transition done". The scenarios now go into fixed-wing LOITER right after the transition. Pilots must start the
  turn at once (README §9.6).

## Re-running it

### Windows (as executed)
```powershell
py -m venv venv; .\venv\Scripts\python -m pip install pymavlink
# get ArduPlane.elf + all cyg*.dll from https://firmware.ardupilot.org/Tools/MissionPlanner/sitl/PlaneStable/
# into one folder, copy ArduPlane.elf to ArduPlane.exe
# get quadplane.parm from https://github.com/ArduPilot/ardupilot/blob/master/Tools/autotest/default_params/quadplane.parm
.\venv\Scripts\python firmware\sitl\mission_test.py --sitl-exe C:\sitl\ArduPlane.exe `
    --defaults-base C:\sitl\quadplane.parm `
    --params firmware\vtol_c0.param firmware\sitl\sitl_overrides.param --out sitl_out
```
A full run (A–G) takes about 25 minutes at 5×. `--scenarios AC` runs a subset.

### Linux / WSL (sim_vehicle.py)
```bash
git clone --recurse-submodules https://github.com/ArduPilot/ardupilot && cd ardupilot
git checkout Plane-4.7.2   # tag name per the releases page
Tools/environment_install/install-prereqs-ubuntu.sh -y && . ~/.profile
sim_vehicle.py -v ArduPlane -f quadplane -w --add-param-file=../firmware/vtol_c0.param \
   --add-param-file=../firmware/sitl/sitl_overrides.param --no-mavproxy
python3 firmware/sitl/mission_test.py --connect tcp:127.0.0.1:5760 \
   --params firmware/vtol_c0.param firmware/sitl/sitl_overrides.param --out sitl_out
```
(Linux route not executed here. `--add-param-file` per `sim_vehicle.py --help`; check against your checkout.)

### Hand-flying practice
Mission Planner > Simulation > Plane > QuadPlane, then load `vtol_c0.param` and `sitl_overrides.param`. Fly with
the Pocket Crush as a USB joystick (https://ardupilot.org/planner/docs/mission-planner-simulation.html).
