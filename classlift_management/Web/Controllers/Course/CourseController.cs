using Core.Interfaces;
using Core.Models;
using Core.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using Core.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using System.Numerics;
using Microsoft.EntityFrameworkCore;
using Core.Services;
using X.PagedList;

//using X.PagedList.Mvc.Core;


using System.Data.SqlClient;
using X.PagedList.Extensions;

namespace Web.Controllers.Courses
{
    [Route("Course")]
    public class CourseController : Controller
    {

        //[ApiController]

        private readonly ICourseService _courseService;
        private readonly ICoachService _coachService;
        private readonly ISpecialtyService _specialtyService;
        private readonly ICourseEnrollmentService _courseEnrollmentService;
        private readonly UserManager<Core.Models.User> _userManager;


        public CourseController(ICourseService courseService, ICourseEnrollmentService courseEnrollmentService, ICoachService coachService, ISpecialtyService specialtyService, UserManager<Core.Models.User> userManager)
        {
            _courseService = courseService;
            _coachService = coachService;
            _specialtyService = specialtyService;
            _userManager = userManager;
            _courseEnrollmentService = courseEnrollmentService;
        }



        [Authorize(Roles = "Staff")]
        [HttpGet("ConfirmDelete/{courseId}")]
        public async Task<IActionResult> ConfirmDelete(int courseId)
        {
            // Fetch the staff details from the database

           
            var course = await _courseService.GetAsync(courseId);
            if(course == null)
            {
                return NotFound();
            }

           
                

                // Pass the staff details to the Delete.cshtml view
            return View(course);
        }

        [Authorize(Roles = "Staff")]
        [HttpPost("DeleteConfirmed")]
        public async Task<IActionResult> DeleteConfirmed(int courseId)
        {
            try
            {
                var enrollments = await _courseEnrollmentService.GetRegisteredEnrollmentsByCourseAsync(courseId);

                if (enrollments != null && enrollments.Any())
                {
                    TempData["ErrorMessage"] = "This course cannot be deleted because it has enrolled students. Please try editing the course and set it to inactive.";
                    return RedirectToAction("List"); // Redirect to the course list page
                }


                var result = await _courseService.RemoveAsync(courseId);

                if (!result)
                {
                    TempData["ErrorMessage"] = "The course could not be deleted.";
                    return RedirectToAction("List");
                }

                TempData["SuccessMessage"] = "The course has been deleted successfully.";
                return RedirectToAction("List"); // Redirect to the course list page
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"{ex.Message}";
                return RedirectToAction("List"); // Redirect to the course list page
            }

            // If delete fails, reload the confirmation page


        }


        [Authorize(Roles = "Staff")]
        [HttpGet("ConfirmDeleteSession/{enrollmentId}")]
        public async Task<IActionResult> ConfirmDeleteSession(int enrollmentId)
        {

            var session = await _courseEnrollmentService.GetAsync(enrollmentId);
            if (session == null)
            {
                return NotFound();
            }

            return View(session);
        }


       


        


       







       

        // GET: Add View
        [Authorize(Roles = "Admin, Staff")]
        [HttpGet("List")]

        public async Task<IActionResult> List(string sortOrder, int? page)
        {

            ViewData["TitleSortParm"] = sortOrder == "title" ? "title_desc" : "title";
            ViewData["CoachSortParm"] = sortOrder == "coach" ? "coach_desc" : "coach";
            ViewData["TypeSortParm"] = sortOrder == "type" ? "type_desc" : "type";
            ViewData["SpecialtySortParm"] = sortOrder == "specialty" ? "specialty_desc" : "specialty";
            ViewData["CurrentSort"] = sortOrder;
            var courses = await _courseService.GetAllAsync();

            courses = sortOrder switch
            {
                "specialty" => courses.OrderBy(c => c.SpecialtyName),
                "specialty_desc" => courses.OrderByDescending(c => c.SpecialtyName),
                "title" => courses.OrderBy(c => c.Title),
                "title_desc" => courses.OrderByDescending(c => c.Title),
                "coach" => courses.OrderBy(c => c.CoachName),
                "coach_desc" => courses.OrderByDescending(c => c.CoachName),
                "type" => courses.OrderBy(c => c.CourseType),
                "type_desc" => courses.OrderByDescending(c => c.CourseType),
                _ => courses.OrderBy(c => c.CourseType) // default
            };

            //return View(courses);

            // Paging logic
            int pageSize = 10;
            int pageNumber = page ?? 1;

            // Replace the problematic line with the following:
            //if (courses == null || !courses.Any())
            //{
            //    return View(new List<CourseViewModel>().ToList().ToPagedList(pageNumber, pageSize));
            //}
            //else
            //{ 
                return View(courses.ToPagedList(pageNumber, pageSize));
            //}
            //Install - Package X.PagedList
        }


