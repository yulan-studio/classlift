
using Core;
using Core.Interfaces;
using Core.Models;
using Core.Repositories;
using Core.Services;
using Core.ViewModels;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using X.PagedList;
using X.PagedList.Extensions;



namespace Web.Controllers.User
{
    [Route("Coach")]
    //[ApiController]
    public class CoachController : Controller
    {
        private readonly ICoachService _coachService;
        private readonly ICoachIncomeService _incomeService;
        private readonly IChildBalanceService _balanceService;
        private readonly ICoachRepository _coachRepository;
        private readonly ICityService _cityService;
        private readonly ISpecialtyService _specialtyService;
        private readonly IEmergencyContactService _emergencyContactService;
        private readonly ICoachSpecialtyService _coachSpecialtyService;
        private readonly ICourseEnrollmentService _courseEnrollmentService;
        private readonly ICourseService _courseService;
        private readonly IChildService _childService;
        private readonly IParentChildService _parentChildService;
        private readonly IFeeService _feeService;

        private readonly EmailService _emailService;
        private readonly UserManager<Core.Models.User> _userManager;
        
        public CoachController(ICoachService coachService, ICoachRepository coachRepository, ICoachIncomeService incomeService,  IEmergencyContactService emergencyService, IChildBalanceService balanceService, ICityService cityService, ISpecialtyService specialtyService, ICoachSpecialtyService coachSpecialtyService, ICourseEnrollmentService courseEnrollmentService, ICourseService courseService, IChildService childService, IParentChildService parentChildService, IFeeService feeService, EmailService emailService, UserManager<Core.Models.User> userManager)
        {
            _coachService = coachService;
            _incomeService = incomeService;
            _balanceService = balanceService;
            _coachRepository = coachRepository;
            _cityService = cityService;
            _specialtyService = specialtyService;
            _coachSpecialtyService = coachSpecialtyService;
            _emergencyContactService = emergencyService;
        _courseEnrollmentService = courseEnrollmentService;
            _courseService = courseService;
            _childService = childService;
            _parentChildService = parentChildService;
            _feeService = feeService;
            _emailService = emailService;
            _userManager = userManager;
            
        }

        [Authorize(Roles = "Staff")]
        // POST: Add Staff Action
        [HttpPost("Add")]
        //[HttpPost]
        public async Task<IActionResult> Add(string name, string email, string password, List<int> specialtyIds, string gender, string phone, string? wechat, int cityId)
        {

           
            if (!ModelState.IsValid)
            {
                var cities = await _cityService.GetAllAsync(); // Replace with your data fetching logic
                ViewBag.CityList = cities.Select(c => new SelectListItem
                {
                    Value = c.CityID.ToString(),
                    Text = c.Name
                }).ToList();

                var specialties = await _specialtyService.GetAllAsync(); // Replace with your data fetching logic
                ViewBag.SpecialtyList = specialties.Select(c => new SelectListItem
                {
                    Value = c.SpecialtyID.ToString(),
                    Text = c.Title
                }).ToList();
                return View();
            }

            try
            {
                var user = await _userManager.GetUserAsync(User);
                
                var result = await _coachService.AddAsync(name, email, password, specialtyIds, gender, phone, wechat, cityId, user);
                if (!result)
                {
                    ModelState.AddModelError(string.Empty, "Failed in adding the coach info.");

                   
                    // Repopulate CityList for the dropdown if validation fails

                    var cities = await _cityService.GetAllAsync(); // Replace with your data fetching logic
                    ViewBag.CityList = cities.Select(c => new SelectListItem
                    {
                        Value = c.CityID.ToString(),
                        Text = c.Name
                    }).ToList();

                    var specialties = await _specialtyService.GetAllAsync(); // Replace with your data fetching logic
                    ViewBag.SpecialtyList = specialties.Select(c => new SelectListItem
                    {
                        Value = c.SpecialtyID.ToString(),
                        Text = c.Title
                    }).ToList();


                    return View();
                }
                TempData["SuccessMessage"] = "Coach info has been added successfully.";
                return RedirectToAction("List"); // Redirect to the coach list page


            }
            catch (Exception ex)
            {
                ModelState.AddModelError(String.Empty, $"{ex.Message}");
                
                // Repopulate CityList for the dropdown if validation fails

                var cities = await _cityService.GetAllAsync(); // Replace with your data fetching logic
                ViewBag.CityList = cities.Select(c => new SelectListItem
                {
                    Value = c.CityID.ToString(),
                    Text = c.Name
                }).ToList();

                var specialties = await _specialtyService.GetAllAsync(); // Replace with your data fetching logic
                ViewBag.SpecialtyList = specialties.Select(c => new SelectListItem
                {
                    Value = c.SpecialtyID.ToString(),
                    Text = c.Title
                }).ToList();

                  
                return View();
            }

            


        }

