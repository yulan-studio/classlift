using Core.Contexts;
using Core.Interfaces;
using Core.Models;
using Core.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Core.Repositories;

public class ActivityEnrollmentRepository : IActivityEnrollmentRepository
{
    private readonly AppDbContext _context;
    public ActivityEnrollmentRepository(AppDbContext context) => _context = context;
    private IQueryable<ActivityEnrollment> Query() => _context.ActivityEnrollments.Include(e => e.Activity).Include(e => e.ActivitySession);
    public async Task<bool> AddAsync(ActivityEnrollment e) { await _context.ActivityEnrollments.AddAsync(e); return await _context.SaveChangesAsync() > 0; }
    public async Task<bool> RemoveAsync(int id) { var e = await _context.ActivityEnrollments.FindAsync(id); if (e == null) return false; e.Status = "Deleted"; e.UpdatedDate = DateTime.UtcNow; return await _context.SaveChangesAsync() > 0; }
    public async Task<IEnumerable<ActivityEnrollment>> GetUpcomingEnrollmentsByChildAsync(int id) => await Query().Where(e => e.ChildID == id && e.ActivitySession != null && e.ActivitySession.ScheduledAt >= DateTime.UtcNow && e.Status == "Confirmed").OrderBy(e => e.ActivitySession!.ScheduledAt).ToListAsync();
    public async Task<IEnumerable<ActivityEnrollment>> GetPastEnrollmentsByChildAsync(int id) => await Query().Where(e => e.ChildID == id && e.ActivitySession != null && e.ActivitySession.ScheduledAt < DateTime.UtcNow && e.Status != "Deleted").OrderBy(e => e.ActivitySession!.ScheduledAt).ToListAsync();
    public async Task<IEnumerable<ActivityEnrollment>> GetEnrollmentsByChildAsync(int id, string status) => await Query().Where(e => e.ChildID == id && e.Status == status).OrderBy(e => e.ActivitySession!.ScheduledAt).ToListAsync();
    public Task<IEnumerable<ActivityEnrollment>> GetRegisteredEnrollmentsByChildAsync(int id) => GetEnrollmentsByChildAsync(id, "Registered");
    public async Task<IEnumerable<ActivityEnrollment>> GetFinishedEnrollmentsByChildAsync(int id) => await Query().Where(e => e.ChildID == id && (e.Status == "Completed" || e.Status == "Canceled") && e.ActivitySession != null && e.ActivitySession.ScheduledAt < DateTime.UtcNow).OrderBy(e => e.ActivitySession!.ScheduledAt).ToListAsync();
    private IQueryable<ActivityEnrollmentViewModel> ViewQuery(int id) => Query().Where(e => e.ChildID == id && e.Status != "Deleted" && e.ActivitySessionID.HasValue && e.ActivitySession != null).Select(e => new ActivityEnrollmentViewModel { ActivityID = e.ActivityID, ActivitySessionID = e.ActivitySessionID!.Value, ChildID = e.ChildID, EnrollmentID = e.EnrollmentID, Title = e.Activity.Title, Address = e.Activity.Address, ScheduleType = e.Activity.ScheduleType, ScheduledAt = e.ActivitySession!.ScheduledAt, Description = e.Activity.Description, Status = e.Status, TotalCost = _context.Fees.Where(f => f.ActivityEnrollmentID == e.EnrollmentID).Select(f => f.TotalCost).FirstOrDefault(), PaymentModel = _context.Fees.Where(f => f.ActivityEnrollmentID == e.EnrollmentID).Select(f => f.PaymentModel).FirstOrDefault()!, PaymentDescription = _context.Fees.Where(f => f.ActivityEnrollmentID == e.EnrollmentID).Select(f => f.Description).FirstOrDefault(), IsPaid = _context.Fees.Where(f => f.ActivityEnrollmentID == e.EnrollmentID).Select(f => f.IsPaid).FirstOrDefault() });
    public async Task<IEnumerable<ActivityEnrollmentViewModel>> GetAllEnrollmentsViewByChildAsync(int id) => await ViewQuery(id).OrderBy(e => e.ScheduledAt).ToListAsync();
    public async Task<IEnumerable<ActivityEnrollmentViewModel>> GetEnrollmentsViewByChildAsync(int id, string status) => await ViewQuery(id).Where(e => e.Status == status).OrderBy(e => e.ScheduledAt).ToListAsync();
    public async Task<IEnumerable<ActivityEnrollmentViewModel>> GetUpcomingEnrollmentsViewByChildAsync(int id) => await ViewQuery(id).Where(e => (e.Status == "Registered" || e.Status == "Confirmed") && e.ScheduledAt >= DateTime.UtcNow).OrderBy(e => e.ScheduledAt).ToListAsync();
    public Task<IEnumerable<ActivityEnrollment>> UpdateActivityStatusToCompletedAsync() => Complete(_context, CancellationToken.None);
    public Task<IEnumerable<ActivityEnrollment>> UpdateActivityStatusToCompletedAsync(AppDbContext db, CancellationToken ct) => Complete(db, ct);
    private static async Task<IEnumerable<ActivityEnrollment>> Complete(AppDbContext db, CancellationToken ct) { var rows = await db.ActivityEnrollments.Include(e => e.ActivitySession).Where(e => e.Status == "Confirmed" && e.ActivitySession != null && e.ActivitySession.Status == "Completed").ToListAsync(ct); foreach (var e in rows) e.Status = "Completed"; await db.SaveChangesAsync(ct); return rows; }
    public async Task<bool> UpdateActivityStatusToCanceledAsync(int id) { var rows = await _context.ActivityEnrollments.Where(e => e.ActivityID == id && (e.Status == "Registered" || e.Status == "Confirmed")).ToListAsync(); foreach (var e in rows) e.Status = "Canceled"; return await _context.SaveChangesAsync() >= 0; }
    public async Task<bool> UpdateActivityEnrollmentStatusToConfirmedAsync(int id) { var e = await _context.ActivityEnrollments.FindAsync(id); if (e == null || e.Status != "Registered") return false; e.Status = "Confirmed"; return await _context.SaveChangesAsync() > 0; }
    public async Task<bool> UpdateActivityStatusToClosedAsync(int id) { var rows = await _context.ActivitySessions.Where(s => s.ActivityID == id && s.Status == "Open").ToListAsync(); foreach (var s in rows) s.Status = "Closed"; return await _context.SaveChangesAsync() >= 0; }
}
