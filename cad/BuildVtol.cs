// Builds the concept CAD model of the lift + cruise VTOL in SOLIDWORKS through its API (v0.3 design).
// Inputs: cad/params.txt (from cad/params.mjs <- analysis/results.json). Outputs: cad/model/*.SLDPRT,
// VTOL_Concept.SLDASM, .STEP, .EASM. Frame: X forward (wing LE at X=0), Y up, Z right; metres.
// Detail level: real aerofoil sections (NACA 2413 wing, NACA 0009 tail), split control surfaces with hinge
// gaps, turned motors/spinner/pitot (revolves), tapered two-blade props, printed saddle clamps and motor
// mounts, gear legs with feet. Dimensions marked EST in params.mjs are estimates.
using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

class BuildVtol
{
    static SldWorks sw;
    static Dictionary<string, string> P = new Dictionary<string, string>();
    static List<string[]> Comps = new List<string[]>();
    static string OutDir, PartTpl, AsmTpl;
    static CultureInfo IC = CultureInfo.InvariantCulture;
    static double D(string k) { return double.Parse(P[k], IC); }
    static void Log(string s) { Console.WriteLine(s); }

    // appearance palette (r, g, b)
    static readonly double[] FOAM = { 0.93, 0.94, 0.96 }, SURF = { 0.80, 0.82, 0.86 }, ALU = { 0.72, 0.74, 0.78 },
        PRINT = { 0.12, 0.13, 0.16 }, MOTOR = { 0.18, 0.19, 0.22 }, ACCENT = { 0.86, 0.38, 0.10 }, PROP = { 0.06, 0.06, 0.07 },
        POD = { 0.95, 0.96, 0.97 }, CANOPY = { 0.10, 0.16, 0.26 }, BATT = { 0.20, 0.45, 0.85 }, RUBBER = { 0.08, 0.08, 0.08 };