        [Authorize(Roles = "Staff")]
        // GET: Add View
        [HttpGet("Add")]
        //[HttpGet]
        public async Task<IActionResult> AddAsync()
        {
            var cities = await _cityService.GetAllAsync(); // Replace with your data fetching logic
            ViewBag.CityList = cities.Select(c => new SelectListItem
            {
                Value = c.CityID.ToString(),
                Text = c.Name
            }).ToList();

            var specialties = await _specialtyService.GetAllAsync(); // Replace with your data fetching logic
            ViewBag.SpecialtyList = specialties.Select(c => new SelectListItem
            {
                Value = c.SpecialtyID.ToString(),
                Text = c.Title
            }).ToList();


            return View();

        }




        [Authorize(Roles = "Staff")]
        // GET: Coach/Delete/{userId}
        [HttpGet("ConfirmDelete/{coachId}")]
        public async Task<IActionResult> ConfirmDelete(int coachId)
        {
            // Fetch the staff details from the database
            var coach = await _coachService.GetAsync(coachId);
            if (coach == null)
            {
                return NotFound();
            }

            // Pass the staff details to the Delete.cshtml view
            return View(coach);
        }

        [Authorize(Roles = "Staff")]
        [HttpPost("DeleteConfirmed")]
        public async Task<IActionResult> DeleteConfirmed(int coachId)
        {
            try
            {
                var result = await _coachService.RemoveAsync(coachId);

                if (!result)
                {
                    TempData["ErrorMessage"] = "The coach member could not be deleted.";
                    return RedirectToAction("List");
                }

                TempData["SuccessMessage"] = "Coach member has been deleted successfully.";
                return RedirectToAction("List"); // Redirect to the coach list page
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"{ex.Message}";
                return RedirectToAction("List");
            }
        }





        // GET: Add View
        [HttpGet("List")]
        //[HttpGet]
        public async Task<IActionResult> List(string sortOrder, int? page, string searchName)
        {

            
            var coachList = await _coachService.GetAllAsync();
            List<CoachWithDeleteViewModel> coaches = new List<CoachWithDeleteViewModel>();
            foreach (Coach coach in coachList)
            {
                CoachWithDeleteViewModel coachWithDelete = new CoachWithDeleteViewModel();
                coachWithDelete.Coach = coach;
                bool canDelete = !(await _courseService.GetCoursesByCoachAsync(coach.CoachID)).Any();
                coachWithDelete.CanDelete = canDelete;
                coaches.Add(coachWithDelete);
            }
            

            if (!string.IsNullOrEmpty(searchName))
            {
                var filteredCoaches = coaches
                    .Where(c => c.Coach.Name.Contains(searchName))
                    .ToList();

                // convert to IPagedList just to match your View model
                return View(filteredCoaches.ToPagedList(1, filteredCoaches.Count == 0 ? 1 : filteredCoaches.Count));


            }

            else {
                ViewData["MemberIDParm"] = sortOrder == "id" ? "id_desc" : "id";
                ViewData["NameSortParm"] = sortOrder == "name" ? "name_desc" : "name";
                ViewData["PreferedNameSortParm"] = sortOrder == "preferedName" ? "preferedName_desc" : "preferedName";
                ViewData["GenderSortParm"] = sortOrder == "gender" ? "gender_desc" : "gender";
                ViewData["CitySortParm"] = sortOrder == "city" ? "city_desc" : "city";
                ViewData["CurrentSort"] = sortOrder;



                coachList = sortOrder switch
                {
                    "id" => coachList.OrderBy(c => c.MemberID),
                    "id_desc" => coachList.OrderByDescending(c => c.MemberID),
                    "name" => coachList.OrderBy(c => c.Name),
                    "name_desc" => coachList.OrderByDescending(c => c.Name),
                    "preferedName" => coachList.OrderBy(c => c.PreferedName),
                    "preferedName_desc" => coachList.OrderByDescending(c => c.PreferedName),
                    "gender" => coachList.OrderBy(c => c.Gender),
                    "gender_desc" => coachList.OrderByDescending(c => c.Gender),
                    "city" => coachList.OrderBy(c => c.City.Name),
                    "city_desc" => coachList.OrderByDescending(c => c.City.Name),

                    _ => coachList.OrderBy(c => c.Name) // default
                };


                //List<CoachWithDeleteViewModel> coaches = new List<CoachWithDeleteViewModel>();

                //foreach (Coach coach in coachList)
                //{
                //    CoachWithDeleteViewModel coachWithDelete = new CoachWithDeleteViewModel();
                //    coachWithDelete.Coach = coach;
                //    bool canDelete = !(await _courseService.GetCoursesByCoachAsync(coach.CoachID)).Any();
                //    coachWithDelete.CanDelete = canDelete;
                //    coaches.Add(coachWithDelete);
                //}
                //return View(coaches); // Ensure there is a corresponding List.cshtml in Views/Staff

                int pageSize = 40;
                int pageNumber = page ?? 1;


                return View(coaches.ToPagedList(pageNumber, pageSize));
            }


        }


