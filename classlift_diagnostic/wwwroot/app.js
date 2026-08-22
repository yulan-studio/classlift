const state = { step: 0, answers: {} };
const option = (value, label = value) => ({ value, label });
const questions = [
  { id:'business_type', section:'ABOUT YOUR BUSINESS', aside:'先了解一点\n你的业务', title:'你的机构属于哪一类？', type:'single', options:['特殊教育','课后辅导','音乐学校','美术学校','编程学校','健身 / 私人教练','非营利组织','其他课程 / 活动机构'].map(x=>option(x)) },
  { id:'student_count', section:'ABOUT YOUR BUSINESS', aside:'业务规模\n会影响诊断基准', title:'目前大约有多少活跃学生 / 客户？', type:'single', options:['1–50','51–100','101–300','301–500','501–1,000','1,000+'].map(x=>option(x)) },
  { id:'admin_count', section:'ABOUT YOUR BUSINESS', aside:'业务规模\n会影响诊断基准', title:'目前有多少行政 / 运营工作人员？', help:'请输入人数（包括全职与固定兼职人员）', type:'number', placeholder:'例如：4' },
  { id:'current_tools', section:'CURRENT STATE', aside:'现在，你的业务\n如何运转？', title:'你们目前主要用什么方式管理客户、课程、收费和员工？', help:'可以选择多项', type:'multi', options:['Excel / Google Sheets','多个不同软件组合','一个课程管理软件','纸张 / 人工记录','邮件 / WhatsApp / 微信 / 短信','主要依赖员工记住和执行','自己开发的系统','其他'].map(x=>option(x)) },
  { id:'primary_pain', section:'THE FRICTION', aside:'找到真正限制\n增长的摩擦力', title:'如果现在只能解决一个问题，你最希望解决哪一个？', type:'single', options:['客户咨询后没有被及时跟进','客户沟通和重要信息容易遗漏','排课和调整课程太繁琐','收费、Credit、课时记录混乱','老师工资计算麻烦','员工每天有大量重复行政工作','老板无法及时了解公司的运营情况','公司太依赖某几个核心员工','客户增加就必须继续增加行政人员','其他'].map(x=>option(x)) },
  { id:'desired_outcome', section:'YOUR DIRECTION', aside:'你真正想要的\n不是更多软件', title:'如果未来 12 个月只能实现一个结果，你最希望是哪一个？', type:'single', options:['服务更多客户，但不增加太多行政人员','提高公司的利润率','老板可以随时了解公司的真实运营情况','减少员工加班和重复工作','公司不再依赖某一个关键员工','老板减少参与日常运营','建立第二家店 / 新的 Location','让现有业务更标准、更稳定','其他'].map(x=>option(x)) },
  { id:'motivation', section:'YOUR WHY', aside:'理解改变背后\n真正的原因', title:'为什么这个结果现在对你很重要？', help:'选择最接近你真实想法的一项', type:'single', options:['希望获得更多利润','希望减少人工成本','希望有更多个人时间','希望公司可以继续扩大','准备建立第二家店','不希望公司过度依赖员工','希望真正经营公司，而不是每天给自己的公司“打工”','其他'].map(x=>option(x)) },
  { id:'cost_of_inaction', section:'LOOKING AHEAD', aside:'如果维持现状，\n会发生什么？', title:'如果未来 12 个月继续按照现在的方法运营，你认为最可能发生什么？', help:'最多选择 3 项', type:'multi', max:3, options:['必须继续增加行政人员','人工成本继续上涨','无法服务更多客户','收入增加，但利润没有明显增加','老板越来越忙','更加依赖几个核心员工','客户体验下降','无法顺利建立第二家店','运营错误越来越多','我认为暂时不会有明显影响','其他'].map(x=>option(x)) },
  { id:'previous_solutions', section:'WHAT YOU TRIED', aside:'过去的方法\n留下了什么？', title:'你过去尝试过哪些方法解决这些问题？', help:'可以选择多项', type:'multi', options:['招更多行政人员','Excel / Google Sheets','买过 / 正在使用课程管理软件','使用多个不同软件组合','找外包人员','依赖一个非常能干的员工','自己建立 SOP / 工作流程','自己开发系统','暂时停止扩大业务','还没有认真解决过','其他'].map(x=>option(x)) },
  { id:'key_person_dependency', section:'BUSINESS RESILIENCE', aside:'员工很重要，\n系统也应该可靠', title:'如果最重要的一名行政 / 运营员工明天无法工作一个月，公司会怎么样？', type:'single', options:[option('20','基本没有影响'),option('15','会有一些影响，但其他人可以快速接手'),option('10','很多工作会变慢'),option('5','多项重要工作可能混乱'),option('0','我甚至不知道一些重要资料在哪里')] },
  { id:'buying_criteria', section:'IDEAL FUTURE', aside:'定义对你有价值的\n解决方案', title:'如果有一个系统真正帮助你解决这些问题，最重要的 3 个条件是什么？', help:'最多选择 3 项', type:'multi', max:3, options:['员工容易学习和使用','不需要同时使用很多软件','老板随时可以看到公司运营情况','大量重复工作可以自动完成','工作不再依赖员工自己记住','员工离职后，新员工可以快速接手','客户增加，但行政人员不需要同比增加','可以支持多个 Location / 第二家店','收费、Credit、工资更加自动化','客户沟通更加统一'].map(x=>option(x)) },
  { id:'implementation_timeline', section:'TIMING', aside:'改变不一定要急，\n但需要清晰', title:'你希望什么时候真正改善这些问题？', type:'single', options:['现在就需要解决','未来 1–3 个月','3–6 个月','6–12 个月','暂时只是了解'].map(x=>option(x)) },
  { id:'self_identified_priority', section:'YOUR CONCLUSION', aside:'最重要的洞察，\n由你自己说出来', title:'回头看刚才的回答，你认为现在最应该优先改变的一件事情是什么？', help:'没有标准答案，请用你自己的话描述', type:'textarea', placeholder:'例如：我们不能继续依赖一个员工处理所有事情……' }
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
  if(q.type==='single'||q.type==='multi') control=`<div class="options">${q.options.map((o,i)=>`<div class="option"><input id="o${i}" name="answer" type="${q.type==='multi'?'checkbox':'radio'}" value="${o.value.replaceAll('"','&quot;')}" ${Array.isArray(saved)?saved.includes(o.value)?'checked':'':saved===o.value?'checked':''}><label for="o${i}">${o.label}</label></div>`).join('')}</div>`;
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
  if(!value || (Array.isArray(value)&&!value.length)){ $('#formError').textContent='请选择或填写一个答案后继续。'; return; }
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
    adminCount:Number(a.admin_count),
    currentTools:a.current_tools,
    primaryPain:a.primary_pain,
    desiredOutcome:a.desired_outcome,
    motivation:a.motivation,
    costOfInaction:a.cost_of_inaction,
    previousSolutions:a.previous_solutions,
    rootCause:a.root_cause||null,
    keyPersonDependency:Number(a.key_person_dependency),
    buyingCriteria:a.buying_criteria,
    implementationTimeline:a.implementation_timeline,
    selfIdentifiedPriority:a.self_identified_priority,
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
 const c=remote?[remote.classification,remote.classificationDescription]:classification(s.total), bs=bottlenecks(), ps=priorities(), impacts=(state.answers.cost_of_inaction||[]).filter(x=>!x.includes('暂时不会')).slice(0,4); const intent=serverResult?.leadIntent||{'现在就需要解决':'VERY HIGH','未来 1–3 个月':'HIGH','3–6 个月':'MEDIUM','6–12 个月':'LOW','暂时只是了解':'RESEARCH'}[state.answers.implementation_timeline];
 const metrics=[['运营效率',s.operational,25],['系统化程度',s.systemization,20],['关键人员独立',s.key,20],['财务控制',s.financial,15],['规模化能力',s.scalability,20]];
 report.innerHTML=`<nav class="report-nav"><a class="brand"><span class="brand-mark">C</span><span>ClassLift</span></a><div class="report-nav-actions"><span class="report-id">Report ${serverResult?.leadId?.slice(0,8)||'Preview'}</span><button class="ghost-btn" onclick="window.print()">打印 / 保存 PDF</button><button class="ghost-btn" onclick="location.reload()">重新诊断</button></div></nav><article class="report-sheet"><header class="report-hero"><div><span class="report-kicker">YOUR BUSINESS SCALABILITY REPORT · ${new Date().toLocaleDateString('zh-CN')}</span><h2>${state.lead.organization||state.lead.name} 的<br>业务可规模化诊断</h2><p>${c[1]}</p></div><div class="big-score"><span>BUSINESS SCALABILITY SCORE</span><strong>${s.total}<span> / 100</span></strong><b>${c[0]}</b></div></header><div class="report-body"><div class="score-grid">${metrics.map(m=>`<div class="metric"><span>${m[0]}</span><strong>${m[1]}<small> / ${m[2]}</small></strong></div>`).join('')}</div>
 <section class="report-section"><span class="section-index">01 · YOUR DIRECTION</span><h3>你希望公司变成什么样？</h3><div class="goal-box">你的主要目标是「${state.answers.desired_outcome}」，因为你「${state.answers.motivation}」。<small>Based on your answers</small></div></section>
 <section class="report-section"><span class="section-index">02 · TOP BOTTLENECKS</span><h3>目前最值得关注的三个瓶颈</h3><div class="bottlenecks">${bs.map((x,i)=>`<div class="insight-card"><b>0${i+1}</b><h4>${x[0]}</h4><p>${x[1]}</p></div>`).join('')}</div></section>
 <section class="report-section"><span class="section-index">03 · COST OF INACTION</span><h3>如果什么都不改变</h3><p>根据你自己的判断，维持目前方式最可能带来：</p><div class="impact-flow">${(impacts.length?impacts:['暂时不会有明显影响']).map((x,i)=>`${i?'<i>→</i>':''}<span>${x}</span>`).join('')}</div></section>
 <section class="report-section"><span class="section-index">04 · YOUR OWN WORDS</span><h3>你认为最应该先改变什么</h3><div class="quote">“${state.answers.self_identified_priority}”</div></section>
 <section class="report-section"><span class="section-index">05 · PRIORITIES</span><h3>建议的三个改善优先级</h3><div class="priorities">${ps.map((x,i)=>`<div class="insight-card"><b>PRIORITY ${i+1}</b><h4>${x[0]}</h4><p>${x[1]}</p></div>`).join('')}</div></section>
 <div class="solution-cta"><div><h3>看看这些问题可以如何被系统化</h3><p>基于你的 ${c[0]} 诊断与 ${intent} 改善意愿，查看与你当前问题相关的能力。</p></div><button class="primary-btn" onclick="alert('下一步可在这里连接 ClassLift 个性化解决方案页面。')">看看系统如何解决我的三个问题 <span>→</span></button></div></div></article>`;
}

document.addEventListener('keydown',e=>{if(e.key==='Enter'&&quiz&&!quiz.classList.contains('hidden')&&e.target.tagName!=='TEXTAREA'){$('#questionForm').requestSubmit()}});