    static int Main(string[] args)
    {
        foreach (string raw in File.ReadAllLines(args[0]))
        {
            string line = raw.Trim(); if (line.Length == 0) continue;
            int eq = line.IndexOf('='); string k = line.Substring(0, eq), v = line.Substring(eq + 1);
            if (k == "comp") Comps.Add(v.Split('|')); else P[k] = v;
        }
        OutDir = Path.GetFullPath(args[1]); Directory.CreateDirectory(OutDir);
        sw = (SldWorks)Activator.CreateInstance(Type.GetTypeFromProgID("SldWorks.Application"));
        sw.Visible = true;
        PartTpl = sw.GetUserPreferenceStringValue((int)swUserPreferenceStringValue_e.swDefaultTemplatePart);
        AsmTpl = sw.GetUserPreferenceStringValue((int)swUserPreferenceStringValue_e.swDefaultTemplateAssembly);
        Log("SOLIDWORKS " + sw.RevisionNumber());

        var parts = new Dictionary<string, string>();
        double bOD = D("boomOD"), bID = D("boomID");
        parts["Wing_Panel_R"] = WingPanel("Wing_Panel_R", false);
        parts["Wing_Panel_L"] = WingPanel("Wing_Panel_L", true);
        parts["Aileron_R"] = Aileron("Aileron_R", false);
        parts["Aileron_L"] = Aileron("Aileron_L", true);
        parts["Spar_Tube"] = Tube("Spar_Tube", 'Z', D("sparOD"), D("sparID"), D("sparLen"), ALU, "6061-T6 1.000 x 0.058 in spar socket, 0.8 m");
        parts["Wing_Joiner"] = Tube("Wing_Joiner", 'Z', D("joinOD"), D("joinID"), D("joinLen"), ALU, "6061-T6 0.875 x 0.058 in joiner, 0.6 m");
        parts["Boom_Front"] = Tube("Boom_Front", 'X', bOD, bID, D("segF"), ALU, "Front boom, 6061-T6 1.000 x 0.035 in");
        parts["Boom_Rear_Stub"] = Tube("Boom_Rear_Stub", 'X', bOD, bID, D("segR"), ALU, "Rear boom stub carrying the tail, 6061-T6");
        parts["Lift_Motor"] = Motor("Lift_Motor", 'Y');
        parts["Pusher_Motor"] = Motor("Pusher_Motor", 'X');
        parts["Lift_Prop"] = Prop("Lift_Prop", false, D("liftProp"));
        parts["Pusher_Prop_14in"] = Prop("Pusher_Prop_14in", true, D("pushProp"));
        parts["Spinner"] = Revolve("Spinner", 'X', new double[,] { { 0, 0 }, { 0, 0.021 }, { -0.012, 0.019 }, { -0.026, 0.012 }, { -0.038, 0 } }, true, POD, "Pusher spinner 42 mm (EST)");
        parts["Motor_Mount"] = Saddle("Motor_Mount", 0.070, 0.036, 0.046, bOD, true, "Printed PA6-CF motor mount on boom, covers the boom splice at the rear station (EST)");
        parts["Boom_Clamp_Front"] = SaddleTB("Boom_Clamp_Front", 0.034, D("clampTopF"), -(bOD / 2 + 0.004), 0.040, bOD, "Printed PETG boom-to-wing clamp at the spar; top meets the wing lower surface");
        parts["Boom_Clamp_Rear"] = SaddleTB("Boom_Clamp_Rear", 0.034, D("clampTopR"), -(bOD / 2 + 0.004), 0.040, bOD, "Printed PETG boom-to-wing clamp at 80 % chord; top meets the wing lower surface");
        parts["Gear_Clamp"] = SaddleTB("Gear_Clamp", 0.034, bOD / 2 + 0.004, -(bOD / 2 + 0.016), 0.040, bOD, "Printed PETG gear socket clamp under the boom");
        parts["Landing_Leg"] = Leg();
        parts["Pitot_Tube"] = Revolve("Pitot_Tube", 'X', new double[,] { { 0, 0 }, { 0, 0.003 }, { 0.22, 0.003 }, { 0.245, 0.0012 }, { 0.25, 0 } }, true, ALU, "Pitot probe on wing tip (MS5525 + PT40)");
        parts["Fuselage_Pod"] = Fuselage();
        parts["Canopy"] = Canopy();
        parts["Battery_6S_LiPo"] = RoundBox("Battery_6S_LiPo", D("battL"), D("battH"), D("battW"), 0.006, BATT, "6S 6500 mAh LiPo envelope (dims EST)");
        parts["H_Tail"] = Tailplane("H_Tail", false);
        parts["Elevator"] = Tailplane("Elevator", true);
        parts["Fin"] = Fin();

        // ---------- assembly ----------
        ModelDoc2 asmDoc = (ModelDoc2)sw.NewDocument(AsmTpl, 0, 0, 0);
        AssemblyDoc asm = (AssemblyDoc)asmDoc;
        MathUtility mu = (MathUtility)sw.GetMathUtility();
        int n = 0;
        foreach (string[] c in Comps)
        {
            if (!parts.ContainsKey(c[0])) throw new Exception("no part " + c[0]);
            double x = double.Parse(c[1], IC), y = double.Parse(c[2], IC), z = double.Parse(c[3], IC);
            Component2 comp = asm.AddComponent5(parts[c[0]], 0, "", false, "", x, y, z);
            if (comp == null) throw new Exception("AddComponent5 failed for " + c[0]);
            double[] t = { 1, 0, 0, 0, 1, 0, 0, 0, 1, x, y, z, 1, 0, 0, 0 };
            comp.Transform2 = (MathTransform)mu.CreateTransform(t);
            n++;
        }
        asmDoc.ClearSelection2(true); asmDoc.Extension.SelectAll(); asm.FixComponent(); asmDoc.ClearSelection2(true);
        asmDoc.ShowNamedView2("*Isometric", 7); asmDoc.ViewZoomtofit2(); asmDoc.EditRebuild3();
        CustomPropertyManager cpm = asmDoc.Extension.get_CustomPropertyManager("");
        cpm.Add3("Description", 30, "Lift + cruise VTOL concept v0.3 (budget design)", 2);
        cpm.Add3("Source", 30, "analysis/results.json via cad/params.mjs; build cad/build.ps1", 2);
        string asmPath = Path.Combine(OutDir, "VTOL_Concept.SLDASM");
        Save(asmDoc, asmPath);
        Save(asmDoc, Path.Combine(OutDir, "VTOL_Concept.STEP"));
        try { Save(asmDoc, Path.Combine(OutDir, "VTOL_Concept.EASM")); } catch (Exception e) { Log("eDrawings export skipped: " + e.Message); }
        MassProperty mp = asmDoc.Extension.CreateMassProperty();
        Log(string.Format(IC, "Assembly: {0} components. Saved {1}", n, asmPath));
        foreach (string k in parts.Keys) sw.CloseDoc(Path.GetFileName(parts[k]));
        return 0;
    }

