using Core.Contexts;
using Core.Interfaces;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Core.Repositories;

public sealed class ProgressReportDraftRepository : IProgressReportDraftRepository
{
    private readonly AppDbContext _context;

    public ProgressReportDraftRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<CourseEnrollment?> GetSessionAsync(int sessionEnrollmentId, CancellationToken cancellationToken = default) =>
        _context.CourseEnrollments
            .Include(x => x.Course)
            .SingleOrDefaultAsync(x => x.EnrollmentID == sessionEnrollmentId, cancellationToken);

    public Task<ProgressReportDraft?> GetCurrentDraftAsync(int sessionEnrollmentId, CancellationToken cancellationToken = default) =>
        _context.ProgressReportDrafts
            .SingleOrDefaultAsync(x => x.SessionEnrollmentId == sessionEnrollmentId && x.Status != "Approved", cancellationToken);

    public Task<ProgressReportDraft?> GetDraftForCoachAsync(int draftId, int coachId, CancellationToken cancellationToken = default) =>
        _context.ProgressReportDrafts
            .SingleOrDefaultAsync(x => x.ProgressReportDraftId == draftId && x.CoachId == coachId, cancellationToken);

    public Task<CourseEnrollment?> GetCompletedSessionAsync(int sessionEnrollmentId, CancellationToken cancellationToken = default) =>
        _context.CourseEnrollments
            .SingleOrDefaultAsync(x => x.EnrollmentID == sessionEnrollmentId && x.Status == "Completed", cancellationToken);

    public void Add(ProgressReportDraft draft) => _context.ProgressReportDrafts.Add(draft);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);
}
