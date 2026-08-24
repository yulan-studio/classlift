const state = { step: 0, answers: {} };
const option = (value, label = value) => ({ value, label });
const questions = [
  { id:'business_type', section:'ABOUT YOUR BUSINESS', aside:'First, tell us about\nyour organization', title:'What type of organization do you run?', type:'single', options:[option('特殊教育 / 儿童发展服务','Special education / Child development services'),option('学科辅导 / 课后教育','Tutoring / After-school education'),option('兴趣与技能培训（音乐、美术、舞蹈、编程、机器人、乐高、外语、棋类等）','Enrichment & skills education'),option('健身 / 体育 / 私人教练','Fitness / Sports / Personal coaching'),option('非营利组织 / 社区机构','Nonprofit / Community organization'),option('其他课程 / 活动机构','Other course / Activity organization')] },
  { id:'student_count', section:'ABOUT YOUR BUSINESS', aside:'Your size helps us\ncalibrate the benchmark', title:'Approximately how many active students or clients do you have?', type:'single', options:['1–50','51–100','101–300','301–500','501–1,000','1,000+'].map(x=>option(x)) },
  { id:'current_tools', section:'CURRENT STATE', aside:'How does your business\nrun today?', title:'How do you currently manage clients, programs, billing, and staff?', help:'Select all that apply', type:'multi', options:[option('Excel / Google Sheets'),option('多个不同软件组合','A combination of different software'),option('一个课程管理软件','One course management platform'),option('纸张 / 人工记录','Paper / Manual records'),option('邮件 / WhatsApp / 微信 / 短信','Email / WhatsApp / WeChat / Text messages'),option('主要依赖员工记住和执行','Staff memory and manual follow-through'),option('自己开发的系统','A custom-built system'),option('其他','Other')] },
  { id:'improvement_areas', section:'WHAT NEEDS TO IMPROVE', aside:'Select every area\nthat needs attention', title:'Which areas of your operation need improvement?', help:'Select all that reflect your current situation', type:'multi', options:[option('客户咨询与 Follow-up','Lead inquiries and follow-up'),option('客户沟通和信息记录','Client communication and information records'),option('排课、改课与教室安排','Scheduling, rescheduling, and room allocation'),option('收费、Credit 与课时记录','Billing, credits, and session tracking'),option('老师工资计算','Instructor payroll calculations'),option('Attendance / 签到','Attendance tracking'),option('重复行政工作太多','Too much repetitive administrative work'),option('老板缺少实时运营数据','Lack of real-time operational visibility'),option('过度依赖核心员工','Overdependence on key staff'),option('不同软件之间数据分散','Data fragmented across different systems'),option('客户增长就要增加行政人员','Growth requires more administrative staff'),option('多 Location / 第二家店管理','Multi-location management'),option('标准工作流程（SOP）与员工交接','Standard workflows (SOPs) and staff handover'),option('其他','Other')] },
  { id:'primary_pain', section:'TOP PRIORITY', aside:'Of all these issues,\nwhich is most urgent?', title:'Which issue do you most urgently need to solve?', help:'Choose one top priority', type:'single', options:()=>((state.answers.improvement_areas||[]).map(value=>option(value,questions[3].options.find(x=>x.value===value)?.label||value))) },
  { id:'implementation_timeline', section:'TIMING', aside:'Change need not be rushed,\nbut timing should be clear', title:'When would you like to solve this top-priority issue?', type:'single', options:[option('现在就需要解决','I need to solve it now'),option('未来 1–3 个月','Within 1–3 months'),option('3–6 个月','Within 3–6 months'),option('6–12 个月','Within 6–12 months'),option('暂时只是了解','I’m researching for now')] },
  { id:'additional_needs', section:'IN YOUR OWN WORDS', aside:'What else is missing\nfrom the options?', title:'What other challenges or needs do you have?', help:'Optional. Describe a specific situation, your current approach, or the outcome you want.', type:'textarea', optional:true, placeholder:'For example: We spend two days reconciling sessions and instructor payroll at month-end, and want to automate it…' }
];

const $ = s => document.querySelector(s);
const landing=$('#landing'), quiz=$('#quiz'), leadGate=$('#leadGate'), report=$('#report');
$('#startBtn').onclick=()=>{ landing.classList.add('hidden'); $('.site-header').classList.add('hidden'); quiz.classList.remove('hidden'); renderQuestion(); window.scrollTo(0,0); };
$('#backBtn').onclick=()=>{ if(state.step>0){state.step--;renderQuestion()}else{quiz.classList.add('hidden');landing.classList.remove('hidden');$('.site-header').classList.remove('hidden')} };
$('#saveExit').onclick=()=>{ localStorage.setItem('classlift-diagnostic-en',JSON.stringify(state)); alert('Your progress has been saved in this browser.'); };

