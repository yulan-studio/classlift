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
        - bottlenecks 和 priorities 必须恰好各 3 项；
        - inactionImpact 只能改写用户明确选择的“不改变的影响”，不能添加新后果；
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
                request.PrimaryPain, request.DesiredOutcome, request.Motivation, request.CostOfInaction,
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
        var bottlenecks = new List<ReportInsight>();
        if (request.KeyPersonDependency <= 10)
            bottlenecks.Add(new("关键人员依赖", "多项核心工作可能集中在少数员工手中，交接韧性有提升空间。"));
        if (Contains(request.CurrentTools, "Excel", "多个", "纸张"))
            bottlenecks.Add(new("系统与数据分散", "信息可能存在于不同工具或表格中，增加重复输入与遗漏风险。"));
        if (Contains(request.PrimaryPain, "排课", "重复行政", "跟进"))
            bottlenecks.Add(new("重复行政工作", "高频、规则明确的工作仍在消耗团队时间。"));
        if (Contains(request.CostOfInaction, "增加行政", "人工成本"))
            bottlenecks.Add(new("增长与人力绑定", "按照目前方式增长，行政人数与人工成本可能需要同步增加。"));
        while (bottlenecks.Count < 3)
            bottlenecks.Add(new("流程标准化", "现有答案显示这一能力值得在下一阶段进一步验证和完善。"));

        var capabilities = new List<string>();
        if (Contains(request.PrimaryPain, "跟进", "沟通")) capabilities.Add("CRM + Automated Follow-up");
        if (Contains(request.PrimaryPain, "排课")) capabilities.Add("Scheduling Automation");
        if (Contains(request.PrimaryPain, "收费", "工资")) capabilities.Add("Billing + Payroll Automation");
        if (request.KeyPersonDependency <= 10) capabilities.Add("Workflow + Centralized Data + Permissions");
        if (Contains(request.DesiredOutcome, "第二家店")) capabilities.Add("Multi-location Management");
        if (capabilities.Count == 0) capabilities.Add("Owner Dashboard");

        return new(
            $"当前业务可规模化得分为 {scores.Total}/100（{scores.Classification}）。主要关注点是：{request.PrimaryPain}。",
            $"你的主要目标是「{request.DesiredOutcome}」，背后的原因是「{request.Motivation}」。",
            bottlenecks.Take(3).ToArray(),
            request.CostOfInaction?.ToArray() ?? [],
            [
                new("建立统一运营基础", $"围绕“{request.PrimaryPain}”梳理负责人、数据与标准流程。"),
                new("自动化重复工作", "选择高频、规则清晰的行政任务先自动化，并记录节省的时间。"),
                new("降低交接风险", "把关键员工经验转化为团队可执行、可追踪的工作流程。")
            ],
            capabilities,
            $"{request.BusinessType}；{request.StudentCount} 名客户；Timeline: {request.ImplementationTimeline}；Priority: {request.SelfIdentifiedPriority}",
            false);
    }

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
                type = "array", minItems = 3, maxItems = 3,
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
                type = "array", minItems = 3, maxItems = 3,
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
