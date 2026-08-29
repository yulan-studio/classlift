
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
        private readonly IProvinceService _provinceService;
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
        private readonly ITimeZoneService _timeZoneService;
        private readonly CurrentTenant _currentTenant;

        private string ProviderName =>
            _currentTenant.Terminology.ProviderSingular;

        private string ProviderNameLower =>
            ProviderName.ToLowerInvariant();
        
        public CoachController(ICoachService coachService, ICoachRepository coachRepository, ICoachIncomeService incomeService,  IEmergencyContactService emergencyService, IChildBalanceService balanceService, ICityService cityService, IProvinceService provinceService, ISpecialtyService specialtyService, ICoachSpecialtyService coachSpecialtyService, ICourseEnrollmentService courseEnrollmentService, ICourseService courseService, IChildService childService, IParentChildService parentChildService, IFeeService feeService, EmailService emailService, UserManager<Core.Models.User> userManager, ITimeZoneService timeZoneService, CurrentTenant currentTenant)
        {
            _coachService = coachService;
            _incomeService = incomeService;
            _balanceService = balanceService;
            _coachRepository = coachRepository;
            _cityService = cityService;
            _provinceService = provinceService;
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
            _timeZoneService = timeZoneService;
            _currentTenant = currentTenant;
            
        }

        private async Task PopulateLocationListsAsync(int? provinceId, int? cityId)
        {
            ViewBag.ProvinceList = (await _provinceService.GetAllAsync())
                .Select(province => new SelectListItem
                {
                    Value = province.ProvinceID.ToString(),
                    Text = province.Name,
                    Selected = province.ProvinceID == provinceId
                }).ToList();

            ViewBag.CityList = (await _cityService.GetAllAsync())
                .Where(city => city.ProvinceID == provinceId)
                .Select(city => new SelectListItem
                {
                    Value = city.CityID.ToString(),
                    Text = city.Name,
                    Selected = city.CityID == cityId
                }).ToList();
        }

        [HttpGet("CitiesByProvince/{provinceId:int}")]
        public async Task<IActionResult> CitiesByProvince(int provinceId)
        {
            var cities = (await _cityService.GetAllAsync())
                .Where(city => city.ProvinceID == provinceId)
                .Select(city => new { value = city.CityID.ToString(), text = city.Name });

            return Json(cities);
        }

        [Authorize(Roles = "Staff")]
        // POST: Add Staff Action
        [HttpPost("Add")]
        //[HttpPost]
        public async Task<IActionResult> Add(string name, string email, string password, List<int> specialtyIds, string gender, string phone, int? provinceId, int? cityId)
        {
            if (!provinceId.HasValue)
                ModelState.AddModelError(nameof(provinceId), "Please select a province.");
            if (!cityId.HasValue)
                ModelState.AddModelError(nameof(cityId), "Please select a city.");
            if (provinceId.HasValue && cityId.HasValue)
            {
                var selectedCity = await _cityService.GetAsync(cityId.Value);
                if (selectedCity?.ProvinceID != provinceId.Value)
                    ModelState.AddModelError(nameof(cityId), "The selected city does not belong to the selected province.");
            }

            if (!ModelState.IsValid)
            {
                await PopulateLocationListsAsync(provinceId, cityId);

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
                
                var result = await _coachService.AddAsync(name, email, password, specialtyIds, gender, phone, cityId!.Value, user);
                if (!result)
                {
                    ModelState.AddModelError(string.Empty, $"Failed to add the {ProviderNameLower} information.");

                   
                    // Repopulate CityList for the dropdown if validation fails

                    await PopulateLocationListsAsync(provinceId, cityId);

                    var specialties = await _specialtyService.GetAllAsync(); // Replace with your data fetching logic
                    ViewBag.SpecialtyList = specialties.Select(c => new SelectListItem
                    {
                        Value = c.SpecialtyID.ToString(),
                        Text = c.Title
                    }).ToList();


                    return View();
                }
                TempData["SuccessMessage"] = $"{ProviderName} information has been added successfully.";
                return RedirectToAction("List"); // Redirect to the coach list page


            }
            catch (Exception ex)
            {
                ModelState.AddModelError(String.Empty, $"{ex.Message}");
                
                // Repopulate CityList for the dropdown if validation fails

                await PopulateLocationListsAsync(provinceId, cityId);

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
            await PopulateLocationListsAsync(null, null);

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
                    TempData["ErrorMessage"] = $"The {ProviderNameLower} member could not be deleted.";
                    return RedirectToAction("List");
                }

                TempData["SuccessMessage"] = $"{ProviderName} member has been deleted successfully.";
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
                ViewData["StatusSortParm"] = sortOrder == "status" ? "status_desc" : "status";
                ViewData["GenderSortParm"] = sortOrder == "gender" ? "gender_desc" : "gender";
                ViewData["CitySortParm"] = sortOrder == "city" ? "city_desc" : "city";
                ViewData["CurrentSort"] = sortOrder;



                coaches = sortOrder switch
                {
                    "id" => coaches.OrderBy(c => c.Coach.MemberID).ToList(),
                    "id_desc" => coaches.OrderByDescending(c => c.Coach.MemberID).ToList(),
                    "name" => coaches.OrderBy(c => c.Coach.Name).ToList(),
                    "name_desc" => coaches.OrderByDescending(c => c.Coach.Name).ToList(),
                    "status" => coaches.OrderBy(c => c.Coach.Status == "InActive" ? "InActive" : "Active").ToList(),
                    "status_desc" => coaches.OrderByDescending(c => c.Coach.Status == "InActive" ? "InActive" : "Active").ToList(),
                    "gender" => coaches.OrderBy(c => c.Coach.Gender).ToList(),
                    "gender_desc" => coaches.OrderByDescending(c => c.Coach.Gender).ToList(),
                    "city" => coaches.OrderBy(c => c.Coach.City.Name).ToList(),
                    "city_desc" => coaches.OrderByDescending(c => c.Coach.City.Name).ToList(),

                    _ => coaches.OrderBy(c => c.Coach.Name).ToList() // default
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

            var selectedProvinceId = (await _cityService.GetAsync(coach.CityID)).ProvinceID;
            await PopulateLocationListsAsync(selectedProvinceId, coach.CityID);


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

       
        public async Task<IActionResult> Edit(int coachId, string name, string email, /*string password,*/List<int> specialtyIds, string gender, string phone, int? provinceId, int? cityId)
        {
            if (!provinceId.HasValue)
                ModelState.AddModelError(nameof(provinceId), "Please select a province.");
            if (!cityId.HasValue)
                ModelState.AddModelError(nameof(cityId), "Please select a city.");
            if (provinceId.HasValue && cityId.HasValue)
            {
                var selectedCity = await _cityService.GetAsync(cityId.Value);
                if (selectedCity?.ProvinceID != provinceId.Value)
                    ModelState.AddModelError(nameof(cityId), "The selected city does not belong to the selected province.");
            }

            if (!ModelState.IsValid)
            {
                var coach = await _coachService.GetAsync(coachId);
                await PopulateLocationListsAsync(provinceId, cityId);
                var specialties = await _specialtyService.GetAllAsync();
                ViewBag.SpecialtyList = specialties.Select(s => new SelectListItem
                {
                    Value = s.SpecialtyID.ToString(), Text = s.Title,
                    Selected = specialtyIds.Contains(s.SpecialtyID)
                }).ToList();
                return View(coach);
            }

            try
            {
                var user = await _userManager.GetUserAsync(User);
                var result = await _coachService.UpdateAsync(coachId, name, email, /*password,*/specialtyIds, gender, phone, cityId!.Value, user);


                if (!result)
                {
                    ModelState.AddModelError(string.Empty, $"Failed to update {ProviderNameLower} information.");
                    var coach = await _coachService.GetAsync(coachId);

                    if (coach == null)
                    {
                        return NotFound();
                    }

                    await PopulateLocationListsAsync(provinceId, cityId);


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

                TempData["SuccessMessage"] = $"{ProviderName} information updated successfully.";
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

                await PopulateLocationListsAsync(provinceId, cityId);


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
        [HttpGet("EmergencyContacts/{coachId}")]
        public async Task<IActionResult> EmergencyContacts(int coachId)
        {
            var coach = await _coachService.GetAsync(coachId);
            return View(coach);
        }

        [Authorize(Roles = "Staff")]
        [HttpPost("CoreInfo/{coachId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CoreInfo(int coachId, string? memberID, string? preferedName, string? wechat, string? whatsApp, string? address, /*int OAPAmount, */string? postCode, int? bank, int? transit, int? account, string status, bool photoConsent)
        {
            if (status is not ("Active" or "InActive"))
                ModelState.AddModelError(nameof(status), "Please select Active or InActive.");

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = string.Join(" ", ModelState.Values
                    .SelectMany(value => value.Errors)
                    .Select(error => error.ErrorMessage)
                    .Where(message => !string.IsNullOrWhiteSpace(message)));
                return RedirectToAction("MoreInfo", new { coachId });
            }

            try
            {
                var saved = await _coachService.UpdateAsync(
                    coachId,
                    memberID,
                    preferedName,
                    wechat,
                    whatsApp,
                    address,
                    postCode,
                    bank,
                    transit,
                    account,
                    status,
                    photoConsent);

                if (!saved)
                {
                    TempData["ErrorMessage"] = $"The {ProviderNameLower} information could not be saved. Please try again.";
                    return RedirectToAction("MoreInfo", new { coachId });
                }

                TempData["SuccessMessage"] = $"The {ProviderNameLower} information was saved successfully.";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = $"An unexpected error occurred while saving the {ProviderNameLower} information. Please try again.";
            }

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

                
                return RedirectToAction("MoreInfo", new { coachId, tab = "EmergencyContacts" });
               

                
            
            }

            return RedirectToAction("MoreInfo", new { coachId, tab = "EmergencyContacts" });
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

            return RedirectToAction("MoreInfo", new { coachId, tab = "EmergencyContacts" });
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
                    TempData["ErrorMessage"] = $"No specialties found for this {ProviderNameLower}.";
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
                if (course.CoachID != coachId)
                    return Forbid();

                var rootEnrollment = await _courseEnrollmentService.GetAsync(enrollmentId);
                if (rootEnrollment.EnrollmentID_Ref != null
                    || rootEnrollment.ChildID != childId
                    || rootEnrollment.CourseID != courseId)
                {
                    return BadRequest("The registration does not match the selected child and course.");
                }

                // ✅ Get schedules for the child and course
                List<CourseEnrollment> schedules = (List<CourseEnrollment>)await _courseEnrollmentService.GetUpcomingByRootEnrollmentAsync(enrollmentId);

                List<CourseEnrollment> completed = (List<CourseEnrollment>)await _courseEnrollmentService.GetCompletesByRootEnrollmentAsync(enrollmentId);

                List<CourseEnrollment> scheduled = (List<CourseEnrollment>)await _courseEnrollmentService.GetSchedulesByRootEnrollmentAsync(enrollmentId);
                ViewBag.UserTimeZoneId = user.TimeZoneId;
                ViewBag.TimeZones = _timeZoneService.GetTimeZones();

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
        public async Task<IActionResult> ScheduleCourse(int childId, int courseId, DateTime scheduledAt, string scheduledTimeZoneId, decimal scheduledHours, string location, int enrollmentId_Ref, bool isRecurring = false, string recurrenceType = "Weekly",  int recurrenceCount = 1 )
        {
            var user = await _userManager.GetUserAsync(User);
            var coach = await _coachRepository.GetCoachByIdAsync(user.Id);
            int coachId = coach.CoachID;


            Child? child = await _childService.GetAsync(childId);
            if (child == null)
            {
                throw new ArgumentException("Child not found");
            }
            
            if (!_timeZoneService.IsValidTimeZone(scheduledTimeZoneId))
            {
                TempData["ErrorMessage"] = "Please select a valid event time zone.";
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

                if (course.CoachID != coachId)
                    return Forbid();


                bool allSuccess = true;

                if (isRecurring && recurrenceCount <= 0)
                {
                    TempData["ErrorMessage"] = "Invalid recurrence count.";
                    return RedirectToAction("ManageSchedules", new { childId, courseId = courseId, enrollmentId = enrollmentId_Ref });
                }

                if (isRecurring && (recurrenceCount > 365 || recurrenceType is not ("Daily" or "Weekly")))
                {
                    TempData["ErrorMessage"] = "Please select a valid recurrence type and a count no greater than 365.";
                    return RedirectToAction("ManageSchedules", new { childId, courseId, enrollmentId = enrollmentId_Ref });
                }

                int totalToSchedule = isRecurring ? recurrenceCount : 1;

                if (course.SessionCount != null)
                {
                    var countedSessionCount = await _courseEnrollmentService
                        .GetCountedSessionCountByRootEnrollmentAsync(enrollmentId_Ref);

                    if (countedSessionCount + totalToSchedule > course.SessionCount)
                    {
                        TempData["ErrorMessage"] = "The maximum number of sessions for this course has been reached.";
                        return RedirectToAction("ManageSchedules", new { childId, courseId, enrollmentId = enrollmentId_Ref });
                    }
                }



                DateTime currentDate = DateTime.SpecifyKind(scheduledAt, DateTimeKind.Unspecified);
                var timings = new List<ScheduleTiming>();

                try
                {
                    for (var i = 0; i < totalToSchedule; i++)
                    {
                        var utc = _timeZoneService.ConvertLocalToUtc(currentDate, scheduledTimeZoneId);
                        if (utc <= DateTime.UtcNow)
                            throw new ArgumentException("Please choose a future time.");
                        timings.Add(new ScheduleTiming
                        {
                            ScheduledAtUtc = utc,
                            ScheduledLocalTime = currentDate,
                            TimeZoneId = scheduledTimeZoneId
                        });
                        currentDate = recurrenceType.Equals("Daily", StringComparison.OrdinalIgnoreCase)
                            ? currentDate.AddDays(1)
                            : currentDate.AddDays(7);
                    }
                }
                catch (ArgumentException ex)
                {
                    TempData["ErrorMessage"] = ex.Message;
                    return RedirectToAction("ManageSchedules", new { childId, courseId, enrollmentId = enrollmentId_Ref });
                }

                foreach (var timing in timings)
                {
                    bool result = await _courseEnrollmentService.ScheduleCourseAsync(
                        childId, courseId, timing, scheduledHours, location, coachId, enrollmentId_Ref);

                    if (!result)
                    {
                        allSuccess = false;
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
        public async Task<IActionResult> ManageEnrollments(
            int childId,
            [FromQuery] int courseId,
            [FromQuery] int enrollmentId)
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
            if (course.CoachID != coachId)
                return Forbid();

            Child? child = await _childService.GetAsync(childId);

            if (child == null)
            {
                throw new ArgumentException("Child not found");
            }

            var rootEnrollment = await _courseEnrollmentService.GetAsync(enrollmentId);
            if (rootEnrollment.EnrollmentID_Ref != null
                || rootEnrollment.ChildID != childId
                || rootEnrollment.CourseID != courseId)
            {
                return BadRequest("The registration does not match the selected child and course.");
            }

            var model = new ManageEnrollmentsViewModel
            {
                EnrollmentID = enrollmentId,
                Course = course,
                Child = child,
                //ScheduledEnrollments = (List<CourseEnrollment>)await _courseEnrollmentService.GetSchedulesByCourseChildAsync(course.CourseID, childId),
                
                
                WaitToCompleteEnrollments = (List<CourseEnrollment>)await _courseEnrollmentService.GetWaitToCompleteByRootEnrollmentAsync(enrollmentId),
                CompletedEnrollments = (List<CourseEnrollment>)await _courseEnrollmentService.GetCompletesByRootEnrollmentAsync(enrollmentId),
                DeletedEnrollments = (List<CourseEnrollment>)await _courseEnrollmentService.GetDeletedByRootEnrollmentAsync(enrollmentId)
            };

            return View(model);
        }

        [Authorize(Roles = "Coach")]
        [HttpGet("ViewEnrollments/{childId}")]
        public async Task<IActionResult> ViewEnrollments(
            int childId,
            [FromQuery] int courseId,
            [FromQuery] int enrollmentId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Challenge();

            var coach = await _coachRepository.GetCoachByIdAsync(user.Id);
            var course = await _courseService.GetAsync(courseId);
            if (coach == null || course.CoachID != coach.CoachID)
                return Forbid();

            var child = await _childService.GetAsync(childId);

            if (child == null)
                return NotFound("Child not found.");

            var rootEnrollment = await _courseEnrollmentService.GetAsync(enrollmentId);
            if (rootEnrollment.EnrollmentID_Ref != null
                || rootEnrollment.ChildID != childId
                || rootEnrollment.CourseID != courseId)
            {
                return BadRequest("The registration does not match the selected child and course.");
            }

            var model = new ManageEnrollmentsViewModel
            {
                EnrollmentID = enrollmentId,
                Course = course,
                Child = child,
                WaitToCompleteEnrollments = new List<CourseEnrollment>(),
                CompletedEnrollments = (List<CourseEnrollment>)await _courseEnrollmentService
                    .GetCompletesByRootEnrollmentAsync(enrollmentId),
                DeletedEnrollments = (List<CourseEnrollment>)await _courseEnrollmentService
                    .GetDeletedByRootEnrollmentAsync(enrollmentId)
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

            return RedirectToAction("ManageEnrollments", new
            {
                childId,
                courseId,
                enrollmentId = courseEnrollment.EnrollmentID_Ref
            });
        }

        [Authorize(Roles = "Coach")]
        [HttpGet("MyHours")]
        public async Task<IActionResult> MyHours()
        {
            // Get current coach based on User ID
            var user = await _userManager.GetUserAsync(User);
            var coach = await _coachRepository.GetCoachByIdAsync(user.Id);


            if (coach == null)
                return NotFound($"{ProviderName} profile not found.");

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

            foreach (var schedule in schedules)
            {
                schedule.Start = _timeZoneService.ConvertUtcToLocal(schedule.Start, user.TimeZoneId);
                schedule.End = _timeZoneService.ConvertUtcToLocal(schedule.End, user.TimeZoneId);
            }

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

