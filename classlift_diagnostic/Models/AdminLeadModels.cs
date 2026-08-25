using System.Text.Json;

namespace ClassLift.Diagnostic.Models;

public sealed record AdminLoginRequest(string? Username, string? Password);

public sealed record AdminLeadSummary(
    Guid Id, DateTimeOffset CreatedAt, string Name, string Email, string? Organization, string? WebsiteUrl,
    string BusinessType, string StudentCount, string PrimaryPain,
    string ImplementationTimeline, int TotalScore, string Classification, string LeadIntent);

public sealed record AdminLeadDetail(
    Guid Id, DateTimeOffset CreatedAt, string Name, string Email, string? Organization, string? WebsiteUrl,
    string BusinessType, string StudentCount, IReadOnlyList<string> CurrentTools,
    IReadOnlyList<string> ImprovementAreas, IReadOnlyList<string> TopPriorities, string PrimaryPain, string ImplementationTimeline,
    string? AdditionalNeeds, int OperationalEfficiencyScore, int SystemizationScore,
    int KeyPersonScore, int FinancialControlScore, int ScalabilityScore, int TotalScore,
    string Classification, string LeadIntent, AiDiagnosticReport? Report)
{
    public static AdminLeadDetail From(DiagnosticLead lead) => new(
        lead.Id, lead.CreatedAt, lead.Name, lead.Email, lead.Organization, lead.WebsiteUrl,
        lead.BusinessType, lead.StudentCount, ParseList(lead.CurrentToolsJson),
        ParseList(lead.ImprovementAreasJson), ParseList(lead.TopPrioritiesJson), lead.PrimaryPain, lead.ImplementationTimeline,
        lead.AdditionalNeeds, lead.OperationalEfficiencyScore, lead.SystemizationScore,
        lead.KeyPersonScore, lead.FinancialControlScore, lead.ScalabilityScore, lead.TotalScore,
        lead.Classification, lead.LeadIntent,
        string.IsNullOrWhiteSpace(lead.AiSummary) ? null : JsonSerializer.Deserialize<AiDiagnosticReport>(lead.AiSummary));

    private static IReadOnlyList<string> ParseList(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return [];
        try { return JsonSerializer.Deserialize<string[]>(json) ?? []; }
        catch (JsonException) { return []; }
    }
}

public sealed record AdminLeadPage(IReadOnlyList<AdminLeadSummary> Items, int Total, int Page, int PageSize);
