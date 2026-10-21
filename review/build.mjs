// Assembles review/vtol-review.html from review/src/* + analysis/model.js + analysis/results.json
import { readFileSync, writeFileSync, existsSync, statSync, mkdirSync } from 'fs';
import { spawnSync } from 'child_process';
import { fileURLToPath } from 'url';
import { packMesh } from './mesh.mjs';
const r = p => readFileSync(new URL(p, import.meta.url), 'utf8');
const data = JSON.parse(r('../analysis/results.json'));
const v02 = JSON.parse(r('../analysis/results_v0.2_baseline.json'));
const aero = JSON.parse(r('../analysis/aero.json'));
// SOLIDWORKS Simulation results: key=value|unit|note
const sim = {};
for (const line of r('../cad/sim/results.txt').split(String.fromCharCode(10)).map(x => x.trim())) { if (!line || line.startsWith('#')) continue; const i = line.indexOf('='); sim[line.slice(0, i)] = parseFloat(line.slice(i + 1)); }
// CAD build parameters (numeric only)
const cadp = {};
for (const line of r('../cad/params.txt').split(String.fromCharCode(10)).map(x => x.trim())) { if (!line || line.startsWith('comp=')) continue; const i = line.indexOf('='); const v = parseFloat(line.slice(i + 1)); if (!isNaN(v) && !line.startsWith('foil=')) cadp[line.slice(0, i)] = v; }
// images embedded as data URIs (page stays self-contained)
// renders are embedded as JPEG (review/tojpeg.ps1, regenerated when the PNG is newer); falls back to the PNG elsewhere
const IMGDIR = new URL('./img/', import.meta.url); mkdirSync(IMGDIR, { recursive: true });
function b64(f) {
  const png = new URL(f, import.meta.url), jpg = new URL(f.split('/').pop().replace(/.png$/i, '.jpg'), IMGDIR);
  if (process.platform === 'win32' && (!existsSync(jpg) || statSync(jpg).mtimeMs < statSync(png).mtimeMs))
    spawnSync('powershell', ['-NoProfile', '-ExecutionPolicy', 'Bypass', '-File', fileURLToPath(new URL('./tojpeg.ps1', import.meta.url)), fileURLToPath(png), fileURLToPath(jpg)]);
  if (existsSync(jpg) && statSync(jpg).mtimeMs >= statSync(png).mtimeMs) return 'data:image/jpeg;base64,' + readFileSync(jpg).toString('base64');
  return 'data:image/png;base64,' + readFileSync(png).toString('base64');
}
const imgs = { fea_boom:'../cad/sim/FEA_Boom_vonMises.png', fea_spar:'../cad/sim/FEA_Spar_vonMises.png', therm_motor:'../cad/sim/THERM_Motor_steady.png' };
const IMG = {}; for (const [k, f] of Object.entries(imgs)) { try { IMG[k] = b64(f); } catch (e) { console.log('missing image', f); } }
// SOLIDWORKS drawing sheets (cad/Drawings.cs) stay PNG: crisp line art at ~20 KB a sheet
for (let i = 1; i <= 5; i++) { try { IMG['drw' + i] = 'data:image/png;base64,' + readFileSync(new URL(`../cad/drawings/DBF-DRW-001_sheet${i}.png`, import.meta.url)).toString('base64'); } catch (e) { console.log('missing drawing sheet', i); } }
const BASIS_VER = (r('../docs/DESIGN-BASIS.md').match(/\(v(\d+\.\d+(?:\.\d+)?)/) || [])[1];
if (!BASIS_VER) throw new Error('design-basis version not found');
const MESH = packMesh(); console.log('CAD mesh:', Object.keys(MESH.parts).length, 'parts,', MESH.comps.length, 'components,', MESH.tris, 'triangles');
const html = `<title>Lift + Cruise VTOL Review</title>
<meta name="description" content="Concept-stage engineering design review of a student lift + cruise VTOL">
<link rel="preconnect" href="https://fonts.googleapis.com"><link rel="preconnect" href="https://fonts.gstatic.com" crossorigin>
<link rel="stylesheet" href="https://fonts.googleapis.com/css2?family=Barlow+Semi+Condensed:wght@500;600&family=Sofia+Sans+Semi+Condensed:wght@700&family=Red+Hat+Mono:wght@400;500;600&family=Source+Sans+3:ital,wght@0,400;0,600;0,700;1,400&display=swap">
<style>
${r('./src/style.css')}
</style>
${r('./src/body.html').replace(/DESIGN-BASIS\.md v\d+\.\d+(?:\.\d+)?/g, 'DESIGN-BASIS.md v' + BASIS_VER)}
<script>const DATA = ${JSON.stringify(data)}; const V02 = ${JSON.stringify(v02)}; const SIM = ${JSON.stringify(sim)}; const AERO = ${JSON.stringify(aero)}; const IMG = ${JSON.stringify(IMG)}; const CADP = ${JSON.stringify(cadp)}; const MESH = ${JSON.stringify(MESH)};</script>
<script>
${r('../analysis/model.js')}
</script>
<script>
${r('./src/cadviewer.js')}
</script>
<script>
${r('./src/app.js')}
</script>
`;
writeFileSync(new URL('./vtol-review.html', import.meta.url), html);
console.log('wrote review/vtol-review.html', html.length, 'bytes');
// Standalone copy for double-click opening from File Explorer (full document skeleton, viewport meta)
const cut = html.indexOf('</style>') + '</style>'.length;
const local = '<!doctype html>\n<html lang="en"><head><meta charset="utf-8">\n'
  + '<meta name="viewport" content="width=device-width, initial-scale=1, viewport-fit=cover">\n'
  + html.slice(0, cut).replace('<style>', '<style>\n[hidden]{display:none!important}')
  + '\n</head><body>\n' + html.slice(cut) + '</body></html>\n';
writeFileSync(new URL('../VTOL-Review.html', import.meta.url), local);
console.log('wrote VTOL-Review.html (project root)');
