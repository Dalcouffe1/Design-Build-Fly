# Regenerates the SOLIDWORKS model, meshes, drawings and renders: params from analysis/results.json + cad/hardware.json,
# compile and run BuildVtol.cs, ExportMesh.cs, Drawings.cs, Snap.cs.
# Usage (from the project folder):  powershell -ExecutionPolicy Bypass -File cad\build.ps1
# Run first:  node analysis/sizing.mjs; node analysis/vlm.mjs; node analysis/export_bom.mjs; py analysis/verify.py
# Afterwards: node review/build.mjs   (packs cad/mesh into the review page)
$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$sw   = 'C:\Program Files\SOLIDWORKS Corp\SOLIDWORKS\api\redist'
$csc  = 'C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe'
Copy-Item "$sw\SolidWorks.Interop.sldworks.dll","$sw\SolidWorks.Interop.swconst.dll" (Join-Path $root 'cad') -Force
function Build($src, $exe) {
  & $csc /nologo /platform:x64 /out:$exe /r:"$sw\SolidWorks.Interop.sldworks.dll" /r:"$sw\SolidWorks.Interop.swconst.dll" $src
  if ($LASTEXITCODE -ne 0) { throw "compile failed: $src" }
}
node (Join-Path $root 'cad\params.mjs')
Build (Join-Path $root 'cad\ListDocs.cs') (Join-Path $root 'cad\ListDocs.exe')
& (Join-Path $root 'cad\ListDocs.exe') close untitled    # close project documents left open in SOLIDWORKS
# model: parts, VTOL_Concept.SLDASM/.STEP/.EASM, VTOL_Exploded.SLDASM, cad/verification.txt (interference, masses)
$exe = Join-Path $root 'cad\BuildVtol.exe'
Build (Join-Path $root 'cad\BuildVtol.cs') $exe
& $exe (Join-Path $root 'cad\params.txt') (Join-Path $root 'cad\model')
# part meshes for the interactive viewer in the review page (cad/mesh/*.STL + meta.json); then rebuild with: node review/build.mjs
$mx = Join-Path $root 'cad\ExportMesh.exe'
Build (Join-Path $root 'cad\ExportMesh.cs') $mx
& $mx (Join-Path $root 'cad\model') (Join-Path $root 'cad\mesh')
# drawing set DBF-DRW-001 rev B (cad/drawings): SLDDRW, vector PDF, sheet PNGs for the review page
$dx = Join-Path $root 'cad\Drawings.exe'
Build (Join-Path $root 'cad\Drawings.cs') $dx
& $dx (Join-Path $root 'cad\model') (Join-Path $root 'cad\drawings') (Join-Path $root 'cad\params.txt')
# renders of the assembly (cad/images)
$sn = Join-Path $root 'cad\Snap.exe'
Build (Join-Path $root 'cad\Snap.cs') $sn
& $sn (Join-Path $root 'cad\model\VTOL_Concept.SLDASM') (Join-Path $root 'cad\images')
& (Join-Path $root 'cad\ListDocs.exe') close
