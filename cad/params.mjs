// Derives SolidWorks build parameters for concept C0 from analysis/results.json.
// SolidWorks frame: X forward (wing LE at X=0), Y up, Z right.  Review frame: x fwd, y right, z up.
// Writes cad/params.txt (key=value and comp=Part|X|Y|Z lines) read by cad/BuildVtol.cs.
import { readFileSync, writeFileSync } from 'fs';
const R = JSON.parse(readFileSync(new URL('../analysis/results.json', import.meta.url)));
const c = R.concepts.C0, s = c.sized, g = c.geo, IN = 0.0254;
const P = {}, comps = [];
const chord = s.chord, half = R.base.span/2, fusR = 0.08, fusL = 0.85;
// NACA 24xx, t/c from the sizing (v0.3.1: 0.15); 2 % camber at 40 % chord, EST airfoil choice
const m = 0.02, pp = 0.4, t = R.base.tc, N = 30, up = [], lo = [];
for (let i = 0; i <= N; i++) {
  const x = (1 - Math.cos(Math.PI * i / N)) / 2;
  const yt = 5*t*(0.2969*Math.sqrt(x) - 0.1260*x - 0.3516*x*x + 0.2843*x**3 - 0.1015*x**4);
  const yc = x < pp ? m/pp**2*(2*pp*x - x*x) : m/(1-pp)**2*((1-2*pp) + 2*pp*x - x*x);
  const dyc = x < pp ? 2*m/pp**2*(pp - x) : 2*m/(1-pp)**2*(pp - x), th = Math.atan(dyc);
  up.push([x - yt*Math.sin(th), yc + yt*Math.cos(th)]); lo.push([x + yt*Math.sin(th), yc - yt*Math.cos(th)]);
}
// one spline TE(upper) -> LE -> TE(lower), model X = -x c
const foil = [...up.slice().reverse(), ...lo.slice(1)].map(([x, y]) => [-x*chord, y*chord]);
P.foil = foil.map(p => p.map(v => v.toFixed(6)).join(',')).join(';');
P.chord = chord; P.panelLen = half - fusR;
const xSp = 0.30;                                                    // spar at max thickness (~30 % c) for foam cover
const xs = -xSp*chord, ys = m/pp**2*(2*pp*xSp - xSp*xSp)*chord;   // on the camber line
// lower-surface height of the section at chord fraction x (for seating the boom clamps)
const ytAt = x => 5*t*(0.2969*Math.sqrt(x) - 0.1260*x - 0.3516*x*x + 0.2843*x**3 - 0.1015*x**4);
const ycAt = x => x < pp ? m/pp**2*(2*pp*x - x*x) : m/(1-pp)**2*((1-2*pp) + 2*pp*x - x*x);
const yLower = x => (ycAt(x) - ytAt(x))*chord, yUpper = x => (ycAt(x) + ytAt(x))*chord;
P.tc = t; P.sparXfrac = xSp;
// v0.3 tubes: 6061-T6 (docs/research/budget-airframe.md §1)
Object.assign(P, { sparX: xs, sparY: ys, sparOD: 1.000*IN, sparID: (1.000-2*0.058)*IN, sparLen: 0.8,
  joinOD: 0.875*IN, joinID: (0.875-2*0.058)*IN, joinLen: 0.76,   // 0.30 m engagement per side (review finding citations-2)
  boomOD: 1.000*IN, boomID: (1.000-2*0.035)*IN,
  segF: g.segs[0], segR: g.segs[1], fusL, fusR, tailR: 0.03,
  motorD: 0.048, motorH: 0.032, liftProp: g.D, propW: 0.035, propT: 0.008,   // X4110S envelope EST (41 mm stator class); prop from sizing
  pushMotorD: 0.048, pushMotorL: 0.032, pushProp: 14*IN,
  hC: g.hC, hSpan: g.hSpan + 1.000*IN, vH: g.vH, vC: g.vC,   // stabiliser spans boom centre to centre plus one boom OD (covers the boom tops)
  elevSpan: 2*g.yIn - 0.012,                                  // elevator between the fins (6 mm gap each side)
  battL: 0.155, battH: 0.050, battW: 0.048,            // 6S 6500 LiPo box EST (pack dims not published)
  legOD: 1.000*IN, legID: (1.000-2*0.058)*IN, legLen: 0.205,  // 4 legs from the 96 in socket tube: 2 x 0.8 + 4 x 0.205 = 2.42 m < 2.438 m incl. kerf
  pitotD: 0.006, pitotL: 0.25 });
// ---- FEA / thermal load cases (analysis/sizing.mjs structures()) ----
const st = c.struct, half_ = R.base.span/2;
const Fhalf = st.nUlt*s.m*9.80665/2;                 // half-wing lift at ultimate load factor
const zc = 4*half_/(3*Math.PI);                      // elliptic-lift centroid from centreline
const zSparMid = fusR + P.sparLen/2;                 // uniform load on the spar outer face acts here
Object.assign(P, { feaBoomL: st.Lb, feaTipForce: st.Tmax, feaTipMass: st.mTip, feaNult: st.nUlt,
  feaWingForce: Fhalf*zc/zSparMid,                   // scaled so root moment = n W b/(3 pi)
  feaRootMomentTarget: st.Mroot, feaHandSigJ: st.sigJ, feaHandSigB: st.sigB, feaHandDefl: st.defl_max, feaHandFn: st.fn,
  thermQ: 0.20*s.P_hover_per_motor,   // EST: 80 % motor+ESC efficiency -> 20 % of hover input becomes heat in the unit
  thermH: 50, thermAmbK: 308.15, thermTime: 90,   // EST: prop-wash convection 50 W/m2K, 35 C hot day, 90 s hover (test plan T3)
  matLib: '6061-T6 (SS)', matE: 69.0e9, matNu: 0.33, matRho: 2700, matSig: 275e6,   // SOLIDWORKS library 6061-T6 (SS)
  motorMass: 0.165, motorCp: 896 });   // X4110S 165 g (budget-lift.md A3); 6061 c_p from SW library, lumped thermal check
