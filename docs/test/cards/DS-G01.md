# Data sheet DS-G01 / DS-G09 — Weight and balance, with and without ballast
DBF-TP-001 rev B data sheet · Test G01 · Date ____ · Operator ____ · Witness ____ · Article/serials ____

Method: two kitchen scales under the wing at stations x1 and x2 from the wing leading edge (or one scale + pivot). MAC = chord = 263.2 mm (results.json sized.chord v0.4); x_LE of the wing at the saddle is the datum. CG % MAC = 100 × x_cg / 263.2.
Targets: 28 ± 2 % MAC (± 5.3 mm) with pack + 0.5 kg ballast (cg.pct.target); never aft of 32.4 % MAC (NP 40.4 % with the pod, aero_results neutral_point.AVL_pod_pct_MAC, − 8 % SM); within 20 mm of the thrust centroid at x = 73.7 mm aft of the LE (cg.thrustCentroid). Mass ≤ 7.40 kg.

| Configuration | x1 (mm) | m1 (kg) | x2 (mm) | m2 (kg) | Total (kg) | x_cg = (m1 x1 + m2 x2)/(m1+m2) (mm) | % MAC | Pass? |
|---|---|---|---|---|---|---|---|---|
| Empty (no pack, no ballast) | | | | | | | | |
| Pack only, no ballast (G09: expected 35.2 % MAC, SM 5.3 % — flight NOT allowed in this state) | | | | | | | | |
| Pack + 0.5 kg ballast (flight config) | | | | | | | | |
| Pack moved fwd/aft ____ mm (if needed) | | | | | | | | |
| Ballast moved ± 30 mm (G09 sensitivity) | | | | | | | | |

Predicted: 26.5–29.5 % MAC over the pack travel; 35.2 % without payload, static margin 5.3 % aero / 4.3 % sizing (results.json cg.pctFwd/pctAft/pct.noPayload/SM_noPayload; aero_results geometry.CG_no_payload_pct_MAC). Ballast rule (cad/README.md; GA sheet note 5): without a payload fly with 0.5 kg in the bay. Ballast station used: ____ mm from the LE. Pack position mark on the pod: ____ mm from the frame.
Lateral CG: tip scales L ____ kg R ____ kg (difference ≤ 2 % EST). Span tip-to-tip: ____ m (2.40 ± 0.02, I03).

## Result
PASS / FAIL · Deviations: ____ · Signed (TD) ______ Date ______