        [Authorize(Roles = "Staff, Coach")]
        // GET: Edit View
        [HttpGet("Edit/{coachId}")]
        //[HttpGet]
        public async Task<IActionResult> Edit(int coachId)
        {
            //Fetch the staff details from the database


           var coach = await _coachService.GetAsync(coachId);

            if (coach == null)
            {
                return NotFound();
            }

            var cities = await _cityService.GetAllAsync(); // Replace with your data fetching logic



            ViewBag.CityList = cities.Select(c => new SelectListItem
            {
                Value = c.CityID.ToString(),
                Text = c.Name,
                Selected = c.CityID == coach.CityID
            }).ToList();


            var specialties = await _specialtyService.GetAllAsync(); // Replace with your data fetching logic

            //var coachSpecialtyIds = (await _coachSpecialtyService.GetSpecialtyIdsByCoachAsync(coachId)).ToHashSet(); // Get coach's specialties
            var coachSpecialtyIds = coach.CoachSpecialties?.Select(cs => cs.SpecialtyID).ToHashSet() ?? new HashSet<int>();


            ViewBag.SpecialtyList = specialties.Select(s => new SelectListItem
            {
                Value = s.SpecialtyID.ToString(),
                Text = s.Title,
                Selected = coachSpecialtyIds.Contains(s.SpecialtyID)
            }).ToList();

            //ViewBag.SpecialtyList = specialties.Select(s => new SelectListItem
            //{
            //    Value = s.SpecialtyID.ToString(),
            //    Text = s.Title,
            //    Selected = s.SpecialtyID == coach.SpecialtyID
            //}).ToList();

            // Pass the coach details to the Edit.cshtml view
            return View(coach);
            

        }

