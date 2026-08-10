using Core.Interfaces;
using Core.Models;
using Core.Services;
using Core.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data.SqlClient;
using X.PagedList;
using X.PagedList.Extensions;



namespace Web.Controllers.Activity
{
    [Route("Activity")]
    public class ActivityController : Controller
    {

        //[ApiController]

        private readonly IActivityService _activityService;
        private readonly IActivityEnrollmentService _activityEnrollmentService;
        private readonly UserManager<Core.Models.User> _userManager;
        private readonly ITimeZoneService _timeZoneService;



        public ActivityController(IActivityService activityService, IActivityEnrollmentService activityEnrollmentService, UserManager<Core.Models.User> userManager, ITimeZoneService timeZoneService)
        {
            _activityService = activityService;
            _activityEnrollmentService = activityEnrollmentService;
            _userManager = userManager;
            _timeZoneService = timeZoneService;
        }


        [Authorize(Roles = "Staff")]
        [HttpGet("ConfirmDelete/{activityId}")]
        public async Task<IActionResult> ConfirmDelete(int activityId)
        {
            // Fetch the staff details from the database
            var activity = await _activityService.GetAsync(activityId);
            if (activity == null)
            {
                return NotFound();
            }

            // Pass the staff details to the Delete.cshtml view
            return View(activity);
        }

        [Authorize(Roles = "Staff")]
        [HttpPost("DeleteConfirmed")]
        public async Task<IActionResult> DeleteConfirmed(int activityId)
        {
            try
            {
                var result = await _activityService.RemoveAsync(activityId);

                if (!result)
                {
                    TempData["ErrorMessage"] = "The activity could not be deleted.";
                    return RedirectToAction("List");
                }

                TempData["SuccessMessage"] = "The activity has been deleted successfully.";
                return RedirectToAction("List"); // Redirect to the course list page
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"{ex.Message}";
                return RedirectToAction("List"); // Redirect to the course list page
            }

            // If delete fails, reload the confirmation page


        }



        // GET: Add View
        [Authorize(Roles = "Admin, Staff")]
        [HttpGet("List")]
        //[HttpGet]
        public async Task<IActionResult> List(string sortOrder, int? page)
        {

            ViewData["NameSortParm"] = sortOrder == "name" ? "name_desc" : "name";
            ViewData["AddressSortParm"] = sortOrder == "address" ? "address_desc" : "address";
            ViewData["ScheduledAtSortParm"] = sortOrder == "scheduledAt" ? "scheduledAt_desc" : "scheduledAt";
            ViewData["StatusSortParm"] = sortOrder == "status" ? "status_desc" : "status";
            ViewData["RegistrationSortParm"] = sortOrder == "registration" ? "registration_desc" : "registration";

            ViewData["CurrentSort"] = sortOrder;

            var activityList = await _activityService.GetAllAsync();

            activityList = sortOrder switch
            {
                "name" => activityList.OrderBy(c => c.Title),
                "name_desc" => activityList.OrderByDescending(c => c.Title),
                "address" => activityList.OrderBy(c => c.Address),
                "address_desc" => activityList.OrderByDescending(c => c.Address),
                "scheduledAt" => activityList.OrderBy(c => c.ScheduledAt),
                "scheduledAt_desc" => activityList.OrderByDescending(c => c.ScheduledAt),
                "status" => activityList.OrderBy(c => c.Status),
                "status_desc" => activityList.OrderByDescending(c => c.Status),
                "registration" => activityList.OrderBy(c => c.RegisteredChildrenCount),
                "registration_desc" => activityList.OrderByDescending(c => c.RegisteredChildrenCount),
                _ => activityList.OrderBy(c => c.ScheduledAt) // default
            };

            //return View(activityList); // Ensure there is a corresponding List.cshtml in Views/Staff
            int pageSize = 10;
            int pageNumber = page ?? 1;

            
            return View(activityList.ToPagedList(pageNumber, pageSize));
        }

        //[HttpGet("GetCoachesBySpecialty")]
        //public async Task<IActionResult> GetCoachesBySpecialty(int specialtyId)
        //{
        //    var coaches = await _coachService.GetCoachesBySpecailtyAsync(specialtyId);
        //    return Json(coaches.Select(c => new { c.UserID, c.Name }));
        //}

        //[HttpGet("Add")]
        //public async Task<IActionResult> Add()
        //{
        //    var specialties = await _activityService.GetAllAsync();
        //    ViewBag.SpecialtyList = specialties.Select(s => new SelectListItem
        //    {
        //        Value = s.SpecialtyID.ToString(),
        //        Text = s.Title
        //    }).ToList();

        //    return View();
        //}

