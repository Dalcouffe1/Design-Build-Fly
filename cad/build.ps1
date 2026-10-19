# Regenerates the SOLIDWORKS concept model: params from analysis/results.json, compile BuildVtol.cs, run it.
# Usage (from the project folder):  powershell -ExecutionPolicy Bypass -File cad\build.ps1
$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$sw   = 'C:\Program Files\SOLIDWORKS Corp\SOLIDWORKS\api\redist'
$csc  = 'C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe'
node (Join-Path $root 'cad\params.mjs')
$exe = Join-Path $root 'cad\BuildVtol.exe'
& $csc /nologo /platform:x64 /out:$exe /r:"$sw\SolidWorks.Interop.sldworks.dll" /r:"$sw\SolidWorks.Interop.swconst.dll" (Join-Path $root 'cad\BuildVtol.cs')
if ($LASTEXITCODE -ne 0) { throw 'compile failed' }
Copy-Item "$sw\SolidWorks.Interop.sldworks.dll","$sw\SolidWorks.Interop.swconst.dll" (Join-Path $root 'cad') -Force
& $exe (Join-Path $root 'cad\params.txt') (Join-Path $root 'cad\model')
# Part meshes for the interactive viewer in the review page (cad/mesh/*.STL + meta.json); then rebuild with: node review/build.mjs
$mx = Join-Path $root 'cad\ExportMesh.exe'
& $csc /nologo /platform:x64 /out:$mx /r:"$sw\SolidWorks.Interop.sldworks.dll" /r:"$sw\SolidWorks.Interop.swconst.dll" (Join-Path $root 'cad\ExportMesh.cs')
if ($LASTEXITCODE -ne 0) { throw 'ExportMesh compile failed' }
& $mx (Join-Path $root 'cad\model') (Join-Path $root 'cad\mesh')
# Drawing set DBF-DRW-001 (cad/drawings): SLDDRW, vector PDF, sheet PNGs for the review page
$dx = Join-Path $root 'cad\Drawings.exe'
& $csc /nologo /platform:x64 /out:$dx /r:"$sw\SolidWorks.Interop.sldworks.dll" /r:"$sw\SolidWorks.Interop.swconst.dll" (Join-Path $root 'cad\Drawings.cs')
if ($LASTEXITCODE -ne 0) { throw 'Drawings compile failed' }
& $dx (Join-Path $root 'cad\model') (Join-Path $root 'cad\drawings') (Join-Path $root 'cad\params.txt')
