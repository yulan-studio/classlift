using System.Text.Json;

namespace ClassLift.Diagnostic.Models;

public sealed class DiagnosticLead
{
    public Guid Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public string BusinessType { get; set; } = "";
    public string StudentCount { get; set; } = "";
    public int AdminCount { get; set; }
    public string CurrentToolsJson { get; set; } = "[]";
    public string? ImprovementAreasJson { get; set; }
    public string? TopPrioritiesJson { get; set; }
    public string PrimaryPain { get; set; } = "";
    public string DesiredOutcome { get; set; } = "";
    public string Motivation { get; set; } = "";
    public string CostOfInactionJson { get; set; } = "[]";
    public string PreviousSolutionsJson { get; set; } = "[]";
    public string? RootCause { get; set; }
    public int KeyPersonDependency { get; set; }
    public string BuyingCriteriaJson { get; set; } = "[]";
    public string ImplementationTimeline { get; set; } = "";
    public string SelfIdentifiedPriority { get; set; } = "";
    public int OperationalEfficiencyScore { get; set; }
    public int SystemizationScore { get; set; }
    public int KeyPersonScore { get; set; }
    public int FinancialControlScore { get; set; }
    public int ScalabilityScore { get; set; }
    public int TotalScore { get; set; }
    public string Classification { get; set; } = "";
    public string LeadIntent { get; set; } = "";
    public string? AiSummary { get; set; }
    public string? RecommendedModulesJson { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string? Organization { get; set; }
    public string? WebsiteUrl { get; set; }
    public string? AdditionalNeeds { get; set; }

    public static DiagnosticLead From(CreateDiagnosticRequest request, ScoreResult score) => new()
    {
        Id = Guid.NewGuid(), CreatedAt = DateTimeOffset.UtcNow,
        BusinessType = request.BusinessType!.Trim(), StudentCount = request.StudentCount!.Trim(),
        AdminCount = request.AdminCount ?? 0, CurrentToolsJson = JsonSerializer.Serialize(request.CurrentTools),
        ImprovementAreasJson = JsonSerializer.Serialize(request.ImprovementAreas),
        TopPrioritiesJson = JsonSerializer.Serialize(request.TopPriorities),
        PrimaryPain = request.PrimaryPain!.Trim(),
        DesiredOutcome = string.IsNullOrWhiteSpace(request.AdditionalNeeds) ? "改善已选择的运营问题" : request.AdditionalNeeds.Trim(),
        Motivation = "改善运营效率与业务稳定性", CostOfInactionJson = JsonSerializer.Serialize(request.CostOfInaction ?? []),
        PreviousSolutionsJson = JsonSerializer.Serialize(request.PreviousSolutions ?? []), RootCause = request.RootCause?.Trim(),
        KeyPersonDependency = request.KeyPersonDependency ?? (request.ImprovementAreas?.Any(x => x.Contains("核心员工")) == true ? 5 : 15),
        BuyingCriteriaJson = JsonSerializer.Serialize(request.BuyingCriteria ?? []),
        ImplementationTimeline = request.ImplementationTimeline!.Trim(), SelfIdentifiedPriority = request.SelfIdentifiedPriority?.Trim() ?? request.PrimaryPain.Trim(),
        OperationalEfficiencyScore = score.OperationalEfficiency, SystemizationScore = score.Systemization,
        KeyPersonScore = score.KeyPersonIndependence, FinancialControlScore = score.FinancialControl,
        ScalabilityScore = score.Scalability, TotalScore = score.Total, Classification = score.Classification,
        LeadIntent = LeadIntentFor(request.ImplementationTimeline), Name = request.Name!.Trim(),
        Email = request.Email!.Trim().ToLowerInvariant(), Organization = request.Organization?.Trim(),
        WebsiteUrl = request.WebsiteUrl?.Trim(), AdditionalNeeds = request.AdditionalNeeds?.Trim()
    };

    public DiagnosticResponse ToResponse() => new(Id, CreatedAt,
        new ScoreResult(OperationalEfficiencyScore, SystemizationScore, KeyPersonScore,
            FinancialControlScore, ScalabilityScore, TotalScore, Classification, ""), LeadIntent,
        string.IsNullOrWhiteSpace(AiSummary) ? null : JsonSerializer.Deserialize<AiDiagnosticReport>(AiSummary));

    private static string LeadIntentFor(string? timeline) => timeline switch
    {
        "现在就需要解决" => "VERY HIGH", "未来 1–3 个月" => "HIGH", "3–6 个月" => "MEDIUM",
        "6–12 个月" => "LOW", _ => "RESEARCH"
    };
}
