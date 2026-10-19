/* Review page logic. DATA = analysis/results.json (embedded at build). VTOLModel = analysis/model.js. */
(function(){
const D = DATA, $ = s=>document.querySelector(s), $$ = s=>Array.from(document.querySelectorAll(s));
const fmtUSD = v => '$'+Number(v).toLocaleString('en-US',{minimumFractionDigits:0,maximumFractionDigits:0});
const fmt = (v,d=1) => Number(v).toLocaleString('en-US',{minimumFractionDigits:d,maximumFractionDigits:d});
function get(path){
  let root = D, pth = path;
  if(path.startsWith('V02.')){ root = V02; pth = path.slice(4); }
  else if(path.startsWith('SIM.')) return SIM[path.slice(4)];
  else if(path.startsWith('AERO.')) return AERO[path.slice(5)];
  else if(path.startsWith('CADP.')) return CADP[path.slice(5)];
  let o = /^C\d/.test(pth) ? root.concepts : root; for(const k of pth.split('.')) o = o?.[k]; return o; }
const set = (id, txt) => { const e=document.getElementById(id); if(e) e.textContent = txt; };
const css = n => getComputedStyle(document.documentElement).getPropertyValue(n).trim();

// ---------- theme ----------
const modes = ['system','light','dark']; let mi = 0;
try{ const s = localStorage.getItem('vtolTheme'); if(s){ mi = Math.max(0,modes.indexOf(s)); } }catch(e){}
function applyTheme(){ const m=modes[mi]; if(m==='system') document.documentElement.removeAttribute('data-theme'); else document.documentElement.setAttribute('data-theme',m);
  $('#themeBtn').textContent = 'Theme: '+m; try{localStorage.setItem('vtolTheme',m);}catch(e){} redrawAll(); }
$('#themeBtn').addEventListener('click',()=>{ mi=(mi+1)%3; applyTheme(); });
if(window.matchMedia) matchMedia('(prefers-color-scheme: dark)').addEventListener?.('change',()=>redrawAll());

// ---------- navigation ----------
const secs = $$('section.sec');
function buildNav(){
  const side = document.createElement('ol'), tabs = $('#tabs');
  secs.forEach(s=>{
    const li = document.createElement('li'); li.dataset.id = s.id;
    li.innerHTML = `<a href="#${s.id}"><span class="n">${s.dataset.n}</span> ${s.dataset.title}</a>`;
    const subs = s.querySelectorAll('h3[id]');
    if(subs.length){ const ol=document.createElement('ol'); ol.className='sub';
      subs.forEach(h=>{ ol.insertAdjacentHTML('beforeend',`<li><a href="#${h.id}" data-sec="${s.id}">${h.textContent}</a></li>`); }); li.appendChild(ol); }
    side.appendChild(li);
    tabs.insertAdjacentHTML('beforeend',`<a href="#${s.id}" data-id="${s.id}"><span class="mono">${s.dataset.n}</span> ${s.dataset.title}</a>`);
    // in-section index + prev/next
    const i = secs.indexOf(s), prev = secs[i-1], next = secs[i+1];
    let h = `<div class="secidx"><div class="eyebrow">In this section</div><ol>`;
    subs.forEach(x=>h+=`<li><a href="#${x.id}" data-sec="${s.id}">${x.textContent}</a></li>`);
    h += `</ol><div class="pn">`;
    h += prev ? `<a class="prev" href="#${prev.id}"><span class="eyebrow">← Previous · ${prev.dataset.n}</span> ${prev.dataset.title}</a>` : '<span></span>';
    h += next ? `<a class="next" href="#${next.id}"><span class="eyebrow">Next · ${next.dataset.n} →</span> ${next.dataset.title}</a>` : '<span></span>';
    s.insertAdjacentHTML('beforeend', h+'</div></div>');
  });
  $('#side').appendChild(side);
}
function show(id, sub, focus){
  const target = secs.find(s=>s.id===id) || secs[0];
  secs.forEach(s=>s.classList.toggle('cur', s===target));
  const mark=(a,on)=>{ a.classList.toggle('cur',on); if(on) a.setAttribute('aria-current','true'); else a.removeAttribute('aria-current'); };
  $$('#side li[data-id]').forEach(li=>{ const on=li.dataset.id===target.id; li.classList.toggle('cur',on); mark(li.querySelector('a'),on); });
  $$('#tabs a').forEach(a=>{ const on=a.dataset.id===target.id; mark(a,on); if(on) a.scrollIntoView({block:'nearest',inline:'center'}); });
  try{ localStorage.setItem('vtolSec', target.id); }catch(e){}
  redrawSection(target.id);
  const dest = sub ? document.getElementById(sub) : null;
  if(dest) dest.scrollIntoView(); else window.scrollTo(0,0);
  if(focus){ const f = dest || target.querySelector('h2'); if(f){ if(!f.hasAttribute('tabindex')) f.setAttribute('tabindex','-1'); f.focus({preventScroll:true}); } }
}
function route(hash, focus){
  const h = (hash!==undefined ? hash : location.hash).replace(/^#/,'');
  if(!h){ let s=null; try{ s=localStorage.getItem('vtolSec'); }catch(e){} show(s||'overview'); return; }
  const el = document.getElementById(h); if(!el) return show('overview');
  const sec = el.closest('section.sec'); if(!sec) return show(secs.find(s=>s.classList.contains('cur'))?.id||'overview');
  show(sec.id, el===sec ? null : h, focus);
}
window.addEventListener('hashchange', ()=>route(undefined, true));
document.addEventListener('click', e=>{ const a=e.target.closest('a[href^="#"]'); if(!a) return; const h=a.getAttribute('href');
  if(a.classList.contains('skip')){ e.preventDefault(); const t=$('section.sec.cur h2')||$('#main'); t.setAttribute('tabindex','-1'); t.focus(); return; }
  if(!document.getElementById(h.slice(1))) return; e.preventDefault(); try{ history.pushState(null,'',h); }catch(err){} route(h, true); });

// ---------- data binding ----------
function fill(){
  $$('[data-k]').forEach(e=>{ let v = get(e.dataset.k); if(v===undefined||v===null||(typeof v==='number'&&isNaN(v))){ e.textContent='—'; return; }
    if(e.dataset.c==='txt'){ e.textContent = String(v); return; }
    if(e.dataset.m) v = v*Number(e.dataset.m);
    e.textContent = e.dataset.c==='usd' ? fmtUSD(v) : e.dataset.c==='int' ? Math.round(v).toLocaleString('en-US') : fmt(v, e.dataset.d!==undefined?Number(e.dataset.d):1); });
}

// ---------- derived numbers used in text ----------
const C0 = D.concepts.C0, s0 = C0.sized, B = D.base;
const cat = (bom, name) => bom.lines.filter(l=>l.cat===name).reduce((a,l)=>a+l.ext,0);
const CATS = ['Lift propulsion','Cruise propulsion','Controls','Energy','Avionics','Airframe','Test & safety','Spares'];
// lumped motor thermal from the steady FEA (section 12): tau = m c / (h A), h A = Q / (T_ss - T_amb)
// c = 470 J/kgK EST mass-weighted (copper 385, steel 460, aluminium 896); band 385-500 J/kgK
const TH = (()=>{ const Q=SIM.motor_Q, Tss=SIM.motor_T_steady, Ta=35, cP=470, mc=0.165*cP;
  if(!(Q>0&&Tss>Ta)) return null; const hA=Q/(Tss-Ta), tau=mc/hA, dT=Tss-Ta, T90=Ta+dT*(1-Math.exp(-90/tau));
  // T3 protocol: 3 x (90 s on, 30 s off), cooling with the same h A (prop wash gone -> conservative)
  let T=Ta; for(let k=0;k<3;k++){ T = Ta + dT + (T-Ta-dT)*Math.exp(-90/tau); if(k<2) T = Ta + (T-Ta)*Math.exp(-30/tau); }
  const lo=Ta+dT*(1-Math.exp(-90/(0.165*500/hA))), hi=Ta+dT*(1-Math.exp(-90/(0.165*385/hA)));
  return {hA, mc, tau, T90, T3: T, lo, hi, cP}; })();
function derived(){
  set('wingT', fmt(s0.chord*0.13,3)); set('segF', fmt(C0.geo.segs[0],2)); set('segR', fmt(C0.geo.segs[1],2));
  set('m3Empty', fmt(s0.m - s0.m_batt - B.mPayload,2));
  const Eus = s0.Epack_Wh*(1-B.reserve)*B.etaBatt;                     // usable Wh within reserve
  const PhLim = (Eus - s0.P_cruise*B.tCruise/3600)/((B.tHover+B.tTrans)/3600);
  set('mHov', fmt(PhLim,0));
  const PcLim = (Eus - s0.P_hover*(B.tHover+B.tTrans)/3600)/(B.tCruise/3600);
  set('mCr', fmt(PcLim,0));
  const CDtot = s0.CD0 + s0.CDi, CD0lim = CDtot*PcLim/s0.P_cruise - s0.CDi;
  set('mCD0', fmt(CD0lim,3));
  set('mPackDel', fmt(s0.E_Wh/(1-B.reserve),0));
  set('iUnit', fmt(s0.P_hover_per_motor/B.Vpack,1));
  const g=C0.geo, cg=C0.cg, L=g.xFront-g.xRear;
  set('smWith', fmt((g.hn-0.28)*100,1)); set('smNo', fmt(cg.SM_noPayload*100,1));
  const xc = cg.cgNoPayload, Tr = (g.xFront-xc)/L, Tf = (xc-g.xRear)/L; set('hoverSplit', fmt((Tr/Tf-1)*100,0));
  set('cgSplit', fmt(0.15*L/4*100,0));
  set('c1Over', fmtUSD(D.concepts.C1.bom.total-2000)); set('c2Over', fmtUSD(D.concepts.C2.bom.total-2000));
  const days = Math.round((Date.UTC(2027,3,30)-Date.UTC(2026,9,13))/864e5); set('kpiDays', days);
  if(TH){ ['thermLumped','thermLumped2','thermLumped3'].forEach(i=>set(i, fmt(TH.T90,0))); set('thermTau', fmt(TH.tau,0)); set('thermTau2', fmt(TH.tau,0));
    set('thermHA', fmt(TH.hA,3)); set('thermMC', fmt(TH.mc,0)); set('thermT3', fmt(TH.T3,0)); set('thermT3b', fmt(TH.T3,0)); set('thermLumped4', fmt(TH.T90,0)); set('thermBand', fmt(TH.lo,0)+'–'+fmt(TH.hi,0)); }
  // stopped-rotor drag: model baseline and R10 failure level per unit
  const q = 0.5*B.rho*B.Vc*B.Vc, baseD = q*C0.params.CdA_rotor, dCdA = (CD0lim - s0.CD0)*s0.S/C0.params.nLift;
  set('rotorBase', fmt(baseD,2)); set('rotorFail', fmt(baseD + q*dCdA,1));
  set('cgAft', fmt((g.hn-0.08)*100,1)); set('engage', fmt(C0.struct.engage,2)); set('engageD', fmt(C0.struct.engageD,0));
  set('iTrans', fmt(s0.I_trans,0)); set('iHovL', fmt(s0.I_hover_loaded,0));
  set('handRF', fmt(C0.struct.SIG_DESIGN/C0.struct.sigJ,2)); set('reserve2', fmtUSD(C0.bom.crash));
  set('longest', fmt(Math.max(B.span/2-0.08, ...C0.geo.segs),2)); set('longest2', fmt(Math.max(B.span/2-0.08, ...C0.geo.segs),2)); set('carMin', fmt(Math.max(B.span/2-0.08, ...C0.geo.segs)+0.03,2));
  set('marginPct', fmt((s0.t_cruise_avail_min/10-1)*100,0));
}

// ---------- tables ----------
function img(id, list){
  const box = $(id); if(!box) return;
  list.forEach(([key, cap])=>{ if(!IMG[key]) return;
    box.insertAdjacentHTML('beforeend', `<figure class="chart" style="margin:0"><figcaption>${cap}</figcaption><img class="cadimg" src="${IMG[key]}" alt="${cap}" loading="lazy"></figure>`); });
}
function tables(){
  img('#cadImages', [['cad_iso','Isometric'],['cad_top','Top'],['cad_side','Side'],['cad_front','Front']]);
  img('#simImages', [['fea_boom','Boom arm, von Mises at full thrust'],['fea_spar','Spar + joiner, von Mises at 4.5 g'],['therm_motor','Lift motor, steady-state temperature']]);
  // v0.2 -> v0.3 change table
  const ctb = $('#changeTable tbody'), b2 = V02.concepts.C0.bom, b3 = C0.bom; let t2=0,t3=0;
  CATS.forEach(cn=>{ const v2=cat(b2,cn), v3=cat(b3,cn); t2+=v2; t3+=v3;
    ctb.insertAdjacentHTML('beforeend', `<tr><td>${cn}</td><td class="n">${fmt(v2,2)}</td><td class="n">${fmt(v3,2)}</td><td class="n ${v2-v3>=0?'ok':'bad'}">${fmt(v2-v3,2)}</td></tr>`); });
  ctb.insertAdjacentHTML('beforeend', `<tr><td>Compliance fees</td><td class="n">${fmt(cat(b2,'Compliance'),2)}</td><td class="n">outside cap</td><td class="n">—</td></tr>`);
  ctb.insertAdjacentHTML('beforeend', `<tr class="total"><td>Hardware goods</td><td class="n">${fmt(t2,2)}</td><td class="n">${fmt(t3,2)}</td><td class="n ${t2-t3>=0?'ok':'bad'}">${fmt(t2-t3,2)}</td></tr>`);
  // parts
  const pt = $('#partsTable tbody'); let tot=0;
  C0.cg.items.forEach(i=>{ tot+=i.m; pt.insertAdjacentHTML('beforeend',`<tr><td>${i.name}</td><td class="n">${fmt(i.m,3)}</td><td class="n">${fmt(i.x,3)}</td><td><small>${i.src}</small></td></tr>`); });
  pt.insertAdjacentHTML('beforeend',`<tr class="total"><td>Total (MTOW)</td><td class="n">${fmt(tot,3)}</td><td></td><td><small>matches sized m = ${fmt(s0.m,3)} kg</small></td></tr>`);
  // concept cards
  const cc = $('#conceptCards');
  for(const k of ['C0','C1','C2','C3']){ const c=D.concepts[k], s=c.sized, fits=c.bom.total<=2000;
    cc.insertAdjacentHTML('beforeend',`<div class="card${k==='C0'?' pick':''}"><span class="tag chip ${k==='C0'?'met':fits?'open':'fail'}">${k==='C0'?'baseline':fits?'alternative':'over cap'}</span><h3>${k} · ${c.label}</h3>
      <dl class="kv"><dt>Lift unit</dt><dd>${c.params.nLift} × ${c.motor}</dd><dt>MTOW</dt><dd>${fmt(s.m,2)} kg</dd><dt>Hover / cruise</dt><dd>${fmt(s.P_hover,0)} W / ${fmt(s.P_cruise,0)} W</dd>
      <dt>L/D cruise</dt><dd>${fmt(s.LD,1)}</dd><dt>T/W · motor-out</dt><dd>${fmt(s.TW,2)} · ${s.TW_motorOut?fmt(s.TW_motorOut,2):'none'}</dd><dt>Pack</dt><dd>${c.pick.id}</dd><dt>Committed spend</dt><dd class="${fits?'ok-t':'bad-t'}">${fmtUSD(c.bom.total)}</dd></dl></div>`); }
  // pugh
  const c1=D.concepts.C1.sized, c2=D.concepts.C2.sized, b0=C0.bom.total, bb1=D.concepts.C1.bom.total, bb2=D.concepts.C2.bom.total;
  const rows = [
    ['Spend vs $2,000 cap (R6)', `${fmtUSD(b0)} / ${fmtUSD(bb1)} / ${fmtUSD(bb2)}`, -1, -1],
    ['MTOW', `${fmt(s0.m,2)} / ${fmt(c1.m,2)} / ${fmt(c2.m,2)} kg`, -1, -1],
    ['Hover power', `${fmt(s0.P_hover,0)} / ${fmt(c1.P_hover,0)} / ${fmt(c2.P_hover,0)} W`, c1.P_hover<=s0.P_hover?1:-1, c2.P_hover<=s0.P_hover?1:-1],
    ['Cruise L/D', `${fmt(s0.LD,1)} / ${fmt(c1.LD,1)} / ${fmt(c2.LD,1)}`, c1.LD>s0.LD?1:-1, c2.LD>s0.LD?1:-1],
    ['Hover motor-out (R14)', `none / ${fmt(c1.TW_motorOut,2)} / ${fmt(c2.TW_motorOut,2)} T/W`, 1, 1],
    ['Build effort', '2 / 4 / 2 booms; 5 / 9 / 9 motors', -1, 0],
    ['Flight controller', 'F405 / F405 (ailerons on a Y-lead) / H743 for OctaQuad', 0, -1],
    ['Prop parking in cruise', '4 / 8 / 8 stopped props', -1, -1],
  ];
  const tb = $('#pugh tbody'); let s1=0,s2=0;
  const sym = v => v>0?'<td class="n ok">+</td>':v<0?'<td class="n bad">−</td>':'<td class="n">0</td>';
  rows.forEach(r=>{ s1+=r[2]; s2+=r[3]; tb.insertAdjacentHTML('beforeend',`<tr><td>${r[0]}</td><td><small>${r[1]}</small></td><td class="n">0</td>${sym(r[2])}${sym(r[3])}</tr>`); });
  tb.insertAdjacentHTML('beforeend',`<tr class="total"><td>Net score</td><td></td><td class="n">0</td><td class="n">${s1}</td><td class="n">${s2}</td></tr>`);
  // roll-up
  const groups = [['Lift units','m_lift'],['Booms + clamps','m_booms'],['Wing + spar','m_wing'],['Tail','m_tail'],['Fixed (pod, gear, avionics, pusher, servos, wiring, payload)','m_fixed'],['Battery','m_batt'],['MTOW','m']];
  const ru = $('#rollup tbody');
  groups.forEach(([n,k])=>ru.insertAdjacentHTML('beforeend',`<tr${k==='m'?' class="total"':''}><td>${n}</td><td class="n">${fmt(C0.design[k],2)}</td><td class="n">${fmt(s0[k],2)}</td><td class="n">${fmt(c1[k],2)}</td><td class="n">${fmt(c2[k],2)}</td><td><small>${k==='m_batt'?'design: E/e_spec; real: pack weight':k==='m_wing'?'F11':k==='m'?'F12':'sizing.mjs'}</small></td></tr>`));
  // calibration from the concepts' own thrust tables
  const ct = $('#calib tbody');
  [['X4110S 17 in (C0)', D.concepts.C0.params], ['X3515S 16 in (C3)', D.concepts.C3.params], ['V3508 15 in (C1/C2)', D.concepts.C1.params]].forEach(([n,pp])=>{
    pp.thrustTable.forEach(([T,P])=>{ const f=VTOLModel.calibFMeta(T,P,pp.liftD,1.225); ct.insertAdjacentHTML('beforeend',`<tr><td>${n}</td><td class="n">${T}</td><td class="n">${fmt(P,1)}</td><td class="n">${fmt(f*P,1)}</td><td class="n">${fmt(f,3)}</td></tr>`); }); });
  // packs
  const pk = $('#packTable tbody');
  D.packs.forEach((p,i)=>{ const sw=D.sweepC0[i]; pk.insertAdjacentHTML('beforeend',`<tr${p.id===C0.pick.id?' class="chosen"':''}><td>${p.id}</td><td>${p.chem}</td><td class="n">${fmt(p.Wh,1)}</td><td class="n">${fmt(p.kg,3)}</td><td class="n">${fmt(p.Wh/p.kg,0)}</td><td class="n">${p.contA}</td><td class="n">${fmt(p.usd,2)}</td><td class="n">${fmt(sw.m,2)}</td><td class="n ${sw.cruise_min>=D.cruiseMarginMin?'ok':'warn'}">${fmt(sw.cruise_min,1)}${sw.cruise_min<D.cruiseMarginMin?' <small>below margin</small>':''}</td></tr>`); });
  // landing
  const lt = $('#landTable tbody');
  C0.struct.strokes.forEach(s=>{ const ns=s.nSpring||2*s.n, cls = ns>10?'bad':ns>8?'warn':'ok'; lt.insertAdjacentHTML('beforeend',`<tr><td class="n">${s.stroke*1000}</td><td class="n">${fmt(s.n,1)}</td><td class="n">${fmt(ns,1)}</td><td class="${cls}">${ns>10?'damages mounts/booms':ns>8?'marginal':'acceptable'}</td></tr>`); });
  // BOM side by side
  const bs = $('#bomSide tbody'), bm = k=>D.concepts[k].bom;
  CATS.forEach(cn=>bs.insertAdjacentHTML('beforeend',`<tr><td>${cn}</td>${['C0','C1','C2'].map(k=>`<td class="n">${fmt(cat(bm(k),cn),2)}</td>`).join('')}</tr>`));
  [['Goods subtotal','goods'],['Sales/use tax 10.25 %','tax'],['Shipping allowance (est.)','shipping']].forEach(([n,k])=>
    bs.insertAdjacentHTML('beforeend',`<tr><td>${n}</td>${['C0','C1','C2'].map(c=>`<td class="n">${fmt(bm(c)[k],2)}</td>`).join('')}</tr>`));
  bs.insertAdjacentHTML('beforeend',`<tr class="total"><td>Committed spend</td>${['C0','C1','C2'].map(c=>`<td class="n ${bm(c).total<=2000?'ok':'bad'}">${fmt(bm(c).total,2)}</td>`).join('')}</tr>`);
  bs.insertAdjacentHTML('beforeend',`<tr><td>Reserve left under $2,000</td>${['C0','C1','C2'].map(c=>`<td class="n ${bm(c).crash>=100?'ok':'bad'}">${fmt(bm(c).crash,2)}</td>`).join('')}</tr>`);
  // BOM full
  const bt = $('#bomTable tbody');
  C0.bom.lines.forEach(l=>bt.insertAdjacentHTML('beforeend',`<tr><td>${l.cat}</td><td>${l.item}</td><td class="n">${l.qty}</td><td class="n">${fmt(l.unit,2)}</td><td class="n">${fmt(l.ext,2)}</td><td><small>${l.src}${l.note?' · '+l.note:''}</small></td></tr>`));
  bt.insertAdjacentHTML('beforeend',`<tr class="total"><td colspan="4">Goods</td><td class="n">${fmt(C0.bom.goods,2)}</td><td><small>sum of lines</small></td></tr>`);
  (C0.bom.fees||[]).forEach(f=>bt.insertAdjacentHTML('beforeend',`<tr><td>Fees (outside cap)</td><td>${f.item}</td><td class="n">1</td><td class="n">${fmt(f.usd,2)}</td><td class="n">—</td><td><small>${f.src}</small></td></tr>`));
  // CG table
  const cgt = $('#cgTable tbody'); let mm=0,mo=0;
  C0.cg.items.forEach(i=>{ mm+=i.m; mo+=i.m*i.x; cgt.insertAdjacentHTML('beforeend',`<tr><td>${i.name}</td><td class="n">${fmt(i.m,3)}</td><td class="n">${fmt(i.x,3)}</td><td class="n">${fmt(i.m*i.x,4)}</td></tr>`); });
  cgt.insertAdjacentHTML('beforeend',`<tr class="total"><td>Total · CG = Σmx/Σm</td><td class="n">${fmt(mm,3)}</td><td class="n">${fmt(mo/mm,3)}</td><td class="n">${fmt(mo,4)}</td></tr>`);
  const st = $('#stabTable tbody'), G3 = ()=>['C0','C1','C2'].map(c=>D.concepts[c]);
  [['Wing lift slope a_w (/rad)', c=>c.geo.aw, 2, 'Helmbold: 2πA/(2+√(A²+4))'],
   ['Tail volume V_h', c=>c.geo.Vh, 2, 'S_h l_h /(S c), target 0.35 est.'],
   ['Fin volume V_v', c=>c.geo.Vv, 3, 'S_v l_h /(S b), target 0.030 est.'],
   ['Downwash dε/dα', c=>c.geo.deda, 3, '2 a_w/(π A)'],
   ['Neutral point, % MAC', c=>c.geo.hn*100, 1, '0.25 + η V_h (a_t/a_w)(1−dε/dα)'],
   ['Static margin at 28 % CG, % MAC', c=>c.geo.SM*100, 1, 'h_n − h_cg'],
   ['Static margin, no payload, % MAC', c=>c.cg.SM_noPayload*100, 1, 'CG aft without 0.5 kg'],
   ['Thrust centroid, % MAC', c=>c.cg.pct.thrust, 1, '(x_front + x_rear)/2'],
   ['Tail arm l_h, m', c=>c.geo.lh, 2, 'wing AC to tail AC']].forEach(([n,f,d,fx])=>
     st.insertAdjacentHTML('beforeend',`<tr><td>${n}</td>${G3().map(c=>`<td class="n">${fmt(f(c),d)}</td>`).join('')}<td><small>${fx}</small></td></tr>`));
  // simulation vs hand
  const sm = $('#simTable tbody'), pct=(a,b)=>((a-b)/b*100);
  [['Boom static (1.5 mm mesh)','von Mises max, MPa', SIM.boom_vm_max_fine, SIM.boom_hand_sigma, 'RF '+fmt(275/SIM.boom_vm_max_fine,1)+' on yield'],
   ['Boom static (3 mm mesh)','von Mises max, MPa', SIM.boom_vm_max, SIM.boom_hand_sigma, 'mesh change '+fmt((SIM.boom_vm_max_fine/SIM.boom_vm_max-1)*100,0)+' %'],
   ['Boom static','tip deflection, mm', SIM.boom_disp_max, SIM.boom_hand_defl, '—'],
   ['Boom modal','first mode, Hz', SIM.boom_f1, SIM.boom_hand_fn, 'tap test §6 M2'],
   ['Spar + joiner static, 2 mm mesh (idealised OD)','von Mises max, MPa', SIM.spar_vm_max_fine, SIM.spar_hand_sigJ, 'hand calc governs: RF '+fmt(C0.struct.SIG_DESIGN/C0.struct.sigJ,2)],
   ['Spar + joiner static, 3 mm mesh','von Mises max, MPa', SIM.spar_vm_max, SIM.spar_hand_sigJ, 'mesh change '+fmt((SIM.spar_vm_max_fine/SIM.spar_vm_max-1)*100,0)+' %'],
   ['Lift motor thermal','steady temperature, °C', SIM.motor_T_steady, SIM.motor_T_steady_hand, TH?('90 s ≈ '+fmt(TH.T90,0)+' °C; 3 back-to-back ≈ '+fmt(TH.T3,0)+' °C'):'—']]
   .forEach(([a,q,f,h,m])=>{ if(f===undefined||h===undefined) return; const d=pct(f,h); sm.insertAdjacentHTML('beforeend',`<tr><td>${a}</td><td>${q}</td><td class="n">${fmt(f,1)}</td><td class="n">${fmt(h,1)}</td><td class="n ${Math.abs(d)<=15?'ok':'warn'}">${d>0?'+':''}${fmt(d,1)} % <small>${Math.abs(d)<=15?'within ±15 %':'outside ±15 %'}</small></td><td>${m}</td></tr>`); });
  const at = $('#aeroTable tbody');
  [['Wing lift slope, /rad', AERO.wing_CLa_per_rad, AERO.helmbold_CLa_per_rad, 2, 'Helmbold estimate used in the sizing'],
   ['Aircraft lift slope (wing + tail), /rad', AERO.full_CLa_per_rad, null, 2, '—'],
   ['Neutral point, % MAC', AERO.neutral_point_pct_mac, AERO.sizing_neutral_point_pct_mac, 1, 'tail-volume formula'],
   ['Static margin at 28 % CG, % MAC', AERO.static_margin_pct_at_28, C0.geo.SM*100, 1, '—'],
   ['Span efficiency (wing, inviscid)', AERO.wing_span_efficiency_inviscid, AERO.sizing_e, 3, 'sizing uses Oswald e = 0.80 (includes viscous and interference losses)']]
   .forEach(([q,v,e,d,cmt])=>at.insertAdjacentHTML('beforeend',`<tr><td>${q}</td><td class="n">${fmt(v,d)}</td><td class="n">${e===null?'—':fmt(e,d)}</td><td><small>${cmt}</small></td></tr>`));
}

// ---------- sortable ----------
function sortable(){
  $$('th.sort').forEach(th=>{ const btn=document.createElement('button'); btn.type='button'; btn.className='thbtn';
    while(th.firstChild) btn.appendChild(th.firstChild); th.appendChild(btn); th.setAttribute('aria-sort','none');
    const go=()=>{
    const table=th.closest('table'), i=[...th.parentNode.children].indexOf(th), body=table.tBodies[0];
    const rows=[...body.rows].filter(r=>!r.classList.contains('total')), totals=[...body.rows].filter(r=>r.classList.contains('total'));
    const dir = th.getAttribute('aria-sort')==='ascending'?-1:1;
    table.querySelectorAll('th.sort').forEach(x=>x.setAttribute('aria-sort','none'));
    th.setAttribute('aria-sort', dir===1?'ascending':'descending');
    const val = r=>{ const t=r.cells[i]?.textContent.trim()||''; const n=parseFloat(t.replace(/[$,≈+]/g,'')); return isNaN(n)?t.toLowerCase():n; };
    rows.sort((a,b)=>{ const x=val(a), y=val(b); return (x>y?1:x<y?-1:0)*dir; });
    rows.concat(totals).forEach(r=>body.appendChild(r)); };
    btn.addEventListener('click',go); });
}

// ---------- SVG helpers ----------
const NS='http://www.w3.org/2000/svg';
function el(tag, attrs, parent, text){ const e=document.createElementNS(NS,tag); for(const k in attrs) e.setAttribute(k,attrs[k]); if(text!==undefined) e.textContent=text; if(parent) parent.appendChild(e); return e; }
function clear(svg){ while(svg.firstChild) svg.removeChild(svg.firstChild); }
function describe(svg, text){ svg.setAttribute('aria-label', text); }
const sgn = v => (v>0?'+':v<0?'−':'')+fmt(Math.abs(v),2);

// ---------- live chain + sliders ----------
const SL = [
  {k:'Vc', l:'Cruise speed Vc', u:'m/s', min:16, max:26, st:0.5, src:'R11'},
  {k:'Vs', l:'Stall speed Vs', u:'m/s', min:9, max:15, st:0.5, src:'R11'},
  {k:'CLmax', l:'CLmax', u:'', min:0.9, max:1.5, st:0.05, est:1},
  {k:'span', l:'Span b', u:'m', min:2.0, max:3.0, st:0.1, src:'R3'},
  {k:'tCruise', l:'Cruise time', u:'min', min:5, max:30, st:1, scale:60, src:'R10'},
  {k:'tHover', l:'Hover time', u:'s', min:60, max:300, st:10, src:'R10'},
  {k:'reserve', l:'Energy reserve', u:'%', min:10, max:35, st:1, scale:0.01, src:'R10'},
  {k:'CD0_clean', l:'CD0 clean', u:'', min:0.020, max:0.060, st:0.002, est:1},
  {k:'CdA_rotor', l:'Stopped-rotor CdA', u:'m²', min:0.001, max:0.010, st:0.0005, est:1},
  {k:'eSpec', l:'Pack specific energy', u:'Wh/kg', min:150, max:260, st:5, src:'PB §2'},
  {k:'mPayload', l:'Payload allowance', u:'kg', min:0, max:2, st:0.1, src:'R12'},
  {k:'kCtrl', l:'Hover control margin', u:'×', min:1.0, max:1.3, st:0.01, est:1},
];
let chainKey='C0', P={};
function resetParams(){ const c=D.concepts[chainKey].params; P = {...c}; delete P.mBattFixed; delete P.EBattFixed; P.Vpack=22.2; }
function buildSliders(){
  const box=$('#sliders'); box.innerHTML='';
  SL.forEach(s=>{ const raw = P[s.k]/(s.scale||1);
    box.insertAdjacentHTML('beforeend',`<div class="sl"><label for="sl_${s.k}"><span>${s.l} ${s.est?'<span class="est">est</span>':`<small>${s.src}</small>`}</span><output id="o_${s.k}">${fmt(raw, s.st<0.01?4:s.st<1?2:0)} ${s.u}</output></label><input type="range" id="sl_${s.k}" min="${s.min}" max="${s.max}" step="${s.st}" value="${raw}" aria-valuetext="${fmt(raw, s.st<0.01?4:s.st<1?2:0)} ${s.u}"></div>`); });
  box.insertAdjacentHTML('beforeend',`<div class="sl" style="align-self:end"><button type="button" class="themebtn" id="slReset">Reset to review values</button></div>`);
  SL.forEach(s=>$('#sl_'+s.k).addEventListener('input',e=>{ const v=Number(e.target.value); P[s.k]=v*(s.scale||1); const vt=`${fmt(v, s.st<0.01?4:s.st<1?2:0)} ${s.u}`; $('#o_'+s.k).textContent=vt; e.target.setAttribute('aria-valuetext',vt); renderChain(); drawTornado(); announceChain(); }));
  $('#slReset').addEventListener('click',()=>{ resetParams(); buildSliders(); renderChain(); drawTornado(); });
}
let announceT=0;
function announceChain(){ clearTimeout(announceT); announceT=setTimeout(()=>{ const r=VTOLModel.size(P); $('#chainLive').textContent=`MTOW ${fmt(r.m,2)} kg, hover power ${fmt(r.P_hover,0)} W, cruise power ${fmt(r.P_cruise,0)} W, hover thrust-to-weight ${fmt(r.TW,2)}.`; }, 600); }
function renderChain(){
  const r = VTOLModel.size(P), c=$('#chain');
  const box=(l,v,f,cls='')=>`<div class="box"><div class="l">${l}</div><div class="v ${cls}">${v}</div><div class="f">${f}</div></div>`;
  const ar='<span class="arr" aria-hidden="true">→</span>';
  c.innerHTML = [
    box('MTOW (closed)', fmt(r.m,2)+' kg', `F12, ${r.iter} iterations`, r.m<25?'ok-t':'bad-t'),
    box('Thrust per unit', fmt(r.Tunit_g,0)+' g', `F1: m g / ${r.nUnits}`),
    box('FM·η at hover', fmt(r.FMeta,3), 'maker table, interpolated'),
    box('Hover power', fmt(r.P_hover,0)+' W', `F2–F3: n T^1.5/√(2ρA)${P.coax?' × '+P.kCoax:''} × ${P.kCtrl}/FMη`),
    box('Wing area', fmt(r.S,3)+' m²', `F4: 2mg/(ρ Vs² CLmax); AR ${fmt(r.AR,1)}`),
    box('Cruise CL / CD0', fmt(r.CL,3)+' / '+fmt(r.CD0,4), `F5–F6: CD0 + ΣCdA/S = ${fmt(P.CD0_clean,3)} + ${fmt(r.CdA_extra,4)}/S`),
    box('L/D', fmt(r.LD,2), 'F6: CL/(CD0 + CL²/(π e AR))'),
    box('Cruise power', fmt(r.P_cruise,0)+' W', `F7: (W/(L/D)) V/(${P.etaProp}×${P.etaMEc})`),
    box('Mission energy', fmt(r.E_Wh,0)+' Wh', `F8: P_h(${P.tHover}+${P.tTrans}s) + P_c·${P.tCruise/60} min`),
    box('Pack energy', fmt(r.Epack_req_Wh,0)+' Wh', `F9: E/((1−${fmt(P.reserve,2)})×${P.etaBatt})`),
    box('Battery mass', fmt(r.m_batt,2)+' kg', `F10: E_pack / ${P.eSpec} Wh/kg`),
    box('Hover T/W', fmt(r.TW,2), 'n T_max / m g (R13 ≥ 1.6)', r.TW>=1.6?'ok-t':'bad-t'),
  ].join(ar);
}
function drawTornado(){
  const svg=$('#tornado'); if(!svg || !svg.closest('section').classList.contains('cur')) return; clear(svg);
  const base = VTOLModel.size(P).m, keys=['Vc','Vs','CLmax','span','tCruise','tHover','reserve','CD0_clean','CdA_rotor','eSpec','mPayload','kCtrl'];
  const res = keys.map(k=>{ const lo=VTOLModel.size({...P,[k]:P[k]*0.8}).m-base, hi=VTOLModel.size({...P,[k]:P[k]*1.2}).m-base; return {k, lo, hi, span:Math.abs(hi-lo)}; }).sort((a,b)=>b.span-a.span);
  const W=640, x0=190, x1=620, mid=(x0+x1)/2, maxA=Math.max(...res.map(r=>Math.max(Math.abs(r.lo),Math.abs(r.hi))))||1;
  const sx = v=>mid + v/maxA*(x1-mid), rowH=20, top=26;
  svg.setAttribute('viewBox',`0 0 ${W} ${top+res.length*rowH+30}`);
  el('text',{x:mid,y:14,'text-anchor':'middle'},svg,`Δ MTOW, kg (base ${fmt(base,2)} kg)`);
  [-maxA,-maxA/2,0,maxA/2,maxA].forEach(t=>{ el('line',{x1:sx(t),x2:sx(t),y1:top-4,y2:top+res.length*rowH,class:t?'grid':'axis'},svg); el('text',{x:sx(t),y:top+res.length*rowH+14,'text-anchor':'middle'},svg,(t>0?'+':'')+fmt(t,2)); });
  const lab = Object.fromEntries(SL.map(s=>[s.k,s.l]));
  res.forEach((r,i)=>{ const y=top+i*rowH;
    el('text',{x:x0-8,y:y+13,'text-anchor':'end'},svg,lab[r.k]);
    el('rect',{x:Math.min(sx(r.lo),mid),y:y+3,width:Math.abs(sx(r.lo)-mid),height:13,fill:css('--c1'),'fill-opacity':0.35,stroke:css('--c1'),'stroke-width':1,rx:2},svg);
    el('rect',{x:Math.min(sx(r.hi),mid),y:y+3,width:Math.abs(sx(r.hi)-mid),height:13,fill:css('--c1'),rx:2},svg); });
  el('rect',{x:x0,y:top+res.length*rowH+20,width:10,height:10,fill:css('--c1'),'fill-opacity':0.35,stroke:css('--c1')},svg); el('text',{x:x0+14,y:top+res.length*rowH+29},svg,'input −20 %');
  el('rect',{x:x0+100,y:top+res.length*rowH+20,width:10,height:10,fill:css('--c1')},svg); el('text',{x:x0+114,y:top+res.length*rowH+29},svg,'input +20 %');
  describe(svg, `Tornado chart: change in MTOW from a base of ${fmt(base,2)} kg when each input moves ±20 %, largest first. `+res.map(r=>`${lab[r.k]}: ${sgn(r.lo)} kg at −20 %, ${sgn(r.hi)} kg at +20 %`).join('; ')+'.');
}
function drawPack(){
  const svg=$('#packChart'); if(!svg) return; clear(svg);
  const data=D.sweepC0, W=640,H=290,l=56,r=56,t=30,b=70;
  const xs = i=> l + (i+0.5)*(W-l-r)/data.length;
  const ms=data.map(d=>d.m), mMin=Math.floor((Math.min(...ms)-0.1)*2)/2, mMax=Math.ceil((Math.max(...ms)+0.1)*2)/2, cMax=Math.ceil(Math.max(...data.map(d=>d.cruise_min))/10)*10+10;
  const ym = v=> t+(H-t-b)*(1-(v-mMin)/(mMax-mMin)), yc = v=> t+(H-t-b)*(1-v/cMax);
  for(let v=mMin; v<=mMax+1e-9; v+=0.5){ el('line',{x1:l,x2:W-r,y1:ym(v),y2:ym(v),class:'grid'},svg); el('text',{x:l-6,y:ym(v)+4,'text-anchor':'end'},svg,fmt(v,1)); }
  for(let v=0; v<=cMax; v+=10) el('text',{x:W-r+6,y:yc(v)+4},svg,v);
  el('text',{x:14,y:t+80,transform:`rotate(-90 14 ${t+80})`,'text-anchor':'middle'},svg,'MTOW, kg');
  el('text',{x:W-12,y:t+80,transform:`rotate(90 ${W-12} ${t+80})`,'text-anchor':'middle'},svg,'cruise available, min');
  el('line',{x1:l,x2:W-r,y1:yc(10),y2:yc(10),stroke:css('--bad'),'stroke-dasharray':'5 4'},svg); el('text',{x:W-r-4,y:yc(10)+13,'text-anchor':'end',fill:css('--bad')},svg,'R10: 10 min');
  el('rect',{x:l+6,y:t-14,width:10,height:10,fill:css('--c2'),opacity:0.55},svg); el('text',{x:l+20,y:t-5},svg,'cruise min (right axis)');
  el('circle',{cx:l+170,cy:t-9,r:4,fill:css('--c3')},svg); el('text',{x:l+178,y:t-5},svg,'MTOW kg (left axis)');
  const bw=(W-l-r)/data.length*0.5;
  data.forEach((d,i)=>{ el('rect',{x:xs(i)-bw/2,y:yc(d.cruise_min),width:bw,height:yc(0)-yc(d.cruise_min),fill:css(i===0?'--accent':'--c2'),opacity:i===0?1:0.55,rx:2},svg);
    el('text',{x:xs(i),y:H-b+14,'text-anchor':'end',transform:`rotate(-30 ${xs(i)} ${H-b+14})`},svg,d.pack.replace(' flight pack','').replace('WREKD ','').replace('Tattu ','Tattu '));
  });
  let pth=''; data.forEach((d,i)=>pth+=(i?'L':'M')+xs(i)+' '+ym(d.m)); el('path',{d:pth,fill:'none',stroke:css('--c3'),'stroke-width':2},svg);
  data.forEach((d,i)=>{ el('circle',{cx:xs(i),cy:ym(d.m),r:3.5,fill:css('--c3')},svg); el('text',{x:xs(i),y:ym(d.m)-8,'text-anchor':'middle'},svg,fmt(d.m,2)); });
  if(mMax>=8){ el('line',{x1:l,x2:W-r,y1:ym(8),y2:ym(8),stroke:css('--warn'),'stroke-dasharray':'2 3'},svg); el('text',{x:W-r-4,y:ym(8)-4,'text-anchor':'end',fill:css('--warn')},svg,'R2: 8 kg target floor'); }
  const mg=D.cruiseMarginMin; el('line',{x1:l,x2:W-r,y1:yc(mg),y2:yc(mg),stroke:css('--ok'),'stroke-dasharray':'2 3'},svg); el('text',{x:l+4,y:yc(mg)-4,fill:css('--ok')},svg,`design margin ${fmt(mg,1)} min`);
  describe(svg, 'Battery packs on C0, MTOW and cruise minutes available (R10 needs 10 min; design margin '+fmt(D.cruiseMarginMin,1)+' min). '+data.map(d=>`${d.pack}: ${fmt(d.m,2)} kg, ${fmt(d.cruise_min,1)} min`).join('; ')+'.');
}
function drawCap(){
  const svg=$('#capChart'); if(!svg) return; clear(svg);
  const b=C0.bom, W=640, l=20, r=20, y0=40, h=46, total=2000, x=v=>l+(W-l-r)*v/total;
  const cols=['--c1','--c2','--c3','--c4','--c5','--c6','--c1','--c2'];
  const parts = CATS.map((cn,i)=>({n:cn, v:cat(b,cn), col:cols[i], op:i>=6?0.55:0.9}));
  parts.push({n:'Tax', v:b.tax, col:'--fg-3', op:0.7}, {n:'Shipping (est.)', v:b.shipping, col:'--line-2', op:0.9});
  let acc=0;
  parts.forEach(p=>{ el('rect',{x:x(acc),y:y0,width:Math.max(0.5,x(acc+p.v)-x(acc)),height:h,fill:css(p.col),opacity:p.op},svg);
    if(x(acc+p.v)-x(acc)>34) el('text',{x:(x(acc)+x(acc+p.v))/2,y:y0+h+14,'text-anchor':'middle',fill:css('--fg-2'),'font-size':'11'},svg,fmtUSD(p.v)); acc+=p.v; });
  el('rect',{x:x(acc),y:y0,width:Math.max(0,x(total)-x(acc)),height:h,fill:'none',stroke:css('--ok'),'stroke-dasharray':'5 4'},svg);
  if(total>acc) el('text',{x:(x(acc)+x(total))/2,y:y0+h/2+4,'text-anchor':'middle',fill:css('--ok'),'font-weight':'700'},svg,'reserve '+fmtUSD(total-acc));
  el('line',{x1:x(total),x2:x(total),y1:y0-14,y2:y0+h+10,stroke:css('--bad'),'stroke-width':2},svg);
  el('text',{x:x(total),y:y0-18,'text-anchor':'end',fill:css('--bad')},svg,'$2,000 cap');
  el('text',{x:l,y:y0-18},svg,'committed '+fmtUSD(acc));
  let lx=l, ly=y0+h+40;
  parts.forEach(p=>{ const w=16+p.n.length*6.2+16; if(lx+w>W-r){ lx=l; ly+=18; }
    el('rect',{x:lx,y:ly-9,width:10,height:10,fill:css(p.col),opacity:p.op},svg); el('text',{x:lx+14,y:ly},svg,p.n); lx+=w; });
  svg.setAttribute('viewBox', '0 0 640 '+(ly+14));
  describe(svg, `C0 committed spend ${fmtUSD(acc)} against the $2,000 cap, leaving ${fmtUSD(total-acc)} reserve. `+parts.map(p=>`${p.n} ${fmtUSD(p.v)}`).join('; ')+'.');
}

function drawDuty(){
  const svg=$('#dutyChart'); if(!svg) return; clear(svg);
  const Ph=s0.P_hover, Pc=s0.P_cruise, ph=[['hover',60,Ph],['trans',20,Ph],['cruise',600,Pc],['trans',20,Ph],['hover',60,Ph]];
  const T=ph.reduce((a,p)=>a+p[1],0), W=640,H=240,l=56,r=16,t=16,b=40, pmax=1000;
  const x=s=>l+(W-l-r)*s/T, y=p=>t+(H-t-b)*(1-p/pmax);
  for(let p=0;p<=pmax;p+=250){ el('line',{x1:l,x2:W-r,y1:y(p),y2:y(p),class:'grid'},svg); el('text',{x:l-6,y:y(p)+4,'text-anchor':'end'},svg,p+' W'); }
  let s=0, d=`M${x(0)} ${y(0)}`; ph.forEach(([n,dt,p])=>{ d+=`L${x(s)} ${y(p)}L${x(s+dt)} ${y(p)}`; s+=dt; }); d+=`L${x(T)} ${y(0)}Z`;
  el('path',{d,fill:css('--accent'),opacity:0.18,stroke:css('--accent'),'stroke-width':2},svg);
  s=0; ph.forEach(([n,dt,p])=>{ el('text',{x:x(s+dt/2),y:y(p)-6,'text-anchor':'middle'},svg,n==='cruise'?`cruise ${fmt(p,0)} W × 10 min`:n==='hover'?`${fmt(p,0)} W`:''); s+=dt; });
  [0,120,240,360,480,600,760].forEach(v=>el('text',{x:x(v),y:H-b+16,'text-anchor':'middle'},svg,Math.round(v/60)+' min'));
  el('text',{x:W-r,y:H-6,'text-anchor':'end'},svg,`mission ${fmt(s0.E_Wh,0)} Wh of ${fmt(s0.Epack_Wh,0)} Wh pack`);
  describe(svg, `Electrical power through one mission: hover ${fmt(Ph,0)} W for 60 s, transition at ${fmt(Ph,0)} W for 20 s, cruise ${fmt(Pc,0)} W for 10 min, transition 20 s, hover 60 s. Mission energy ${fmt(s0.E_Wh,0)} Wh of a ${fmt(s0.Epack_Wh,0)} Wh pack.`);
}
function drawCG(){
  const svg=$('#cgChart'); if(!svg) return; clear(svg);
  const cg=C0.cg, g=C0.geo, W=640,l=30,r=30, x=p=>l+(W-l-r)*p/60, yb=70;
  for(let p=0;p<=60;p+=10){ el('line',{x1:x(p),x2:x(p),y1:30,y2:120,class:'grid'},svg); el('text',{x:x(p),y:140,'text-anchor':'middle'},svg,p+' %'); }
  el('rect',{x:x(0),y:yb-6,width:x(60)-x(0),height:12,fill:css('--surface-2'),stroke:css('--line-2')},svg);
  el('rect',{x:x(Math.min(cg.pctFwd,cg.pct.noPayload)),y:yb-14,width:x(Math.max(cg.pctAft,cg.pct.noPayload))-x(Math.min(cg.pctFwd,cg.pct.noPayload)),height:28,fill:css('--ok'),opacity:0.35,rx:2},svg);
  el('text',{x:x((cg.pctFwd+cg.pct.noPayload)/2),y:yb+30,'text-anchor':'middle'},svg,`CG envelope ${fmt(cg.pctFwd,1)}–${fmt(cg.pct.noPayload,1)} %`);
  const mark=(p,lab,col,dy)=>{ el('line',{x1:x(p),x2:x(p),y1:yb-24,y2:yb+14,stroke:css(col),'stroke-width':2.5},svg); el('text',{x:x(p),y:yb-28+dy,'text-anchor':'middle',fill:css(col)},svg,lab); };
  mark(cg.pct.target,'CG target / thrust centroid 28 %','--accent',-6);
  mark(g.hn*100,`NP ${fmt(g.hn*100,1)} %`,'--bad',0);
  describe(svg, `C0 CG envelope from ${fmt(cg.pctFwd,1)} % to ${fmt(cg.pct.noPayload,1)} % MAC, against a CG target and thrust centroid of 28 % and a neutral point of ${fmt(g.hn*100,1)} % MAC.`);
}
function drawState(){
  const svg=$('#stateSvg'); if(!svg) return; clear(svg);
  const defs=el('defs',{},svg), mk=el('marker',{id:'arr',viewBox:'0 0 10 10',refX:'9',refY:'5',markerWidth:'7',markerHeight:'7',orient:'auto-start-reverse'},defs);
  el('path',{d:'M0 0L10 5L0 10z',fill:css('--fg-2')},mk);
  const N={ DIS:[70,60,110,'Disarmed'], QH:[235,60,150,'QHOVER / QLOITER'], TR:[425,60,120,'Transition'], FW:[610,60,130,'Fixed-wing cruise'],
            QL:[235,200,120,'QLAND'], BT:[425,200,150,'Back-transition'], AS:[610,200,130,'Q_ASSIST'] };
  const E=(x1,y1,x2,y2,lab,lx,ly,anchor='middle')=>{ el('line',{x1,y1,x2,y2,stroke:css('--fg-2'),'stroke-width':1.5,'marker-end':'url(#arr)'},svg); if(lab) el('text',{x:lx,y:ly,'text-anchor':anchor},svg,lab); };
  const R=k=>N[k][0]+N[k][2]/2, L=k=>N[k][0]-N[k][2]/2;
  E(R('DIS'),60,L('QH'),60,'arm',(R('DIS')+L('QH'))/2,34);
  E(R('QH'),60,L('TR'),60,'≥ 30 m AGL',(R('QH')+L('TR'))/2,34);
  E(R('TR'),60,L('FW'),60,'≥ 15 m/s',(R('TR')+L('FW'))/2,34);
  E(595,77,595,183,'< 12 m/s',588,110,'end');
  E(625,183,625,77,'recovered',632,134,'start');
  E(555,77,470,183,'Q mode',500,140,'end');
  E(L('BT'),200,R('QL'),200,'speed ≈ 0',(L('BT')+R('QL'))/2,176);
  E(190,183,100,77,'landed → disarm',135,140,'end');
  Object.entries(N).forEach(([k,[x,y,w,lab]])=>{
    el('rect',{x:x-w/2,y:y-17,width:w,height:34,rx:2,fill:css(k==='FW'?'--accent-soft':k==='QL'?'--warn-soft':'--surface-2'),stroke:css(k==='FW'?'--accent':'--line-2')},svg);
    el('text',{x,y:y+4,'text-anchor':'middle','font-weight':'600',fill:css('--fg')},svg,lab); });
  const fy=258; el('rect',{x:20,y:fy,width:660,height:58,rx:2,fill:'none',stroke:css('--line-2'),'stroke-dasharray':'4 3'},svg);
  el('text',{x:32,y:fy+18,'font-weight':'600',fill:css('--fg')},svg,'From any flying mode (failsafes)');
  el('text',{x:32,y:fy+36},svg,'Transition longer than 15 s (Q_TRANS_FAIL) → QRTL · RC link loss → RTL, then QLAND (R20)');
  el('text',{x:32,y:fy+52},svg,'Low battery → QLAND · Geofence breach at 120 m AGL → RTL · Pilot can select QHOVER at any time');
  describe(svg, 'QuadPlane flight modes for C0. Disarmed to QHOVER or QLOITER on arm. QHOVER to Transition at 30 m AGL or higher. Transition to fixed-wing cruise at 15 m/s or more. Fixed-wing cruise to Q_ASSIST below 12 m/s, and back to cruise once recovered. Cruise to Back-transition on selecting a Q mode. Back-transition to QLAND at about zero airspeed. QLAND to Disarmed when landed. Failsafes from any flying mode: transition longer than 15 s (Q_TRANS_FAIL) goes to QRTL; RC link loss goes to RTL then QLAND; low battery goes to QLAND; geofence breach at 120 m AGL goes to RTL; the pilot can select QHOVER at any time.');
}
function drawGantt(){
  const svg=$('#gantt'); if(!svg) return; clear(svg);
  const d0=Date.UTC(2026,9,13), d1=Date.UTC(2027,4,7), W=700, l=210, r=14, t=28;
  const x=d=>l+(W-l-r)*(d-d0)/(d1-d0), D_=(y,m,d)=>Date.UTC(y,m-1,d);
  const rows=[
    ['Design basis sign-off',D_(2026,10,13),D_(2026,10,27),'c1'],
    ['Site / legal path decision',D_(2026,10,13),D_(2026,11,6),'bad'],
    ['Order A: test parts + packs + charger',D_(2026,11,2),D_(2026,11,9),'c3'],
    ['Thrust stand T1–T5',D_(2026,11,16),D_(2026,12,4),'c2'],
    ['CAD + design freeze',D_(2026,11,2),D_(2026,12,4),'c1'],
    ['Order B: airframe, propulsion, avionics',D_(2026,12,7),D_(2026,12,14),'c3'],
    ['ArduPilot SITL rehearsal (§13)',D_(2027,1,11),D_(2027,2,19),'c5'],
    ['Airframe build',D_(2027,1,4),D_(2027,2,19),'c1'],
    ['Proof load, drop test, CG',D_(2027,2,19),D_(2027,2,26),'c2'],
    ['Avionics integration + bench',D_(2027,2,22),D_(2027,3,12),'c5'],
    ['Flight request ≥ 7 business days',D_(2027,3,3),D_(2027,3,12),'warn'],
    ['Hover tests',D_(2027,3,13),D_(2027,3,31),'c4'],
    ['Transition tests',D_(2027,4,3),D_(2027,4,17),'c4'],
    ['Full mission (R9/R10)',D_(2027,4,17),D_(2027,4,18),'ok'],
    ['Buffer: repair / weather / re-fly',D_(2027,4,18),D_(2027,4,30),'c6'],
  ];
  const rh=24; svg.setAttribute('viewBox',`0 0 ${W} ${t+rows.length*rh+30}`);
  for(let m=0;m<8;m++){ const dd=Date.UTC(2026,9+m,1); if(dd<d0) continue; el('line',{x1:x(dd),x2:x(dd),y1:t-6,y2:t+rows.length*rh,class:'grid'},svg);
    el('text',{x:x(dd)+3,y:t-10},svg,new Date(dd).toLocaleString('en-US',{month:'short',timeZone:'UTC'})+(m===3?' 27':'')); }
  rows.forEach(([n,a,b,c],i)=>{ const y=t+i*rh; el('text',{x:l-8,y:y+15,'text-anchor':'end'},svg,n);
    el('rect',{x:x(a),y:y+4,width:Math.max(4,x(b)-x(a)),height:15,rx:2,fill:css('--'+c),opacity:0.85},svg); });
  const dl=D_(2027,4,30); el('line',{x1:x(dl),x2:x(dl),y1:t-6,y2:t+rows.length*rh,stroke:css('--bad'),'stroke-width':2},svg);
  el('text',{x:x(dl)-4,y:t+rows.length*rh+16,'text-anchor':'end',fill:css('--bad')},svg,'deadline 2027-04-30');
  const iso=d=>new Date(d).toISOString().slice(0,10);
  describe(svg, 'Schedule to the 2027-04-30 deadline. '+rows.map(([n,a,b])=>`${n}: ${iso(a)}${b-a>86400000?' to '+iso(b):''}`).join('; ')+'.');
}

// ---------- viewer ----------
let V=null, vKey='C0';
function viewerGeom(k){ const c=D.concepts[k], g=c.geo, s=c.sized, x=c.cg.items.find(i=>i.name.startsWith('Battery')).x;
  return {span:D.base.span, chord:s.chord, fusL:0.85, fusW:0.16, fusH:0.16, fusX:0.45-0.425, battX:x, booms:g.booms, liftD:g.D, xRear:g.xRear,
    tailLE:g.xRear-g.D/2-0.05, hC:g.hC, vH:g.vH, vC:g.vC, pushX:0.45-0.85-0.038, pushD:14*0.0254}; }
function initViewer(){ if(V) return; V = Viewer($('#viewer')); V.build(viewerGeom(vKey)); touchGate($('#viewer').closest('.viewer'));
  $$('.viewer [data-view]').forEach(b=>b.addEventListener('click',()=>V.view(b.dataset.view)));
  $('#vDiscs').addEventListener('click',e=>e.currentTarget.setAttribute('aria-pressed',V.toggleDiscs()));
  $('#vExplode').addEventListener('click',e=>e.currentTarget.setAttribute('aria-pressed',V.toggleExplode()));
  $$('#vConcept button').forEach(b=>b.addEventListener('click',()=>{ vKey=b.dataset.c; $$('#vConcept button').forEach(x=>x.setAttribute('aria-pressed',x===b)); V.build(viewerGeom(vKey)); })); }

// ---------- SOLIDWORKS model viewer ----------
let CV=null;
const THREE_SRC=['https://cdnjs.cloudflare.com/ajax/libs/three.js/r128/three.min.js','https://cdn.jsdelivr.net/npm/three@0.128.0/examples/js/controls/OrbitControls.js'];
const loadScript = src => new Promise((ok,fail)=>{ const s=document.createElement('script'); s.src=src; s.onload=ok; s.onerror=fail; document.head.appendChild(s); });
function initCad(){ if(CV!==null) { CV && CV.refresh && CV.refresh(); return; }
  CV='loading'; $('#cadTris').textContent=MESH.tris.toLocaleString('en-US'); $('#cadComps').textContent=MESH.comps.length;
  const view=$('#cadView'); view.classList.add('loading');
  (window.THREE && THREE.OrbitControls ? Promise.resolve() : loadScript(THREE_SRC[0]).then(()=>loadScript(THREE_SRC[1])))
    .catch(()=>{}).then(()=>{ view.classList.remove('loading'); startCad(); }); }
function startCad(){
  const view=$('#cadView'), parts=$('.cadparts');
  CV = CadViewer(view, MESH) || false;
  view.classList.toggle('offline', !CV); parts.hidden = !CV;
  if(!CV){ const r=$('#cadRetry'); if(!r.dataset.bound){ r.dataset.bound=1; r.addEventListener('click',()=>{ CV=null; view.querySelector('.fallback').hidden=true; $('#cadCanvas').hidden=false; initCad(); }); } return; }
  touchGate($('#cadView'));
  const counts={}; MESH.comps.forEach(c=>counts[c[0]]=(counts[c[0]]||0)+1);
  const list=$('#cadParts'); Object.keys(MESH.parts).sort().forEach(n=>{ const li=document.createElement('li'), b=document.createElement('button');
    b.type='button'; b.textContent=n.replace(/_/g,' ')+(counts[n]>1?' ×'+counts[n]:''); b.title=MESH.parts[n].d;
    const on=()=>CV.highlight(n), off=()=>CV.highlight(null);
    b.addEventListener('focus',on); b.addEventListener('mouseenter',on); b.addEventListener('blur',off); b.addEventListener('mouseleave',off); b.addEventListener('click',on);
    li.appendChild(b); list.appendChild(li); });
  $$('#cadView [data-cview]').forEach(b=>b.addEventListener('click',()=>CV.view(b.dataset.cview)));
  $('#cadXray').addEventListener('click',e=>e.currentTarget.setAttribute('aria-pressed',CV.toggleXray()));
  $('#cadExplode').addEventListener('click',e=>e.currentTarget.setAttribute('aria-pressed',CV.toggleExplode()));
  $('#cadEdges').addEventListener('click',e=>e.currentTarget.setAttribute('aria-pressed',CV.toggleEdges())); }

// touch screens: a viewer canvas takes gestures only after a tap, so swipes over it scroll the page
function touchGate(view){
  if(!view || !window.matchMedia || !matchMedia('(pointer: coarse)').matches || view.querySelector('.gate')) return;
  const c=view.querySelector('canvas'), g=document.createElement('button');
  g.type='button'; g.className='gate'; g.innerHTML='<span>Tap to rotate the model</span>'; view.appendChild(g);
  const size=()=>{ g.style.height=c.offsetHeight+'px'; }; size(); if(window.ResizeObserver) new ResizeObserver(size).observe(c);
  const arm=on=>{ view.classList.toggle('armed',on); g.hidden=on; };
  g.addEventListener('click',()=>arm(true));
  document.addEventListener('pointerdown',e=>{ if(view.classList.contains('armed') && !view.contains(e.target)) arm(false); });
  if(window.IntersectionObserver) new IntersectionObserver(es=>es.forEach(en=>{ if(!en.isIntersecting) arm(false); })).observe(view);
}

function redrawSection(id){
  if(id==='model3d'){ initCad(); initViewer(); V.build(viewerGeom(vKey)); }
  if(id==='mass'){ drawTornado(); drawPack(); }
  if(id==='budget') drawCap();
  if(id==='testing') drawDuty();
  if(id==='cg') drawCG();
  if(id==='controls') drawState();
  if(id==='schedule') drawGantt();
}
function redrawAll(){ const cur=secs.find(s=>s.classList.contains('cur')); if(cur) redrawSection(cur.id); }

// ---------- boot ----------
buildNav(); fill(); derived(); tables(); sortable();
$$('[data-cad="parts"]').forEach(e=>e.textContent=Object.keys(MESH.parts).length); $$('[data-cad="comps"]').forEach(e=>e.textContent=MESH.comps.length);
$$('main a[href^="#"]').forEach(a=>{ const t=a.textContent.trim(); if(!/^§\s?\d+$/.test(t)) return; const sec=document.getElementById(a.getAttribute('href').slice(1)); const s=sec&&(sec.closest('section.sec')); if(s) a.setAttribute('aria-label', `${t} ${s.dataset.title}`); });
resetParams(); buildSliders(); renderChain();
$$('#chainConcept button').forEach(b=>b.addEventListener('click',()=>{ chainKey=b.dataset.c; $$('#chainConcept button').forEach(x=>x.setAttribute('aria-pressed',x===b)); resetParams(); buildSliders(); renderChain(); drawTornado(); }));
try{ const s=localStorage.getItem('vtolTheme'); if(s) mi=Math.max(0,modes.indexOf(s)); }catch(e){}
applyTheme(); route();
})();
