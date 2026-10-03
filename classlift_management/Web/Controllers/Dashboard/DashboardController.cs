


using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Core.Services;

using Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Core.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Core.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Web.Controllers.Dashboard
{
    [Route("Dashboard")]
    public class DashboardController : Controller
    {


        private readonly AppDbContext _db;

        public DashboardController(AppDbContext db)
        {
            _db = db;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("Admin")]
        public IActionResult Admin()
        {
            return View();
        }

        [Authorize(Roles = "Staff")]
        [HttpGet("Staff")]
        public IActionResult Staff()
        {
            return View();
        }

        [Authorize(Roles = "Staff")]
        [HttpGet("/Staff/Notifications")]
        public async Task<IActionResult> StaffNotifications()
        {
            var roots = _db.CourseEnrollments
                .AsNoTracking()
                .Where(e => e.EnrollmentID_Ref == null && e.ChildID.HasValue);

            var model = new StaffNotificationsViewModel
            {
                UnconfirmedPrivate = await GetItems(roots.Where(e => e.Status == "Registered" && e.Course.CourseType == "Private" && !_db.Fees.Any(f => f.CourseEnrollmentID == e.EnrollmentID && f.IsPaid)), "/Child/Participation/{0}?tab=ManageRegistrations"),
                UnconfirmedGroup = await GetItems(roots.Where(e => e.Status == "Registered" && e.Course.CourseType == "Group" && !_db.Fees.Any(f => f.CourseEnrollmentID == e.EnrollmentID && f.IsPaid)), "/Child/Participation/{0}?tab=ManageRegistrations"),
                PaidUnconfirmedPrivate = await GetItems(roots.Where(e => e.Status == "Registered" && e.Course.CourseType == "Private" && _db.Fees.Any(f => f.CourseEnrollmentID == e.EnrollmentID && f.IsPaid)), "/Child/Participation/{0}?tab=ManageRegistrations"),
                PaidUnconfirmedGroup = await GetItems(roots.Where(e => e.Status == "Registered" && e.Course.CourseType == "Group" && _db.Fees.Any(f => f.CourseEnrollmentID == e.EnrollmentID && f.IsPaid)), "/Child/Participation/{0}?tab=ManageRegistrations"),
                UnpaidPrivate = await GetItems(roots.Where(e => e.Status == "Confirmed" && e.Course.CourseType == "Private" && _db.Fees.Any(f => f.CourseEnrollmentID == e.EnrollmentID && !f.IsPaid)), "/Child/Participation/{0}?tab=ManageRegistrations"),
                UnpaidGroup = await GetItems(roots.Where(e => e.Status == "Confirmed" && e.Course.CourseType == "Group" && _db.Fees.Any(f => f.CourseEnrollmentID == e.EnrollmentID && !f.IsPaid)), "/Child/Participation/{0}?tab=ManageRegistrations", true),
                LeaveRequests = await GetItems(_db.CourseEnrollments.AsNoTracking().Where(e => e.EnrollmentID_Ref != null && e.ChildID.HasValue && e.Status == "RequestToLeave"), "/Child/ManageSessionRegistrations?childId={0}&courseId={1}")
            };

            return View(model);
        }

        private async Task<List<StaffNotificationItem>> GetItems(IQueryable<Core.Models.CourseEnrollment> query, string linkFormat, bool useFirstChildSessionDate = false)
        {
            var rows = await query
                .Include(e => e.Child)
                .Include(e => e.Course)
                .OrderBy(e => e.Child!.Name)
                .ThenBy(e => e.Course.Title)
                .ThenBy(e => e.ScheduledAt)
                .ToListAsync();
            var firstSessionDates = useFirstChildSessionDate
                ? await _db.CourseEnrollments
                    .Where(e => e.EnrollmentID_Ref.HasValue && rows.Select(root => root.EnrollmentID).Contains(e.EnrollmentID_Ref.Value) && e.ScheduledAt.HasValue)
                    .GroupBy(e => e.EnrollmentID_Ref!.Value)
                    .Select(group => new { EnrollmentId = group.Key, FirstDate = group.Min(e => e.ScheduledAt) })
                    .ToDictionaryAsync(item => item.EnrollmentId, item => item.FirstDate)
                : new Dictionary<int, DateTime?>();

            return rows.Select(e => new StaffNotificationItem
            {
                EnrollmentId = e.EnrollmentID,
                ChildId = e.ChildID!.Value,
                CourseId = e.CourseID,
                ParticipantName = e.Child!.Name,
                CourseTitle = e.Course.Title,
                Status = e.Status,
                ScheduledAt = firstSessionDates.TryGetValue(e.EnrollmentID, out var firstDate) ? firstDate : e.ScheduledAt,
                Link = string.Format(linkFormat, e.ChildID.Value, e.CourseID)
            }).ToList();
        }

        [Authorize(Roles = "Coach")]
        [HttpGet("Coach")]
        public IActionResult Coach()
        {
            return View();
        }


        [Authorize(Roles = "Child")]
        [HttpGet("Child")]
        public IActionResult Child()
        {
            return View();
        }

    }
}