        [Authorize(Roles = "Staff")]
        [HttpGet("Add")]
        public async Task<IActionResult> Add()
        {
            var user = await _userManager.GetUserAsync(User);
            SetTimeZoneOptions(user?.TimeZoneId);
            return View();
        }

        [Authorize(Roles = "Staff")]
        [HttpPost("Add")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(string title, string description, string address, int maxCapacity, DateTime scheduledAt, string scheduledTimeZoneId, /*Decimal cost,*/ /*bool isActive,*/ string status)
        {
            //createdBy = 1; //temparary set

            if (!ModelState.IsValid)
            {
                SetTimeZoneOptions(scheduledTimeZoneId);
                return View();
            }

            try
            {
                var user = await _userManager.GetUserAsync(User);
                var timing = CreateTiming(scheduledAt, scheduledTimeZoneId);
                if (timing.ScheduledAtUtc <= DateTime.UtcNow)
                    throw new ArgumentException("Scheduled time must be in the future.");
                var result = await _activityService.AddAsync( title,  description,  address,  maxCapacity, timing,  /*cost,*/  status, user!);

                if (!result)
                {
                    ModelState.AddModelError(string.Empty, "Failed in adding the activity info.");
                   
                    return View();
                }

                TempData["SuccessMessage"] = "Activity info has been added successfully.";
                return RedirectToAction("List"); // Redirect to the course list page
            }
            catch (Exception ex)
            {

                ModelState.AddModelError(string.Empty, $"{ex.Message}");
                SetTimeZoneOptions(scheduledTimeZoneId);
                return View();
            }


        }






        // GET: Edit View
        [Authorize(Roles = "Staff")]
        [HttpGet("Edit/{activityId}")]
        //[HttpGet]
        public async Task<IActionResult> Edit(int activityId)
        {
            // Fetch the staff details from the database
            var activity = await _activityService.GetAsync(activityId);
            if (activity == null)
            {
                return NotFound();
            }

            // Pass the staff details to the Delete.cshtml view
            var user = await _userManager.GetUserAsync(User);
            var zoneId = activity.ScheduledTimeZoneId ?? user?.TimeZoneId ?? TimeZoneService.DefaultTimeZoneId;
            SetTimeZoneOptions(zoneId);
            ViewBag.ScheduledLocalTime = activity.ScheduledLocalTime
                ?? _timeZoneService.ConvertUtcToLocal(activity.ScheduledAt, zoneId);
            return View(activity);

        }

        [Authorize(Roles = "Staff")]
        [HttpPost("Edit/{activityId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int activityId, string title, string description, string address, int maxCapacity, DateTime scheduledAt, string scheduledTimeZoneId, /*decimal cost,*/ /*bool isActive,*/ string status)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var timing = CreateTiming(scheduledAt, scheduledTimeZoneId);
                var result = await _activityService.UpdateAsync(activityId,  title,  description,  address,  maxCapacity, timing, /* cost,*/ /*isActive, */status, user!);

                if (!result)
                {
                    ModelState.AddModelError(string.Empty, "Failed to update activity information.");
                    var activity = await _activityService.GetAsync(activityId);
                    SetTimeZoneOptions(scheduledTimeZoneId);
                    ViewBag.ScheduledLocalTime = scheduledAt;
                    return View(activity);
                }

                if(status == "Canceled")
                {
                    await _activityEnrollmentService.UpdateActivityStatusToCanceledAsync(activityId);
                }

                TempData["SuccessMessage"] = "Activity information updated successfully.";
                return RedirectToAction("List");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var activity = await _activityService.GetAsync(activityId);
                SetTimeZoneOptions(scheduledTimeZoneId);
                ViewBag.ScheduledLocalTime = scheduledAt;
                return View(activity);
            }
        }

        private ScheduleTiming CreateTiming(DateTime localTime, string timeZoneId)
        {
            if (!_timeZoneService.IsValidTimeZone(timeZoneId))
                throw new ArgumentException("Please select a valid event time zone.");

            var unspecified = DateTime.SpecifyKind(localTime, DateTimeKind.Unspecified);
            return new ScheduleTiming
            {
                ScheduledAtUtc = _timeZoneService.ConvertLocalToUtc(unspecified, timeZoneId),
                ScheduledLocalTime = unspecified,
                TimeZoneId = timeZoneId
            };
        }

        private void SetTimeZoneOptions(string? selectedTimeZoneId)
        {
            ViewBag.TimeZones = _timeZoneService.GetTimeZones();
            ViewBag.SelectedTimeZoneId = selectedTimeZoneId ?? TimeZoneService.DefaultTimeZoneId;
        }
    }
}
