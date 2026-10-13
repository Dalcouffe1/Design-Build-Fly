// Lists documents open in SOLIDWORKS; with "close", closes only documents saved under this project's cad folder.
using System; using SolidWorks.Interop.sldworks;
class ListDocs { static void Main(string[] a) {
  SldWorks sw = (SldWorks)System.Runtime.InteropServices.Marshal.GetActiveObject("SldWorks.Application");
  string root = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ".")).ToLowerInvariant();
  var toClose = new System.Collections.Generic.List<string>();
  ModelDoc2 d = (ModelDoc2)sw.GetFirstDocument();
  while (d != null) { string p = d.GetPathName(); Console.WriteLine(d.GetTitle() + " | unsaved=" + d.GetSaveFlag() + " | " + p);
    if (p.Length > 0 && p.ToLowerInvariant().StartsWith(root)) toClose.Add(d.GetTitle()); d = (ModelDoc2)d.GetNext(); }
  if (a.Length > 0 && a[0] == "close") foreach (string t in toClose) { sw.CloseDoc(t); Console.WriteLine("closed " + t); }
} }
