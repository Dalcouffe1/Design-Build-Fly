// Builds the SOLIDWORKS drawing set for the concept model (one .SLDDRW, five ANSI B sheets):
//   1 General arrangement (GA): front elevation, plan, side elevation, isometric; overall dimensions
//   2 Parts list: ballooned isometric and bill of materials
//   3 Wing panel and wing saddle
//   4 Printed parts: motor mount, boom clamps, gear clamp
//   5 Tube cut list and landing leg
// The parts are built by API without sketch dimensions, so every dimension is a driven dimension between
// sketch points placed at known model coordinates (cad/params.txt): the values are the model geometry.
// Exports DBF-DRW-001.SLDDRW, DBF-DRW-001.PDF (all sheets) and a PNG per sheet.
// Usage: Drawings.exe <cad/model dir> <cad/drawings dir> <cad/params.txt>
using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

class Drawings
{
    static SldWorks sw; static MathUtility mu;
    static ModelDoc2 Doc; static DrawingDoc Dwg;
    static string ModelDir, OutDir, Fmt, Tpl;
    static Dictionary<string, string> P = new Dictionary<string, string>();
    static CultureInfo IC = CultureInfo.InvariantCulture;
    static double D(string k) { return double.Parse(P[k], IC); }
    static void Log(string s) { Console.WriteLine(s); }

    static int Main(string[] a)
    {
        ModelDir = Path.GetFullPath(a[0]); OutDir = Path.GetFullPath(a[1]); Directory.CreateDirectory(OutDir);
        foreach (string raw in File.ReadAllLines(a[2])) { int eq = raw.IndexOf('='); if (eq > 0 && !raw.StartsWith("comp=")) P[raw.Substring(0, eq)] = raw.Substring(eq + 1).Trim(); }
        sw = (SldWorks)Activator.CreateInstance(Type.GetTypeFromProgID("SldWorks.Application"));
        sw.Visible = true; mu = (MathUtility)sw.GetMathUtility();
        sw.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swInputDimValOnCreate, false);   // no Modify dialog when adding dimensions
        CloseUnsavedDrawings();
        sw.SetUserPreferenceIntegerValue((int)swUserPreferenceIntegerValue_e.swTiffScreenOrPrintCapture, 0);   // screen capture (print capture via the API returns 11x8 px images)
        Tpl = sw.GetUserPreferenceStringValue((int)swUserPreferenceStringValue_e.swDefaultTemplateDrawing);
        Fmt = @"C:\ProgramData\SOLIDWORKS\SOLIDWORKS 2025\lang\english\sheetformat\b - landscape.slddrt";
        foreach (string f in Directory.GetFiles(OutDir)) if (Path.GetFileName(f).StartsWith("probe")) File.Delete(f);

        Doc = (ModelDoc2)sw.NewDocument(Tpl, (int)swDwgPaperSizes_e.swDwgPaperBsize, 0, 0);
        Dwg = (DrawingDoc)Doc;
        Doc.Extension.SetUserPreferenceInteger((int)swUserPreferenceIntegerValue_e.swUnitSystem, 0, (int)swUnitSystem_e.swUnitSystem_MMGS);
        Doc.Extension.SetUserPreferenceInteger((int)swUserPreferenceIntegerValue_e.swUnitsLinearDecimalPlaces, 0, 0);
        SetProps();
        var titles = new List<string[]>();
        Sheet1_GA(); titles.Add(new[] { "LIFT + CRUISE VTOL", "GENERAL ARRANGEMENT" });
        Sheet2_PartsList(); titles.Add(new[] { "LIFT + CRUISE VTOL", "PARTS LIST" });
        Sheet3_Wing(); titles.Add(new[] { "WING PANEL AND", "WING SADDLE" });
        Sheet4_Printed(); titles.Add(new[] { "PRINTED PARTS", "MOUNTS AND CLAMPS" });
        Sheet5_Tubes(); titles.Add(new[] { "TUBE CUT LIST", "AND LANDING LEG" });

