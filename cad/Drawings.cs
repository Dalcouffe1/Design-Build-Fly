// Builds the SOLIDWORKS drawing set DBF-DRW-001 rev B for the model (one .SLDDRW, nine ANSI B sheets):
//   1 General arrangement (GA): front elevation, plan, side elevation, isometric; overall dimensions; revision table
//   2 Exploded assembly with balloons (VTOL_Exploded.SLDASM, offsets from cad/params.mjs)
//   3 Parts list (bill of materials of the assembly, every fastener included)
//   4 Wing panel, wing saddle and the M5 wing-bolt pillar (fits FT01/FT05/FT06)
//   5 Motor mount (upper half + cap) and splice plug: hole callouts, counterbores, insert holes, floor thickness, fits
//   6 Boom clamps, gear clamp and tail clamp (split halves) with hole callouts
//   7 Tube cut list with ISO 2768-m tolerances, cross-hole positions and the landing leg
//   8 Fastener schedule (every fastener: ID, standard, size, qty, joints, torque, threadlocker) and assembly notes
//   9 Small printed parts: fin shoe, spar collar, rear hardpoint, pusher cup, servo tray, root tongue
// The parts are built by API without sketch dimensions, so every dimension is a driven dimension between sketch points
// placed at known model coordinates (cad/params.txt): the values are the model geometry. Tolerances are set on the
// dimensions from docs/research/hardware-spec.md (cad/hardware.json fits and general_tolerances).
// Exports DBF-DRW-001.SLDDRW, DBF-DRW-001.PDF (all sheets) and a PNG per sheet.
// Usage: Drawings.exe <cad/model dir> <cad/drawings dir> <cad/params.txt>
using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
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
    const string REV = "B", DATE = "2026-10-21";

    static int Main(string[] a)
    {
        ModelDir = Path.GetFullPath(a[0]); OutDir = Path.GetFullPath(a[1]); Directory.CreateDirectory(OutDir);
        foreach (string raw in File.ReadAllLines(a[2])) { int eq = raw.IndexOf('='); if (eq > 0 && !raw.StartsWith("comp=") && !raw.StartsWith("xcomp=")) P[raw.Substring(0, eq)] = raw.Substring(eq + 1).Trim(); }
        sw = (SldWorks)Activator.CreateInstance(Type.GetTypeFromProgID("SldWorks.Application"));
        sw.Visible = true; sw.UserControl = true; mu = (MathUtility)sw.GetMathUtility();
        sw.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swInputDimValOnCreate, false);   // no Modify dialog when adding dimensions
        CloseUnsavedDrawings();
        sw.SetUserPreferenceIntegerValue((int)swUserPreferenceIntegerValue_e.swTiffScreenOrPrintCapture, 0);   // screen capture (print capture via the API returns 11x8 px images)
        Tpl = sw.GetUserPreferenceStringValue((int)swUserPreferenceStringValue_e.swDefaultTemplateDrawing);
        Fmt = @"C:\ProgramData\SOLIDWORKS\SOLIDWORKS 2025\lang\english\sheetformat\b - landscape.slddrt";
        foreach (string f in Directory.GetFiles(OutDir)) if (Path.GetFileName(f).StartsWith("probe")) File.Delete(f);

        Doc = (ModelDoc2)sw.NewDocument(Tpl, (int)swDwgPaperSizes_e.swDwgPaperBsize, 0, 0);
        Dwg = (DrawingDoc)Doc;
        Doc.Extension.SetUserPreferenceInteger((int)swUserPreferenceIntegerValue_e.swUnitSystem, 0, (int)swUnitSystem_e.swUnitSystem_MMGS);
        Doc.Extension.SetUserPreferenceInteger((int)swUserPreferenceIntegerValue_e.swUnitsLinearDecimalPlaces, 0, 1);
        SetProps();
        var titles = new List<string[]>();
        Sheet1_GA(); titles.Add(new[] { "LIFT + CRUISE VTOL", "GENERAL ARRANGEMENT" });
        Sheet2_Exploded(); titles.Add(new[] { "EXPLODED ASSEMBLY", "WITH BALLOONS" });
        Sheet3_PartsList(); titles.Add(new[] { "LIFT + CRUISE VTOL", "PARTS LIST" });
        Balloons();   // after the parts list exists, so the balloons carry its item numbers
        Sheet4_Wing(); titles.Add(new[] { "WING PANEL, SADDLE", "AND WING BOLT" });
        Sheet5_MotorMount(); titles.Add(new[] { "MOTOR MOUNT", "AND BOOM SPLICE" });
        Sheet6_Clamps(); titles.Add(new[] { "BOOM, GEAR AND", "TAIL CLAMPS" });
        Sheet7_Tubes(); titles.Add(new[] { "TUBE CUT LIST", "AND LANDING LEG" });
        Sheet8_Fasteners(); titles.Add(new[] { "FASTENER SCHEDULE", "AND TORQUES" });
        Sheet9_Printed(); titles.Add(new[] { "PRINTED PARTS", "HARDPOINTS AND TRAYS" });

        string[] names = (string[])Dwg.GetSheetNames();
        int e = 0, w = 0;
        double[] scales = { 15, 12, 40, 10, 1, 1, 2, 1, 1 };
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
        put("DrawnBy", "DBF"); put("DrawnDate", DATE); put("Revision", REV); put("COMPANYNAME", "DESIGN-BUILD-FLY"); put("CheckedBy", "—"); put("EngAppBy", "—");
    }
    // title block text, units and tolerance block, release note: edited in each sheet's format
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
            else if (lt.StartsWith("DIMENSIONS ARE IN INCHES")) { Size(n, 0.0014); n.SetText("DIMENSIONS IN MILLIMETRES. UNLESS NOTED:\nPRINTED PARTS ±0.5 % OF SIZE, MIN ±0.5 (DESKTOP FDM)\nBORES AND INSERT HOLES: SEE FIT TABLE (FT02/FT03/FT08)\nHOLE POSITION Ø0.4 (MOTOR PATTERN Ø0.2)\nCUT TUBES ISO 2768-m: ±0.8 (400–1000), ±0.5 (120–400)\nANGULAR ±1°"); }
            else if (lt.StartsWith("THE INFORMATION CONTAINED")) { Size(n, 0.0014); n.SetText("STUDENT CAPSTONE PROJECT, UC BERKELEY.\nREV B ISSUED FOR PROTOTYPE BUILD, " + DATE + ".\nFIRST-ARTICLE CHECKS M1–M7 OF\ndocs/research/hardware-spec.md APPLY BEFORE\nANY PART IS USED. GENERATED FROM\nanalysis/results.json, cad/hardware.json, cad/params.txt."); }
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
    // driven dimension between two model points; side = 'T','B','L','R' places the text beyond the view outline.
    // tolMin/tolMax (m) add a bilateral tolerance (symmetric when tolMin = -tolMax); prefix/suffix add callout text.
    static DisplayDimension Dim(View v, double[] m1, double[] m2, char side, double gap, string prefix = null, int decimals = 0, double tolMin = 0, double tolMax = 0, string suffix = null)
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
        if (p1 == null || p2 == null) { Log("  dim points failed"); return null; }
        Doc.ClearSelection2(true); p1.Select4(false, null); p2.Select4(true, null);
        DisplayDimension dd = (side == 'T' || side == 'B') ? (DisplayDimension)Doc.AddHorizontalDimension2(tx, ty, 0) : (DisplayDimension)Doc.AddVerticalDimension2(tx, ty, 0);   // text position is in sheet coordinates
        if (dd == null) { Log("  dimension failed"); return null; }
        if (decimals > 0) dd.SetPrecision3(decimals, decimals, decimals, decimals);
        if (prefix != null) dd.SetText((int)swDimensionTextParts_e.swDimensionTextPrefix, prefix);
        if (suffix != null) dd.SetText((int)swDimensionTextParts_e.swDimensionTextSuffix, suffix);
        if (tolMax > 0 || tolMin < 0)
        {
            Dimension dim = (Dimension)dd.GetDimension2(0);
            dim.SetToleranceType(Math.Abs(tolMin + tolMax) < 1e-9 ? (int)swTolType_e.swTolSYMMETRIC : (int)swTolType_e.swTolBILAT);
            dim.SetToleranceValues(tolMin, tolMax);
            dim.SetToleranceFontInfo(1, 0.8, 0);
        }
        Doc.ClearSelection2(true);
        Log(string.Format(IC, "  dim {0} = {1:0.###} mm", v.Name, ((Dimension)dd.GetDimension2(0)).SystemValue * 1000));
        return dd;
    }
    static View ModelView(string path, string orient, double x, double y, double scaleDen, bool hidden = false)
    {
        View v = Dwg.CreateDrawViewFromModelView3(path, orient, x, y, 0);
        if (v == null) { Log("VIEW FAILED " + orient + " " + path); return null; }
        v.ScaleDecimal = 1.0 / scaleDen;
        if (hidden) v.SetDisplayMode3(false, (int)swDisplayMode_e.swHIDDEN_GREYED, false, true);
        return v;
    }
    static string Wrap(string text, int width)
    {
        var outp = new List<string>();
        foreach (string line in text.Split('\n'))
        {
            string l = line; string indent = "    ";
            while (l.Length > width)
            {
                int cut = l.LastIndexOf(' ', width); if (cut < width / 2) cut = width;
                outp.Add(l.Substring(0, cut)); l = indent + l.Substring(cut).TrimStart();
            }
            outp.Add(l);
        }
        return string.Join("\n", outp.ToArray());
    }
    static void Label(double x, double y, string text, double h = 0.0035)
    {
        if (y < 0.095 && x < 0.21) text = Wrap(text, (int)(0.185 / (h * 0.78)));   // stay left of the title block
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
    // general table from rows; column widths in m
    static TableAnnotation Table(double x, double y, List<string[]> rows, double[] widths, double charH = 0.0024, double rowH = 0.0045)
    {
        int cols = widths.Length;
        TableAnnotation t = (TableAnnotation)Dwg.InsertTableAnnotation2(false, x, y, (int)swBOMConfigurationAnchorType_e.swBOMConfigurationAnchor_TopLeft, "", rows.Count, cols);
        if (t == null) { Log("table failed"); return null; }
        for (int i = 0; i < rows.Count; i++) for (int j = 0; j < cols; j++) t.set_Text(i, j, j < rows[i].Length ? rows[i][j] : "");
        TextFormat tf = (TextFormat)t.GetTextFormat(); tf.CharHeight = charH; t.SetTextFormat(false, tf);
        for (int j = 0; j < cols; j++) t.SetColumnWidth(j, widths[j], 0);
        for (int i = 0; i < rows.Count; i++) t.SetRowHeight(i, rowH, 0);
        return t;
    }

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
        double half = D("panelLen") + D("fusR"), yB = D("yB"), yIn = D("yIn"), xF = D("xFront"), xR = D("xRear"), x0 = D("xStub0");
        double yG = D("yGround"), finTop = D("yFin") + D("vH"), finX = D("xFinLE"), pushX = D("pushX"), yProp = D("yProp"), yPod = D("yPod");
        Dim(plan, new[] { 0.0, 0, half }, new[] { 0.0, 0, -half }, 'T', 0.010);
        Dim(plan, new[] { xF, yB, yIn }, new[] { xF, yB, -yIn }, 'T', 0.002);
        Dim(plan, new[] { 0.45, yPod, 0.0 }, new[] { x0, yB, yIn }, 'R', 0.012);
        Dim(plan, new[] { xF, yB, -yIn }, new[] { xR, yB, -yIn }, 'L', 0.010);
        Dim(front, new[] { finX, yG, -yIn }, new[] { finX, finTop, -yIn }, 'L', 0.010);
        Dim(front, new[] { pushX, yPod - D("pushProp") / 2, 0.0 }, new[] { pushX, yPod + D("pushProp") / 2, 0.0 }, 'R', 0.010);
        Dim(side, new[] { xF - D("liftProp") / 2, yProp, yIn }, new[] { xF + D("liftProp") / 2, yProp, yIn }, 'T', 0.006);
        Dim(side, new[] { pushX, yPod - D("pushProp") / 2, 0.0 }, new[] { pushX, yG, 0.0 }, 'R', 0.008);
        Label(0.022, 0.052, "NOTES\n1. DIMENSIONS IN MM. VIEWS 1:15, THIRD-ANGLE; ISOMETRIC 1:22.\n2. PROPS SHOWN PARKED FORE-AFT (NOTIONAL): THE ESCs CANNOT PARK THEM.\n3. SIZED CONCEPT, MTOW FROM analysis/results.json (aero inputs 2026-10-21). EVERY SCREW, NUT,\n    WASHER AND INSERT IS MODELLED: SHEETS 2, 3 AND 8.\n4. BREAKDOWN FOR TRANSPORT: 2 x M5 WING BOLTS, 4 x M3 PER BOOM CLAMP\n    PAIR, 4 x M3 TAIL SHOE SCREWS; JOINER SLIDES OUT OF THE SADDLE (SHEET 4).", 0.0026);
        LabelBelow(front, "FRONT ELEVATION"); LabelBelow(plan, "PLAN"); LabelBelow(side, "SIDE ELEVATION"); LabelBelow(isoV, "ISOMETRIC");
        var rev = new List<string[]> { new[] { "REV", "DATE", "DESCRIPTION", "BY" },
            new[] { "A", "2026-10-18", "CONCEPT GEOMETRY FOR DESIGN REVIEW (v0.3.1), 5 SHEETS", "DBF" },
            new[] { "B", DATE, "HARDWARE SPEC APPLIED: SPLIT MOUNTS AND CLAMPS, BOOM BUTT SPLICE, M5x20 WING BOLTS, RUDDERS,\nEVERY FASTENER MODELLED, TOLERANCES AND FITS, FASTENER SCHEDULE; 9 SHEETS", "DBF" } };
        TableAnnotation t = Table(0.300, 0.072, rev, new[] { 0.012, 0.022, 0.090, 0.012 }, 0.0022, 0.0045);
        if (t != null) t.SetRowHeight(2, 0.0085, 0);
    }

    // ---------- sheet 2: exploded assembly ----------
    static View explodedView;
    static void Sheet2_Exploded()
    {
        NewSheet("EXPLODED", 12);
        string asm = Path.Combine(ModelDir, "VTOL_Exploded.SLDASM");
        Open(asm, (int)swDocumentTypes_e.swDocASSEMBLY);
        explodedView = ModelView(asm, "*Isometric", 0.215, 0.150, 12);
        Doc.EditRebuild3();
        Label(0.022, 0.040, "NOTES\n1. EXPLODED LAYOUT (VTOL_Exploded.SLDASM): PARTS MOVED BY THE OFFSETS IN cad/params.mjs; FASTENERS PULLED 50 mm ALONG THEIR AXES.\n2. BALLOON ITEM NUMBERS REFER TO THE PARTS LIST ON SHEET 3. ONE BALLOON PER PART TYPE.\n3. JOINT IDs (J01–J20) AND FITS (FT01–FT11): docs/research/hardware-spec.md.", 0.0026);
        LabelBelow(explodedView, "EXPLODED ISOMETRIC  1:12");
    }
    static void Balloons()
    {
        string[] names = (string[])Dwg.GetSheetNames(); Dwg.ActivateSheet(names[1]);
        try
        {
            Doc.ClearSelection2(true); Doc.Extension.SelectByID2(explodedView.Name, "DRAWINGVIEW", 0, 0, 0, false, 0, null, 0);
            AutoBalloonOptions ab = (AutoBalloonOptions)Dwg.CreateAutoBalloonOptions();
            ab.Layout = (int)swBalloonLayoutType_e.swDetailingBalloonLayout_Square; ab.IgnoreMultiple = true; ab.ReverseDirection = false;
            ab.Style = (int)swBalloonStyle_e.swBS_Circular; ab.Size = (int)swBalloonFit_e.swBF_2Chars;
            object balloons = Dwg.AutoBalloon5(ab);
            Log("balloons " + (balloons != null ? ((object[])balloons).Length.ToString() : "none"));
        }
        catch (Exception ex) { Log("balloon error " + ex.Message); }
    }

    // ---------- sheet 3: parts list ----------
    static void Sheet3_PartsList()
    {
        NewSheet("PARTS", 40);
        string asm = Path.Combine(ModelDir, "VTOL_Concept.SLDASM");
        View iso = ModelView(asm, "*Isometric", 0.160, 0.040, 40);
        Doc.ClearSelection2(true); Doc.Extension.SelectByID2(iso.Name, "DRAWINGVIEW", 0, 0, 0, false, 0, null, 0);
        try
        {
            BomTableAnnotation bom = iso.InsertBomTable4(false, 0.022, 0.262, (int)swBOMConfigurationAnchorType_e.swBOMConfigurationAnchor_TopLeft,
                (int)swBomType_e.swBomType_PartsOnly, "Default", @"C:\Program Files\SOLIDWORKS Corp\SOLIDWORKS\lang\english\bom-standard.sldbomtbt", false, 0, false);
            Log("bom " + (bom != null));
            if (bom != null)
            {
                TableAnnotation ta = (TableAnnotation)bom;
                TextFormat tf = (TextFormat)ta.GetTextFormat(); tf.CharHeight = 0.0019; ta.SetTextFormat(false, tf);
                ta.SetColumnWidth(0, 0.010, 0); ta.SetColumnWidth(1, 0.036, 0); ta.SetColumnWidth(2, 0.138, 0); ta.SetColumnWidth(3, 0.009, 0);
                int rows = ta.RowCount; for (int i = 0; i < rows; i++) ta.SetRowHeight(i, 0.0034, 0);
                Log("bom rows " + rows);
                if (rows > 40)
                {
                    try
                    {
                        TableAnnotation t2 = ta.Split((int)swTableSplitLocations_e.swTableSplit_AfterRow, 38);
                        if (t2 != null) { Annotation an = (Annotation)t2.GetAnnotation(); an.SetPosition2(0.222, 0.262, 0); Log("bom split ok"); }
                    }
                    catch (Exception ex) { Log("bom split error " + ex.Message); }
                }
            }
        }
        catch (Exception ex) { Log("bom error " + ex.Message); }
        Label(0.022, 0.072, "NOTES\n1. ITEM NUMBERS ARE THE BALLOONS OF SHEET 2; QTY IS PER AIRCRAFT.\n2. PART NUMBER = SOLIDWORKS FILE (cad/model). FASTENER IDs F01–F13,\n    N01–N04, W01, I01 ARE THOSE OF cad/hardware.json AND SHEET 8.\n3. PURCHASED ITEMS, PRICES AND VENDORS: docs/purchasing/C0_BOM.csv.\n4. MATERIALS: 6061-T6 TUBES; A2 STAINLESS FASTENERS; BRASS INSERTS;\n    PETG PRINTS EXCEPT MOTOR MOUNTS AND PUSHER CUP IN PA6-CF; XPS FOAM.", 0.0026);
    }

    // ---------- sheet 4: wing panel, saddle, wing bolt ----------
    static double NacaY(double x, bool upper)
    {
        double m = 0.02, p = 0.4, t = D("tc");
        double yt = 5 * t * (0.2969 * Math.Sqrt(x) - 0.1260 * x - 0.3516 * x * x + 0.2843 * x * x * x - 0.1015 * x * x * x * x);
        double yc = x < p ? m / (p * p) * (2 * p * x - x * x) : m / ((1 - p) * (1 - p)) * ((1 - 2 * p) + 2 * p * x - x * x);
        return (upper ? yc + yt : yc - yt) * D("chord");
    }
    static void Sheet4_Wing()
    {
        NewSheet("WING", 10);
        double c = D("chord"), L = D("panelLen"), Lin = L * 0.45, sx = D("sparX"), sy = D("sparY"), r = D("sparOD") / 2, w = 2 * D("fusR");
        string wp = Part("Wing_Panel_R"); Open(wp, (int)swDocumentTypes_e.swDocPART);
        View plan = ModelView(wp, "*Top", 0.070, 0.175, 10, true);
        View sec = ModelView(wp, "*Front", 0.195, 0.235, 2, true);
        Doc.EditRebuild3();
        Dim(plan, new[] { 0.0, 0, 0 }, new[] { 0.0, 0, L }, 'L', 0.010);
        Dim(plan, new[] { 0.0, 0, 0 }, new[] { -c, 0, 0 }, 'T', 0.006);
        Dim(plan, new[] { -c, 0, Lin }, new[] { -c, 0, L }, 'R', 0.010);
        Dim(plan, new[] { 0.0, 0, 0 }, new[] { sx, sy, 0 }, 'T', 0.014);
        Dim(plan, new[] { sx, sy, 0 }, new[] { sx, sy, D("sparLen") }, 'R', 0.020);
        Dim(plan, new[] { 0.0, 0, 0 }, new[] { 0.0, 0, D("zBoomPanel") }, 'L', 0.022, "BOOM STATION ");
        Dim(sec, new[] { 0.0, NacaY(0, false), 0 }, new[] { -c, 0, 0 }, 'B', 0.008);
        Dim(sec, new[] { -0.30 * c, NacaY(0.30, false), 0 }, new[] { -0.30 * c, NacaY(0.30, true), 0 }, 'R', 0.010);
        Dim(sec, new[] { sx - r, sy, 0 }, new[] { sx + r, sy, 0 }, 'T', 0.006, "SOCKET <MOD-DIAM>", 1, -0.0001, 0.0001, " (TUBE OD, BONDED FT05)");
        string sd = Part("Wing_Saddle"); Open(sd, (int)swDocumentTypes_e.swDocPART);
        View sp = ModelView(sd, "*Top", 0.215, 0.120, 4, true);
        View ss = ModelView(sd, "*Front", 0.345, 0.235, 2, true);
        Doc.EditRebuild3();
        Dim(sp, new[] { 0.0, 0, -w / 2 }, new[] { 0.0, 0, w / 2 }, 'L', 0.008);
        Dim(sp, new[] { 0.0, 0, -w / 2 }, new[] { -c, 0, -w / 2 }, 'B', 0.006);
        Dim(sp, new[] { D("boltX"), 0, -D("boltZ") }, new[] { D("boltX"), 0, D("boltZ") }, 'T', 0.006, "PILLARS 2X <MOD-DIAM>16.4 ");
        double rj = D("joinOD") / 2 + 0.000115;
        Dim(ss, new[] { sx - rj, sy, 0 }, new[] { sx + rj, sy, 0 }, 'T', 0.006, "<MOD-DIAM>", 2, -0.0002, 0.0002, " SLIP FIT FT06");
        Dim(ss, new[] { 0.0, sy, 0 }, new[] { sx, sy, 0 }, 'B', 0.008);
        string bp = Part("Bolt_Pillar"); Open(bp, (int)swDocumentTypes_e.swDocPART);
        View pv = ModelView(bp, "*Front", 0.345, 0.120, 1, true);
        Doc.EditRebuild3();
        double h = D("yU70") - D("yL70");
        Dim(pv, new[] { -0.008, 0, 0 }, new[] { 0.008, 0, 0 }, 'B', 0.006, "<MOD-DIAM>");
        Dim(pv, new[] { 0.008, 0, 0 }, new[] { 0.008, h, 0 }, 'R', 0.008);
        Dim(pv, new[] { -0.00285, h, 0 }, new[] { 0.00285, h, 0 }, 'T', 0.006, "<MOD-DIAM>", 1, -0.0002, 0.0002, " THRU (M5 CLEARANCE, 5.7 PRINT)");
        Dim(pv, new[] { -0.0041, 0, 0 }, new[] { 0.0041, 0, 0 }, 'B', 0.014, "NUT POCKET ", 1, 0, 0, " AF x 8.5 DEEP");
        LabelBelow(plan, "WING PANEL, RIGHT (LEFT IS MIRROR)  1:10"); LabelBelow(sec, "ROOT SECTION  1:2", 0.018);
        LabelBelow(sp, "WING SADDLE  1:4", 0.018); LabelBelow(ss, "SADDLE SECTION  1:2", 0.018); LabelBelow(pv, "BOLT PILLAR  1:1", 0.022);
        Label(0.022, 0.052, "NOTES\n1. NACA 2415 (t/c FROM SIZING). XPS CORE, PACKING-TAPE SKIN. AILERON 26 % CHORD, 2 % HINGE GAP, OUTER 55 % SPAN.\n2. SPAR SOCKET 1.000 x 0.058 in 6061-T6 BONDED 800 mm INTO THE ROOT AT 30 % CHORD (FT05, FOAM-SAFE GLUE).\n3. JOINER 0.875 x 0.058 in, 760 mm, SLIP FIT IN THE SOCKETS (FT01: -0.08/+0.53 DIAMETRAL, MEASURE M1) AND IN THE\n    SADDLE (FT06). NOT BONDED: CAPTURED BY TWO PRINTED STOPS 300 mm INSIDE THE SOCKETS ONCE BOTH PANELS ARE ON.\n4. WING BOLT J10: F09 ISO 7380 M5 x 20 THROUGH THE PILLAR AND THE PANEL ROOT TONGUE (20 x 6.4 x 50, 25 BONDED)\n    INTO N02 M5 NYLOCK, 1.0 N·m EST. HEAD IN A <MOD-DIAM>10.3 x 1.0 SPOT-FACE.\n5. POCKETS IN THE PANEL (SPAR COLLAR, REAR HARDPOINT, SERVO TRAY, TONGUE, PITOT CLAMP): CUT TO THE PART OUTLINES\n    OF SHEET 9, BONDED WITH FOAM-SAFE GLUE.\n6. SADDLE TABS: 2 x F08 M3 x 12 PER SIDE INTO RX-M3x5.7 INSERTS IN THE POD FRAME RAILS (J11).", 0.0024);
    }

    // ---------- sheet 5: motor mount and splice ----------
    static void Sheet5_MotorMount()
    {
        NewSheet("MOUNT", 1);
        string up = Part("Motor_Mount_Upper"); Open(up, (int)swDocumentTypes_e.swDocPART);
        View sec = ModelView(up, "*Right", 0.070, 0.215, 1, true);
        View top = ModelView(up, "*Top", 0.190, 0.215, 1, true);
        View sid = ModelView(up, "*Front", 0.320, 0.215, 1, true);
        Doc.EditRebuild3();
        Dim(sec, new[] { 0.0, 0.0006, -0.023 }, new[] { 0.0, 0.0006, 0.023 }, 'B', 0.006, null, 0, -0.0005, 0.0005);
        Dim(sec, new[] { 0.0, 0.0006, 0.023 }, new[] { 0.0, 0.021, 0.023 }, 'R', 0.008, null, 1);
        Dim(sec, new[] { 0.0, 0, -0.0128 }, new[] { 0.0, 0, 0.0128 }, 'T', 0.005, "<MOD-DIAM>", 1, -0.0002, 0.0002, " BORE FT02");
        Dim(sec, new[] { 0.0, 0.0165, 0.0 }, new[] { 0.0, 0.021, 0.0 }, 'L', 0.008, "FLOOR ", 2, -0.00015, 0.00015);
        Dim(sec, new[] { 0.0, 0.0006, -0.0175 }, new[] { 0.0, 0.0006, 0.0175 }, 'B', 0.014, "INSERTS ");
        Dim(top, new[] { -0.028, 0.021, 0 }, new[] { 0.028, 0.021, 0 }, 'T', 0.006, "4X <MOD-DIAM>4.0 ±0.1 x 7.5 DEEP FROM SPLIT FACE (RX-M3x5.7 I01)  ");
        Dim(top, new[] { -0.00884, 0.021, -0.00884 }, new[] { 0.00884, 0.021, -0.00884 }, 'B', 0.006, "4X <MOD-DIAM>3.4 THRU (3.6 PRINT, DRILL) ON <MOD-DIAM>25 B.C. POS <MOD-DIAM>0.2, CBORE <MOD-DIAM>7.5 FROM BORE  ", 2);
        Dim(top, new[] { -0.018, 0.021, 0.023 }, new[] { 0.018, 0.021, 0.023 }, 'T', 0.014, "SPLICE BOLTS 2X <MOD-DIAM>3.4 THRU Z  ");
        Dim(sid, new[] { -0.035, 0.021, 0 }, new[] { 0.035, 0.021, 0 }, 'T', 0.006);
        Dim(sid, new[] { 0.035, 0.0006, 0 }, new[] { 0.035, 0.021, 0 }, 'R', 0.008, null, 1);
        string cap = Part("Motor_Mount_Cap"); Open(cap, (int)swDocumentTypes_e.swDocPART);
        View cs = ModelView(cap, "*Right", 0.070, 0.105, 1, true);
        View cb = ModelView(cap, "*Bottom", 0.190, 0.105, 1, true);
        Doc.EditRebuild3();
        Dim(cs, new[] { 0.0, -0.015, 0.023 }, new[] { 0.0, -0.0006, 0.023 }, 'R', 0.008, null, 1);
        Dim(cs, new[] { 0.0, -0.015, -0.0175 }, new[] { 0.0, -0.015, 0.0175 }, 'B', 0.006);
        Dim(cb, new[] { -0.028, -0.015, 0 }, new[] { 0.028, -0.015, 0 }, 'B', 0.006, "4X <MOD-DIAM>3.4 THRU (3.6 PRINT), CBORE <MOD-DIAM>6.5 x 5.6 DEEP  ");
        string plug = Part("Splice_Plug"); Open(plug, (int)swDocumentTypes_e.swDocPART);
        View pv = ModelView(plug, "*Front", 0.320, 0.105, 1, true);
        Doc.EditRebuild3();
        Dim(pv, new[] { -0.040, 0.0116, 0 }, new[] { 0.040, 0.0116, 0 }, 'T', 0.006, null, 0, -0.0005, 0.0005);
        Dim(pv, new[] { 0.040, -0.0116, 0 }, new[] { 0.040, 0.0116, 0 }, 'R', 0.008, "<MOD-DIAM>", 1, -0.0002, 0.0002, " FT04");
        Dim(pv, new[] { -0.018, -0.0116, 0 }, new[] { 0.018, -0.0116, 0 }, 'B', 0.006, "2X <MOD-DIAM>3.4  ", 0, -0.0003, 0.0003);
        LabelBelow(sec, "UPPER HALF, SECTION  1:1"); LabelBelow(top, "UPPER HALF, TOP  1:1"); LabelBelow(sid, "UPPER HALF, SIDE  1:1");
        LabelBelow(cs, "CAP, SECTION  1:1"); LabelBelow(cb, "CAP, BOTTOM  1:1"); LabelBelow(pv, "SPLICE PLUG  1:1");
        Label(0.022, 0.046, "NOTES\n1. MATERIAL PA6-CF (HDT 173 °C), 4 UPPER HALVES + 4 CAPS. PRINT BORES 0.2 UNDER (COUPON M2), REAM TO 25.6 ±0.2 (FT02). SPLIT GAP 1.2 (0.6 EACH HALF).\n2. J01 MOTOR: 4X F01 ISO 4762 M3 x 8 + W01 WASHER FROM INSIDE THE BORE BEFORE THE HALVES CLOSE. ENGAGEMENT 3.0 (LIMIT 3.3, CHECK M4). 0.6 N·m EST, LOCTITE 242 ON THE THREADS ONLY.\n3. J02 CLAMP: 4X F02 ISO 4762 M3 x 16 FROM BELOW INTO THE INSERTS, 0.5 N·m EST, NO THREADLOCKER, WITNESS MARK.\n4. J04 SPLICE (REAR STATION ONLY): BOOMS BUTT AT THE MOUNT CENTRE OVER THE PLUG; 2X F04 M3 x 40 + W01 + N01 NYLOCK ALONG Z AT x ±18, y 0;\n    HEAD CBORE <MOD-DIAM>7.5 x 5.0 (+Z FACE), NUT POCKET 5.7 AF x 8.0 (-Z FACE). DRILL BOTH TUBE WALLS <MOD-DIAM>3.4 THROUGH THE MOUNT AS A JIG, POSITION ±0.3.\n5. WIRE GROOVE 6 x 2 ON THE TOP FACE TOWARDS +X; CENTRE RELIEF <MOD-DIAM>8 x 2 FOR THE MOTOR BASE BOSS.", 0.0022);
    }

    // ---------- sheet 6: clamps ----------
    static void Half(string name, double cx, double cy, double y0, double y1, double lx, string title, bool holes, double holeX, double dia, string callout)
    {
        string p = Part(name); Open(p, (int)swDocumentTypes_e.swDocPART);
        View sec = ModelView(p, "*Right", cx, cy, 1, true);
        View sid = ModelView(p, "*Front", cx + 0.070, cy, 1, true);
        Doc.EditRebuild3();
        Dim(sec, new[] { 0.0, y0, -0.023 }, new[] { 0.0, y0, 0.023 }, 'B', 0.006, null, 0, -0.0005, 0.0005);
        Dim(sec, new[] { 0.0, y0, 0.023 }, new[] { 0.0, y1, 0.023 }, 'R', 0.006, null, 1);
        Dim(sec, new[] { 0.0, 0, -0.0128 }, new[] { 0.0, 0, 0.0128 }, 'T', 0.005, "<MOD-DIAM>", 1, -0.0002, 0.0002, " FT02");
        Dim(sid, new[] { -lx / 2, y1, 0 }, new[] { lx / 2, y1, 0 }, 'T', 0.005);
        if (holes) Dim(sid, new[] { -holeX, y0, 0 }, new[] { holeX, y0, 0 }, 'B', 0.006, callout);
        Label(cx - 0.040, cy + 0.040, title, 0.0028);
    }
    static void Sheet6_Clamps()
    {
        NewSheet("CLAMPS", 1);
        Half("Boom_Clamp_F_Upper", 0.060, 0.225, 0.0006, D("clampTopF"), 0.034, "FRONT BOOM CLAMP, UPPER (PETG) QTY 2  1:1", true, 0.012, 0.0036, "2X <MOD-DIAM>3.4 THRU (DIAGONAL)  ");
        Half("Boom_Clamp_F_Lower", 0.205, 0.225, -0.0167, -0.0006, 0.034, "FRONT BOOM CLAMP, LOWER (PETG) QTY 2  1:1", true, 0.012, 0.0036, "2X <MOD-DIAM>3.4, CBORE <MOD-DIAM>6.5 x 4.3  ");
        Half("Boom_Clamp_R_Lower", 0.350, 0.225, -0.0167, -0.0006, 0.034, "REAR BOOM CLAMP, LOWER (PETG) QTY 2  1:1", true, 0.012, 0.0036, "2X <MOD-DIAM>3.4, CBORE <MOD-DIAM>6.5 x 7.5  ");
        Half("Gear_Clamp_Upper", 0.060, 0.145, 0.0006, 0.0167, 0.034, "GEAR CLAMP, UPPER (PETG) QTY 4  1:1", true, 0.012, 0.004, "4X <MOD-DIAM>4.0 ±0.1 x 7.5 INSERT  ");
        Half("Gear_Clamp_Lower", 0.205, 0.145, -0.0287, -0.0006, 0.034, "GEAR CLAMP, LOWER (PETG) QTY 4  1:1", true, 0.012, 0.0036, "4X <MOD-DIAM>3.4, CBORE <MOD-DIAM>6.5 x 15.3  ");
        Half("Tail_Clamp_Upper", 0.350, 0.145, 0.0006, 0.0157, 0.050, "TAIL CLAMP, UPPER (PETG) QTY 2  1:1", true, 0.018, 0.004, "4X <MOD-DIAM>4.0 x 7.5 INSERT; 2X FROM TOP AT x 0  ");
        Half("Tail_Clamp_Cap", 0.045, 0.078, -0.015, -0.0006, 0.050, "TAIL CLAMP, CAP (PETG) QTY 2  1:1", true, 0.018, 0.0036, "4X <MOD-DIAM>3.4, CBORE <MOD-DIAM>6.5 x 5.6  ");
        string gl = Part("Gear_Clamp_Lower"); View gs = ModelView(gl, "*Front", 0.185, 0.082, 1, true); Doc.EditRebuild3();
        Dim(gs, new[] { 0.0, -0.0287, 0 }, new[] { 0.0, -0.0147, 0 }, 'R', 0.006, "SOCKET ", 1, 0, 0, " DEEP");
        Dim(gs, new[] { -0.01285, -0.0287, 0 }, new[] { 0.01285, -0.0287, 0 }, 'B', 0.006, "<MOD-DIAM>", 1, -0.0002, 0.0002, " BLIND FT03");
        Dim(gs, new[] { 0.017, -0.0287, 0 }, new[] { 0.017, -0.0217, 0 }, 'L', 0.008, "F05 BOLT ", 1);
        Label(0.160, 0.118, "GEAR CLAMP LOWER, LEG SOCKET  1:1", 0.0028);
        Label(0.022, 0.048, "NOTES\n1. ALL CLAMPS 46 WIDE (C3), BORE 25.6 ±0.2 (FT02) PRINTED 0.2 UNDER AND REAMED; SPLIT GAP 1.2. REAR BOOM CLAMP UPPER IS THE FRONT UPPER WITH TOP AT " + (D("clampTopR") * 1000).ToString("0.0", IC) + ".\n2. J05 BOOM CLAMPS: F06 M3 x 35 (FRONT) / F04 M3 x 40 (REAR), 2 PER CLAMP ON A DIAGONAL, THROUGH BOTH HALVES INTO THE WING HARDPOINT INSERTS; e 6.0; 0.5 N·m EST. FIELD BREAKDOWN JOINT.\n3. J06 GEAR CLAMP: 4X F03 M3 x 20 INTO THE UPPER-HALF INSERTS, 0.5 N·m EST. J07 LEG: F05 M3 x 30 + W01 + N01 ALONG X AT y -21.7; HEAD CBORE <MOD-DIAM>7.5 x 4.0 (+X), NUT POCKET 5.7 AF x 4.3 (-X), BOTH CLEAR OF THE LEG TUBE; LEG DRILLED <MOD-DIAM>3.4 THROUGH THE CLAMP.\n4. J08 TAIL CLAMP: 4X F02 M3 x 16, 0.5 N·m EST; FLAT TOP CARRIES THE STABILISER; 2 INSERTS FROM THE TOP TAKE THE FIN-SHOE SCREWS (J09).", 0.0022);
    }

    // ---------- sheet 7: tube cut list and landing leg ----------
    static void Sheet7_Tubes()
    {
        NewSheet("TUBES", 2);
        string leg = Part("Landing_Leg"); Open(leg, (int)swDocumentTypes_e.swDocPART);
        View lv = ModelView(leg, "*Front", 0.330, 0.165, 2, true);
        Doc.EditRebuild3();
        double R = D("legOD") / 2, Ll = D("legLen");
        Dim(lv, new[] { R, 0.0, 0 }, new[] { R, Ll, 0 }, 'R', 0.010, null, 0, -0.0005, 0.0005);
        Dim(lv, new[] { -R, Ll, 0 }, new[] { R, Ll, 0 }, 'T', 0.006, "<MOD-DIAM>", 1, -0.0001, 0.0001);
        Dim(lv, new[] { -R, Ll - 0.007, 0 }, new[] { -R, Ll, 0 }, 'L', 0.010, "<MOD-DIAM>3.4 HOLE ", 1, -0.0003, 0.0003);
        string bf = Part("Boom_Front"); Open(bf, (int)swDocumentTypes_e.swDocPART);
        View bv = ModelView(bf, "*Top", 0.180, 0.095, 4, true); Doc.EditRebuild3();
        Dim(bv, new[] { 0.0, 0, 0.0127 }, new[] { D("segF"), 0, 0.0127 }, 'T', 0.006, null, 1, -0.0008, 0.0008);
        Dim(bv, new[] { 0.0, 0, -0.0127 }, new[] { 0.018, 0, -0.0127 }, 'B', 0.006, "<MOD-DIAM>3.4 THRU ", 0, -0.0003, 0.0003);
        string gf = Part("Gear_Fairing"); Open(gf, (int)swDocumentTypes_e.swDocPART);
        View fv = ModelView(gf, "*Top", 0.330, 0.062, 1, true); Doc.EditRebuild3();
        Dim(fv, new[] { 0.3 * 0.0816, 0, 0 }, new[] { -0.7 * 0.0816, 0, 0 }, 'B', 0.006, "CHORD ", 1);
        Dim(fv, new[] { 0.0, 0, -0.0136 }, new[] { 0.0, 0, 0.0136 }, 'R', 0.006, null, 1, 0, 0, " (BORE 25.6 FOR THE LEG)");
        LabelBelow(lv, "LANDING LEG  1:2"); LabelBelow(bv, "FRONT BOOM, SPLICE END  1:4"); LabelBelow(fv, "LEG FAIRING SECTION (PETG, 170 LONG) QTY 4  1:1");
        string mm = "{0:0.0}";
        var rows = new List<string[]> {
            new[] { "ITEM", "PART", "STOCK (6061-T6)", "CUT LENGTH mm", "TOL mm (ISO 2768-m)", "QTY", "HOLES" },
            new[] { "T01", "Wing spar socket", "1.000 x 0.058 in", string.Format(IC, mm, D("sparLen") * 1000), "±0.8", "2", "none; bonded in the panel (FT05)" },
            new[] { "T02", "Wing joiner", "0.875 x 0.058 in", string.Format(IC, mm, D("joinLen") * 1000), "±0.8", "1", "none; slip fit FT01/FT06, lap if tight" },
            new[] { "T03", "Front boom", "1.000 x 0.035 in", string.Format(IC, mm, D("segF") * 1000), "±0.8", "2", "1 x Ø3.4 cross hole (Z) 18 ±0.3 from the splice end, drilled through the mount" },
            new[] { "T04", "Rear boom stub", "1.000 x 0.035 in", string.Format(IC, mm, D("segR") * 1000), "±0.8", "2", "1 x Ø3.4 cross hole (Z) 18 ±0.3 from the splice end" },
            new[] { "T05", "Landing leg", "1.000 x 0.058 in", string.Format(IC, mm, D("legLen") * 1000), "±0.5", "4", "1 x Ø3.4 cross hole (X) 7 ±0.3 from the top end, drilled through the gear clamp" } };
        Table(0.022, 0.250, rows, new[] { 0.012, 0.034, 0.034, 0.028, 0.034, 0.012, 0.120 });
        Label(0.022, 0.060, "NOTES\n1. LEGS ARE CUT FROM THE SPAR-SOCKET TUBE OFFCUT: 2 x 800 + 4 x 205 = 2420 mm OF A 96 in (2438 mm) LENGTH. BOOMS: 2 x (" + (D("segF") * 1000).ToString("0.0", IC) + " + " + (D("segR") * 1000).ToString("0.0", IC) + ") = " + (2 * (D("segF") + D("segR")) * 1000).ToString("0", IC) + " mm FROM TWO 72 in LENGTHS.\n2. END SQUARENESS 0.5 (EST). DEBURR ALL CUT ENDS AND HOLES. TUBE OD ±0.10, WALL ±0.05 (VENDOR, HS §5a): MEASURE ON RECEIPT (M1).\n3. BOOMS BUTT AT THE REAR MOTOR MOUNT CENTRE OVER THE PRINTED SPLICE PLUG (SHEET 5); FRONT AND REAR LENGTHS FROM analysis/results.json geo.segs.\n4. LEG FAIRINGS: 3:1 TEARDROP (NACA 0033 SECTION) SLIPPED OVER EACH LEG, TRAILING EDGE AFT, VASE-MODE 0.8 mm WALL (EST), CLOCKED AGAINST THE GEAR CLAMP AND TAPED; NO FASTENERS.\n    DRAG BUILD-UP analysis/aero/aero_results.json: ROUND LEGS 0.0364 OF CD0 -> FAIRED 0.0036.", 0.0024);
    }

    // ---------- sheet 8: fastener schedule ----------
    static void Sheet8_Fasteners()
    {
        NewSheet("FASTENERS", 1);
        var rows = new List<string[]> { new[] { "ID", "STANDARD", "SIZE", "QTY", "ORDER", "JOINTS", "TORQUE N·m", "THREADLOCKER", "VENDOR / PART NO." } };
        var keys = new List<string>(); foreach (string k in P.Keys) if (k.StartsWith("sched_")) keys.Add(k); keys.Sort();
        foreach (string k in keys)
        {
            string[] f = P[k].Split('|');
            var js = new List<string>(); foreach (Match m in Regex.Matches(f[5], @"J\d\d[ab]?")) if (!js.Contains(m.Value)) js.Add(m.Value);
            string joints = js.Count > 0 ? string.Join(", ", js.ToArray()) : (f[0] == "I01" ? "J02 J03b J05 J06 J08 J09 J11 J12 J13 J15 J16" : "—");
            string tl = f[7]; if (tl.Length > 34) tl = tl.Substring(0, 32) + "…";
            rows.Add(new[] { f[0], f[1].Length > 40 ? f[1].Substring(0, 38) + "…" : f[1], f[2], f[3], f[4], joints, f[6], tl, f[8] + " " + f[9] });
        }
        Table(0.022, 0.262, rows, new[] { 0.012, 0.062, 0.028, 0.012, 0.014, 0.064, 0.018, 0.064, 0.060 }, 0.0021, 0.0046);
        Label(0.022, 0.118, "NOTES (docs/research/hardware-spec.md §2d, §4)\n1. QTY PER AIRCRAFT; ORDER INCLUDES SPARES. F12 PROP SCREWS AND F13 ADAPTER SCREWS: LENGTH PROVISIONAL, CHECK M5 (HUB THICKNESS NOT PUBLISHED).\n2. TORQUES MARKED EST IN THE SPEC: CONFIRM INSERT TORQUE-OUT >= 2x ON A COUPON (M3). NO TORQUE DRIVER ON HAND: HANG A KNOWN MASS FROM THE HEX KEY,\n    T = m g L (0.6 N·m = 1.02 kg AT L 60 mm; 0.5 N·m = 0.85 kg; 1.0 N·m = 1.70 kg).\n3. LOCTITE 242 ONLY ON METAL-TO-METAL THREADS (F01 MOTOR BASE, F12/F13 HUB SCREWS), ONE DROP ON THE LAST 2–3 THREADS, KEPT OFF THE PA6-CF. NEVER ON NYLOCKS.\n4. SCREWS IN BRASS INSERTS GET NO THREADLOCKER: TORQUE, THEN A PAINT WITNESS MARK CHECKED PRE-FLIGHT. REPLACE A NYLOCK THAT TURNS BY HAND.\n5. INSERTS RX-M3x5.7 IN Ø4.0 ±0.1 HOLES >= 7.0 DEEP (FT08, INTERFERENCE INTENDED); SOLDERING IRON ~20 °C ABOVE THE PRINT TEMPERATURE, SET PER MATERIAL.\n6. CLEARANCE HOLES: ISO 273 NORMAL (M2 2.4, M3 3.4, M5 5.5) PRINTED AT +0.2 AND CHECKED ON THE HOLE COUPON (M2); MOTOR-PATTERN HOLES DRILLED TO 3.4 AFTER PRINTING.\n7. MOTOR SCREWS: TIP <= 3.2 mm PROUD OF THE FLOOR BEFORE EVERY MOTOR INSTALL (M4); 3.3 mm REACHES THE WINDINGS.\n8. FIELD BREAKDOWN (R4): 2 x F09, 8 x BOOM-CLAMP SCREWS (F06/F04), 4 x F02 SHOE SCREWS; 14 SCREWS IN A LABELLED BAG.", 0.0022);
    }

    // ---------- sheet 9: small printed parts ----------
    static void Sheet9_Printed()
    {
        NewSheet("HARDPOINTS", 1);
        string fs = Part("Fin_Shoe"); Open(fs, (int)swDocumentTypes_e.swDocPART);
        View fv = ModelView(fs, "*Top", 0.085, 0.215, 1, true); Doc.EditRebuild3();
        double xc = D("xTC") - D("xFinLE");
        Dim(fv, new[] { xc - 0.016, 0.0, 0.023 }, new[] { xc + 0.016, 0.0, 0.023 }, 'T', 0.006, "FLANGE ");
        Dim(fv, new[] { xc, 0, -0.0175 }, new[] { xc, 0, 0.0175 }, 'R', 0.008, "2X <MOD-DIAM>3.4 ");
        string sc = Part("Spar_Collar"); Open(sc, (int)swDocumentTypes_e.swDocPART);
        View cv = ModelView(sc, "*Front", 0.230, 0.215, 1, true); Doc.EditRebuild3();
        double yb = D("yL30") - D("sparY");
        Dim(cv, new[] { -0.016, yb, 0 }, new[] { 0.016, yb, 0 }, 'B', 0.006);
        Dim(cv, new[] { 0.016, yb, 0 }, new[] { 0.016, 0.0157, 0 }, 'R', 0.006, null, 1);
        Dim(cv, new[] { -0.0128, 0, 0 }, new[] { 0.0128, 0, 0 }, 'T', 0.006, "<MOD-DIAM>", 1, -0.0002, 0.0002, " ON THE SPAR SOCKET, BONDED");
        string rh = Part("Rear_Hardpoint"); Open(rh, (int)swDocumentTypes_e.swDocPART);
        View rv = ModelView(rh, "*Front", 0.340, 0.215, 1, true); Doc.EditRebuild3();
        Dim(rv, new[] { -0.030, 0, 0 }, new[] { 0.030, 0, 0 }, 'B', 0.006);
        Dim(rv, new[] { 0.030, 0, 0 }, new[] { 0.030, 0.010, 0 }, 'R', 0.006);
        string pc = Part("Pusher_Cup"); Open(pc, (int)swDocumentTypes_e.swDocPART);
        View pv = ModelView(pc, "*Top", 0.085, 0.105, 1, true); Doc.EditRebuild3();
        Dim(pv, new[] { 0.0, 0, -0.0278 }, new[] { 0.0, 0, 0.0278 }, 'L', 0.008, "<MOD-DIAM>", 1, -0.0002, 0.0002, " FT11");
        Dim(pv, new[] { 0.0, 0, 0.0278 }, new[] { 0.030, 0, 0.0278 }, 'T', 0.006);
        Dim(pv, new[] { 0.0, 0, -0.0278 }, new[] { 0.0045, 0, -0.0278 }, 'B', 0.006, "FLOOR ", 1, -0.00015, 0.00015);
        string st = Part("Servo_Tray"); Open(st, (int)swDocumentTypes_e.swDocPART);
        View sv = ModelView(st, "*Front", 0.230, 0.105, 1, true); Doc.EditRebuild3();
        Dim(sv, new[] { -0.025, 0, 0 }, new[] { 0.025, 0, 0 }, 'T', 0.006);
        Dim(sv, new[] { -0.023, D("trayBodyY"), 0 }, new[] { 0.023, D("trayBodyY"), 0 }, 'B', 0.006, "4X <MOD-DIAM>2.6 + M2 NUT POCKET 4.2 AF x 2  ");
        string rt = Part("Root_Tongue"); Open(rt, (int)swDocumentTypes_e.swDocPART);
        View tv = ModelView(rt, "*Top", 0.340, 0.105, 1, true); Doc.EditRebuild3();
        Dim(tv, new[] { -0.010, 0, 0 }, new[] { -0.010, 0, 0.050 }, 'L', 0.008);
        Dim(tv, new[] { -0.010, 0, 0.050 }, new[] { 0.010, 0, 0.050 }, 'T', 0.006);
        Dim(tv, new[] { 0.010, 0, 0 }, new[] { 0.010, 0, 0.010 }, 'R', 0.006, "<MOD-DIAM>5.7 AT ", 0, -0.0002, 0.0002);
        LabelBelow(fv, "FIN SHOE (PETG) QTY 2  1:1"); LabelBelow(cv, "SPAR COLLAR (PETG) QTY 2  1:1"); LabelBelow(rv, "REAR HARDPOINT (PETG) QTY 2  1:1");
        LabelBelow(pv, "PUSHER CUP (PA6-CF) QTY 1  1:1"); LabelBelow(sv, "SERVO TRAY (PETG) QTY 5  1:1"); LabelBelow(tv, "ROOT TONGUE (PETG) QTY 2  1:1");
        Label(0.022, 0.044, "NOTES\n1. FIN SHOE J09: FLOOR 3.0, WALLS 20 x 1.6 AROUND THE FIN ROOT SECTION (FIN BONDED IN); 2X F02 M3 x 16 THROUGH THE FLANGE, A PRINTED CRUSH SLEEVE 7 x 3.4 x 6.8 IN THE STABILISER AND INTO THE TAIL-CLAMP INSERTS, e 6.2, 0.5 N·m EST.\n2. SPAR COLLAR J05a AND REAR HARDPOINT J05b: BONDED FLUSH INTO THE WING LOWER SURFACE AT THE BOOM STATION; INSERTS Ø4.0 x 7.5 FACE DOWN (BOSS WALL >= 1.6).\n3. PUSHER CUP J12: 4X F01 + W01 FROM INSIDE INTO THE MOTOR BASE (e 3.0); 4X F07 M3 x 8 RADIAL THROUGH THE POD RING INTO THE BOSS INSERTS, 0.4 N·m EST.\n4. SERVO TRAY J14: ONE PART FOR ALL FIVE SERVOS (AILERONS IN THE WING POCKET, ELEVATOR AND RUDDERS ON THE STABILISER TOP); 4X F10 M2 x 10 + N03; TAB HOLES PROVISIONAL (M6).\n5. ROOT TONGUE J10: 25 mm BONDED INTO THE PANEL ROOT SLOT, 25 mm INTO THE SADDLE PILLAR SLOT.\n6. OTHER PRINTED PARTS (SPLICE PLUG, JOINER STOP, BOLT PILLAR, CRUSH SLEEVE, HORN HARDPOINT, PITOT CLAMP, BATTERY AND FC TRAYS): DIMENSIONS IN THE PART FILES; SEE THE PARTS LIST DESCRIPTIONS.", 0.0022);
    }
}
