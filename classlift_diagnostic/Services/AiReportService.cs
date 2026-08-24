using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ClassLift.Diagnostic.Models;

namespace ClassLift.Diagnostic.Services;

public sealed class AiReportService(HttpClient httpClient, IConfiguration configuration, ILogger<AiReportService> logger)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private const string Instructions = """
        你是一名帮助课程、活动和服务型机构改善运营的 Business Scalability Consultant。
        你的任务是整理用户已经表达的事实，而不是销售软件。必须遵守：
        - 不制造恐惧，不夸大问题，不编造用户没有提到的痛点；
        - 优先使用用户自己的回答，将问题归因于流程和系统而不是个人；
        - 不评价员工好坏，不主动劝说购买；
        - 建议必须具体、简洁、可执行；信息不足时明确写“不足以判断”；
        - bottlenecks 和 priorities 必须各提供 1–3 项，最高优先级必须排在第一项；
        - bottlenecks 必须严格来自用户选择的 topPriorities，并保持 primaryPain 排在第一项；
        - inactionImpact 必须是与瓶颈直接对应、措辞克制的可能影响，不能描述为必然结果；
        - 全部面向客户的内容使用简体中文，SalesBrief 可以中英混合但要简洁。
        """;

    public async Task<AiDiagnosticReport> GenerateAsync(
        CreateDiagnosticRequest request,
        ScoreResult scores,
        CancellationToken cancellationToken)
    {
        var apiKey = configuration["OPENAI_API_KEY"];
        if (string.IsNullOrWhiteSpace(apiKey)) return BuildFallback(request, scores);

        try
        {
            var model = configuration["OPENAI_MODEL"] ?? "gpt-5-mini";
            var userInput = JsonSerializer.Serialize(new
            {
                request.BusinessType, request.StudentCount, request.AdminCount, request.CurrentTools,
                request.PrimaryPain, request.TopPriorities, request.ImprovementAreas, request.AdditionalNeeds,
                request.DesiredOutcome, request.Motivation, request.CostOfInaction,
                request.PreviousSolutions, request.RootCause, request.KeyPersonDependency,
                request.BuyingCriteria, request.ImplementationTimeline, request.SelfIdentifiedPriority,
                Scores = scores
            }, JsonOptions);

            var payload = new
            {
                model,
                instructions = Instructions,
                input = userInput,
                store = false,
                max_output_tokens = 1800,
                text = new
                {
                    format = new
                    {
                        type = "json_schema",
                        name = "business_scalability_report",
                        strict = true,
                        schema = ReportSchema
                    }
                }
            };

            using var message = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/responses");
            message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            message.Content = new StringContent(JsonSerializer.Serialize(payload, JsonOptions), Encoding.UTF8, "application/json");
            using var response = await httpClient.SendAsync(message, cancellationToken);
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"OpenAI returned {(int)response.StatusCode}: {body[..Math.Min(body.Length, 500)]}");

            using var envelope = JsonDocument.Parse(body);
            var outputText = envelope.RootElement.GetProperty("output")
                .EnumerateArray()
                .Where(item => item.TryGetProperty("type", out var type) && type.GetString() == "message")
                .SelectMany(item => item.GetProperty("content").EnumerateArray())
                .First(item => item.GetProperty("type").GetString() == "output_text")
                .GetProperty("text").GetString();

            var report = JsonSerializer.Deserialize<AiDiagnosticReport>(outputText!, JsonOptions)
                ?? throw new JsonException("OpenAI report was empty.");
            return report with { AiGenerated = true };
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            logger.LogWarning(exception, "AI report generation failed; deterministic fallback was used.");
            return BuildFallback(request, scores);
        }
    }

    private static AiDiagnosticReport BuildFallback(CreateDiagnosticRequest request, ScoreResult scores)
    {
        var orderedAreas = new[] { request.PrimaryPain }
            .Concat(request.TopPriorities ?? [])
            .Where(area => !string.IsNullOrWhiteSpace(area))
            .Distinct()
            .Select(area => ProfileFor(area!, request.AdditionalNeeds))
            .GroupBy(profile => profile.Title)
            .Select(group => group.First())
            .Take(3)
            .ToArray();

        var bottlenecks = orderedAreas.Select(profile => new ReportInsight(profile.Title, profile.Explanation)).ToArray();
        var impacts = orderedAreas.Select(profile => profile.Impact).ToArray();
        var priorities = orderedAreas.Select(profile => new ReportPriority(profile.PriorityTitle, profile.PriorityGoal)).ToArray();
        var capabilities = orderedAreas.Select(profile => profile.Capability).Distinct().ToArray();

        return new(
            $"当前业务可规模化得分为 {scores.Total}/100（{scores.Classification}）。主要关注点是：{request.PrimaryPain}。",
            string.IsNullOrWhiteSpace(request.AdditionalNeeds)
                ? $"你希望优先解决「{request.PrimaryPain}」，并改善其他已选择的运营问题。"
                : $"你希望优先解决「{request.PrimaryPain}」。你补充的需求是：{request.AdditionalNeeds}",
            bottlenecks,
            impacts,
            priorities,
            capabilities,
            $"{request.BusinessType}；{request.StudentCount} 名客户；Timeline: {request.ImplementationTimeline}；Priority: {request.PrimaryPain}；Additional needs: {request.AdditionalNeeds ?? "无"}",
            false);
    }

    private static AreaProfile ProfileFor(string area, string? additionalNeeds) => area switch
    {
        "客户咨询与 Follow-up" => new("客户跟进流程", "客户咨询后的负责人、下一步和跟进时间缺少统一、可追踪的流程。", "潜在线索可能无法及时进入下一步，团队也难以判断哪些客户需要继续跟进。", "统一客户跟进", "明确每条咨询的负责人、状态、下一步和跟进时间。", "CRM + Automated Follow-up"),
        "客户沟通和信息记录" => new("客户信息与沟通", "沟通记录和重要信息没有集中在所有相关员工都能查看的位置。", "不同员工可能掌握不同信息，回复一致性和交接效率会继续受到影响。", "集中客户沟通记录", "让客户资料、沟通历史和待办事项可以在一个位置查询。", "Centralized Customer Data"),
        "排课、改课与教室安排" => new("排课与资源协调", "排课、改课和教室安排需要较多人工协调，流程容易被临时变化打断。", "课程调整高峰可能继续占用行政时间，并增加时间或资源冲突的可能性。", "标准化排课与改课", "统一课程、教师和教室的可用信息，减少重复确认。", "Scheduling Automation"),
        "收费、Credit 与课时记录" => new("收费与课时控制", "收费、Credit 和剩余课时之间缺少清晰、统一的记录与核对方式。", "对账时间可能继续增加，收费状态、剩余课时和调整记录也会更难及时核实。", "统一收费与课时记录", "让付款、Credit、课时消耗和调整记录保持关联并可追踪。", "Billing + Credit Automation"),
        "老师工资计算" => new("老师工资核算", "工资计算依赖人工汇总课程、出勤或不同计费规则。", "每个工资周期可能继续产生重复核算和复核工作，占用管理时间。", "简化工资核算", "从已确认的课程和出勤数据生成可复核的工资计算基础。", "Payroll Automation"),
        "Attendance / 签到" => new("出勤记录", "Attendance 的记录、补录和后续处理缺少统一流程。", "缺失或延迟的出勤数据可能继续影响课时、收费和工资核对。", "统一 Attendance 流程", "在课程发生时完成记录，并明确缺勤、补课和更正的处理方式。", "Attendance Management"),
        "重复行政工作太多" => new("重复行政工作", "团队在高频、规则明确的任务上投入了较多手工时间。", "客户数量增加时，相同的行政工作量可能同步增加，挤压更高价值工作的时间。", "优先自动化重复任务", "选择频率最高、规则最清楚的一项工作先自动化并记录节省时间。", "Workflow Automation"),
        "老板缺少实时运营数据" => new("经营数据可视性", "关键运营信息无法被及时汇总为老板可以直接使用的数据。", "经营判断可能继续依赖人工汇报或滞后数据，问题较难被及时发现。", "建立经营仪表板", "先定义每天或每周必须看到的核心指标及其数据来源。", "Owner Dashboard"),
        "过度依赖核心员工" => new("关键人员依赖", "部分核心流程、资料或判断集中在少数员工手中。", "当关键员工休假或离职时，相关工作可能变慢，其他员工也需要更多时间接手。", "降低关键人员依赖", "把关键资料、判断规则和操作步骤转化为团队可执行的流程。", "Workflow + Centralized Data + Permissions"),
        "不同软件之间数据分散" => new("系统与数据分散", "客户、课程和财务信息分布在多个工具中，需要重复查找或录入。", "重复输入和人工核对可能继续增加，也更难获得完整、及时的运营视图。", "建立统一数据来源", "明确每类核心数据的唯一来源，并逐步减少重复录入。", "Integrated Operations Platform"),
        "客户增长就要增加行政人员" => new("增长与行政人力绑定", "现有流程的工作量会随着客户数量增加而近似同步增加。", "业务增长可能继续带来相近比例的行政成本，限制利润率和服务容量。", "解除增长与人力绑定", "优先改造随客户数量增长最快的行政流程。", "Scalable Workflow Automation"),
        "多 Location / 第二家店管理" => new("多地点复制能力", "现有流程和数据结构还不容易在新的 Location 中一致执行。", "新增地点时可能需要重新建立大量人工流程，也更难统一比较各地点表现。", "建立可复制的运营标准", "先统一地点、员工、课程和权限的管理规则。", "Multi-location Management"),
        "标准工作流程（SOP）与员工交接" => new("流程标准化与交接", "重要工作缺少清晰、可执行并能持续更新的标准流程。", "新员工培训和工作交接可能继续依赖口头说明，执行结果容易因人而异。", "建立可执行的标准流程", "从最高频、风险最高的流程开始记录步骤、负责人和异常处理。", "SOP + Workflow Management"),
        _ => new("其他运营需求", string.IsNullOrWhiteSpace(additionalNeeds) ? "你选择了其他需要改善的运营问题，但目前信息不足以进一步判断。" : additionalNeeds, "如果缺少更具体的流程和目标，目前不足以判断它对运营的长期影响。", "进一步定义问题", "记录问题发生的场景、频率、参与人员和希望达到的结果。", "Needs Assessment")
    };

    private sealed record AreaProfile(string Title, string Explanation, string Impact, string PriorityTitle, string PriorityGoal, string Capability);

    private static bool Contains(string? value, params string[] terms) =>
        value is not null && terms.Any(value.Contains);
    private static bool Contains(IEnumerable<string>? values, params string[] terms) =>
        values is not null && values.Any(value => terms.Any(value.Contains));

    private static readonly object ReportSchema = new
    {
        type = "object",
        additionalProperties = false,
        properties = new
        {
            situationSummary = new { type = "string" },
            desiredOutcomeSummary = new { type = "string" },
            bottlenecks = new
            {
                type = "array", minItems = 1, maxItems = 3,
                items = new
                {
                    type = "object", additionalProperties = false,
                    properties = new { title = new { type = "string" }, explanation = new { type = "string" } },
                    required = new[] { "title", "explanation" }
                }
            },
            inactionImpact = new { type = "array", items = new { type = "string" } },
            priorities = new
            {
                type = "array", minItems = 1, maxItems = 3,
                items = new
                {
                    type = "object", additionalProperties = false,
                    properties = new { title = new { type = "string" }, goal = new { type = "string" } },
                    required = new[] { "title", "goal" }
                }
            },
            relevantCapabilities = new { type = "array", items = new { type = "string" } },
            salesBrief = new { type = "string" }
        },
        required = new[] { "situationSummary", "desiredOutcomeSummary", "bottlenecks", "inactionImpact", "priorities", "relevantCapabilities", "salesBrief" }
    };
}
