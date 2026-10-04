
using Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Core.Interfaces;
using Core.Models;
using Core.Contexts;
using Core.ViewModels;

using System.Diagnostics;
using Core.Repositories;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;



namespace Web.Controllers.User
{
    [Route("Staff")]
    //[ApiController]
    public class StaffController : Controller
    {
        private readonly IStaffService _staffService;
        private readonly ICourseEnrollmentService _courseEnrollmentService;
        private readonly UserManager<Core.Models.User> _userManager;
        private readonly AppDbContext _db;


        public StaffController(IStaffService staffService, ICourseEnrollmentService courseEnrollmentService, UserManager<Core.Models.User> userManager, AppDbContext db)
        {
            _staffService = staffService;
            _courseEnrollmentService = courseEnrollmentService;
            _userManager = userManager;
            _db = db;
        }

        [Authorize(Roles = "Staff")]
        [HttpGet("Notifications")]
        public async Task<IActionResult> Notifications()
        {
            var roots = _db.CourseEnrollments
                .AsNoTracking()
                .Where(e => e.EnrollmentID_Ref == null && e.ChildID.HasValue);

            var model = new StaffNotificationsViewModel
            {
                UnconfirmedPrivate = await GetNotificationItems(roots.Where(e => e.Status == "Registered" && e.Course.CourseType == "Private" && !_db.Fees.Any(f => f.CourseEnrollmentID == e.EnrollmentID && f.IsPaid)), "/Child/Participation/{0}?tab=ManageRegistrations"),
                UnconfirmedGroup = await GetNotificationItems(roots.Where(e => e.Status == "Registered" && e.Course.CourseType == "Group" && !_db.Fees.Any(f => f.CourseEnrollmentID == e.EnrollmentID && f.IsPaid)), "/Child/Participation/{0}?tab=ManageRegistrations"),
                PaidUnconfirmedPrivate = await GetNotificationItems(roots.Where(e => e.Status == "Registered" && e.Course.CourseType == "Private" && _db.Fees.Any(f => f.CourseEnrollmentID == e.EnrollmentID && f.IsPaid)), "/Child/Participation/{0}?tab=ManageRegistrations"),
                PaidUnconfirmedGroup = await GetNotificationItems(roots.Where(e => e.Status == "Registered" && e.Course.CourseType == "Group" && _db.Fees.Any(f => f.CourseEnrollmentID == e.EnrollmentID && f.IsPaid)), "/Child/Participation/{0}?tab=ManageRegistrations"),
                UnpaidPrivate = await GetNotificationItems(roots.Where(e => e.Status == "Confirmed" && e.Course.CourseType == "Private" && _db.Fees.Any(f => f.CourseEnrollmentID == e.EnrollmentID && !f.IsPaid)), "/Child/Participation/{0}?tab=ManageRegistrations"),
                UnpaidGroup = await GetNotificationItems(roots.Where(e => e.Status == "Confirmed" && e.Course.CourseType == "Group" && _db.Fees.Any(f => f.CourseEnrollmentID == e.EnrollmentID && !f.IsPaid)), "/Child/Participation/{0}?tab=ManageRegistrations", true),
                LeaveRequests = await GetNotificationItems(_db.CourseEnrollments.AsNoTracking().Where(e => e.EnrollmentID_Ref != null && e.ChildID.HasValue && e.Status == "RequestToLeave"), "/Child/ManageSessionRegistrations?childId={0}&courseId={1}")
            };

            return View(model);
        }

        [Authorize(Roles = "Staff")]
        [HttpGet("GroupCourseAttendance")]
        public async Task<IActionResult> GroupCourseAttendance()
        {
            var activeGroupCourses = await _db.Courses
                .AsNoTracking()
                .Where(c => c.IsActive && c.CourseType == "Group")
                .OrderBy(c => c.Title)
                .ToListAsync();

            var attendance = new List<SessionAttendanceViewModel>();
            var now = DateTime.UtcNow;

            foreach (var course in activeGroupCourses)
            {
                var courseAttendance = await _courseEnrollmentService.GetAttendanceAsync(course.CourseID);
                courseAttendance.Sessions = courseAttendance.Sessions
                    .OrderBy(session => session.ScheduledAt)
                    .ToList();

                if (courseAttendance.Sessions.Any(session => session.ScheduledAt > now))
                {
                    attendance.Add(courseAttendance);
                }
            }

            return View(attendance);
        }