function renderQuestion(){
  const q=questions[state.step], saved=state.answers[q.id];
  $('#sectionLabel').textContent=q.section; $('#asideTitle').innerHTML=q.aside.replace('\n','<br>'); $('#asideHint').textContent=state.step===questions.length-1?'Use your own words—we will preserve them.':'Your answers calibrate the score and remain private.';
  $('#stepCounter').textContent=String(state.step+1).padStart(2,'0')+' / '+questions.length; $('#progressBar').style.width=((state.step+1)/questions.length*100)+'%';
  let control='';
  const availableOptions=typeof q.options==='function'?q.options():q.options;
  if(q.type==='single'||q.type==='multi') control=`<div class="options">${availableOptions.map((o,i)=>`<div class="option"><input id="o${i}" name="answer" type="${q.type==='multi'?'checkbox':'radio'}" value="${o.value.replaceAll('"','&quot;')}" ${Array.isArray(saved)?saved.includes(o.value)?'checked':'':saved===o.value?'checked':''}><label for="o${i}">${o.label}</label></div>`).join('')}</div>`;
  else if(q.type==='number') control=`<input class="text-input" type="number" min="0" max="9999" name="answer" value="${saved||''}" placeholder="${q.placeholder}" autofocus>`;
  else control=`<textarea class="text-input textarea" name="answer" placeholder="${q.placeholder}" autofocus>${saved||''}</textarea>`;
  $('#questionMount').innerHTML=`<div class="question-wrap"><div class="q-number">Q${String(state.step+1).padStart(2,'0')}</div><h3>${q.title}</h3>${q.help?`<p class="q-help">${q.help}</p>`:''}${control}<div id="formError" class="error"></div></div>`;
  $('#backBtn span').textContent=state.step?'Previous':'Back to home';
}

$('#questionForm').onsubmit=e=>{
  e.preventDefault(); const q=questions[state.step]; let value;
  if(q.type==='multi') value=[...document.querySelectorAll('input[name=answer]:checked')].map(x=>x.value);
  else if(q.type==='single') value=$('input[name=answer]:checked')?.value;
  else value=$('[name=answer]')?.value.trim();
  if((!value || (Array.isArray(value)&&!value.length))&&!q.optional){ $('#formError').textContent='Choose or enter an answer to continue.'; return; }
  if(q.max && value.length>q.max){ $('#formError').textContent=`Select no more than ${q.max} options.`; return; }
  state.answers[q.id]=value;
  if(state.step<questions.length-1){ state.step++;renderQuestion();window.scrollTo(0,0) } else { quiz.classList.add('hidden');leadGate.classList.remove('hidden');window.scrollTo(0,0) }
};

