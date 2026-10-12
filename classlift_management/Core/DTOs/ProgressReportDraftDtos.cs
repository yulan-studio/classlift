namespace Core.DTOs;

public sealed record ProgressReportInput(
    int SessionEnrollmentId,
    int CoachId,
    string WorkedOn,
    string PositiveResponse,
    string Difficulty,
    string ContinueNextTime);

public sealed record ProgressReportDraftResult(
    int DraftId,
    string DraftText,
    bool IsExisting);