        [Authorize(Roles = "Staff, Coach")]
        [HttpPost("Edit/{coachId}")]
        [ValidateAntiForgeryToken]

       
        public async Task<IActionResult> Edit(int coachId, string name, string email, /*string password,*/List<int> specialtyIds, string gender, string phone, string wechat, int cityId)
        {
           

            try
            {
                var user = await _userManager.GetUserAsync(User);
                var result = await _coachService.UpdateAsync(coachId, name, email, /*password,*/specialtyIds, gender, phone, wechat, cityId, user);


                if (!result)
                {
                    ModelState.AddModelError(string.Empty, "Failed to update coach information.");
                    var coach = await _coachService.GetAsync(coachId);

                    if (coach == null)
                    {
                        return NotFound();
                    }

                    var cities = await _cityService.GetAllAsync(); // Replace with your data fetching logic



                    ViewBag.CityList = cities.Select(c => new SelectListItem
                    {
                        Value = c.CityID.ToString(),
                        Text = c.Name,
                        Selected = c.CityID == coach.CityID
                    }).ToList();


                    //var specialties = await _specialtyService.GetAllAsync(); // Replace with your data fetching logic

                    //ViewBag.SpecialtyList = specialties.Select(s => new SelectListItem
                    //{
                    //    Value = s.SpecialtyID.ToString(),
                    //    Text = s.Title,
                    //    Selected = s.SpecialtyID == coach.SpecialtyID
                    //}).ToList();

                    var specialties = await _specialtyService.GetAllAsync(); // Replace with your data fetching logic

                    //var coachSpecialtyIds = (await _coachSpecialtyService.GetSpecialtyIdsByCoachAsync(coachId)).ToHashSet(); // Get coach's specialties
                    var coachSpecialtyIds = coach.CoachSpecialties?.Select(cs => cs.SpecialtyID).ToHashSet() ?? new HashSet<int>();

                    ViewBag.SpecialtyList = specialties.Select(s => new SelectListItem
                    {
                        Value = s.SpecialtyID.ToString(),
                        Text = s.Title,
                        Selected = coachSpecialtyIds.Contains(s.SpecialtyID)
                    }).ToList();


                    // Pass the coach details to the Edit.cshtml view
                    return View(coach);
                }

                TempData["SuccessMessage"] = "coach information updated successfully.";
                return RedirectToAction("List");
            }
            catch (Exception ex)
            {
                //TempData["ErrorMessage"] = $"Error: {ex.Message}";
                var coach = await _coachService.GetAsync(coachId);

                if (coach == null)
                {
                    return NotFound();
                }

                var cities = await _cityService.GetAllAsync(); // Replace with your data fetching logic



                ViewBag.CityList = cities.Select(c => new SelectListItem
                {
                    Value = c.CityID.ToString(),
                    Text = c.Name,
                    Selected = c.CityID == coach.CityID
                }).ToList();


                //var specialties = await _specialtyService.GetAllAsync(); // Replace with your data fetching logic

                //ViewBag.SpecialtyList = specialties.Select(s => new SelectListItem
                //{
                //    Value = s.SpecialtyID.ToString(),
                //    Text = s.Title,
                //    Selected = s.SpecialtyID == coach.SpecialtyID
                //}).ToList();

                var specialties = await _specialtyService.GetAllAsync(); // Replace with your data fetching logic

                var coachSpecialtyIds = coach.CoachSpecialties?.Select(cs => cs.SpecialtyID).ToHashSet() ?? new HashSet<int>();

                ViewBag.SpecialtyList = specialties.Select(s => new SelectListItem
                {
                    Value = s.SpecialtyID.ToString(),
                    Text = s.Title,
                    Selected = coachSpecialtyIds.Contains(s.SpecialtyID)
                }).ToList();


                // Pass the coach details to the Edit.cshtml view
                return View(coach);
            }
        }


        [Authorize(Roles = "Staff")]
        [HttpGet("MoreInfo/{coachId}")]
        public async Task<IActionResult> MoreInfo(int coachId, string tab = "CoreInfo")
        {
            var coach = await _coachService.GetAsync(coachId);

            ViewBag.ActiveTab = tab;
           
            return View(coach);
        }


        [HttpGet("CoreInfo/{coachId}")]
        public async Task<IActionResult> CoreInfo(int coachId)
        {
            var coach = await _coachService.GetAsync(coachId);
            return View(coach);
        }



