using Core.Models;

namespace Core.Interfaces;

public interface IProgressReportDraftRepository
{
    Task<CourseEnrollment?> GetSessionAsync(int sessionEnrollmentId, CancellationToken cancellationToken = default);
    Task<ProgressReportDraft?> GetCurrentDraftAsync(int sessionEnrollmentId, CancellationToken cancellationToken = default);
    Task<ProgressReportDraft?> GetDraftForCoachAsync(int draftId, int coachId, CancellationToken cancellationToken = default);
    Task<CourseEnrollment?> GetCompletedSessionAsync(int sessionEnrollmentId, CancellationToken cancellationToken = default);
    void Add(ProgressReportDraft draft);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
