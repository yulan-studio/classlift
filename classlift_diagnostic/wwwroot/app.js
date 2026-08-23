const state = { step: 0, answers: {} };
const option = (value, label = value) => ({ value, label });
const questions = [
  { id:'business_type', section:'ABOUT YOUR BUSINESS', aside:'先了解一点\n你的业务', title:'你的机构属于哪一类？', type:'single', options:['特殊教育 / 儿童发展服务','学科辅导 / 课后教育','兴趣与技能培训（音乐、美术、舞蹈、编程、机器人、乐高、外语、棋类等）','健身 / 体育 / 私人教练','非营利组织 / 社区机构','其他课程 / 活动机构'].map(x=>option(x)) },
  { id:'student_count', section:'ABOUT YOUR BUSINESS', aside:'业务规模\n会影响诊断基准', title:'目前大约有多少活跃学生 / 客户？', type:'single', options:['1–50','51–100','101–300','301–500','501–1,000','1,000+'].map(x=>option(x)) },
  { id:'current_tools', section:'CURRENT STATE', aside:'现在，你的业务\n如何运转？', title:'你们目前主要用什么方式管理客户、课程、收费和员工？', help:'可以选择多项', type:'multi', options:['Excel / Google Sheets','多个不同软件组合','一个课程管理软件','纸张 / 人工记录','邮件 / WhatsApp / 微信 / 短信','主要依赖员工记住和执行','自己开发的系统','其他'].map(x=>option(x)) },
  { id:'improvement_areas', section:'WHAT NEEDS TO IMPROVE', aside:'把所有需要改善的\n地方选出来', title:'你认为目前哪些方面需要改善？', help:'请选择所有符合现状的项目，不限数量', type:'multi', options:['客户咨询与 Follow-up','客户沟通和信息记录','排课、改课与教室安排','收费、Credit 与课时记录','老师工资计算','Attendance / 签到','重复行政工作太多','老板缺少实时运营数据','过度依赖核心员工','不同软件之间数据分散','客户增长就要增加行政人员','多 Location / 第二家店管理','标准工作流程（SOP）与员工交接','其他'].map(x=>option(x)) },
  { id:'primary_pain', section:'TOP PRIORITY', aside:'所有问题中，\n哪一个最紧急？', title:'这些问题中，你最急需解决哪一个？', help:'请选择一个最高优先级', type:'single', options:()=>((state.answers.improvement_areas||[]).map(x=>option(x))) },
  { id:'implementation_timeline', section:'TIMING', aside:'改变不一定要急，\n但需要清晰', title:'你希望什么时候解决这个最紧急的问题？', type:'single', options:['现在就需要解决','未来 1–3 个月','3–6 个月','6–12 个月','暂时只是了解'].map(x=>option(x)) },
  { id:'additional_needs', section:'IN YOUR OWN WORDS', aside:'选项之外，\n还有什么？', title:'除了以上选项，你还有哪些痛点或需求？', help:'选填。可以描述具体场景、目前的处理方式，或你希望达到的结果。', type:'textarea', optional:true, placeholder:'例如：每个月月底都要花两天核对课时和老师工资，希望可以自动完成……' }
];

const $ = s => document.querySelector(s);
const landing=$('#landing'), quiz=$('#quiz'), leadGate=$('#leadGate'), report=$('#report');
$('#startBtn').onclick=()=>{ landing.classList.add('hidden'); $('.site-header').classList.add('hidden'); quiz.classList.remove('hidden'); renderQuestion(); window.scrollTo(0,0); };
$('#backBtn').onclick=()=>{ if(state.step>0){state.step--;renderQuestion()}else{quiz.classList.add('hidden');landing.classList.remove('hidden');$('.site-header').classList.remove('hidden')} };
$('#saveExit').onclick=()=>{ localStorage.setItem('classlift-diagnostic',JSON.stringify(state)); alert('进度已保存在当前浏览器。'); };