        [HttpGet("GetCoachesBySpecialty")]
        public async Task<IActionResult> GetCoachesBySpecialty(int specialtyId)
        {
            var coaches = await _coachService.GetCoachesBySpecailtyAsync(specialtyId);
            return Json(coaches.Select(c => new { c.CoachID, c.Name }));
        }

        [Authorize(Roles = "Staff")]
        [HttpGet("Add")]
        public async Task<IActionResult> Add()
        {
            var specialties = await _specialtyService.GetAllAsync();
            ViewBag.SpecialtyList = specialties.Select(s => new SelectListItem
            {
                Value = s.SpecialtyID.ToString(),
                Text = s.Title
            }).ToList();

            return View();
        }

        [Authorize(Roles = "Staff")]
        [HttpPost("Add")]
        public async Task<IActionResult> Add(AddCourseViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var specialties = await _specialtyService.GetAllAsync();
                ViewBag.SpecialtyList = specialties.Select(s => new SelectListItem
                {
                    Value = s.SpecialtyID.ToString(),
                    Text = s.Title
                }).ToList();
                return View();
            }

            try
            {
                var user = await _userManager.GetUserAsync(User);

                var result = await _courseService.AddAsync(model.Title, model.Description,model.CourseType, model.MaxCapacity, model.SessionCount, model.HourlyCost, model.HourlyCost2, model.IsActive, model.CoachID, model.SpecialtyID, user);

                if (!result)
                {
                    ModelState.AddModelError(string.Empty, "Failed in adding the course info.");
                    var specialties = await _specialtyService.GetAllAsync();
                    ViewBag.SpecialtyList = specialties.Select(s => new SelectListItem
                    {
                        Value = s.SpecialtyID.ToString(),
                        Text = s.Title
                    }).ToList();
                    return View();
                }

                TempData["SuccessMessage"] = "Course info has been added successfully.";
                return RedirectToAction("List"); // Redirect to the course list page
            }
            catch (Exception ex)
            {
               
                ModelState.AddModelError(string.Empty, $"{ex.Message}");
                var specialties = await _specialtyService.GetAllAsync();
                ViewBag.SpecialtyList = specialties.Select(s => new SelectListItem
                {
                    Value = s.SpecialtyID.ToString(),
                    Text = s.Title
                }).ToList();
                return View(); 
            }

                
        }






        // GET: Edit View
        [Authorize(Roles = "Staff")]
        [HttpGet("Edit/{courseId}")]
        //[HttpGet]
        public async Task<IActionResult> Edit(int courseId)
        {
            // Fetch the staff details from the database
            var course = await _courseService.GetAsync(courseId);
            if (course == null)
            {
                return NotFound();
            }

            // Pass the staff details to the Delete.cshtml view
            return View(course);

        }