    // ---------- generic helpers ----------
    static ModelDoc2 NewPart() { return (ModelDoc2)sw.NewDocument(PartTpl, 0, 0, 0); }
    static SketchManager Sk(ModelDoc2 d, string plane)
    {
        d.ClearSelection2(true);
        if (!d.Extension.SelectByID2(plane, "PLANE", 0, 0, 0, false, 0, null, 0)) throw new Exception("Cannot select " + plane);
        d.SketchManager.InsertSketch(true); d.SketchManager.AddToDB = true;
        return d.SketchManager;
    }
    static void EndSk(ModelDoc2 d)
    {
        d.SketchManager.AddToDB = false; d.SketchManager.InsertSketch(true);
        d.ClearSelection2(true); ((Feature)d.FeatureByPositionReverse(0)).Select2(false, 0);
    }
    static Feature Ext(ModelDoc2 d, double depth, bool flip, bool mid, bool merge = true, double draftDeg = 0, double startOff = 0)
    {
        int end = mid ? (int)swEndConditions_e.swEndCondMidPlane : (int)swEndConditions_e.swEndCondBlind;
        Feature f = d.FeatureManager.FeatureExtrusion3(true, false, flip, end, 0, depth, 0, draftDeg > 0, false, false, false,
            draftDeg * Math.PI / 180, 0, false, false, false, false, merge, true, true,
            startOff > 0 ? (int)swStartConditions_e.swStartOffset : 0, startOff, false);
        if (f == null && draftDeg > 0) return Ext(d, depth, flip, mid, merge, 0, startOff);   // draft not accepted: plain extrude
        if (f == null) throw new Exception("Extrude failed in " + d.GetTitle());
        return f;
    }
    static Feature Cut(ModelDoc2 d, double depth, bool flip, bool through = false)
    {
        int end = through ? (int)swEndConditions_e.swEndCondThroughAll : (int)swEndConditions_e.swEndCondBlind;
        return d.FeatureManager.FeatureCut4(true, false, flip, end, 0, depth, 0, false, false, false, false, 0, 0,
            false, false, false, false, false, true, true, true, true, false, 0, 0, false, false);
    }
    static Feature Rev(ModelDoc2 d)
    {
        Feature f = d.FeatureManager.FeatureRevolve2(true, true, false, false, false, false, 0, 0, 2 * Math.PI, 0,
            false, false, 0, 0, 0, 0, 0, true, true, true);
        if (f == null) throw new Exception("Revolve failed in " + d.GetTitle());
        return f;
    }
    static double Volume(ModelDoc2 d) { d.EditRebuild3(); MassProperty mp = d.Extension.CreateMassProperty(); return mp == null ? 0 : mp.Volume; }
    static string Finish(ModelDoc2 d, string name, double[] rgb, string desc)
    {
        d.MaterialPropertyValues = new double[] { rgb[0], rgb[1], rgb[2], 0.5, 0.85, 0.45, 0.35, 0, 0 };
        CustomPropertyManager cpm = d.Extension.get_CustomPropertyManager("");
        cpm.Add3("Description", (int)swCustomInfoType_e.swCustomInfoText, desc, (int)swCustomPropertyAddOption_e.swCustomPropertyReplaceValue);
        cpm.Add3("Source", (int)swCustomInfoType_e.swCustomInfoText, "Concept geometry from analysis/results.json via cad/params.mjs (v0.3, 2026-10-13)", (int)swCustomPropertyAddOption_e.swCustomPropertyReplaceValue);
        d.EditRebuild3();
        string path = Path.Combine(OutDir, name + ".SLDPRT");
        Save(d, path);
        Log(string.Format(IC, "  part {0,-18} volume {1:0.000000} m3", name, Volume(d)));
        return path;
    }
    static void Save(ModelDoc2 d, string path)
    {
        int err = 0, warn = 0;
        if (!d.Extension.SaveAs(path, (int)swSaveAsVersion_e.swSaveAsCurrentVersion, (int)swSaveAsOptions_e.swSaveAsOptions_Silent, null, ref err, ref warn))
            throw new Exception("Save failed " + path + " err " + err);
    }
    static string PlaneFor(char axis) { return axis == 'X' ? "Right Plane" : axis == 'Y' ? "Top Plane" : "Front Plane"; }
    static void Poly(SketchManager sm, double[,] p, bool close)
    {
        int n = p.GetLength(0);
        for (int i = 0; i < n - 1; i++) sm.CreateLine(p[i, 0], p[i, 1], 0, p[i + 1, 0], p[i + 1, 1], 0);
        if (close) sm.CreateLine(p[n - 1, 0], p[n - 1, 1], 0, p[0, 0], p[0, 1], 0);
    }
    static void RoundRect(SketchManager sm, double cx, double cy, double w, double h, double r)
    {
        double x0 = cx - w / 2, x1 = cx + w / 2, y0 = cy - h / 2, y1 = cy + h / 2;
        sm.CreateLine(x0 + r, y0, 0, x1 - r, y0, 0); sm.CreateLine(x1, y0 + r, 0, x1, y1 - r, 0);
        sm.CreateLine(x1 - r, y1, 0, x0 + r, y1, 0); sm.CreateLine(x0, y1 - r, 0, x0, y0 + r, 0);
        sm.CreateArc(x1 - r, y0 + r, 0, x1 - r, y0, 0, x1, y0 + r, 0, 1);
        sm.CreateArc(x1 - r, y1 - r, 0, x1, y1 - r, 0, x1 - r, y1, 0, 1);
        sm.CreateArc(x0 + r, y1 - r, 0, x0 + r, y1, 0, x0, y1 - r, 0, 1);
        sm.CreateArc(x0 + r, y0 + r, 0, x0, y0 + r, 0, x0 + r, y0, 0, 1);
    }
    // NACA 4-digit section, points from TE(upper) -> LE -> TE(lower), x in [0,1]; returns (x, y) list
    static List<double[]> Naca(double m, double p, double t, int N)
    {
        var up = new List<double[]>(); var lo = new List<double[]>();
        for (int i = 0; i <= N; i++)
        {
            double x = (1 - Math.Cos(Math.PI * i / N)) / 2;
            double yt = 5 * t * (0.2969 * Math.Sqrt(x) - 0.1260 * x - 0.3516 * x * x + 0.2843 * x * x * x - 0.1015 * x * x * x * x);
            double yc = 0, dy = 0;
            if (m > 0) { if (x < p) { yc = m / (p * p) * (2 * p * x - x * x); dy = 2 * m / (p * p) * (p - x); } else { yc = m / ((1 - p) * (1 - p)) * ((1 - 2 * p) + 2 * p * x - x * x); dy = 2 * m / ((1 - p) * (1 - p)) * (p - x); } }
            double th = Math.Atan(dy);
            up.Add(new[] { x - yt * Math.Sin(th), yc + yt * Math.Cos(th) }); lo.Add(new[] { x + yt * Math.Sin(th), yc - yt * Math.Cos(th) });
        }
        up.Reverse(); var all = new List<double[]>(up); for (int i = 1; i < lo.Count; i++) all.Add(lo[i]);
        return all;
    }
    // portion of a section between chord fractions [x0, x1] as a closed outline (model X = -x c), split with straight cuts
    static void SectionPart(SketchManager sm, List<double[]> foil, double c, double x0, double x1)
    {
        var top = new List<double[]>(); var bot = new List<double[]>();
        int le = 0; double minx = 9; for (int i = 0; i < foil.Count; i++) if (foil[i][0] < minx) { minx = foil[i][0]; le = i; }
        for (int i = le; i >= 0; i--) top.Add(foil[i]);            // LE -> TE upper
        for (int i = le; i < foil.Count; i++) bot.Add(foil[i]);    // LE -> TE lower
        Func<List<double[]>, double, double> yAt = (s, x) => { for (int i = 1; i < s.Count; i++) if (s[i][0] >= x) { double f = (x - s[i - 1][0]) / (s[i][0] - s[i - 1][0]); return s[i - 1][1] + f * (s[i][1] - s[i - 1][1]); } return s[s.Count - 1][1]; };
        var pts = new List<double>();
        Action<double, double> add = (x, y) => { pts.Add(-x * c); pts.Add(y * c); pts.Add(0); };
        // upper surface x0 -> x1
        if (x0 > 0) add(x0, yAt(top, x0)); foreach (var q in top) if (q[0] > x0 + 1e-6 && q[0] < x1 - 1e-6) add(q[0], q[1]); add(x1, yAt(top, x1));
        var upper = pts.ToArray(); pts.Clear();
        if (x0 > 0) add(x0, yAt(bot, x0)); foreach (var q in bot) if (q[0] > x0 + 1e-6 && q[0] < x1 - 1e-6) add(q[0], q[1]); add(x1, yAt(bot, x1));
        var lower = pts.ToArray();
        if (x0 <= 0)
        {   // includes the leading edge: one spline around the nose from upper x1 to lower x1
            var nose = new List<double>();
            for (int i = upper.Length / 3 - 1; i >= 0; i--) { nose.Add(upper[3 * i]); nose.Add(upper[3 * i + 1]); nose.Add(0); }
            nose.Add(-0 * c); nose.Add(foil[le][1] * c); nose.Add(0);
            for (int i = 0; i < lower.Length / 3; i++) { nose.Add(lower[3 * i]); nose.Add(lower[3 * i + 1]); nose.Add(0); }
            // remove duplicate LE point if present
            sm.CreateSpline2(nose.ToArray(), true);
        }
        else
        {
            sm.CreateSpline2(upper, true); sm.CreateSpline2(lower, true);
            sm.CreateLine(upper[0], upper[1], 0, lower[0], lower[1], 0);
        }
        int nu = upper.Length / 3, nl = lower.Length / 3;
        sm.CreateLine(upper[3 * (nu - 1)], upper[3 * (nu - 1) + 1], 0, lower[3 * (nl - 1)], lower[3 * (nl - 1) + 1], 0);
    }