function renderQuestion(){
  const q=questions[state.step], saved=state.answers[q.id];
  $('#sectionLabel').textContent=q.section; $('#asideTitle').innerHTML=q.aside.replace('\n','<br>'); $('#asideHint').textContent=state.step===questions.length-1?'请保留最真实的原话。':'你的回答将用于校准评分，不会公开。';
  $('#stepCounter').textContent=String(state.step+1).padStart(2,'0')+' / '+questions.length; $('#progressBar').style.width=((state.step+1)/questions.length*100)+'%';
  let control='';
  const availableOptions=typeof q.options==='function'?q.options():q.options;
  if(q.type==='single'||q.type==='multi') control=`<div class="options">${availableOptions.map((o,i)=>`<div class="option"><input id="o${i}" name="answer" type="${q.type==='multi'?'checkbox':'radio'}" value="${o.value.replaceAll('"','&quot;')}" ${Array.isArray(saved)?saved.includes(o.value)?'checked':'':saved===o.value?'checked':''}><label for="o${i}">${o.label}</label></div>`).join('')}</div>`;
  else if(q.type==='number') control=`<input class="text-input" type="number" min="0" max="9999" name="answer" value="${saved||''}" placeholder="${q.placeholder}" autofocus>`;
  else control=`<textarea class="text-input textarea" name="answer" placeholder="${q.placeholder}" autofocus>${saved||''}</textarea>`;
  $('#questionMount').innerHTML=`<div class="question-wrap"><div class="q-number">Q${String(state.step+1).padStart(2,'0')}</div><h3>${q.title}</h3>${q.help?`<p class="q-help">${q.help}</p>`:''}${control}<div id="formError" class="error"></div></div>`;
  $('#backBtn span').textContent=state.step?'上一题':'返回首页';
}

