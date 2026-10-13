/* Concept-geometry viewer: flat-shaded polygons, painter's sort, orbit by drag. No libraries. */
function Viewer(canvas, opts){
  const ctx = canvas.getContext('2d');
  let yaw = -0.75, pitch = 0.5, zoom = 1, dragging = false, lx = 0, ly = 0;
  let faces = [], showDiscs = true, exploded = false, model = null;
  const css = n => getComputedStyle(document.documentElement).getPropertyValue(n).trim();

  // ---- primitive builders (x fwd, y right, z up; metres) ----
  function quad(a,b,c,d,col,grp){ faces.push({p:[a,b,c,d],col,grp}); }
  function box(cx,cy,cz,lx,ly,lz,col,grp){
    const x0=cx-lx/2,x1=cx+lx/2,y0=cy-ly/2,y1=cy+ly/2,z0=cz-lz/2,z1=cz+lz/2;
    const P=(x,y,z)=>[x,y,z];
    quad(P(x0,y0,z1),P(x1,y0,z1),P(x1,y1,z1),P(x0,y1,z1),col,grp);
    quad(P(x0,y0,z0),P(x0,y1,z0),P(x1,y1,z0),P(x1,y0,z0),col,grp);
    quad(P(x0,y0,z0),P(x1,y0,z0),P(x1,y0,z1),P(x0,y0,z1),col,grp);
    quad(P(x0,y1,z0),P(x0,y1,z1),P(x1,y1,z1),P(x1,y1,z0),col,grp);
    quad(P(x1,y0,z0),P(x1,y1,z0),P(x1,y1,z1),P(x1,y0,z1),col,grp);
    quad(P(x0,y0,z0),P(x0,y0,z1),P(x0,y1,z1),P(x0,y1,z0),col,grp);
  }
  function tubeX(x0,x1,y,z,r,col,grp,n=8){ // cylinder along x
    for(let i=0;i<n;i++){const a=i/n*2*Math.PI,b=(i+1)/n*2*Math.PI;
      quad([x0,y+r*Math.cos(a),z+r*Math.sin(a)],[x1,y+r*Math.cos(a),z+r*Math.sin(a)],[x1,y+r*Math.cos(b),z+r*Math.sin(b)],[x0,y+r*Math.cos(b),z+r*Math.sin(b)],col,grp);}
  }
  function cylZ(x,y,z0,z1,r,col,grp,n=10){ if(z1<z0){const t=z0;z0=z1;z1=t;}
    for(let i=0;i<n;i++){const a=i/n*2*Math.PI,b=(i+1)/n*2*Math.PI;
      quad([x+r*Math.cos(a),y+r*Math.sin(a),z0],[x+r*Math.cos(b),y+r*Math.sin(b),z0],[x+r*Math.cos(b),y+r*Math.sin(b),z1],[x+r*Math.cos(a),y+r*Math.sin(a),z1],col,grp);}
    const top=[];for(let i=0;i<n;i++){const a=i/n*2*Math.PI;top.push([x+r*Math.cos(a),y+r*Math.sin(a),z1]);}
    faces.push({p:top,col,grp});
  }
  function disc(x,y,z,r,col,grp){const p=[];for(let i=0;i<28;i++){const a=i/28*2*Math.PI;p.push([x+r*Math.cos(a),y+r*Math.sin(a),z]);}faces.push({p,col,grp,disc:true});}
  function discYZ(x,y,z,r,col,grp){const p=[];for(let i=0;i<28;i++){const a=i/28*2*Math.PI;p.push([x,y+r*Math.cos(a),z+r*Math.sin(a)]);}faces.push({p,col,grp,disc:true});}
  function wing(xle,y0,y1,z,c,t,col,grp){ // rectangular panel, thickness t, simple wedge TE
    quad([xle,y0,z+t/2],[xle,y1,z+t/2],[xle-c,y1,z],[xle-c,y0,z],col,grp);
    quad([xle,y0,z-t/2],[xle-c,y0,z],[xle-c,y1,z],[xle,y1,z-t/2],col,grp);
    quad([xle,y0,z-t/2],[xle,y1,z-t/2],[xle,y1,z+t/2],[xle,y0,z+t/2],col,grp);
    faces.push({p:[[xle,y0,z-t/2],[xle,y0,z+t/2],[xle-c,y0,z]],col,grp});
    faces.push({p:[[xle,y1,z-t/2],[xle-c,y1,z],[xle,y1,z+t/2]],col,grp});
  }
  function fin(xle,y,z0,h,c,col,grp){faces.push({p:[[xle,y,z0],[xle-c,y,z0],[xle-c,y,z0+h],[xle-0.6*c,y,z0+h]],col,grp});}

  function build(g){
    faces=[]; model=g;
    const ex = exploded ? 1 : 0;
    const C={fus:css('--c5'),wing:css('--c1'),boom:css('--fg-3'),mot:css('--c3'),disc:css('--c2'),tail:css('--c4'),bat:css('--c6')};
    const zW = 0;                                   // wing mid-plane
    box(g.fusX,0,-0.03,g.fusL,g.fusW,g.fusH,C.fus,'fuselage');
    box(g.battX,0,-0.05,0.17,0.09,0.07,C.bat,'battery');
    const half=g.span/2, t=g.chord*0.13;
    wing(0,-half-ex*0.3,-g.fusW/2-ex*0.3,zW,g.chord,t,C.wing,'wing L');
    wing(0,g.fusW/2+ex*0.3,half+ex*0.3,zW,g.chord,t,C.wing,'wing R');
    const zB = -0.035;                              // booms hang under the wing
    g.booms.forEach(b=>{
      const dy = ex*(b.y<0?-0.12:0.12), dz = -ex*0.22;
      const xs = b.tail ? g.xRear-0.03 : b.x0;     // tail-carrying booms split at the rear motor mount
      tubeX(xs,b.x1,b.y+dy,zB+dz,0.0127,C.boom,'boom');
      if(b.tail) tubeX(b.x0-ex*0.3,xs-ex*0.3,b.y+dy,zB+dz,0.0127,C.boom,'boom rear');
      b.rotors.forEach(r=>{
        cylZ(r.x,b.y+dy,zB+dz,zB+0.05+dz,0.028,C.mot,'lift motor');
        if(r.coax) cylZ(r.x,b.y+dy,zB-0.07+dz,zB-0.02+dz,0.028,C.mot,'lift motor');
        if(showDiscs) disc(r.x,b.y+dy,zB+0.065+dz,g.liftD/2,C.disc,'rotor disc');
        if(showDiscs && r.coax) disc(r.x,b.y+dy,zB-0.085+dz,g.liftD/2,C.disc,'rotor disc');
      });
    });
    const tb = g.booms.filter(b=>b.tail), yT = Math.abs(tb[0].y);
    const tLE = g.tailLE - ex*0.3, zT = zB+0.01;
    wing(tLE,-yT,yT,zT,g.hC,0.012,C.tail,'h-tail');
    fin(tLE,-yT,zT,g.vH,g.vC,C.tail,'fin'); fin(tLE,yT,zT,g.vH,g.vC,C.tail,'fin');
    cylZ(g.pushX+0.02,0,-0.05,0.0,0.03,C.mot,'pusher');
    if(showDiscs) discYZ(g.pushX,0,-0.03,g.pushD/2,C.disc,'pusher disc');
    draw();
  }

  function project(p,W,H,s){
    const [x,y,z]=p, cy=Math.cos(yaw), sy=Math.sin(yaw), cp=Math.cos(pitch), sp=Math.sin(pitch);
    const X = x*cy - y*sy, Y = x*sy + y*cy;      // yaw about z
    const Yv = Y, Zv = z*cp - X*sp, D = X*cp + z*sp; // pitch
    return [W/2 + Yv*s, H/2 - Zv*s, D];
  }
  function draw(){
    const dpr=window.devicePixelRatio||1, W=canvas.clientWidth, H=canvas.clientHeight;
    canvas.width=W*dpr; canvas.height=H*dpr; ctx.setTransform(dpr,0,0,dpr,0,0);
    ctx.fillStyle=css('--surface'); ctx.fillRect(0,0,W,H);
    const s = Math.min(W,H)/ (model?model.span*1.15:3) * zoom * (W<500?1.25:1);
    // ground grid 0.5 m
    ctx.strokeStyle=css('--line'); ctx.lineWidth=1;
    for(let i=-3;i<=3;i+=0.5){
      let a=project([i,-1.6,-0.3],W,H,s),b=project([i,1.6,-0.3],W,H,s);ctx.beginPath();ctx.moveTo(a[0],a[1]);ctx.lineTo(b[0],b[1]);ctx.stroke();
      a=project([-1.6*1.4,i*0.55,-0.3],W,H,s);b=project([1.6,i*0.55,-0.3],W,H,s);
    }
    const light=[0.3,-0.4,0.86];
    const pf = faces.map(f=>{
      const pts=f.p.map(p=>project(p,W,H,s)); const d=pts.reduce((a,p)=>a+p[2],0)/pts.length;
      // normal for shading
      const a=f.p[0],b=f.p[1],c=f.p[2]; const u=[b[0]-a[0],b[1]-a[1],b[2]-a[2]],v=[c[0]-a[0],c[1]-a[1],c[2]-a[2]];
      let n=[u[1]*v[2]-u[2]*v[1],u[2]*v[0]-u[0]*v[2],u[0]*v[1]-u[1]*v[0]]; const L=Math.hypot(...n)||1; n=n.map(q=>q/L);
      const sh=0.55+0.45*Math.abs(n[0]*light[0]+n[1]*light[1]+n[2]*light[2]);
      return {pts,d,col:f.col,sh,disc:f.disc};
    }).sort((A,B)=>A.d-B.d);
    for(const f of pf){
      ctx.beginPath(); f.pts.forEach((p,i)=>i?ctx.lineTo(p[0],p[1]):ctx.moveTo(p[0],p[1])); ctx.closePath();
      ctx.globalAlpha = f.disc?0.18:1; ctx.fillStyle=f.col; ctx.fill();
      if(!f.disc){ctx.globalAlpha=1-f.sh; ctx.fillStyle='#000'; ctx.fill();}
      ctx.globalAlpha = f.disc?0.6:0.25; ctx.strokeStyle=f.disc?f.col:css('--fg'); ctx.lineWidth=f.disc?1:0.5; ctx.stroke();
      ctx.globalAlpha=1;
    }
    // scale bar 1 m
    const p0=project([0,0,0],W,H,s), p1=project([0,1,0],W,H,s); const len=Math.hypot(p1[0]-p0[0],p1[1]-p0[1]);
    ctx.strokeStyle=css('--fg-2'); ctx.lineWidth=2; ctx.beginPath(); ctx.moveTo(12,H-14); ctx.lineTo(12+len,H-14); ctx.stroke();
    ctx.fillStyle=css('--fg-2'); ctx.font='11px '+css('--f-body'); ctx.fillText('1 m (at model centre)',12,H-20);
  }
  canvas.addEventListener('pointerdown',e=>{dragging=true;lx=e.clientX;ly=e.clientY;canvas.setPointerCapture(e.pointerId);});
  canvas.addEventListener('pointermove',e=>{if(!dragging)return;yaw+= (e.clientX-lx)*0.01;pitch=Math.max(-1.4,Math.min(1.4,pitch+(e.clientY-ly)*0.01));lx=e.clientX;ly=e.clientY;draw();});
  canvas.addEventListener('pointerup',()=>dragging=false);
  canvas.addEventListener('wheel',e=>{e.preventDefault();zoom=Math.max(0.5,Math.min(3,zoom*(e.deltaY<0?1.1:0.9)));draw();},{passive:false});
  window.addEventListener('resize',()=>model&&draw());
  return {
    build, draw,
    view(name){ if(name==='top'){yaw=-Math.PI/2;pitch=1.5;} else if(name==='side'){yaw=-Math.PI/2;pitch=0;} else if(name==='front'){yaw=0;pitch=0.05;} else {yaw=-0.75;pitch=0.5;} zoom=1; draw(); },
    toggleDiscs(){showDiscs=!showDiscs; build(model); return showDiscs;},
    toggleExplode(){exploded=!exploded; build(model); return exploded;}
  };
}
