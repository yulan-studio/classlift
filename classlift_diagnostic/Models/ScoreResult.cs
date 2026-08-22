namespace ClassLift.Diagnostic.Models;

public sealed record ScoreResult(
    int OperationalEfficiency,
    int Systemization,
    int KeyPersonIndependence,
    int FinancialControl,
    int Scalability,
    int Total,
    string Classification,
    string ClassificationDescription);

public sealed record DiagnosticResponse(
    Guid LeadId,
    DateTimeOffset CreatedAt,
    ScoreResult Scores,
    string LeadIntent);