function includes(id,terms){const v=state.answers[id]||[];return terms.some(t=>(Array.isArray(v)?v:[v]).some(x=>x.includes(t)))}
function calculateScores(){
  let operational=22, systemization=18, financial=13, scalability=17;
  if(includes('current_tools',['纸张','员工记住'])){operational-=6;systemization-=7} if(includes('current_tools',['Excel'])){operational-=3;systemization-=4} if(includes('current_tools',['多个'])){operational-=3;systemization-=3} if(includes('current_tools',['一个课程管理软件','自己开发'])){systemization+=2}
  if(includes('primary_pain',['排课','重复行政']))operational-=5; if(includes('primary_pain',['收费','工资'])){operational-=3;financial-=6} if(includes('primary_pain',['了解公司的运营']))financial-=3;
  if(includes('previous_solutions',['增加行政','依赖一个']))scalability-=5; if(includes('desired_outcome',['服务更多','第二家店']))scalability-=2; if(includes('cost_of_inaction',['增加行政','无法服务','利润']))scalability-=5;
  const key=Number(state.answers.key_person_dependency||10); if(key<=5){systemization-=2;scalability-=2}
  const clamp=(n,max)=>Math.max(0,Math.min(max,n)); operational=clamp(operational,25);systemization=clamp(systemization,20);financial=clamp(financial,15);scalability=clamp(scalability,20);
  return {operational,systemization,key,financial,scalability,total:operational+systemization+key+financial+scalability};
}
function classification(n){return n>=80?['Highly Scalable','Your operation has a strong systemized foundation. The next opportunity is higher-value automation and optimization.']:n>=60?['Growth Ready','You have a degree of systemization, but some processes still limit growth.']:n>=40?['People Dependent','Your operation still relies significantly on manual work and key staff, so operating costs may rise with growth.']:['High Operational Dependency','Many core processes rely on manual work, individual experience, or fragmented systems. Build a standardized foundation before scaling.']}
function bottlenecks(){const a=[];if(includes('improvement_areas',['核心员工']))a.push(['Key-person dependency','Important operational knowledge may be concentrated in a few employees, creating handover and continuity risk.']);if(includes('current_tools',['Excel','多个','纸张']))a.push(['Fragmented systems and data','Information may be spread across tools or spreadsheets, increasing duplicate entry and missed information.']);if(includes('primary_pain',['收费','工资']))a.push(['Manual financial processes','Manual billing, credit, session, or payroll work reduces the timeliness and reliability of financial information.']);if(includes('primary_pain',['重复行政','排课','Follow-up']))a.push(['Repetitive administrative work','Frequent, rules-based tasks still consume staff time and limit the team’s service capacity.']);if(includes('improvement_areas',['增加行政人员']))a.push(['Growth tied to headcount','Under the current model, administrative staffing and labour costs may need to rise alongside client growth.']);while(a.length<3)a.push([['Operational visibility','Process standardization','Repeatable growth'][a.length],'Your answers suggest this capability should be validated and strengthened in the next stage.']);return a.slice(0,3)}
function priorities(){return bottlenecks().map((_,i)=>i===0?['Build a unified operating foundation',`Start with “${englishLabel(state.answers.primary_pain)}” and define its owner, data source, and standard workflow.`]:i===1?['Automate repetitive work','Begin with high-frequency, rules-based administrative tasks and measure the time saved.']:['Reduce handover risk','Turn key employees’ knowledge into workflows the team can execute and track.'])}
function englishLabel(value){for(const q of questions){const opts=typeof q.options==='function'?q.options():q.options||[];const found=opts.find(x=>x.value===value);if(found)return found.label}return value||'Not provided'}
function potentialImpacts(){const a=[];if(includes('improvement_areas',['增加行政人员','重复行政']))a.push('Administrative workload and labour costs may continue to rise with growth.');if(includes('improvement_areas',['核心员工','员工交接']))a.push('Staff absence or turnover may disrupt important workflows.');if(includes('improvement_areas',['数据分散','实时运营数据']))a.push('Decisions may continue to rely on incomplete or delayed information.');if(includes('improvement_areas',['收费','工资']))a.push('Manual reconciliation may keep consuming time and increase the risk of errors.');return a.slice(0,4)}