        [Authorize(Roles = "Staff")]
        [HttpPost("CoreInfo/{coachId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CoreInfo(int coachId, string? memberID, string? preferedName, string? address, /*int OAPAmount, */string? postCode, int? bank, int? transit, int? account, string status, bool photoConsent)
        {
            var coach = await _coachService.GetAsync(coachId);
            if (ModelState.IsValid)
            {

                await _coachService.UpdateAsync(coachId, memberID, preferedName, address, postCode, bank, transit, account, status, photoConsent);

                return RedirectToAction("MoreInfo", new { coachId });
            }
            else
                //return View(child);
                return RedirectToAction("MoreInfo", new { coachId });
        }



        [Authorize(Roles = "Staff")]
        [HttpPost("AddEmergencyContact/{coachId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEmergencyContact(int coachId, string contactName, string relationship, string phone, string email)
        {
            if (ModelState.IsValid)
            {
                EmergencyContact contact = new EmergencyContact
                {
                    ContactName = contactName,
                    Relationship = relationship,
                    Phone = phone,
                    Email = email
                };
                contact.CoachID = coachId;

                var result = await _emergencyContactService.AddAsync(contact);

                
                return RedirectToAction("MoreInfo", new { coachId });
               

                
            
            }

            return RedirectToAction("MoreInfo", new { coachId });
        }

        [Authorize(Roles = "Staff")]
        [HttpPost("DeleteEmergencyContact/{contactId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteEmergencyContact(int contactId, int coachId)
        {
            var contact = await _emergencyContactService.GetAsync(contactId);
            if (contact != null)
            {
                await _emergencyContactService.DeleteAsync(contactId);
            }

            return RedirectToAction("MoreInfo", new { coachId });
        }



        [Authorize(Roles = "Coach")]
        [HttpGet("ManageCourse")]
        public async Task<IActionResult> ManageCourse()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var coach = await _coachRepository.GetCoachByIdAsync(user.Id);
                int coachId = coach.CoachID;

                var model = new ManageCourseViewModel();
                model.Coach = coach;


                var specialties = await _coachSpecialtyService.GetSpecialtiesByCoachAsync(coachId);

                if (specialties == null || !specialties.Any())
                {
                    TempData["ErrorMessage"] = "No specialties found for this coach.";
                    return RedirectToAction("Index", "Home"); // Redirect to a safe page
                }

                var specialtiesCourses = new List<SpecialtyCoursesViewModel>();

                foreach (Specialty specialty in specialties)
                {
                    var specialtyCourses = new SpecialtyCoursesViewModel();
                    specialtyCourses.SpecialtyID = specialty.SpecialtyID;
                    specialtyCourses.SpecialtyTitle = specialty.Title;


                    var courses = await _courseService.GetActiveCourseByCoachBySpecialtyAsync(coachId, specialty.SpecialtyID);
                    //if (courses == null || !courses.Any())
                    //{
                    //    continue; // Skip if no courses
                    //}

                    var coursesChildren = new List<CourseChildrenViewModel>();

                    

                    foreach (Course course in courses)
                    {
                        var courseChildren = new CourseChildrenViewModel();
                        courseChildren.CourseID = course.CourseID;
                        courseChildren.CourseTitle = course.Title;
                        courseChildren.CourseDescription = course.Description;
                        courseChildren.SessionCount = course.SessionCount;
                        courseChildren.CourseType = course.CourseType;



                        var children = (List<ChildViewModel>)await _courseEnrollmentService.GetRegisterationByCourseAsync(course.CourseID);
                        courseChildren.RegisteredChildren = children;



                        coursesChildren.Add(courseChildren);

                    }

                    specialtyCourses.Courses = coursesChildren;

                    specialtiesCourses.Add(specialtyCourses);

                }

                model.Specialties = specialtiesCourses;

                return View(model);

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                return RedirectToAction("Index", "Home"); // Redirect to a safe page
            }


        }




       





        [Authorize(Roles = "Coach")]
        [HttpGet("ManageSchedules/{childId}")]
        public async Task<IActionResult> ManageSchedules(int childId, [FromQuery] int courseId, [FromQuery] int enrollmentId)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var coach = await _coachRepository.GetCoachByIdAsync(user.Id);
                int coachId = coach.CoachID;
                // ✅ Get children who are enrolled in the coach's courses
                var child = await _childService.GetAsync(childId);
                var parents = await _parentChildService.GetParentsByChildIdAsync(childId);

                // ✅ Get courses assigned to the coach
                //var course = await _courseService.GetActiveCourseByCoachAsync(coachId);
                //int courseId = 2; //need to change later
                var course = await _courseService.GetAsync(courseId);

                // ✅ Get schedules for the child and course
                List<CourseEnrollment> schedules = (List<CourseEnrollment>)await _courseEnrollmentService.GetUpcomingEnrollmentsByCourseChildAsync(course.CourseID, childId);

                List<CourseEnrollment> completed = (List<CourseEnrollment>)await _courseEnrollmentService.GetCompletesByCourseChildAsync(courseId, childId);

                List<CourseEnrollment> scheduled = (List<CourseEnrollment>)await _courseEnrollmentService.GetSchedulesByCourseChildAsync(courseId, childId);

               var model = new ManageSchedulesViewModel
                {
                    EnrollmentID = enrollmentId,
                    Child = child,
                    Parents = parents,
                    Course = course,
                    Schedules = schedules,
                    ScheduledCount = scheduled.Count,
                    CompletedCount = completed.Count
                };

                return View(model);
            }

            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"{ex.Message}";
                return RedirectToAction("ManageCourse");
            }
        }

        [Authorize(Roles = "Coach")]
        [HttpPost("ScheduleCourse")]
        public async Task<IActionResult> ScheduleCourse(int childId, int courseId, DateTime scheduledAt, decimal scheduledHours, string location, int enrollmentId_Ref, bool isRecurring = false, string recurrenceType = "Weekly",  int recurrenceCount = 1 )
        {
            var user = await _userManager.GetUserAsync(User);
            var coach = await _coachRepository.GetCoachByIdAsync(user.Id);
            int coachId = coach.CoachID;


            Child? child = await _childService.GetAsync(childId);
            if (child == null)
            {
                throw new ArgumentException("Child not found");
            }
            
            var nowToronto = DateTimeHelper.GetTorontoTime();

            if (scheduledAt < nowToronto)
            {
                TempData["ErrorMessage"] = "Please choose a future time.";
                return RedirectToAction("ManageSchedules", new { childId, courseId = courseId, enrollmentId = enrollmentId_Ref });
            }

            else
            { 
                var course = await _courseService.GetAsync(courseId); // Ensure the course exists

                if (course == null)
                {
                    TempData["ErrorMessage"] = "Course not found.";
                    return RedirectToAction("ManageSchedules", new { childId, courseId = courseId, enrollmentId = enrollmentId_Ref });
                }


                bool allSuccess = true;

                if (isRecurring && recurrenceCount <= 0)
                {
                    TempData["ErrorMessage"] = "Invalid recurrence count.";
                    return RedirectToAction("ManageSchedules", new { childId, courseId = courseId, enrollmentId = enrollmentId_Ref });
                }

                int totalToSchedule = isRecurring ? recurrenceCount : 1;

                if (course.SessionCount != null)
                {
                    var scheduledCount = (await _courseEnrollmentService.GetSchedulesByCourseChildAsync(courseId, childId)).Count();
                    var completedCount = (await _courseEnrollmentService.GetCompletesByCourseChildAsync(courseId, childId)).Count();

                    if (scheduledCount + completedCount + totalToSchedule > course.SessionCount)
                    {
                        TempData["ErrorMessage"] = "The maximum number of sessions for this course has been reached.";
                        return RedirectToAction("ManageSchedules", new { childId, courseId, enrollmentId = enrollmentId_Ref });
                    }
                }



                DateTime currentDate = scheduledAt;

                for (int i = 0; i < totalToSchedule; i++)
                {
                    bool result = await _courseEnrollmentService.ScheduleCourseAsync(
                        childId, courseId, currentDate, scheduledHours, location, coachId, enrollmentId_Ref);

                    if (!result)
                    {
                        allSuccess = false;
                        break;
                    }

                    if (isRecurring)
                    {
                        if (recurrenceType.Equals("Weekly", StringComparison.OrdinalIgnoreCase))
                            currentDate = currentDate.AddDays(7);
                        else if (recurrenceType.Equals("Daily", StringComparison.OrdinalIgnoreCase))
                            currentDate = currentDate.AddDays(1);
                        else
                            break;
                    }
                }

                if (allSuccess)
                {
                    TempData["SuccessMessage"] = "Session(s) scheduled successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to schedule one or more sessions.";
                }
            }

            return RedirectToAction("ManageSchedules", new { childId, courseId = courseId, enrollmentId = enrollmentId_Ref });
        }



        



        [Authorize(Roles = "Coach")]
        [HttpPost("DeleteSchedule")]
        public async Task<IActionResult> DeleteSchedule(int enrollmentId, int childId, int courseId, string coachNote, int enrollmentId_Ref)
        {

            var child = await _childService.GetAsync(childId);
            var course = await _courseService.GetAsync(courseId);
            var user = await _userManager.GetUserAsync(User);
            var coach = await _coachRepository.GetCoachByIdAsync(user.Id);

            var enrollment = await _courseEnrollmentService.GetAsync(enrollmentId);

            bool result = await _courseEnrollmentService.RemoveScheduleAsync(enrollmentId, coachNote);
            

            if (result)
            {
                //var subject = "A Course schedule has been deleted";

                //var message = "A course schedule has been deleted for the child: " + child.Name + ":\n" +
                //    "Course: " + course.Title + "\n" +
                //    "Coach: " + coach.Name + "\n" +
                //    "Scheduled At: " + enrollment.ScheduledAt?.ToString("yyyy - MM - dd HH: mm") + "\n" +
                //    "Scheduled Hours: " + enrollment.ScheduledHours;

                //await _emailService.SendEmailAsync(child.User.Email, subject, message);  //send to child

                TempData["SuccessMessage"] = "Schedule deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete the schedule.";
            }

            return RedirectToAction("ManageSchedules", new { childId, courseId = courseId , enrollmentId = enrollmentId_Ref });
        }


        [Authorize(Roles = "Coach")]
        [HttpGet("ManageEnrollments/{childId}")]
        public async Task<IActionResult> ManageEnrollments(int childId, [FromQuery] int courseId)
        {
            var user = await _userManager.GetUserAsync(User);
            var coach = await _coachRepository.GetCoachByIdAsync(user.Id);
            int coachId = coach.CoachID;

            // Get all children registered in the coach's course
            //var children = await _courseEnrollmentService.GetRegisterationByCoachAsync(coachId);

            // Get enrollment details
            //var course = await _courseService.GetActiveCourseByCoachAsync(coachId);
            //int courseId = 2;  //need to change later
            var course = await _courseService.GetAsync(courseId);
            Child? child = await _childService.GetAsync(childId);

            if (child == null)
            {
                throw new ArgumentException("Child not found");
            }

            var model = new ManageEnrollmentsViewModel
            {
                Course = course,
                Child = child,
                //ScheduledEnrollments = (List<CourseEnrollment>)await _courseEnrollmentService.GetSchedulesByCourseChildAsync(course.CourseID, childId),
                
                
                WaitToCompleteEnrollments = (List<CourseEnrollment>)await _courseEnrollmentService.GetWaitToCompleteByCourseChildAsync(course.CourseID, childId),
                CompletedEnrollments = (List<CourseEnrollment>)await _courseEnrollmentService.GetCompletesByCourseChildAsync(course.CourseID, childId),
                DeletedEnrollments = (List<CourseEnrollment>)await _courseEnrollmentService.GetDeletedByCourseChildAsync(course.CourseID, childId)
            };

            return View(model);
        }

        [Authorize(Roles = "Coach")]
        [HttpPost("CompleteSession")]
        public async Task<IActionResult> CompleteSession(int enrollmentId, int childId, int courseId, decimal? actualHours, string? coachNote)
        {
            //int coachId = 16; // GetLoggedInCoachId(); // Replace with actual logic to get coach ID
            var user = await _userManager.GetUserAsync(User);

            Child? child = await _childService.GetAsync(childId);
            if (child == null)
            {
                throw new ArgumentException("Child not found");
            }

            var course = await _courseService.GetAsync(courseId); // Ensure the course exists
            var courseEnrollment = await _courseEnrollmentService.GetAsync(enrollmentId);

            try
            {
                //decimal hoursToUse = actualHours ?? courseEnrollment.ScheduledHours;
                decimal hoursToUse = actualHours ?? courseEnrollment.ScheduledHours ?? 0;
                string noteToUse = !string.IsNullOrWhiteSpace(coachNote) ? coachNote : "";

                bool result1 = true;

                if (hoursToUse > 0)
                {
                    result1 = await _courseEnrollmentService.CompleteSessionAsync(enrollmentId, hoursToUse, noteToUse);
                }

                if (hoursToUse == 0)
                { 
                    result1 = await _courseEnrollmentService.RemoveScheduleAsync(enrollmentId, noteToUse);
                }

                //We don't calculate income for Coachs because this will be done by accounting manually

                bool result2 = true;

                if (hoursToUse > 0)
                {
                    result2 = await _incomeService.UpdateCoachIncomeAsync(enrollmentId, user.Id);
                }

                bool result3 = true;

                if(courseEnrollment.EnrollmentID_Ref!=null)
                {
                    Core.Models.Fee? fee = await _feeService.GetFeeForCourseEnrollmentAsync((int)courseEnrollment.EnrollmentID_Ref);
                    if (fee != null && fee.PaymentModel == "Token")
                    {
                        result3 = await _balanceService.DeductCourseSessionCostAsync(enrollmentId, user.Id); // Deduct private course cost from child's balance
                    }
                }
                
                


                //if (result1 && result2 && result3)
                if (result1 && result2 && result3)
                    //if (result1)
                    {
                    TempData["SuccessMessage"] = "Course Completed successfully.";


                    var subject = "Your Child’s Course Session Has Been Successfully Completed";


                    var htmlMessage =
                                        "<p>Hello,</p>" +
                                        "<p>We’re happy to let you know that the following course session for <strong>" +
                                        WebUtility.HtmlEncode(child.Name) +
                                        "</strong> has been completed successfully:</p>" +
                                        "<ul>" +
                                          "<li><strong>Course:</strong> " + WebUtility.HtmlEncode(course.Title) + "</li>" +
                                          "<li><strong>Scheduled At:</strong> " +
                                            WebUtility.HtmlEncode(courseEnrollment.ScheduledAt?.ToString("yyyy-MM-dd HH:mm") ?? "N/A") + "</li>" +
                                          "<li><strong>Actual Hours Completed:</strong> " + WebUtility.HtmlEncode(hoursToUse.ToString()) + "</li>" +
                                        "</ul>" +
                                        "<p>If you have any questions about this session or need any further information, please feel free to contact us anytime.</p>" +
                                        "<p>Thank you for your continued support!</p>" +
                                        "<p>NSNS Support Team</p>";


                    //await _emailService.SendEmailAsync(child.User.Email, subject, htmlMessage);  //send to child



                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to complete the course.";
                }
            }
            catch (Exception ex)
            {
                
                TempData["ErrorMessage"] = $"{ex.Message}";
            }

            return RedirectToAction("ManageEnrollments", new { childId, courseId = courseId});
        }

        [Authorize(Roles = "Coach")]
        [HttpGet("MyHours")]
        public async Task<IActionResult> MyHours()
        {
            // Get current coach based on User ID
            var user = await _userManager.GetUserAsync(User);
            var coach = await _coachRepository.GetCoachByIdAsync(user.Id);


            if (coach == null)
                return NotFound("Coach profile not found.");

            // Get income records
            //var incomeRecords = await _incomeService.GetCoachIncomeAsync(coach.CoachID);

            //var viewModel = incomeRecords.Select(i => new HoursViewModel
            //{
            //    EnrollmentID = i.EnrollmentID,
            //    CourseName = i.Course?.Title ?? "N/A",
            //    ChildName = i.Enrollment?.Child.Name ?? "N/A",
            //    SessionDate = i.Enrollment?.ScheduledAt ?? DateTime.MinValue,
            //    SessionHours = i.Enrollment?.ActualHours ?? 0,

            //}).ToList();

            //ViewBag.TotalIncome = viewModel.LastOrDefault()?.TotalIncomeSoFar ?? 0;

            var incomeRecords = await _incomeService.GetCoachMonthlyIncomeAsync(coach.CoachID);

            return View(incomeRecords.ToList());
        }

        
        [HttpGet("GetCoachSchedules")]
        public async Task<IActionResult> GetCoachSchedules()
        {
            // Get current coach based on User ID
            var user = await _userManager.GetUserAsync(User);
            var coach = await _coachRepository.GetCoachByIdAsync(user.Id);

            var schedules = await _courseEnrollmentService.GetCoachSchedulesAsync(coach.CoachID);

            return Json(schedules);

        }

        [Authorize(Roles = "Coach")]
        [HttpPost("UpdateSchedule")]
        public async Task<IActionResult> UpdateSchedule([FromBody] UpdateCoachScheduleViewModel vm)
        {
            await _courseEnrollmentService.UpdateCoachSchedule(vm);
            // You need to provide values for childId, courseId, and enrollmentId_Ref here if you want to redirect.
            // For now, just return Ok or a suitable result.
            //return RedirectToAction("ManageSchedules", new { vm.ChildId, courseId = vm.CourseId, enrollmentId = vm.EnrollmentId_Ref });
            return Ok(new
            {
                redirectUrl = Url.Action(
                "ManageSchedules",
                "Coach",
                new
                {
                    childId = vm.ChildId,
                    courseId = vm.CourseId,
                    enrollmentId = vm.EnrollmentId_Ref
                })
            });
           
        }


        [Authorize(Roles = "Coach")]
        [HttpGet("MyCalendar")]
        public async Task<IActionResult> MyCalendar()
        {
            // Get current coach based on User ID
          
            return View();
        }


        [Authorize(Roles = "Staff")]
        [HttpGet("Hours/{coachId}")]
        public async Task<IActionResult> Hours(int coachId)
        {
            // Get current coach based on User ID
           var coach = await _coachService.GetAsync(coachId);
           var incomeRecords = await _incomeService.GetCoachMonthlyIncomeAsync(coachId);



            var viewModel = new CoachHoursViewModel
            {
                Coach = coach,
                MonthlyIncomes = incomeRecords.ToList()
            };



            return View(viewModel);



        }
    }
}