$('#questionForm').onsubmit=e=>{
  e.preventDefault(); const q=questions[state.step]; let value;
  if(q.type==='multi') value=[...document.querySelectorAll('input[name=answer]:checked')].map(x=>x.value);
  else if(q.type==='single') value=$('input[name=answer]:checked')?.value;
  else value=$('[name=answer]')?.value.trim();
  if((!value || (Array.isArray(value)&&!value.length))&&!q.optional){ $('#formError').textContent='请选择或填写一个答案后继续。'; return; }
  if(q.max && value.length>q.max){ $('#formError').textContent=`最多选择 ${q.max} 项。`; return; }
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
function classification(n){return n>=80?['Highly Scalable','你的运营已经具备较好的系统化基础，下一步是寻找更高价值的自动化机会。']:n>=60?['Growth Ready','你已经具备一定系统化能力，但部分流程仍在限制增长。']:n>=40?['People Dependent','业务目前仍比较依赖人工和关键员工，增长时运营成本可能同步上升。']:['High Operational Dependency','大量核心流程依赖人工、个人经验或分散系统，扩大前值得先建立标准化基础。']}
function bottlenecks(){const a=[];if(Number(state.answers.key_person_dependency)<=10)a.push(['关键人员依赖','多项核心工作可能集中在少数员工手中，交接韧性有提升空间。']);if(includes('current_tools',['Excel','多个','纸张']))a.push(['系统与数据分散','信息可能存在于不同工具或表格中，增加重复输入与遗漏风险。']);if(includes('primary_pain',['收费','工资']))a.push(['财务流程依赖人工','收费、课时或工资环节的人工处理，会降低财务信息的及时性。']);if(includes('primary_pain',['重复行政','排课','跟进']))a.push(['重复行政工作','高频、规则明确的工作仍消耗员工时间，限制了团队服务能力。']);if(includes('cost_of_inaction',['增加行政','人工成本']))a.push(['增长与人力绑定','按照目前方式增长，行政人数与人工成本可能需要同步增加。']);while(a.length<3)a.push([['运营可视性','流程标准化','规模复制能力'][a.length],'目前答案显示这一能力值得在下一阶段进一步验证和完善。']);return a.slice(0,3)}
function priorities(){const b=bottlenecks();return b.map(([x],i)=>i===0?['建立统一运营基础',`优先围绕“${state.answers.primary_pain}”梳理负责人、数据与标准流程。`]:i===1?['自动化重复工作','选择高频、规则清楚的行政任务先自动化，并记录节省的时间。']:['降低交接风险','把关键员工的经验转化为团队可执行、可追踪的工作流程。'])}

$('#leadForm').onsubmit=async e=>{
  e.preventDefault();
  const form=e.currentTarget, button=form.querySelector('button[type=submit]'), fd=new FormData(form);
  state.lead=Object.fromEntries(fd);
  button.disabled=true; button.textContent='正在生成报告…';
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
    organization:state.lead.organization||null
  };
  try{
    const response=await fetch('/api/diagnostics',{method:'POST',headers:{'Content-Type':'application/json'},body:JSON.stringify(payload)});
    if(!response.ok){
      const problem=await response.json().catch(()=>null);
      const details=problem?.errors?Object.values(problem.errors).flat().join(' '):problem?.detail;
      throw new Error(details||`提交失败 (${response.status})`);
    }
    state.serverResult=await response.json();
    localStorage.removeItem('classlift-diagnostic');
    leadGate.classList.add('hidden'); renderReport(state.serverResult); report.classList.remove('hidden'); window.scrollTo(0,0);
  }catch(error){
    button.disabled=false; button.innerHTML='重新生成报告 <span>→</span>';
    let message=form.querySelector('.submit-error');
    if(!message){message=document.createElement('p');message.className='error submit-error';form.appendChild(message)}
    message.textContent=`暂时无法生成报告：${error.message}。请稍后重试。`;
  }
};
function renderReport(serverResult){
 const remote=serverResult?.scores;
 const s=remote?{operational:remote.operationalEfficiency,systemization:remote.systemization,key:remote.keyPersonIndependence,financial:remote.financialControl,scalability:remote.scalability,total:remote.total}:calculateScores();
 const ai=serverResult?.report;
 const c=remote?[remote.classification,remote.classificationDescription]:classification(s.total);
 const bs=ai?.bottlenecks?.map(x=>[x.title,x.explanation])||bottlenecks();
 const ps=ai?.priorities?.map(x=>[x.title,x.goal])||priorities();
 const impacts=(ai?.inactionImpact||(state.answers.cost_of_inaction||[])).filter(x=>!x.includes('暂时不会')).slice(0,4);
 const intent=serverResult?.leadIntent||{'现在就需要解决':'VERY HIGH','未来 1–3 个月':'HIGH','3–6 个月':'MEDIUM','6–12 个月':'LOW','暂时只是了解':'RESEARCH'}[state.answers.implementation_timeline];
 const metrics=[['运营效率',s.operational,25],['系统化程度',s.systemization,20],['关键人员独立',s.key,20],['财务控制',s.financial,15],['规模化能力',s.scalability,20]];
 report.innerHTML=`<nav class="report-nav"><a class="brand"><img class="brand-logo" src="/images/classlift_logo.png" alt=""><span>ClassLift</span></a><div class="report-nav-actions"><span class="report-id">Report ${serverResult?.leadId?.slice(0,8)||'Preview'}</span><button class="ghost-btn" onclick="window.print()">打印 / 保存 PDF</button><button class="ghost-btn" onclick="location.reload()">重新诊断</button></div></nav><article class="report-sheet"><header class="report-hero"><div><span class="report-kicker">YOUR BUSINESS SCALABILITY REPORT · ${new Date().toLocaleDateString('zh-CN')}</span><h2>${state.lead.organization||state.lead.name} 的<br>业务可规模化诊断</h2><p>${c[1]}</p></div><div class="big-score"><span>BUSINESS SCALABILITY SCORE</span><strong>${s.total}<span> / 100</span></strong><b>${c[0]}</b></div></header><div class="report-body"><div class="score-grid">${metrics.map(m=>`<div class="metric"><span>${m[0]}</span><strong>${m[1]}<small> / ${m[2]}</small></strong></div>`).join('')}</div>
 <section class="report-section"><span class="section-index">01 · YOUR DIRECTION</span><h3>你希望公司变成什么样？</h3><div class="goal-box">${ai?.desiredOutcomeSummary||`你的主要目标是「${state.answers.desired_outcome}」，因为你「${state.answers.motivation}」。`}<small>Based on your answers${ai?.aiGenerated?' · AI organized':''}</small></div></section>
 <section class="report-section"><span class="section-index">02 · TOP BOTTLENECKS</span><h3>目前最值得关注的运营瓶颈</h3><div class="bottlenecks">${bs.map((x,i)=>`<div class="insight-card"><b>0${i+1}</b><h4>${x[0]}</h4><p>${x[1]}</p></div>`).join('')}</div></section>
 <section class="report-section"><span class="section-index">03 · POTENTIAL IMPACTS</span><h3>这些问题可能持续造成的影响</h3><p>以下内容根据你选择的运营问题推断，用于帮助确定改善优先级，并不代表一定会发生。</p><div class="impact-grid">${(impacts.length?impacts:['目前信息不足以判断具体影响。']).map((x,i)=>`<div class="impact-card"><b>0${i+1}</b><span>${x}</span></div>`).join('')}</div></section>
 <section class="report-section"><span class="section-index">04 · TOP PRIORITY</span><h3>你最急需解决的问题</h3><div class="quote">“${state.answers.primary_pain}”</div></section>
 <section class="report-section"><span class="section-index">05 · PRIORITIES</span><h3>建议的三个改善优先级</h3><div class="priorities">${ps.map((x,i)=>`<div class="insight-card"><b>PRIORITY ${i+1}</b><h4>${x[0]}</h4><p>${x[1]}</p></div>`).join('')}</div></section>
 <div class="solution-cta"><div><h3>看看这些问题可以如何被系统化</h3><p>基于你的 ${c[0]} 诊断与 ${intent} 改善意愿，查看与你当前问题相关的能力。</p></div><button class="primary-btn" onclick="alert('下一步可在这里连接 ClassLift 个性化解决方案页面。')">看看系统如何解决我的三个问题 <span>→</span></button></div></div></article>`;
}

document.addEventListener('keydown',e=>{if(e.key==='Enter'&&quiz&&!quiz.classList.contains('hidden')&&e.target.tagName!=='TEXTAREA'){$('#questionForm').requestSubmit()}});
