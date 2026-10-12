using Core.DTOs;
using Core.Models;

namespace Core.Interfaces;

public interface IProgressReportGenerator
{
    Task<string> GenerateAsync(ProgressReportInput input, CancellationToken cancellationToken = default);
}

public interface IProgressReportDraftService
{
    Task<ProgressReportDraftResult> GenerateDraftAsync(ProgressReportInput input, CancellationToken cancellationToken = default);
    Task<bool> ApproveAsync(int draftId, int coachId, string finalText, CancellationToken cancellationToken = default);
}