        private async Task<List<StaffNotificationItem>> GetNotificationItems(IQueryable<Core.Models.CourseEnrollment> query, string linkFormat, bool useFirstChildSessionDate = false)
        {
            var rows = await query.Include(e => e.Child).Include(e => e.Course)
                .OrderBy(e => e.Child!.Name).ThenBy(e => e.Course.Title).ThenBy(e => e.ScheduledAt).ToListAsync();
            var firstSessionDates = useFirstChildSessionDate
                ? await _db.CourseEnrollments.Where(e => e.EnrollmentID_Ref.HasValue && rows.Select(root => root.EnrollmentID).Contains(e.EnrollmentID_Ref.Value) && e.ScheduledAt.HasValue)
                    .GroupBy(e => e.EnrollmentID_Ref!.Value).Select(group => new { EnrollmentId = group.Key, FirstDate = group.Min(e => e.ScheduledAt) })
                    .ToDictionaryAsync(item => item.EnrollmentId, item => item.FirstDate)
                : new Dictionary<int, DateTime?>();

            return rows.Select(e => new StaffNotificationItem
            {
                EnrollmentId = e.EnrollmentID, ChildId = e.ChildID!.Value, CourseId = e.CourseID,
                ParticipantName = e.Child!.Name, CourseTitle = e.Course.Title, Status = e.Status,
                ScheduledAt = firstSessionDates.TryGetValue(e.EnrollmentID, out var firstDate) ? firstDate : e.ScheduledAt,
                Link = string.Format(linkFormat, e.ChildID.Value, e.CourseID)
            }).ToList();
        }


        [Authorize(Roles = "Admin")]
        // POST: Add Staff Action
        [HttpPost("Add")]
        //[HttpPost]
        public async Task<IActionResult> Add(string name, string email, string password, string? phone, string? wechat)
        {

            if (!ModelState.IsValid)
            {
                return View();
            }

            
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var result = await _staffService.AddAsync( name, email, password,  phone,  wechat, user);
                if (!result)
                {
                    ModelState.AddModelError(string.Empty, "Failed in adding the staff info.");
                    return View();
                }

                TempData["SuccessMessage"] = "Staff info has been added successfully.";
                return RedirectToAction("List"); // Redirect to the staff list page


            }
            catch (Exception ex)
            {
                ModelState.AddModelError(String.Empty, $"{ex.Message}");
                return View();

            }
           

        }

        [Authorize(Roles = "Admin")]
        // GET: Add View
        [HttpGet("Add")]
        //[HttpGet]
        public async Task<IActionResult> Add()
        {
            return View();

        }

        [Authorize(Roles = "Admin")]
        // GET: Staff/Delete/{userId}
        [HttpGet("ConfirmDelete/{staffId}")]
        public async Task<IActionResult> ConfirmDelete(int staffId)
        {
            // Fetch the staff details from the database
            var staff = await _staffService.GetAsync(staffId);
            if (staff == null)
            {
                return NotFound();
            }

            // Pass the staff details to the Delete.cshtml view
            return View(staff);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("DeleteConfirmed")]
        public async Task<IActionResult> DeleteConfirmed(int staffId)
        {
            try
            {
                var result = await _staffService.RemoveAsync(staffId);

                if (!result)
                {
                    TempData["ErrorMessage"] = "The staff member could not be deleted.";
                    return RedirectToAction("List");
                }

                TempData["SuccessMessage"] = "Staff member has been deleted successfully.";
                return RedirectToAction("List"); // Redirect to the staff list page
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"{ex.Message}";
                return RedirectToAction("List"); // Redirect to the staff list page
            }
        }



        [Authorize(Roles = "Admin")]
        // GET: Add View
        [HttpGet("List")]
        //[HttpGet]
        public async Task<IActionResult> List()
        {

            var staffList = await _staffService.GetAllAsync();
            return View(staffList); // Ensure there is a corresponding List.cshtml in Views/Staff

        }

        [Authorize(Roles = "Admin")]
        // GET: Edit View
        [HttpGet("Edit/{staffId}")]
        //[HttpGet]
        public async Task<IActionResult> Edit(int staffId)
        {
            // Fetch the staff details from the database
            var staff = await _staffService.GetAsync(staffId);
            if (staff == null)
            {
                return NotFound();
            }

            // Pass the staff details to the Delete.cshtml view
            return View(staff);

        }


        [Authorize(Roles = "Admin")]
        [HttpPost("Edit/{staffId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int staffId, string name, string email, /*string password,*/ string? phone, string? wechat)
        {
            

            try
            {
                var user = await _userManager.GetUserAsync(User);
                var result = await _staffService.UpdateAsync(staffId, name, email, /*password,*/ phone, wechat, user);


                if (!result)
                {
                    ModelState.AddModelError(string.Empty, "Failed to update staff information.");
                    var staff = await _staffService.GetAsync(staffId);
                    return View(staff);
                }

                TempData["SuccessMessage"] = "Staff information updated successfully.";
                return RedirectToAction("List");
            }
            catch (Exception ex)
            {
                //TempData["ErrorMessage"] = $"Error: {ex.Message}";
                var staff = await _staffService.GetAsync(staffId);
                return View(staff);
            }

           
        }
       
    }
}
