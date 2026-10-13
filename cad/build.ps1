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