const yB = -0.03, rB = P.boomOD/2, yMount = yB + 0.003 + 0.018, yMotor = yMount, yGround = yB - rB - P.legLen + 0.004;
// clamp tops meet the wing lower surface at each station (review finding cad_sim-7)
P.clampTopF = yLower(xSp) - yB; P.clampTopR = yLower(0.80) - yB;
P.yB = yB; P.tailT = 0.09*g.hC;
const yProp = yMotor + P.motorH + 0.006;
const cg = c.cg.items.find(i => i.name.startsWith('Battery'));
const add = (part, X, Y, Z) => comps.push([part, X, Y, Z].map((v, i) => i ? (+v).toFixed(5) : v).join('|'));
const L = P.panelLen, Lin = 0.45*L, tailX = 0.45 - fusL;
add('Fuselage_Pod', 0.45, -0.03, 0);
add('Canopy', 0.33, -0.03 + 0.045, 0);
add('Wing_Panel_R', 0, 0, fusR); add('Wing_Panel_L', 0, 0, -fusR);
add('Aileron_R', 0, 0, fusR + Lin + 0.002); add('Aileron_L', 0, 0, -(fusR + Lin + 0.002));
add('Spar_Tube', xs, ys, fusR); add('Spar_Tube', xs, ys, -fusR - P.sparLen);
add('Wing_Joiner', xs, ys, -P.joinLen/2);
// wing centre-section saddle: the root section carried across the pod (XPS block + PETG root plates over the joiner; no new BOM line),
// so the panels butt onto it instead of onto the round, tapering pod; one M5 screw per side at 70 % chord (existing hardware kit)
add('Wing_Saddle', 0, 0, 0);
P.boltZ = fusR - 0.015;
for (const zz of [-P.boltZ, P.boltZ]) add('Wing_Bolt', -0.70*chord, yUpper(0.70), zz);
add('Battery_6S_LiPo', cg.x, -0.05, 0);
for (const zz of [-g.yIn, g.yIn]) {
  add('Boom_Front', g.xRear - 0.03, yB, zz); add('Boom_Rear_Stub', g.booms[0].x0, yB, zz);
  for (const xr of [g.xFront, g.xRear]) { add('Motor_Mount', xr, yB, zz); add('Lift_Motor', xr, yMotor, zz); add('Lift_Prop', xr, yProp, zz); }
  add('Boom_Clamp_Front', xs, yB, zz); add('Boom_Clamp_Rear', -0.8*chord, yB, zz);
  add('Fin', g.xRear - g.D/2 - 0.05, yB + rB, zz);
  for (const xl of [g.xFront - 0.09, g.xRear + 0.09]) { add('Gear_Clamp', xl, yB, zz); add('Landing_Leg', xl, yGround, zz); }
}
add('H_Tail', g.xRear - g.D/2 - 0.05, yB + rB + P.tailT/2, 0);      // seated on the boom tops
add('Elevator', g.xRear - g.D/2 - 0.05, yB + rB + P.tailT/2, 0);
add('Pusher_Motor', tailX, -0.03, 0);
add('Pusher_Prop_14in', tailX - (P.pushMotorL + 0.006), -0.03, 0);
add('Spinner', tailX - (P.pushMotorL + 0.010), -0.03, 0);
add('Pitot_Tube', -0.05, -0.02, -1.0);
{ const pushX = 0.45 - fusL - P.pushMotorL - 0.006, pr = P.pushProp/2, lr = P.liftProp/2, dy = yProp - (-0.03);
  const pushReach = Math.sqrt(Math.max(0, pr*pr - dy*dy)), dx = Math.abs(pushX - g.xRear), liftIn = g.yIn - Math.sqrt(Math.max(0, lr*lr - dx*dx));
  P.discClear = liftIn - pushReach; if (P.discClear < 0.02) throw new Error('pusher/lift disc clearance ' + P.discClear.toFixed(3) + ' m < 20 mm'); }
const lines = Object.entries(P).map(([k, v]) => `${k}=${typeof v === 'number' ? v.toFixed(6) : v}`).concat(comps.map(x => 'comp=' + x));
writeFileSync(new URL('./params.txt', import.meta.url), lines.join('\n') + '\n');
console.log('params:', Object.keys(P).length, 'components:', comps.length, 'ground Y', yGround.toFixed(3), 'pusher tip Y', (-0.03 - P.pushProp/2).toFixed(3), 'clearance m', (-0.03 - P.pushProp/2 - yGround).toFixed(3), 'prop disc Y', yProp.toFixed(3), 'pusher-lift disc clearance m', P.discClear.toFixed(3));