        string[] names = (string[])Dwg.GetSheetNames();
        int e = 0, w = 0;
        double[] scales = { 15, 17, 10, 1, 2 };
        for (int i = 0; i < names.Length; i++)
        {
            Dwg.ActivateSheet(names[i]);
            Sheet sh = (Sheet)Dwg.GetCurrentSheet();
            sh.SetProperties2((int)swDwgPaperSizes_e.swDwgPaperBsize, (int)swDwgTemplates_e.swDwgTemplateCustom, 1, scales[i], false, 0, 0, true);
            FillFormat(titles[i][0], titles[i][1]);
        }
        string path = Path.Combine(OutDir, "DBF-DRW-001.SLDDRW");
        Doc.ForceRebuild3(false);
        if (!Doc.Extension.SaveAs(path, 0, 1, null, ref e, ref w)) Log("SAVE FAILED " + e);
        for (int i = 0; i < names.Length; i++)
        {
            Dwg.ActivateSheet(names[i]); Doc.ViewZoomtofit2();
            string png = Path.Combine(OutDir, string.Format("DBF-DRW-001_sheet{0}.png", i + 1));
            Doc.Extension.SaveAs(png, 0, 1, null, ref e, ref w);
            Log("png " + Path.GetFileName(png) + " " + (File.Exists(png) ? new FileInfo(png).Length.ToString() : "missing"));
        }
        ExportPdfData pdf = (ExportPdfData)sw.GetExportFileData((int)swExportDataFileType_e.swExportPdfData);
        pdf.SetSheets((int)swExportDataSheetsToExport_e.swExportData_ExportAllSheets, null);
        Doc.Extension.SaveAs(Path.Combine(OutDir, "DBF-DRW-001.PDF"), 0, 1, pdf, ref e, ref w);
        Dwg.ActivateSheet(names[0]);
        Doc.Extension.SaveAs(path, 0, 1, null, ref e, ref w);
        Log("saved " + path);
        sw.CloseDoc(Doc.GetTitle());
        return 0;
    }

    static void CloseUnsavedDrawings()
    {
        object[] docs = (object[])sw.GetDocuments();
        if (docs != null) foreach (object o in docs) { ModelDoc2 m = (ModelDoc2)o; if (m.GetType() == (int)swDocumentTypes_e.swDocDRAWING && (m.GetPathName() == "" || m.GetPathName().StartsWith(OutDir, StringComparison.OrdinalIgnoreCase))) sw.CloseDoc(m.GetTitle()); }
    }
    static void SetProps()
    {
        CustomPropertyManager c = Doc.Extension.get_CustomPropertyManager("");
        Action<string, string> put = (k, v) => c.Add3(k, (int)swCustomInfoType_e.swCustomInfoText, v, (int)swCustomPropertyAddOption_e.swCustomPropertyReplaceValue);
        put("DrawnBy", "DBF"); put("DrawnDate", "2026-10-18"); put("Revision", "0.3.1"); put("COMPANYNAME", "DESIGN-BUILD-FLY");
    }
    // title block text, units and tolerance block, ownership note: edited in each sheet's format
    static void FillFormat(string t1, string t2)
    {
        Dwg.EditTemplate();
        View sh = (View)Dwg.GetFirstView();
        object[] notes = (object[])sh.GetNotes();
        if (notes != null) foreach (object o in notes)
        {
            Note n = (Note)o; string lt = n.PropertyLinkedText ?? "";
            if (lt.Contains("$PRPSHEET:\"Description\"")) { n.SetText(t1 + "\n" + t2); Size(n, 0.0036); }
            else if (lt.Contains("SW-File Name")) Size(n, 0.0042);
            else if (lt.Contains("$PRP:\"Revision\"")) Size(n, 0.0030);
            else if (lt.Contains("COMPANYNAME")) { Size(n, 0.0040); Annotation an = (Annotation)n.GetAnnotation(); TextFormat tf = (TextFormat)an.GetTextFormat(0); tf.TypeFaceName = "Century Gothic"; an.SetTextFormat(0, false, tf); }
            else if (lt.StartsWith("DIMENSIONS ARE IN INCHES")) n.SetText("DIMENSIONS ARE IN MILLIMETRES\nGENERAL TOLERANCES (EST):\nLINEAR  X ±1   X.X ±0.5\nANGULAR ±1°\nPRINTED PARTS ±0.3");
            else if (lt.StartsWith("THE INFORMATION CONTAINED")) n.SetText("STUDENT CAPSTONE PROJECT, UC BERKELEY.\nCONCEPT GEOMETRY FOR DESIGN REVIEW,\nGENERATED FROM analysis/results.json\nAND cad/params.txt (v0.3.1).\nNOT RELEASED FOR MANUFACTURE.");
            else if (lt == "PROPRIETARY AND CONFIDENTIAL") n.SetText("DESIGN-BUILD-FLY");
            else if (lt.Contains("$PRPSHEET:\"Material\"") || lt.Contains("$PRPSHEET:\"Finish\"")) n.SetText("SEE PARTS LIST");
        }
        Dwg.EditSheet();
    }

    static void Size(Note n, double h) { Annotation an = (Annotation)n.GetAnnotation(); TextFormat tf = (TextFormat)an.GetTextFormat(0); tf.CharHeight = h; an.SetTextFormat(0, false, tf); }
    // ---------- geometry helpers ----------
    static double[] ToSheet(View v, double x, double y, double z)
    {
        MathPoint p = (MathPoint)mu.CreatePoint(new double[] { x, y, z });
        MathPoint q = (MathPoint)p.MultiplyTransform((MathTransform)v.ModelToViewTransform);
        double[] r = (double[])q.ArrayData; return new[] { r[0], r[1] };
    }
    static double[] SheetToViewSketch(View v, double sx, double sy)
    {
        MathPoint p = (MathPoint)mu.CreatePoint(new double[] { sx, sy, 0 });
        MathPoint q = (MathPoint)p.MultiplyTransform((MathTransform)((Sketch)v.GetSketch()).ModelToSketchTransform);
        double[] r = (double[])q.ArrayData; return new[] { r[0], r[1] };
    }
    // driven dimension between two model points; side = 'T','B','L','R' places the text beyond the view outline
    static void Dim(View v, double[] m1, double[] m2, char side, double gap, string prefix = null, int decimals = 0)
    {
        double[] s1 = ToSheet(v, m1[0], m1[1], m1[2]), s2 = ToSheet(v, m2[0], m2[1], m2[2]);
        double[] o = (double[])v.GetOutline();
        double tx, ty;
        if (side == 'T') { tx = (s1[0] + s2[0]) / 2; ty = o[3] + gap; }
        else if (side == 'B') { tx = (s1[0] + s2[0]) / 2; ty = o[1] - gap; }
        else if (side == 'L') { tx = o[0] - gap; ty = (s1[1] + s2[1]) / 2; }
        else { tx = o[2] + gap; ty = (s1[1] + s2[1]) / 2; }
        Doc.ClearSelection2(true); Doc.Extension.SelectByID2(v.Name, "DRAWINGVIEW", 0, 0, 0, false, 0, null, 0);
        Dwg.ActivateView(v.Name);
        double[] b1 = SheetToViewSketch(v, s1[0], s1[1]), b2 = SheetToViewSketch(v, s2[0], s2[1]);
        Doc.SketchManager.AddToDB = true;
        SketchPoint p1 = Doc.SketchManager.CreatePoint(b1[0], b1[1], 0), p2 = Doc.SketchManager.CreatePoint(b2[0], b2[1], 0);
        Doc.SketchManager.AddToDB = false;
        if (p1 == null || p2 == null) { Log("  dim points failed"); return; }
        Doc.ClearSelection2(true); p1.Select4(false, null); p2.Select4(true, null);
        DisplayDimension dd = (side == 'T' || side == 'B') ? (DisplayDimension)Doc.AddHorizontalDimension2(tx, ty, 0) : (DisplayDimension)Doc.AddVerticalDimension2(tx, ty, 0);   // text position is in sheet coordinates
        if (dd == null) { Log("  dimension failed"); return; }
        if (decimals > 0) dd.SetPrecision3(decimals, decimals, decimals, decimals);
        if (prefix != null) dd.SetText((int)swDimensionTextParts_e.swDimensionTextPrefix, prefix);
        Doc.ClearSelection2(true);
        Log(string.Format(IC, "  dim {0} = {1:0.###} mm", v.Name, ((Dimension)dd.GetDimension2(0)).SystemValue * 1000));
    }
    static View ModelView(string path, string orient, double x, double y, double scaleDen, bool hidden = false)
    {
        View v = Dwg.CreateDrawViewFromModelView3(path, orient, x, y, 0);
        if (v == null) { Log("VIEW FAILED " + orient + " " + path); return null; }
        v.ScaleDecimal = 1.0 / scaleDen;
        if (hidden) v.SetDisplayMode3(false, (int)swDisplayMode_e.swHIDDEN_GREYED, false, true);
        return v;
    }
    static void Label(double x, double y, string text, double h = 0.0035)
    {
        Doc.ClearSelection2(true);
        Note n = (Note)Doc.InsertNote(text);
        if (n == null) { Log("  note failed"); return; }
        Annotation an = (Annotation)n.GetAnnotation(); an.SetPosition2(x, y, 0);
        TextFormat tf = (TextFormat)an.GetTextFormat(0); tf.CharHeight = h; an.SetTextFormat(0, false, tf);
    }
    static void LabelBelow(View v, string text, double gap = 0.006) { double[] o = (double[])v.GetOutline(); Label(o[0], o[1] - gap, text, 0.003); }
    static void NewSheet(string name, double scaleDen)
    {
        Dwg.NewSheet4(name, (int)swDwgPaperSizes_e.swDwgPaperBsize, (int)swDwgTemplates_e.swDwgTemplateCustom, 1, scaleDen, false, Fmt, 0, 0, "Default", 0, 0, 0, 0, 0, 0);
        Dwg.ActivateSheet(name);
    }
    static string Part(string n) { return Path.Combine(ModelDir, n + ".SLDPRT"); }
    static void Open(string path, int type) { int e = 0, w = 0; sw.OpenDoc6(path, type, (int)swOpenDocOptions_e.swOpenDocOptions_Silent, "", ref e, ref w); sw.ActivateDoc3(Doc.GetTitle(), false, 0, ref e); }

    // ---------- sheet 1: general arrangement ----------
    static void Sheet1_GA()
    {
        Sheet s = (Sheet)Dwg.GetCurrentSheet(); s.SetName("GA");
        Dwg.SetupSheet5("GA", (int)swDwgPaperSizes_e.swDwgPaperBsize, (int)swDwgTemplates_e.swDwgTemplateCustom, 1, 15, false, Fmt, 0, 0, "Default", true);
        string asm = Path.Combine(ModelDir, "VTOL_Concept.SLDASM");
        Open(asm, (int)swDocumentTypes_e.swDocASSEMBLY);
        View front = ModelView(asm, "*Right", 0.115, 0.092, 15);
        Doc.ClearSelection2(true); Doc.Extension.SelectByID2(front.Name, "DRAWINGVIEW", 0, 0, 0, false, 0, null, 0);
        View plan = Dwg.CreateUnfoldedViewAt3(0.115, 0.188, 0, false);
        Doc.ClearSelection2(true); Doc.Extension.SelectByID2(front.Name, "DRAWINGVIEW", 0, 0, 0, false, 0, null, 0);
        View side = Dwg.CreateUnfoldedViewAt3(0.300, 0.092, 0, false);
        View isoV = ModelView(asm, "*Isometric", 0.335, 0.205, 22);
        Doc.EditRebuild3();
        double half = D("panelLen") + D("fusR"), yB = D("yB"), rB = D("boomOD") / 2, yIn = 0.4337, xF = 0.37091707, xR = -0.50457515, x0 = -0.99047515;
        double yG = yB - rB - D("legLen") + 0.004, finTop = yB + rB + D("vH"), finX = xR - D("liftProp") / 2 - 0.05;
        double pushX = 0.45 - D("fusL") - D("pushMotorL") - 0.006, yProp = yB + 0.003 + 0.018 + D("motorH") + 0.006;
        Dim(plan, new[] { 0.0, 0, half }, new[] { 0.0, 0, -half }, 'T', 0.010);
        Dim(plan, new[] { xF, yB, yIn }, new[] { xF, yB, -yIn }, 'T', 0.002);
        Dim(plan, new[] { 0.45, -0.03, 0.0 }, new[] { x0, yB, yIn }, 'R', 0.012);
        Dim(plan, new[] { xF, yB, -yIn }, new[] { xR, yB, -yIn }, 'L', 0.010);
        Dim(front, new[] { finX, yG, -yIn }, new[] { finX, finTop, -yIn }, 'L', 0.010);
        Dim(front, new[] { pushX, -0.03 - D("pushProp") / 2, 0.0 }, new[] { pushX, -0.03 + D("pushProp") / 2, 0.0 }, 'R', 0.010);
        Dim(side, new[] { xF - D("liftProp") / 2, yProp, yIn }, new[] { xF + D("liftProp") / 2, yProp, yIn }, 'T', 0.006);
        Dim(side, new[] { pushX, -0.03 - D("pushProp") / 2, 0.0 }, new[] { pushX, yG, 0.0 }, 'R', 0.008);
        Label(0.022, 0.040, "NOTES\n1. DIMENSIONS IN MM. VIEWS 1:15, THIRD-ANGLE; ISOMETRIC 1:22.\n2. PROPS SHOWN PARKED FORE-AFT (NOTIONAL): THE ESCs\n    CANNOT PARK THEM.\n3. SIZES ARE THE SIZED CONCEPT (MTOW 6.18 kg), NOT A\n    BUILT ARTICLE.", 0.0026);
        LabelBelow(front, "FRONT ELEVATION"); LabelBelow(plan, "PLAN"); LabelBelow(side, "SIDE ELEVATION"); LabelBelow(isoV, "ISOMETRIC");
    }

    // ---------- sheet 2: parts list ----------
    static void Sheet2_PartsList()
    {
        NewSheet("PARTS", 12);
        string asm = Path.Combine(ModelDir, "VTOL_Concept.SLDASM");
        View iso = ModelView(asm, "*Isometric", 0.125, 0.165, 17);
        Doc.ClearSelection2(true); Doc.Extension.SelectByID2(iso.Name, "DRAWINGVIEW", 0, 0, 0, false, 0, null, 0);
        try
        {
            BomTableAnnotation bom = iso.InsertBomTable4(false, 0.262, 0.262, (int)swBOMConfigurationAnchorType_e.swBOMConfigurationAnchor_TopLeft,
                (int)swBomType_e.swBomType_PartsOnly, "Default", @"C:\Program Files\SOLIDWORKS Corp\SOLIDWORKS\lang\english\bom-standard.sldbomtbt", false, 0, false);
            Log("bom " + (bom != null));
            if (bom != null) { TableAnnotation ta = (TableAnnotation)bom; ta.DeleteColumn2(2, false);
                TextFormat tf = (TextFormat)ta.GetTextFormat(); tf.CharHeight = 0.0024; ta.SetTextFormat(false, tf);
                ta.SetColumnWidth(0, 0.014, 0); ta.SetColumnWidth(1, 0.070, 0); ta.SetColumnWidth(2, 0.012, 0);
                for (int i = 0; i < ta.RowCount; i++) ta.SetRowHeight(i, 0.0045, 0); }
        }
        catch (Exception ex) { Log("bom error " + ex.Message); }
        try
        {
            Doc.ClearSelection2(true); Doc.Extension.SelectByID2(iso.Name, "DRAWINGVIEW", 0, 0, 0, false, 0, null, 0);
            AutoBalloonOptions ab = (AutoBalloonOptions)Dwg.CreateAutoBalloonOptions();
            ab.Layout = (int)swBalloonLayoutType_e.swDetailingBalloonLayout_Square; ab.IgnoreMultiple = true; ab.ReverseDirection = false;
            ab.Style = (int)swBalloonStyle_e.swBS_Circular; ab.Size = (int)swBalloonFit_e.swBF_2Chars;
            object balloons = Dwg.AutoBalloon5(ab);
            Log("balloons " + (balloons != null ? ((object[])balloons).Length.ToString() : "none"));
        }
        catch (Exception ex) { Log("balloon error " + ex.Message); }
        Label(0.022, 0.040, "NOTES\n1. ITEM NUMBERS MATCH THE CAD ASSEMBLY.\n2. PURCHASED ITEMS, PRICES AND VENDORS: docs/purchasing/C0_BOM.csv.", 0.0026);
    }

    // ---------- sheet 3: wing panel and saddle ----------
    static double NacaY(double x, bool upper)
    {
        double m = 0.02, p = 0.4, t = D("tc");
        double yt = 5 * t * (0.2969 * Math.Sqrt(x) - 0.1260 * x - 0.3516 * x * x + 0.2843 * x * x * x - 0.1015 * x * x * x * x);
        double yc = x < p ? m / (p * p) * (2 * p * x - x * x) : m / ((1 - p) * (1 - p)) * ((1 - 2 * p) + 2 * p * x - x * x);
        return (upper ? yc + yt : yc - yt) * D("chord");
    }
    static void Sheet3_Wing()
    {
        NewSheet("WING", 10);
        double c = D("chord"), L = D("panelLen"), Lin = L * 0.45, sx = D("sparX"), sy = D("sparY"), r = D("sparOD") / 2, w = 2 * D("fusR");
        string wp = Part("Wing_Panel_R"); Open(wp, (int)swDocumentTypes_e.swDocPART);
        View plan = ModelView(wp, "*Top", 0.070, 0.165, 10, true);
        View sec = ModelView(wp, "*Front", 0.200, 0.225, 2, true);
        Doc.EditRebuild3();
        Dim(plan, new[] { 0.0, 0, 0 }, new[] { 0.0, 0, L }, 'L', 0.010);
        Dim(plan, new[] { 0.0, 0, 0 }, new[] { -c, 0, 0 }, 'T', 0.006);
        Dim(plan, new[] { -c, 0, Lin }, new[] { -c, 0, L }, 'R', 0.010);
        Dim(plan, new[] { 0.0, 0, 0 }, new[] { sx, sy, 0 }, 'T', 0.014);
        Dim(plan, new[] { sx, sy, 0 }, new[] { sx, sy, D("sparLen") }, 'R', 0.020);
        Dim(sec, new[] { 0.0, NacaY(0, false), 0 }, new[] { -c, 0, 0 }, 'B', 0.008);
        Dim(sec, new[] { -0.30 * c, NacaY(0.30, false), 0 }, new[] { -0.30 * c, NacaY(0.30, true), 0 }, 'R', 0.010);
        Dim(sec, new[] { sx - r, sy, 0 }, new[] { sx + r, sy, 0 }, 'T', 0.006, "<MOD-DIAM>", 1);
        string sd = Part("Wing_Saddle"); Open(sd, (int)swDocumentTypes_e.swDocPART);
        View sp = ModelView(sd, "*Top", 0.215, 0.120, 4, true);
        View ss = ModelView(sd, "*Front", 0.345, 0.225, 2, true);
        Doc.EditRebuild3();
        Dim(sp, new[] { 0.0, 0, -w / 2 }, new[] { 0.0, 0, w / 2 }, 'L', 0.008);
        Dim(sp, new[] { 0.0, 0, -w / 2 }, new[] { -c, 0, -w / 2 }, 'B', 0.006);
        double rj = D("joinOD") / 2 + 0.000115;
        Dim(ss, new[] { sx - rj, sy, 0 }, new[] { sx + rj, sy, 0 }, 'T', 0.006, "<MOD-DIAM>", 1);
        Dim(ss, new[] { 0.0, sy, 0 }, new[] { sx, sy, 0 }, 'B', 0.008);
        LabelBelow(plan, "WING PANEL, RIGHT (LEFT IS MIRROR)  1:10"); LabelBelow(sec, "ROOT SECTION  1:2", 0.018);
        LabelBelow(sp, "WING SADDLE  1:4", 0.018); LabelBelow(ss, "SADDLE SECTION  1:2", 0.018);
        Label(0.022, 0.040, "NOTES\n1. NACA 2415 (t/c FROM SIZING). XPS CORE, PACKING-TAPE SKIN.\n2. AILERON CUT-OUT: AFT 28 % OF CHORD, OUTER 55 % OF SPAN.\n    AILERON 26 % CHORD WITH A 2 % HINGE GAP.\n3. SPAR SOCKET: 1.000 x 0.058 in 6061-T6, BONDED 800 mm\n    INTO THE ROOT AT 30 % CHORD.\n4. SADDLE: XPS BLOCK, PRINTED PETG ROOT PLATES; BORE FOR\n    THE 0.875 in JOINER (0.23 mm CLEARANCE).", 0.0026);
    }

    // ---------- sheet 4: printed parts ----------
    static void Block(string name, double cx, double cy, double lx, double top, double bottom, double width, string title)
    {
        string p = Part(name); Open(p, (int)swDocumentTypes_e.swDocPART);
        View sec = ModelView(p, "*Right", cx, cy, 1);
        View sid = ModelView(p, "*Front", cx + 0.075, cy, 1);
        Doc.EditRebuild3();
        double r = D("boomOD") / 2;
        Dim(sec, new[] { 0.0, bottom, -width / 2 }, new[] { 0.0, bottom, width / 2 }, 'B', 0.006);
        Dim(sec, new[] { 0.0, bottom, width / 2 }, new[] { 0.0, top, width / 2 }, 'L', 0.008);
        Dim(sec, new[] { 0.0, 0, -r }, new[] { 0.0, 0, r }, 'T', 0.005, "<MOD-DIAM>", 1);
        Dim(sid, new[] { -lx / 2, top, 0 }, new[] { lx / 2, top, 0 }, 'T', 0.005);
        Label(cx - 0.04, cy + 0.052, title, 0.003);
    }
    static void Sheet4_Printed()
    {
        NewSheet("PRINTED", 1);
        double bOD = D("boomOD");
        Block("Motor_Mount", 0.070, 0.205, 0.070, 0.003 + 0.018, 0.003 - 0.018, 0.046, "MOTOR MOUNT (PA6-CF), QTY 4  1:1");
        Block("Boom_Clamp_Front", 0.270, 0.205, 0.034, D("clampTopF"), -(bOD / 2 + 0.004), 0.040, "BOOM CLAMP, FRONT (PETG), QTY 2  1:1");
        Block("Boom_Clamp_Rear", 0.070, 0.108, 0.034, D("clampTopR"), -(bOD / 2 + 0.004), 0.040, "BOOM CLAMP, REAR (PETG), QTY 2  1:1");
        Block("Gear_Clamp", 0.270, 0.108, 0.034, bOD / 2 + 0.004, -(bOD / 2 + 0.016), 0.040, "GEAR CLAMP (PETG), QTY 4  1:1");
        Label(0.022, 0.040, "NOTES\n1. BORE 25.4 mm FOR THE 1.000 in BOOM: PRINT 0.2 mm UNDER,\n    REAM TO FIT.\n2. MOTOR MOUNT IN PA6-CF (HDT 173 °C): THE MOTOR CAN REACH\n    ABOUT 73 °C IN A 90 s HOVER (ESTIMATE).\n3. CLAMP TOPS MEET THE WING LOWER SURFACE. M3 HEAT-SET\n    INSERTS FOR THE CLAMP SCREWS.", 0.0026);
    }

    // ---------- sheet 5: tube cut list and landing leg ----------
    static void Sheet5_Tubes()
    {
        NewSheet("TUBES", 2);
        string leg = Part("Landing_Leg"); Open(leg, (int)swDocumentTypes_e.swDocPART);
        View lv = ModelView(leg, "*Front", 0.300, 0.175, 2, true);
        Doc.EditRebuild3();
        double R = D("legOD") / 2;
        Dim(lv, new[] { R, 0.0, 0 }, new[] { R, D("legLen"), 0 }, 'R', 0.010);
        Dim(lv, new[] { -R, D("legLen"), 0 }, new[] { R, D("legLen"), 0 }, 'T', 0.006, "<MOD-DIAM>", 1);
        LabelBelow(lv, "LANDING LEG  1:2");
        string mm = "{0:0}";
        var rows = new List<string[]> {
            new[] { "ITEM", "PART", "STOCK (6061-T6)", "CUT LENGTH mm", "QTY" },
            new[] { "1", "Wing spar socket", "1.000 x 0.058 in", string.Format(IC, mm, D("sparLen") * 1000), "2" },
            new[] { "2", "Wing joiner", "0.875 x 0.058 in", string.Format(IC, mm, D("joinLen") * 1000), "1" },
            new[] { "3", "Front boom", "1.000 x 0.035 in", string.Format(IC, mm, D("segF") * 1000), "2" },
            new[] { "4", "Rear boom stub", "1.000 x 0.035 in", string.Format(IC, mm, D("segR") * 1000), "2" },
            new[] { "5", "Landing leg", "1.000 x 0.058 in", string.Format(IC, mm, D("legLen") * 1000), "4" } };
        TableAnnotation t = (TableAnnotation)Dwg.InsertTableAnnotation2(false, 0.022, 0.250, (int)swBOMConfigurationAnchorType_e.swBOMConfigurationAnchor_TopLeft, "", rows.Count, 5);
        if (t != null)
        {
            for (int i = 0; i < rows.Count; i++) for (int j = 0; j < 5; j++) t.set_Text(i, j, rows[i][j]);
            t.SetColumnWidth(0, 0.014, 0); t.SetColumnWidth(1, 0.050, 0); t.SetColumnWidth(2, 0.042, 0); t.SetColumnWidth(3, 0.036, 0); t.SetColumnWidth(4, 0.014, 0);
            Log("cut list table " + rows.Count + " rows");
        }
        else Log("table failed");
        Label(0.022, 0.075, "NOTES\n1. LEGS ARE CUT FROM THE SPAR-SOCKET TUBE OFFCUT:\n    2 x 800 + 4 x 205 = 2420 mm OF A 96 in (2438 mm) LENGTH.\n2. DEBURR ALL CUT ENDS. JOINER 0.23 mm DIAMETRAL\n    CLEARANCE IN THE SOCKETS.\n3. LENGTHS FROM cad/params.txt (v0.3.1).", 0.0026);
    }
}
