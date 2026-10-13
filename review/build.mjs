// Assembles review/vtol-review.html from review/src/* + analysis/model.js + analysis/results.json
import { readFileSync, writeFileSync } from 'fs';
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
const b64 = f => 'data:image/png;base64,' + readFileSync(new URL(f, import.meta.url)).toString('base64');
const imgs = { cad_iso:'../cad/images/VTOL_Concept_iso.png', cad_top:'../cad/images/VTOL_Concept_top.png', cad_side:'../cad/images/VTOL_Concept_side.png',
  cad_front:'../cad/images/VTOL_Concept_front.png', fea_boom:'../cad/sim/FEA_Boom_vonMises.png', fea_spar:'../cad/sim/FEA_Spar_vonMises.png', therm_motor:'../cad/sim/THERM_Motor_steady.png' };
const IMG = {}; for (const [k, f] of Object.entries(imgs)) { try { IMG[k] = b64(f); } catch (e) { console.log('missing image', f); } }
const html = `<title>Lift + Cruise VTOL Review</title>
<meta name="description" content="Concept-stage engineering design review of a student lift + cruise VTOL">
<link rel="preconnect" href="https://fonts.googleapis.com"><link rel="preconnect" href="https://fonts.gstatic.com" crossorigin>
<link rel="stylesheet" href="https://fonts.googleapis.com/css2?family=Archivo:wght@600;700;800&family=IBM+Plex+Mono:wght@400;600&family=IBM+Plex+Sans:wght@400;600;700&display=swap">
<style>
${r('./src/style.css')}
</style>
${r('./src/body.html')}
<script>const DATA = ${JSON.stringify(data)}; const V02 = ${JSON.stringify(v02)}; const SIM = ${JSON.stringify(sim)}; const AERO = ${JSON.stringify(aero)}; const IMG = ${JSON.stringify(IMG)}; const CADP = ${JSON.stringify(cadp)};</script>
<script>
${r('../analysis/model.js')}
</script>
<script>
${r('./src/viewer.js')}
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