    // ---------- part builders ----------
    static string Tube(string name, char axis, double od, double id, double len, double[] rgb, string desc)
    {
        ModelDoc2 d = NewPart(); var sm = Sk(d, PlaneFor(axis));
        sm.CreateCircleByRadius(0, 0, 0, od / 2); if (id > 0) sm.CreateCircleByRadius(0, 0, 0, id / 2);
        EndSk(d); Ext(d, len, false, false);
        return Finish(d, name, rgb, desc);
    }
    // profile as (axial, radial) pairs, revolved about the X or Y axis through the origin
    static string Revolve(string name, char axis, double[,] ar, bool closeToAxis, double[] rgb, string desc)
    {
        ModelDoc2 d = NewPart(); var sm = Sk(d, "Front Plane");
        int n = ar.GetLength(0); var p = new double[n, 2];
        for (int i = 0; i < n; i++) { if (axis == 'X') { p[i, 0] = ar[i, 0]; p[i, 1] = ar[i, 1]; } else { p[i, 0] = ar[i, 1]; p[i, 1] = ar[i, 0]; } }
        Poly(sm, p, closeToAxis);
        if (axis == 'X') sm.CreateCenterLine(-0.01, 0, 0, 1, 0, 0); else sm.CreateCenterLine(0, -0.01, 0, 0, 1, 0);
        EndSk(d); Rev(d);
        return Finish(d, name, rgb, desc);
    }
    static string Motor(string name, char axis)
    {   // outrunner: base flange, stator neck, bell with top chamfer, prop adapter + shaft (EST proportions of a 41 mm class motor)
        double R = D("motorD") / 2, H = D("motorH");
        var prof = new double[,] { { 0, 0 }, { 0, R * 0.92 }, { 0.004, R * 0.92 }, { 0.004, R * 0.70 }, { 0.007, R * 0.70 }, { 0.007, R }, { H - 0.004, R },
            { H, R - 0.004 }, { H, 0.009 }, { H + 0.010, 0.009 }, { H + 0.010, 0.004 }, { H + 0.018, 0.004 }, { H + 0.018, 0 } };
        if (axis == 'X') for (int i = 0; i < prof.GetLength(0); i++) prof[i, 0] = -prof[i, 0];   // pusher faces aft (-X)
        string path = Revolve(name, axis, prof, true, MOTOR, axis == 'Y' ? "SunnySky X4110S lift motor envelope (EST proportions)" : "SunnySky X4110S pusher motor envelope (EST proportions)");
        return path;
    }
    static string Prop(string name, bool pusher, double dia)
    {   // two tapered blades with rounded tips + hub; lift props parked fore-aft (blade along X), pusher blade vertical
        ModelDoc2 d = NewPart(); double Rt = dia / 2, t = D("propT");
        var sm = Sk(d, pusher ? "Right Plane" : "Top Plane");
        foreach (double s in new[] { 1.0, -1.0 })
        {
            double r0 = 0.012, cr = dia * 0.045, cm = dia * 0.075, ct = dia * 0.035;   // chord: root, max (30 % R), tip — EST planform
            var pts = new double[,] { { s * r0, -cr / 2 }, { s * Rt * 0.30, -cm * 0.55 }, { s * Rt * 0.85, -ct * 0.55 }, { s * Rt * 0.98, -ct * 0.20 },
                { s * Rt, 0 }, { s * Rt * 0.98, ct * 0.30 }, { s * Rt * 0.85, ct * 0.45 }, { s * Rt * 0.30, cm * 0.45 }, { s * r0, cr / 2 } };
            if (pusher) for (int i = 0; i < pts.GetLength(0); i++) { double a = pts[i, 0]; pts[i, 0] = pts[i, 1]; pts[i, 1] = a; }
            Poly(sm, pts, true);
        }
        EndSk(d); Ext(d, t, false, true);
        sm = Sk(d, pusher ? "Right Plane" : "Top Plane"); sm.CreateCircleByRadius(0, 0, 0, 0.014); EndSk(d); Ext(d, t * 2.2, false, true);
        return Finish(d, name, PROP, pusher ? "Pusher prop APC 14 x 8.5EP envelope (planform EST)" : "Lift prop 17 in, shown parked fore-aft (notional: BLHeli_S ESCs cannot park props; planform EST)");
    }
    static string Saddle(string name, double lx, double h, double w, double tube, bool flatTop, string desc)
    {   // printed block around the boom: rounded section in the YZ plane, extruded along X, bore for the tube
        ModelDoc2 d = NewPart(); var sm = Sk(d, "Right Plane");
        RoundRect(sm, 0, flatTop ? 0.003 : 0, w, h, 0.006);
        sm.CreateCircleByRadius(0, 0, 0, tube / 2);
        EndSk(d); Ext(d, lx, false, true);
        return Finish(d, name, PRINT, desc);
    }
    static string SaddleTB(string name, double lx, double top, double bottom, double w, double tube, string desc)
    {   // printed block around the boom spanning y = bottom..top in the YZ plane (boom axis at the origin)
        ModelDoc2 d = NewPart(); var sm = Sk(d, "Right Plane");
        RoundRect(sm, 0, (top + bottom) / 2, w, top - bottom, 0.005);
        sm.CreateCircleByRadius(0, 0, 0, tube / 2);
        EndSk(d); Ext(d, lx, false, true);
        return Finish(d, name, PRINT, desc);
    }
    static string RoundBox(string name, double lx, double ly, double lz, double r, double[] rgb, string desc)
    {
        ModelDoc2 d = NewPart(); var sm = Sk(d, "Front Plane"); RoundRect(sm, 0, 0, lx, ly, r); EndSk(d); Ext(d, lz, false, true);
        return Finish(d, name, rgb, desc);
    }
    static string Leg()
    {   // gear leg (spar-tube offcut) with a printed dome foot
        ModelDoc2 d = NewPart(); double R = D("legOD") / 2;
        var sm = Sk(d, "Top Plane"); sm.CreateCircleByRadius(0, 0, 0, R); sm.CreateCircleByRadius(0, 0, 0, D("legID") / 2); EndSk(d);
        Ext(d, D("legLen"), false, false);
        sm = Sk(d, "Front Plane");
        double fr = 0.020, fh = 0.018;
        Poly(sm, new double[,] { { 0, 0 }, { fr * 0.85, 0 }, { fr, 0.004 }, { fr, fh * 0.55 }, { R * 1.15, fh }, { 0, fh } }, true);
        sm.CreateCenterLine(0, -0.01, 0, 0, 0.05, 0);
        EndSk(d); Rev(d);
        return Finish(d, "Landing_Leg", RUBBER, "Gear leg: 6061 1.000 x 0.058 in offcut + printed TPU/PETG foot (EST)");
    }
    static string Fuselage()
    {   // streamlined pod: elliptic nose, parallel mid-body, boat-tail to the pusher mount
        ModelDoc2 d = NewPart(); var sm = Sk(d, "Front Plane");
        double L = D("fusL"), R = D("fusR"), rt = D("tailR"), a = 0.22;
        var pts = new List<double>(); int N = 30;
        for (int i = 0; i <= N; i++)
        {
            double s = L * i / N, r;
            if (s < a) r = R * Math.Sqrt(Math.Max(0, 1 - Math.Pow((s - a) / a, 2)));
            else if (s < 0.50) r = R;
            else { double u = (s - 0.50) / (L - 0.50); r = rt + (R - rt) * (1 - u * u * (3 - 2 * u)); }   // smooth-step boat-tail
            pts.Add(-s); pts.Add(i == 0 ? 0 : r); pts.Add(0);
        }
        sm.CreateSpline2(pts.ToArray(), true);
        sm.CreateLine(-L, rt, 0, -L, 0, 0); sm.CreateLine(-L, 0, 0, 0, 0, 0);
        sm.CreateCenterLine(0.05, 0, 0, -L - 0.05, 0, 0);
        EndSk(d); Rev(d);
        return Finish(d, "Fuselage_Pod", POD, "Fuselage pod, 0.85 m x 0.16 m, foam + PETG frames + tape (EST)");
    }
    static string Canopy()
    {   // avionics hatch blister: half-ellipsoid, smooth spline profile revolved about X
        ModelDoc2 d = NewPart(); var sm = Sk(d, "Front Plane");
        int N = 20; double L = 0.26, r = 0.055; var pts = new List<double>();
        for (int i = 0; i <= N; i++) { double s = L * i / N; pts.Add(-s); pts.Add(r * Math.Sqrt(Math.Max(0, 1 - Math.Pow((2 * s - L) / L, 2)))); pts.Add(0); }
        sm.CreateSpline2(pts.ToArray(), true);
        sm.CreateLine(-L, 0, 0, 0, 0, 0);
        sm.CreateCenterLine(0.01, 0, 0, -L - 0.01, 0, 0);
        EndSk(d); Rev(d);
        return Finish(d, "Canopy", CANOPY, "Avionics hatch blister (EST)");
    }
    static string WingPanel(string name, bool left)
    {   // NACA 2413 panel: full chord inboard of the aileron, 0-72 % chord along the aileron span; spar socket bored 0.8 m
        ModelDoc2 d = NewPart(); double c = D("chord"), L = D("panelLen"), Lail = L * 0.55, Lin = L - Lail;
        var foil = Naca(0.02, 0.4, D("tc"), 40);
        var sm = Sk(d, "Front Plane"); SectionPart(sm, foil, c, 0, 0.72); EndSk(d);
        Ext(d, L, left, false);
        sm = Sk(d, "Front Plane"); SectionPart(sm, foil, c, 0.72, 1.0); EndSk(d);
        Ext(d, Lin, left, false);
        double v0 = Volume(d);
        sm = Sk(d, "Front Plane"); sm.CreateCircleByRadius(D("sparX"), D("sparY"), 0, D("sparOD") / 2); EndSk(d);
        Feature cut = Cut(d, D("sparLen"), left);
        if (cut == null || Volume(d) >= v0 - 1e-7)
        {
            if (cut != null) d.EditUndo2(1);
            d.ClearSelection2(true); ((Feature)d.FeatureByPositionReverse(0)).Select2(false, 0);
            cut = Cut(d, D("sparLen"), !left);
        }
        if (cut == null) throw new Exception("spar socket cut failed");
        return Finish(d, name, FOAM, (left ? "Left" : "Right") + " wing panel, NACA 2413 (EST), XPS core + tape skin, chord " + P["chord"] + " m");
    }
    static string Aileron(string name, bool left)
    {   // 74-100 % chord (2 % hinge gap), outer 55 % of the panel, 4 mm tip clearance
        ModelDoc2 d = NewPart(); double c = D("chord"), L = D("panelLen"), Lail = L * 0.55 - 0.004;
        var foil = Naca(0.02, 0.4, D("tc"), 40);
        var sm = Sk(d, "Front Plane"); SectionPart(sm, foil, c, 0.74, 1.0); EndSk(d);
        Ext(d, Lail, left, false);
        return Finish(d, name, SURF, (left ? "Left" : "Right") + " aileron, 26 % chord, 55 % of panel span");
    }
    static string Tailplane(string name, bool elevator)
    {   // NACA 0009, fixed part 0-68 % chord, elevator 70-100 %
        ModelDoc2 d = NewPart(); double c = D("hC"), span = D("hSpan");
        var foil = Naca(0, 0.4, 0.09, 30);
        var sm = Sk(d, "Front Plane");
        if (elevator) SectionPart(sm, foil, c, 0.70, 1.0); else SectionPart(sm, foil, c, 0, 0.68);
        EndSk(d); Ext(d, elevator ? D("elevSpan") : span, false, true);
        return Finish(d, name, elevator ? SURF : FOAM, elevator ? "Elevator, 30 % chord, NACA 0009" : "Horizontal stabiliser, NACA 0009 (EST), area from V_h = 0.35");
    }
    static string Fin()
    {   // NACA 0009 root and tip sections lofted: tip chord 65 % of root, LE swept back by 35 % of root chord (EST planform)
        ModelDoc2 d = NewPart(); double c = D("vC"), h = D("vH");
        var foil = Naca(0, 0.4, 0.09, 30);
        Func<double, double, double[]> sec = (scale, xoff) => {
            var pts = new double[foil.Count * 3];
            for (int i = 0; i < foil.Count; i++) { pts[3 * i] = -foil[i][0] * c * scale - xoff; pts[3 * i + 1] = foil[i][1] * c * scale; pts[3 * i + 2] = 0; }
            return pts; };
        var sm = Sk(d, "Top Plane"); var a = sec(1.0, 0); sm.CreateSpline2(a, true); sm.CreateLine(a[0], a[1], 0, a[a.Length - 3], a[a.Length - 2], 0);
        d.SketchManager.AddToDB = false; d.SketchManager.InsertSketch(true);
        string rootSk = ((Feature)d.FeatureByPositionReverse(0)).Name;
        d.ClearSelection2(true); d.Extension.SelectByID2("Top Plane", "PLANE", 0, 0, 0, false, 0, null, 0);
        RefPlane pl = (RefPlane)d.FeatureManager.InsertRefPlane(8, h, 0, 0, 0, 0);
        bool lofted = false;
        if (pl != null)
        {
            string plName = ((Feature)pl).Name;
            d.ClearSelection2(true); d.Extension.SelectByID2(plName, "PLANE", 0, 0, 0, false, 0, null, 0);
            d.SketchManager.InsertSketch(true); d.SketchManager.AddToDB = true;
            var b = sec(0.65, 0.35 * c); d.SketchManager.CreateSpline2(b, true); d.SketchManager.CreateLine(b[0], b[1], 0, b[b.Length - 3], b[b.Length - 2], 0);
            d.SketchManager.AddToDB = false; d.SketchManager.InsertSketch(true);
            string tipSk = ((Feature)d.FeatureByPositionReverse(0)).Name;
            d.ClearSelection2(true);
            d.Extension.SelectByID2(rootSk, "SKETCH", 0, 0, 0, true, 1, null, 0);
            d.Extension.SelectByID2(tipSk, "SKETCH", 0, 0, 0, true, 1, null, 0);
            Feature loft = d.FeatureManager.InsertProtrusionBlend2(false, true, false, 1, 0, 0, 1, 1, true, true, false, 0, 0, 0, true, true, true, 0);
            lofted = loft != null;
        }
        if (!lofted)
        {
            Log("  fin loft failed: plain extrude of the root section used (no taper)");
            d.ClearSelection2(true); d.Extension.SelectByID2(rootSk, "SKETCH", 0, 0, 0, false, 0, null, 0);
            Ext(d, h, false, false);
        }
        else Log("  fin lofted (65 % tip chord, swept LE)");
        return Finish(d, "Fin", FOAM, lofted ? "Vertical fin, NACA 0009, lofted taper 65 % tip chord" : "Vertical fin, NACA 0009, constant chord");
    }
}