$('#leadForm').onsubmit=async e=>{
  e.preventDefault();
  const form=e.currentTarget, button=form.querySelector('button[type=submit]'), fd=new FormData(form);
  state.lead=Object.fromEntries(fd);
  button.disabled=true; button.textContent='Generating your report…';
  const a=state.answers;
  const payload={
    businessType:a.business_type,
    studentCount:a.student_count,
    currentTools:a.current_tools,
    improvementAreas:a.improvement_areas,
    primaryPain:a.primary_pain,
    implementationTimeline:a.implementation_timeline,
    additionalNeeds:a.additional_needs||null,
    name:state.lead.name,
    email:state.lead.email,
    organization:state.lead.organization||null,
    websiteUrl:state.lead.websiteUrl||null
  };
  try{
    const response=await fetch('/api/diagnostics',{method:'POST',headers:{'Content-Type':'application/json'},body:JSON.stringify(payload)});
    if(!response.ok){
      const problem=await response.json().catch(()=>null);
      const details=problem?.errors?Object.values(problem.errors).flat().join(' '):problem?.detail;
      throw new Error(details||`提交失败 (${response.status})`);
    }
    state.serverResult=await response.json();
    localStorage.removeItem('classlift-diagnostic-en');
    leadGate.classList.add('hidden'); renderReport(state.serverResult); report.classList.remove('hidden'); window.scrollTo(0,0);
  }catch(error){
    button.disabled=false; button.innerHTML='Try generating again <span>→</span>';
    let message=form.querySelector('.submit-error');
    if(!message){message=document.createElement('p');message.className='error submit-error';form.appendChild(message)}
    message.textContent=`We couldn’t generate your report: ${error.message}. Please try again.`;
  }
};
function renderReport(serverResult){
 const remote=serverResult?.scores;
 const s=remote?{operational:remote.operationalEfficiency,systemization:remote.systemization,key:remote.keyPersonIndependence,financial:remote.financialControl,scalability:remote.scalability,total:remote.total}:calculateScores();
 const ai=null;
 const c=remote?[remote.classification,remote.classificationDescription]:classification(s.total);
 const bs=ai?.bottlenecks?.map(x=>[x.title,x.explanation])||bottlenecks();
 const ps=ai?.priorities?.map(x=>[x.title,x.goal])||priorities();
 const impacts=potentialImpacts();
 const intent=serverResult?.leadIntent||{'现在就需要解决':'VERY HIGH','未来 1–3 个月':'HIGH','3–6 个月':'MEDIUM','6–12 个月':'LOW','暂时只是了解':'RESEARCH'}[state.answers.implementation_timeline];
 const metrics=[['Operational Efficiency',s.operational,25],['Systemization',s.systemization,20],['Key-person Independence',s.key,20],['Financial Control',s.financial,15],['Scalability',s.scalability,20]];
 report.innerHTML=`<nav class="report-nav"><a class="brand"><img class="brand-logo" src="/images/classlift_logo.png" alt=""><span>ClassLift</span></a><div class="report-nav-actions"><span class="report-id">Report ${serverResult?.leadId?.slice(0,8)||'Preview'}</span><button class="ghost-btn" onclick="window.print()">Print / Save PDF</button><button class="ghost-btn" onclick="location.reload()">Start again</button></div></nav><article class="report-sheet"><header class="report-hero"><div><span class="report-kicker">YOUR BUSINESS SCALABILITY REPORT · ${new Date().toLocaleDateString('en-CA')}</span><h2>${state.lead.organization||state.lead.name}’s<br>Scalability Diagnostic</h2><p>${c[1]}</p></div><div class="big-score"><span>BUSINESS SCALABILITY SCORE</span><strong>${s.total}<span> / 100</span></strong><b>${c[0]}</b></div></header><div class="report-body"><div class="score-grid">${metrics.map(m=>`<div class="metric"><span>${m[0]}</span><strong>${m[1]}<small> / ${m[2]}</small></strong></div>`).join('')}</div>
 <section class="report-section"><span class="section-index">01 · YOUR DIRECTION</span><h3>What do you want to improve?</h3><div class="goal-box">Your highest-priority goal is to resolve “${englishLabel(state.answers.primary_pain)}” ${state.answers.additional_needs?`while also addressing: ${state.answers.additional_needs}`:'and build a more scalable operation.'}<small>Based on your answers</small></div></section>
 <section class="report-section"><span class="section-index">02 · TOP BOTTLENECKS</span><h3>Your most important operational bottlenecks</h3><div class="bottlenecks">${bs.map((x,i)=>`<div class="insight-card"><b>0${i+1}</b><h4>${x[0]}</h4><p>${x[1]}</p></div>`).join('')}</div></section>
 <section class="report-section"><span class="section-index">03 · POTENTIAL IMPACTS</span><h3>What may happen if these issues continue</h3><p>These are reasonable implications of the issues you selected. They help establish priorities and are not predictions.</p><div class="impact-grid">${(impacts.length?impacts:['There is not enough information to identify a specific impact.']).map((x,i)=>`<div class="impact-card"><b>0${i+1}</b><span>${x}</span></div>`).join('')}</div></section>
 <section class="report-section"><span class="section-index">04 · TOP PRIORITY</span><h3>Your most urgent issue</h3><div class="quote">“${englishLabel(state.answers.primary_pain)}”</div></section>
 <section class="report-section"><span class="section-index">05 · PRIORITIES</span><h3>Three recommended improvement priorities</h3><div class="priorities">${ps.map((x,i)=>`<div class="insight-card"><b>PRIORITY ${i+1}</b><h4>${x[0]}</h4><p>${x[1]}</p></div>`).join('')}</div></section>
 <div class="solution-cta"><div><h3>See how these issues can be systemized</h3><p>Based on your ${c[0]} result and ${intent} timeline, explore capabilities relevant to your current priorities.</p></div><button class="primary-btn" onclick="alert('This can connect to your personalized ClassLift solution page next.')">See how a system can solve my top issues <span>→</span></button></div></div></article>`;
}

document.addEventListener('keydown',e=>{if(e.key==='Enter'&&quiz&&!quiz.classList.contains('hidden')&&e.target.tagName!=='TEXTAREA'){$('#questionForm').requestSubmit()}});