        [Authorize(Roles = "Staff")]
        [HttpPost("Edit/{courseId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int courseId, string title, string description, string courseType, int? maxCapacity, int? sessionCount, decimal hourlyCost, decimal? hourlyCost2, bool isActive/*, int userId, int updatedBy*/)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                var enrollments = await _courseEnrollmentService.GetScheduledEnrollmentsByCourseAsync(courseId);
                if (enrollments != null && enrollments.Any() && isActive == false)
                {
                    TempData["ErrorMessage"] = "This course cannot be set to inactive because it has scheduled sessions.  Please wait until all scheduled sessions completed or deleted all scheduled sessions before deactivating the course.";
                    return RedirectToAction("List");
                }

               

                var result = await _courseService.UpdateAsync(courseId, title, description, courseType, maxCapacity, sessionCount, hourlyCost, hourlyCost2, isActive, user);

                if (!result)
                {
                    ModelState.AddModelError(string.Empty, "Failed to update course information.");
                    var course = await _courseService.GetAsync(courseId);
                    return View(course);
                }

                TempData["SuccessMessage"] = "Course information updated successfully.";
                return RedirectToAction("List");
            }
            catch (Exception ex)
            {
                //TempData["ErrorMessage"] = $"{ex.Message}";
                ModelState.AddModelError(string.Empty, $"{ex.Message}");
                var course = await _courseService.GetAsync(courseId);
                return View(course);
            }
        }

        [Authorize(Roles = "Staff")]
        [HttpGet("ManageSessions/{courseId}")]
        public async Task<IActionResult> ManageSessions(int courseId)
        {
          

            Course course = await _courseService.GetAsync(courseId);
            if (course == null || course.CourseType != "Group")
            {
                TempData["ErrorMessage"] = "Invalid course.";
                return RedirectToAction("List");
            }

            var openSessions = await _courseEnrollmentService.GetOpenSessionsByCourseAsync(courseId);
            var closedSessions = await _courseEnrollmentService.GetClosedSessionsByCourseAsync(courseId);
            var canceledSessions = await _courseEnrollmentService.GetCanceledSessionsByCourseAsync(courseId);
            var finishedSessions = await _courseEnrollmentService.GetAllPastSessionsByCourseAsync(courseId);

            var allUpcomingSessions = await _courseEnrollmentService.GetAllUpcomingSessionsByCourseAsync(courseId);
            var allRegisteredUpcomingSessionIds = await _courseEnrollmentService.GetRegisteredUpcomingSessionsByCourseAsync(courseId);

            ViewBag.CourseID = courseId;

           

            var model = new ManageSessionsViewModel
            {
                Course = course,
                OpenSessions = (List<CourseEnrollment>?)openSessions,
                FinishedSessions = (List<CourseEnrollment>?)finishedSessions,
                //CanceledSessions = (List<CourseEnrollment>?)canceledSessions,
                ClosedSessions = (List<CourseEnrollment>?)closedSessions,
                AllUpcomingSessions = (List<CourseEnrollment>?)allUpcomingSessions,
                RegisteredUpcomingSessionIds = allRegisteredUpcomingSessionIds

            };

            return View(model);
        }



