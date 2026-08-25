namespace ClassLift.Diagnostic.Models;

public sealed record ReportInsight(string Title, string Explanation);
public sealed record ReportPriority(string Title, string Goal);

public sealed record AiDiagnosticReport(
    string SituationSummary,
    string DesiredOutcomeSummary,
    IReadOnlyList<ReportInsight> Bottlenecks,
    IReadOnlyList<string> InactionImpact,
    IReadOnlyList<ReportPriority> Priorities,
    IReadOnlyList<string> RelevantCapabilities,
    string SalesBrief,
    bool AiGenerated = false);