        [HttpPost]
        public async Task<IActionResult> AddSession(int courseId, DateTime scheduledAt, decimal scheduledHours, string location, string staffNote, string repeatType, int repeatCount)
        {



            var user = await _userManager.GetUserAsync(User);

            Course course = await _courseService.GetAsync(courseId);
            if (course == null || course.CourseType != "Group")
            {
                TempData["ErrorMessage"] = "Invalid course.";
                return RedirectToAction("ManageSessions", new { courseId });
            
            }
       

            try
            {
                var result = false;
                if (repeatType == "None" || repeatCount <= 1)
                {
                    result = await _courseEnrollmentService.AddSessionToGroupCourseAsync(courseId, scheduledAt, scheduledHours, location, staffNote, user);
                }
                else
                {
                    DateTime currentDate = scheduledAt;

                    for (int i = 0; i < repeatCount; i++)
                    {
                        result = await _courseEnrollmentService.AddSessionToGroupCourseAsync(
                            courseId, currentDate, scheduledHours, location, staffNote, user);

                        if (repeatType == "Daily")
                        {
                            currentDate = currentDate.AddDays(1);
                        }
                        else if (repeatType == "Weekly")
                        {
                            currentDate = currentDate.AddDays(7);
                        }
                    }
                }
                if (result)
                {
                    TempData["SuccessMessage"] = "Session(s) added successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to add one or more sessions.";
                }
                
                return RedirectToAction("ManageSessions", new { courseId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"{ex.Message}";
                return RedirectToAction("ManageSessions", new { courseId });
            }

           
        }



        


        [HttpGet("EditSession/{enrollmentId}")]
        public async Task<IActionResult> EditSession(int enrollmentId)
        {

            var session = await _courseEnrollmentService.GetAsync(enrollmentId);
            if (session == null) return NotFound();

            return PartialView("_EditSession", session);
        }



        // ✅ Save Session (Add / Edit)
        [HttpPost("SaveSession")]
        public async Task<IActionResult> SaveSession(int enrollmentId, string location, string? staffNote, string status)
        {

            if (!ModelState.IsValid)
                return BadRequest("Invalid data");
            //city.CreatedBy = 1;  //temparaly set it to 1
            var user = await _userManager.GetUserAsync(User);

            CourseEnrollment session = await _courseEnrollmentService.GetAsync(enrollmentId);
            try
            {
                var result = false;
                session.Location = location;
                session.StaffNote = staffNote;
                session.Status = status;

                //This also include if update the session Status to 'Canceled', all children's registration to the session need to be canceled. 
                result = await _courseEnrollmentService.UpdateSessionAsync(session);


                if (result)
                {
                    if (status == "Canceled")
                    { 
                        await _courseEnrollmentService.UpdateChildCanceledSessionsAsync(session.EnrollmentID, staffNote);

                        //Send email to all enrolled children

                    }

                    return Json(new { success = true });

                }
                else
                {
                    return Json(new { success = false });
                }

            }

            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }); // ✅ Return error message
            }


        }

        // ✅ Load Partial View for Add/Edit Form
        [HttpGet("DeleteSessionConfirm/{enrollmentId}")]
        public async Task<IActionResult> DeleteSessionConfirm(int enrollmentId)
        {
            //if (cityId == 0) return PartialView("_DeleteConfirm", new City { Name = string.Empty });

            //var city = await _context.Cities.FindAsync(cityId);
            var session = await _courseEnrollmentService.GetAsync(enrollmentId);
            if (session == null) return NotFound();

            return PartialView("_DeleteSessionConfirm", session);
        }

        // ✅ Delete a course session
        [Authorize(Roles = "Staff")]
        [HttpPost("DeleteSessionConfirmed/{enrollmentId}")]
        public async Task<IActionResult> DeleteSessionConfirmed(int enrollmentId)
        {

            try
            {
                var result = await _courseEnrollmentService.RemoveAsync(enrollmentId);

                if (result)
                    return Json(new { success = true });
                else
                    return Json(new { success = false });
            }

            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }); // ✅ Return error message
            }

        }

        
        [Authorize(Roles = "Staff, Coach")]
        [HttpGet("Attendance/{courseId}")]
        public async Task<IActionResult> Attendance(int courseId)
        {
            var vm = await _courseEnrollmentService.GetAttendanceAsync(courseId);
            return View(vm);
        }

       



        //[Authorize(Roles = "Staff")]
        //[HttpPost("DeleteSessionConfirmed")]
        //public async Task<IActionResult> DeleteSessionConfirmed(int enrollmentId)
        //{
        //    try
        //    {


        //        var result = await _courseEnrollmentService.RemoveAsync(enrollmentId);

        //        if (!result)
        //        {
        //            TempData["ErrorMessage"] = "The session could not be deleted.";
        //            return RedirectToAction("List");
        //        }

        //        TempData["SuccessMessage"] = "The session has been deleted successfully.";
        //        return RedirectToAction("List"); // Redirect to the course list page
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["ErrorMessage"] = $"{ex.Message}";
        //        return RedirectToAction("List"); // Redirect to the course list page
        //    }

        //    // If delete fails, reload the confirmation page


        //}

    }   
}

