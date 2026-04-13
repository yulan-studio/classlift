
using Core;
using Core.Contexts;
using Core.FormModels;
using Core.Interfaces;
using Core.Models;
using Core.Repositories;
using Core.Services;
using Core.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using NuGet.Protocol.Core.Types;
using System.Data.SqlClient;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using X.PagedList;
using X.PagedList.Extensions;
using static Core.ViewModels.ManageSessionRegistrationsViewModel;



namespace Web.Controllers.User
{


    [Route("Child")]

    //[ApiController]
    public class ChildController : Controller
    {
        private readonly IChildService _childService;
        private readonly ICourseService _courseService;
        private readonly IParentService _parentService;
        private readonly ICityService _cityService;
        private readonly IParentChildService _parentChildService;
        private readonly IEmergencyContactService _emergencyContactService;
        private readonly ICourseEnrollmentService _courseEnrollmentService;
        private readonly ISpecialtyService _specialtyService;
        private readonly IActivityEnrollmentService _activityEnrollmentService;
        private readonly IActivityService _activityService;
        private readonly IFeeService _feeService;
        private readonly IPaymentService _paymentService;
        private readonly IChildBalanceService _balanceService;
        private readonly IChildCalendarService _calendarService;
        private readonly EmailService _emailService;
        private readonly UserManager<Core.Models.User> _userManager;
        private readonly Core.R2.R2StorageService _r2UploadService;
        //private readonly AppDbContext _context;


        public ChildController(IChildService childService, IEmergencyContactService emergencyContactService, ICourseService courseService, IChildBalanceService balanceService, IParentService parentService, ICityService cityService, IParentChildService parentChildService, ISpecialtyService specialtyService, IActivityService activityService, ICourseEnrollmentService courseEnrollmentService, IActivityEnrollmentService activityEnrollmentService, IFeeService feeService, IPaymentService paymentService, IChildCalendarService calendarService, EmailService emailService, UserManager<Core.Models.User> userManager, Core.R2.R2StorageService r2UploadService/*, AppDbContext context*/)
        {
            _r2UploadService = r2UploadService;
            _childService = childService;
            _balanceService = balanceService;
            _parentService = parentService;
            _cityService = cityService;
            _parentChildService = parentChildService;
            _courseService = courseService;
            _specialtyService = specialtyService;
            _courseEnrollmentService = courseEnrollmentService;
            _activityService = activityService;
            _activityEnrollmentService = activityEnrollmentService;
            _feeService = feeService;
            _paymentService = paymentService;
            _userManager = userManager;
            _emergencyContactService = emergencyContactService;
            _emailService = emailService;
            _calendarService = calendarService;

            //_context = context;   // For transaction
        }

        // ✅ Helper method to get City List
        private async Task<List<SelectListItem>> GetCityList()
        {
            return (await _cityService.GetAllAsync())
                .Select(c => new SelectListItem { Value = c.CityID.ToString(), Text = c.Name })
                .ToList();
        }


        private async Task<List<SelectListItem>> GetCityList(Child child)
        {

            var cities = await _cityService.GetAllAsync();

            return cities.Select(c => new SelectListItem
            {
                Value = c.CityID.ToString(),
                Text = c.Name,
                Selected = c.CityID == child.CityID
            }).ToList();
        }


        [HttpGet("List")]
        // ✅ List all children
        public async Task<IActionResult> List(string sortOrder, int? page, string searchName)
        {
            var children = await _childService.GetAllAsync();
            var childrenWithRequestOrConcerns = await _courseEnrollmentService.GetChildrenWithRequestsOrConcernsAsync();
            var childrenWithDelete = new List<ChildWithDeleteViewModel>();

            // 🔍 If searching → ignore paging & sorting
            if (!string.IsNullOrEmpty(searchName))
            {
                var filteredChildren = children
                    .Where(c => c.Name.Contains(searchName))
                    .ToList();

                foreach (Child c in filteredChildren)
                {
                    var canDelete = !await _childService.CheckPaidAsync(c.ChildID) && !await _childService.CheckRegisteredAsync(c.ChildID);
                    var childWithDelete = new ChildWithDeleteViewModel();
                    childWithDelete.Child = c;
                    childWithDelete.CanDelete = canDelete;
                    childrenWithDelete.Add(childWithDelete);
                }

                // convert to IPagedList just to match your View model
                return View(childrenWithDelete.ToPagedList(1, childrenWithDelete.Count == 0 ? 1 : childrenWithDelete.Count));

                
            }


            else
            {
                ViewData["RequestConcernChildIds"] = childrenWithRequestOrConcerns;
                ViewData["MemberIDParm"] = sortOrder == "id" ? "id_desc" : "id";
                ViewData["NameSortParm"] = sortOrder == "name" ? "name_desc" : "name";
                ViewData["BirthDateSortParm"] = sortOrder == "birthday" ? "birthday_desc" : "birthday";
                ViewData["GenderSortParm"] = sortOrder == "gender" ? "gender_desc" : "gender";
                ViewData["CitySortParm"] = sortOrder == "city" ? "city_desc" : "city";
                ViewData["OAPSortParm"] = sortOrder == "oap" ? "oap_desc" : "oap";
                ViewData["CurrentSort"] = sortOrder;





                children = sortOrder switch
                {
                    "id" => children.OrderBy(c => c.MemberID),
                    "id_desc" => children.OrderByDescending(c => c.MemberID),
                    "name" => children.OrderBy(c => c.Name),
                    "name_desc" => children.OrderByDescending(c => c.Name),
                    "birthday" => children.OrderBy(c => c.BirthDate),
                    "birthday_desc" => children.OrderByDescending(c => c.BirthDate),
                    "gender" => children.OrderBy(c => c.Gender),
                    "gender_desc" => children.OrderByDescending(c => c.Gender),
                    "city" => children.OrderBy(c => c.City.Name),
                    "city_desc" => children.OrderByDescending(c => c.City.Name),
                    "oap" => children.OrderBy(c => c.HasOAP),
                    "oap_desc" => children.OrderByDescending(c => c.HasOAP),
                    _ => children.OrderBy(c => c.Name) // default
                };


                foreach (Child c in children)
                {
                    var canDelete = !await _childService.CheckPaidAsync(c.ChildID) && !await _childService.CheckRegisteredAsync(c.ChildID);
                    var childWithDelete = new ChildWithDeleteViewModel();
                    childWithDelete.Child = c;
                    childWithDelete.CanDelete = canDelete;
                    childrenWithDelete.Add(childWithDelete);
                }

                //return View(childrenWithDelete);

                int pageSize = 40;
                int pageNumber = page ?? 1;


                return View(childrenWithDelete.ToPagedList(pageNumber, pageSize));

            }
                //ViewBag.RequestConcernChildIds = childrenWithConcerns;
                



        }




        //public async Task<IActionResult> List(string searchName)
        ////public IActionResult List(string sortOrder, int? page, string searchName)
        //{

        //    var children = await _childService.GetAllAsync();
        //    if (!string.IsNullOrEmpty(searchName))
        //    {
        //        children = children.Where(c => c.Name.Contains(searchName));
        //    }

        //    //int pageSize = 10;
        //    //int pageNumber = (page ?? 1);

        //    return View(children);
        //}



        // GET: Add View
        [HttpGet("Add")]
        //[HttpGet]
        public async Task<IActionResult> Add()
        {
            ViewBag.CityList = await GetCityList();

            return View();
        }

        [Authorize(Roles = "Staff")]
        [HttpPost("Add")]
        public async Task<IActionResult> Add(string name, DateTime birthDate, string gender, int cityId, string email, string password, bool hasOAP)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            try
            {
                Core.Models.User user = await _userManager.GetUserAsync(User);
                var result = await _childService.AddAsync(name, birthDate, gender, cityId, email, password, hasOAP, user);
                if (!result)
                {
                    ModelState.AddModelError(string.Empty, "Failed in adding the child info.");
                    ViewBag.CityList = await GetCityList();
                    return View();
                }

                TempData["SuccessMessage"] = "Child info has been added successfully.";
                return RedirectToAction("List"); // Redirect to the child list page


            }
            catch (Exception ex)
            {
                //ModelState.AddModelError(String.Empty, $"{ex.Message}");
                TempData["ErrorMessage"] = ex.Message;
                ViewBag.CityList = await GetCityList();
                return View();

            }

        }

        [Authorize(Roles = "Staff")]
        // ✅ GET: Show Edit form with Child data
        [HttpGet("Edit/{childId}")]
        public async Task<IActionResult> Edit(int childId)
        {
            var child = await _childService.GetAsync(childId);
            if (child == null)
            {
                TempData["ErrorMessage"] = "Child not found.";
                return RedirectToAction("List");
            }

            ViewBag.CityList = await GetCityList(child);
            return View(child);
        }


        [Authorize(Roles = "Staff")]
        [HttpPost("Edit/{childId}")]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Edit(int childId, string name, DateTime birthDate, string gender, int cityId, string email, bool hasOAP/*, string password*/)
        {


            try
            {
                var result = await _childService.UpdateAsync(childId, name, birthDate, gender, cityId, email, hasOAP/*, string password*/);


                if (!result)
                {
                    ModelState.AddModelError(string.Empty, "Failed to update child information.");
                    var child = await _childService.GetAsync(childId);

                    if (child == null)
                    {
                        return NotFound();
                    }


                    ViewBag.CityList = await GetCityList(child);

                    // Pass the coach details to the Edit.cshtml view
                    return View(child);
                }

                TempData["SuccessMessage"] = "Child information updated successfully.";
                return RedirectToAction("List");
            }
            catch (Exception ex)
            {
                //TempData["ErrorMessage"] = $"Error: {ex.Message}";
                var child = await _childService.GetAsync(childId);

                if (child == null)
                {
                    return NotFound();
                }

                ViewBag.CityList = await GetCityList(child);

                // Pass the child details to the Edit.cshtml view
                return View(child);
            }
        }

        [Authorize(Roles = "Staff")]
        [HttpGet("ManageNotes/{childId}")]
        public async Task<IActionResult> ManageNotes(int childId)
        {
            var child = await _childService.GetAsync(childId);
            if (child == null) return NotFound();
            return View(child);
        }

        [Authorize(Roles = "Staff")]
        // POST: /Child/Notes/5
        [HttpPost ("ManageNotes/{childId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageNotes(int childId, [Bind("ChildID,Notes")] Child child)
        {
            
            var existingChild = await _childService.GetAsync(childId);
            if (existingChild == null) return NotFound();

            existingChild.Notes = child.Notes;
             await _childService.UpdateAsync(existingChild);

            TempData["SuccessMessage"] = "Notes saved successfully!";
           
            return RedirectToAction("Participation", new { childId, tab = "ManageNotes" });
        }



        [Authorize(Roles = "Staff")]
        // ✅ GET: Confirm delete page
        [HttpGet("ConfirmDelete/{childId}")]
        public async Task<IActionResult> ConfirmDelete(int childId)
        {
            var child = await _childService.GetAsync(childId);
            if (child == null)
            {
                TempData["ErrorMessage"] = "Child not found.";
                return RedirectToAction("List");
            }

            return View(child);
        }

        [Authorize(Roles = "Staff")]
        // ✅ POST: Delete Child
        [HttpPost("DeleteConfirmed")]
        public async Task<IActionResult> DeleteConfirmed(int childId)
        {
            var success = await _childService.RemoveAsync(childId);
            if (!success)
            {
                TempData["ErrorMessage"] = "Failed to delete child.";
            }
            else
            {
                TempData["SuccessMessage"] = "Child deleted successfully.";
            }

            return RedirectToAction("List");
        }



        [HttpGet("CoreInfo/{childId}")]
        public async Task<IActionResult> CoreInfo(int childId)
        {
            var child = await _childService.GetAsync(childId);
            return View(child);
        }



        [Authorize(Roles = "Staff")]
        [HttpPost("CoreInfo/{childId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CoreInfo(int childId, string? memberID, string? address, /*int OAPAmount, */string? primaryDiagnosis, bool photoConsent)
        {
            var child = await _childService.GetAsync(childId);
            if (ModelState.IsValid)
            {

                await _childService.UpdateAsync(childId, memberID, address, /*OAPAmount,*/ primaryDiagnosis, photoConsent);

                return RedirectToAction("MoreInfo", new { childId });
            }
            else
                //return View(child);
                return RedirectToAction("MoreInfo", new { childId });
        }



        [Authorize(Roles = "Staff")]
        [HttpPost("AddEmergencyContact/{childId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEmergencyContact(int childId, string contactName, string relationship, string phone, string email)
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
                contact.ChildID = childId;

                await _emergencyContactService.AddAsync(contact);

                return RedirectToAction("MoreInfo", new { childId });
            }

            return RedirectToAction("MoreInfo", new { childId });
        }

        [Authorize(Roles = "Staff")]
        [HttpPost("DeleteEmergencyContact/{contactId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteEmergencyContact(int contactId, int childId)
        {
            var contact = await _emergencyContactService.GetAsync(contactId);
            if (contact != null)
            {
                await _emergencyContactService.DeleteAsync(contactId);
            }

            return RedirectToAction("MoreInfo", new { childId });
        }





        [Authorize(Roles = "Staff")]
        [HttpGet("Parents/{childId}")]
        public async Task<IActionResult> Parents(int childId, int? editParentId = null)
        {
            var child = await _childService.GetAsync(childId);
            if (child == null)
            {
                TempData["ErrorMessage"] = "Child not found.";
                return RedirectToAction("List");
            }

            var parents = await _parentChildService.GetParentsByChildIdAsync(childId);


            var model = new ManageParentsViewModel
            {
                Child = child,
                Parents = parents
            };

            ViewBag.EditParentId = editParentId; // tells view which row is in edit mode

            return View(model);






        }

        [Authorize(Roles = "Staff")]
        [HttpGet("MoreInfo/{childId}")]
        public async Task<IActionResult> MoreInfo(int childId, string tab = "CoreInfo", int? editParentId = null)
        {
            var child = await _childService.GetAsync(childId);
            //if (child == null)
            //{
            //    TempData["ErrorMessage"] = "Child not found.";
            //    return RedirectToAction("List");
            //}

            ViewBag.ActiveTab = tab;
            ViewBag.EditParentId = editParentId; // pass to view
            return View(child);
        }


        [Authorize(Roles = "Staff")]
        [HttpGet("Participation/{childId}")]
        //public async Task<IActionResult> Participation(int childId, string tab = "ManageRegistrations"/*, int? editParentId = null*/)
        public async Task<IActionResult> Participation(int childId, string tab = "ManageRegistrations"/*, int? editParentId = null*/)
        {
            var child = await _childService.GetAsync(childId);
            //if (child == null)
            //{
            //    TempData["ErrorMessage"] = "Child not found.";
            //    return RedirectToAction("List");
            //}

            ViewBag.ActiveTab = tab;
            //ViewBag.EditParentId = editParentId; // pass to view
            return View(child);
        }




        [Authorize(Roles = "Staff")]
        [HttpPost("AddParent")]
        public async Task<IActionResult> AddParent(int childId, string Name, string Email, string Phone, string Wechat, string Relationship)
        {
            try
            {
                // ✅ 1. Create a new Parent object
                var user = await _userManager.GetUserAsync(User);
                var newParent = new Parent
                {
                    Name = Name,
                    Phone = Phone,
                    Email = Email,
                    Wechat = Wechat,
                    CreatedBy = user.Id, // Assume the user ID of admin/creator
                    CreatedDate = DateTimeHelper.GetTorontoTime()
                };

                // ✅ 2. Save the parent in the database
                var parentId = await _parentService.AddAndReturnIdAsync(newParent);
                if (parentId == 0)
                {
                    TempData["ErrorMessage"] = "Failed to add parent.";
                    //return RedirectToAction("ManageParents", new { childId });
                }


                var success = await _parentChildService.AddParentToChild(parentId, newParent, childId, Relationship, user.Id); // Assuming CreatedBy = 1
                if (!success)
                {
                    TempData["ErrorMessage"] = "Failed to add parent to child.";
                }
                else
                {
                    TempData["SuccessMessage"] = "Parent added to child successfully.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"{ex.Message}";
            }


            return RedirectToAction("MoreInfo", new { childId, tab = "Parents" });
        }



        [Authorize(Roles = "Staff")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateParent(int ParentChildID, int ChildID, string Name, string Email, string Phone, string Wechat, string Relationship)
        {


            var parentChild = await _parentChildService.GetParentByParentChildIdAsync(ParentChildID);
            if (parentChild != null)
            {
                var parent = parentChild.Parent;
                parent.Name = Name;
                parent.Email = Email;
                parent.Phone = Phone;
                parent.Wechat = Wechat;
                //parent.Relationship = Relationship;

                await _parentService.UpdateAsync(parent);
                TempData["SuccessMessage"] = "Parent updated successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Parent not found.";
            }

            //return RedirectToAction("MoreInfo", new { childId = ChildID });
            return RedirectToAction("MoreInfo", new { ChildID, tab = "Parents" });
        }

        //[Authorize(Roles = "Staff")]
        //[HttpGet("ConfirmDeleteParent/{parentId}")]
        //public async Task<IActionResult> ConfirmDeleteParent(int parentChildId, int childId, int parentId)
        //{
        //    try
        //    {
        //        ViewBag.ChildId = childId;
        //        ViewBag.ParentChildId = parentChildId;

        //        var parent = await _parentService.GetParentByIdAsync(parentId);
        //        return View(parent);
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["ErrorMessage"] = $"{ex.Message}";
        //        return RedirectToAction("ManageParents", new { childId });
        //    }
        //}


        [Authorize(Roles = "Staff")]
        [HttpPost("DeleteParentConfirmed")]
        public async Task<IActionResult> DeleteParent(int parentChildId, int childId, int parentId)
        {
            try
            {

                var success = await _parentChildService.RemoveParentFromChild(parentChildId);
                success = await _parentService.DeleteAsync(parentId);
                if (!success)
                {
                    TempData["ErrorMessage"] = "Failed to remove parent.";
                }
                else
                {
                    TempData["SuccessMessage"] = "Parent removed successfully.";
                }
                return RedirectToAction("MoreInfo", new { childId, tab = "Parents" });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"{ex.Message}";
                //return RedirectToAction("MoreInfo", new { childId });
                return RedirectToAction("MoreInfo", new { childId, tab = "Parents" });
            }

        }

        
        [Authorize(Roles = "Staff")]
        //[Route("Child/ManageRegistrations/{childId}")]
        [HttpGet("ManageRegistrations/{childId}")]
        public async Task<IActionResult> ManageRegistrations(int childId)
        {
            var enrollmentsWithConcerns = await _courseEnrollmentService.GeEnrollmentsWithScheduleConcernsAsync();
            //ViewBag.RequestConcernChildIds = childrenWithConcerns;
            ViewData["ConcernEnrollmentIds"] = enrollmentsWithConcerns;


            var child = await _childService.GetAsync(childId);
            if (child == null)
            {
                TempData["ErrorMessage"] = "Child not found.";
                return RedirectToAction("List"); // Redirect to child list page if not found
            }

            //Get Registered and Completed Courses
            var courseEnrollments = await _courseEnrollmentService.GetRegisteredEnrollmentsByChildAsync(childId);
            var specialties = await _specialtyService.GetAllAsync();

            ViewBag.SpecialtyList = specialties.Select(s => new SelectListItem
            {
                Value = s.SpecialtyID.ToString(),
                Text = s.Title
            }).ToList();

            //var activityRegisteredEnrollments = await _activityEnrollmentService.GetRegisteredEnrollmentsByChildAsync(childId);
            //var activityCanceledEnrollments = await _activityEnrollmentService.GetCanceledEnrollmentsByChildAsync(childId);
            //var activityEnrollments = activityRegisteredEnrollments.Concat(activityCanceledEnrollments);

            var activityEnrollments = await _activityEnrollmentService.GetAllEnrollmentsViewByChildAsync(childId);
            var activities = await _activityService.GetAllActiveOpenAsync();

            ViewBag.ActivityList = activities.Select(a => new SelectListItem
            {
                Value = a.ActivityID.ToString(),
                Text = a.Title
            }).ToList();

            return View("ManageRegistrations", new ManageRegisterationsViewModel
            {
                Child = child,
                CourseEnrollments = courseEnrollments,
                ActivityEnrollments = activityEnrollments
            });
        }






        [Authorize(Roles = "Child")]
        [HttpGet("MyRegistrations")]
        public async Task<IActionResult> MyRegistrations()
        {
            Core.Models.User user = await _userManager.GetUserAsync(User);
            var child = await _childService.GetByIdAsync(user.Id);

            //Get Registered and Completed Courses
            var courseEnrollments = await _courseEnrollmentService.GetRegisteredEnrollmentsByChildAsync(child.ChildID);


            //var activityRegisteredEnrollments = await _activityEnrollmentService.GetRegisteredEnrollmentsByChildAsync(child.ChildID);
            //var activityCanceledEnrollments = await _activityEnrollmentService.GetCanceledEnrollmentsByChildAsync(child.ChildID);
            //var activityEnrollments = activityRegisteredEnrollments.Concat(activityCanceledEnrollments);

            var activityEnrollments = await _activityEnrollmentService.GetAllEnrollmentsViewByChildAsync(child.ChildID);


            return View("MyRegistrations", new ManageRegisterationsViewModel
            {
                Child = child,
                CourseEnrollments = courseEnrollments,
                ActivityEnrollments = activityEnrollments
            });
        }




        [HttpGet("GetCoursesBySpecialty")]
        public async Task<IActionResult> GetActiveCoursesBySpecialty(int specialtyId)
        {
            var courses = await _courseService.GetActiveCoursesBySpecialtyAsync(specialtyId);
            return Json(courses.Select(c => new { c.CourseID, c.Title, c.CourseType }));
        }

        [Authorize(Roles = "Staff")]
        [HttpPost("RegisterCourse")]
        public async Task<IActionResult> RegisterCourse(int childId, int courseId, decimal scheduledHours, string paymentModel, decimal? totalCost, string? description)
        {

            var user = await _userManager.GetUserAsync(User);


            //using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                int newEnrollmentId = await _courseEnrollmentService.AddRegisteredEnrollmentAsync(childId, courseId, scheduledHours, "Registered", user);

                if (totalCost == null)
                {
                    //totalCost = 0;
                    description = "Use Token - Fee will be deducted from your balance per session"; // Ensure description is not null
                }


                bool success = await _feeService.AddCourseFeeAsync(newEnrollmentId, paymentModel, totalCost, description, user);
                if (!success)
                {
                    throw new Exception("Adding course fee failed.");
                }
               

                
                //await transaction.CommitAsync();

                TempData["SuccessMessage1"] = "Child enrolled successfully!";
            }
            catch (Exception ex)
            {
                //await transaction.RollbackAsync();
                TempData["ErrorMessage1"] = $"{ex.Message}";
                //Send email to Yulan
            }

            //return RedirectToAction("ManageRegistrations", new { childId });
            return RedirectToAction("Participation", new { childId, tab = "ManageRegistrations" });


        }

        [Authorize(Roles = "Staff")]
        [HttpPost("UnregisterCourse")]
        public async Task<IActionResult> UnregisterCourse(int enrollmentId, int childId)
        {
            try
            {
                var success1 = await _feeService.DeleteCourseFeeAsync(enrollmentId);
                var success2 = await _courseEnrollmentService.RemoveRegisteredEnrollmentAsync(enrollmentId);
                

                if (success1&&success2)
                {
                    TempData["SuccessMessage1"] = "Registration removed successfully.";
                }
                else
                {
                    TempData["ErrorMessage1"] = "Failed to remove registration.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage1"] = $"{ex.Message}";
            }

            //return RedirectToAction("ManageRegistrations", new { childId });
            return RedirectToAction("Participation", new { childId, tab = "ManageRegistrations" });
        }


        [Authorize(Roles = "Staff")]
        [HttpPost("RegisterActivity")]
        public async Task<IActionResult> RegisterActivity(int childId, int activityId, string paymentModel, decimal totalCost, string? description)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var success1 = false;
                var success2 = false;
                
                int newEnrollmentId = await _activityEnrollmentService.AddRegisteredEnrollmentAsync(childId, activityId, "Registered", user);

                success1 = await _feeService.AddActivityFeeAsync(newEnrollmentId, paymentModel, totalCost, description, user);


                success2 = await _activityEnrollmentService.UpdateActivityStatusToClosedAsync(activityId);
                if (!success1 || !success2)
                {
                    TempData["ErrorMessage2"] = "Failed to enroll in activity.";
                }
                else
                {
                    TempData["SuccessMessage2"] = "Successfully enrolled in activity.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage2"] = $"{ex.Message}";
            }

            //return RedirectToAction("ManageRegistrations", new { childId });
            return RedirectToAction("Participation", new { childId, tab = "ManageRegistrations" });
        }

        [Authorize(Roles = "Staff")]
        [HttpPost("UnregisterActivity")]
        public async Task<IActionResult> UnregisterActivity(int enrollmentId, int childId)
        {
            try
            {
                var success1 = await _feeService.DeleteActivityFeeAsync(enrollmentId);
                var success2 = await _activityEnrollmentService.RemoveRegisteredEnrollmentAsync(enrollmentId);

                if (success1 && success2)
                {
                    TempData["SuccessMessage2"] = "Registration removed successfully.";
                }
                else
                {
                    TempData["ErrorMessage2"] = "Failed to remove registration.";
                }

               
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage2"] = $"{ex.Message}";
            }

            //return RedirectToAction("ManageRegistrations", new { childId });
            return RedirectToAction("Participation", new { childId, tab = "ManageRegistrations" });
        }


        [Authorize(Roles = "Staff")]
        [HttpGet("ManagePayments/{childId}")]
        public async Task<IActionResult> ManagePayments(int childId)
        {
            // ✅ Pass ChildID to View
            //ViewBag.ChildID = childId;


            var payments = await _paymentService.GetByChildAsync(childId);
            var child = await _childService.GetAsync(childId);
            var unpaidDirectItems = await _paymentService.GetUnpaidDirectEnrollmentsByChildAsync(childId);
            var unpaidOAPItems = await _paymentService.GetUnpaidOAPEnrollmentsByChildAsync(childId);

            if (child == null)
            {
                TempData["ErrorMessage"] = "Child not found.";
                return RedirectToAction("List"); // Redirect to child list page if not found
            }
            ManagePaymentsViewModel payment = new ManagePaymentsViewModel
            {
                Payments = payments,
                Child = child
            };

            var parents = await _paymentService.GetParentsByChildAsync(childId);

            // ✅ Populate Parent dropdown
            ViewBag.ParentList = parents.Select(p => new SelectListItem
            {
                Value = p.ParentID.ToString(),
                Text = p.Name
            }).ToList();

            // ✅ Fetch all active payment packages
            var packages = await _paymentService.GetAllActivePackagesAsync();

            // ✅ Populate ViewBag for dropdown
            ViewBag.PaymentPackages = packages.Select(p => new SelectListItem
            {
                Value = p.PackageID.ToString(),
                Text = p.Title
            }).ToList();

            ViewBag.UnpaidDirectItems = unpaidDirectItems;
            ViewBag.UnpaidOAPItems = unpaidOAPItems;

            return View("ManagePayments", payment);
        }

        [Authorize(Roles = "Staff")]
        [HttpPost("AddPayment")]

        public async Task<IActionResult> AddPayment(int childId, int parentId, int? packageId, int? feeId, decimal amount, DateTime? paymentDate, IFormFile file)
        {

            try
            {
                string receiptPath = null;

                // ✅ Save the receipt file
                //if (file != null && file.Length > 0)
                //{
                //string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/receipts");
                //Directory.CreateDirectory(uploadsFolder);

                //string uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
                //string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                //using (var fileStream = new FileStream(filePath, FileMode.Create))
                //{
                //    await receiptFile.CopyToAsync(fileStream);
                //}

                //receiptPath = $"/receipts/{uniqueFileName}";
           // }

                string fileUrl = "";

                if (file != null)
                {
                    // Upload to R2
                    string fileName = String.Concat(childId, "-", DateTimeHelper.GetTorontoTime().ToString("yyyyMMdd-HHmmss"));
                    fileUrl = await _r2UploadService.UploadAsync(file, "payments", fileName);
                }
               

                Core.Models.User user = await _userManager.GetUserAsync(User);
                //if(packageId == null)
                //{
                //    packageId = 0; // Default to 0 if no package is selected
                //}

                var result = false;

                if (packageId != null)
                { 
                    var paymentId = await _paymentService.AddTokenPaymentAsync(childId, parentId, packageId, amount, paymentDate, fileUrl, user);
                    result = await _balanceService.AddPaymentToBalanceAsync(childId, paymentId, amount, user.Id);
                }

                if (feeId != null)
                {
                    var paymentId = await _paymentService.AddNoneTokenPaymentAsync(childId, parentId, feeId, amount, paymentDate, fileUrl, user);

                    if (paymentId > 0)
                        result = true;
                }
                if (result)
                {
                    TempData["SuccessMessage"] = "Payment info has been added successfully.";
                    //return RedirectToAction("ManagePayments");
                    //return RedirectToAction("ManagePayments", new { childId });
                    return RedirectToAction("Participation", new { childId, tab = "ManagePayments" });
                }
                else
                {
                    TempData["ErrorMessage"] = "Payment info was not added.";
                    //return RedirectToAction("ManagePayments", new { childId });
                    return RedirectToAction("Participation", new { childId, tab = "ManagePayments" });
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"{ex.Message}";
                //return RedirectToAction("ManagePayments", new { childId });
                return RedirectToAction("Participation", new { childId, tab = "ManagePayments" });
            }
        }

        [Authorize(Roles = "Staff")]
        [HttpPost("RemovePayment")]
        public async Task<IActionResult> RemovePayment(int paymentID, int childId)
        {
            try
            {
                Core.Models.User user = await _userManager.GetUserAsync(User);

                var payment = await _paymentService.GetByIdAsync(paymentID);
                if (payment.PaymentPackageID != null)
                {

                    var result1 = await _balanceService.RemovePaymentToBalanceAsync(childId, paymentID, user.Id);
                    var result2 = await _paymentService.RemoveAsync(paymentID);
                    if (result1 && result2)
                    {
                        TempData["SuccessMessage"] = "Payment info has been deleted.";
                        //return RedirectToAction("ManagePayments", new { childId });
                        return RedirectToAction("Participation", new { childId, tab = "ManagePayments" });
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Payment info is not deleted.";
                        //return RedirectToAction("ManagePayments", new { childId });
                        return RedirectToAction("Participation", new { childId, tab = "ManagePayments" });
                    }
                }

                else if (payment.FeeID != null)
                {
                    var feeId = payment.FeeID.Value;
                    var result1 = await _feeService.MarkFeeAsUnpaidAsync(feeId);
                    var result2 = await _paymentService.RemoveAsync(paymentID);
                    if (result1 && result2)
                    {
                        TempData["SuccessMessage"] = "Payment info has been deleted.";
                        //return RedirectToAction("ManagePayments", new { childId });
                        return RedirectToAction("Participation", new { childId, tab = "ManagePayments" });
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Payment info is not deleted.";
                        //return RedirectToAction("ManagePayments", new { childId });
                        return RedirectToAction("Participation", new { childId, tab = "ManagePayments" });
                    }
                }

                else
                {
                    return RedirectToAction("Participation", new { childId, tab = "ManagePayments" });
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"{ex.Message}";
                //return RedirectToAction("ManagePayments", new { childId });
                return RedirectToAction("Participation", new { childId, tab = "ManagePayments" });
            }


        }

        [Authorize(Roles = "Staff")]
        [HttpGet("EnrollmentsHistory/{childId}")]

        public async Task<IActionResult> EnrollmentsHistory(int childId)
        {

            var child = await _childService.GetAsync(childId);
            var completedCourses = await _courseEnrollmentService.GetFinishedEnrollmentsByChildAsync(childId);
            var completedActivities = await _activityEnrollmentService.GetFinishedEnrollmentsByChildAsync(childId);

            if (child == null)
                throw new Exception("The child can't be found");

            EnrollmentsHistoryViewModel enrollmentHistory = new EnrollmentsHistoryViewModel
            {
                Child = child,
                CompletedCourses = (List<CourseEnrollment>)completedCourses,
                CompletedActivities = (List<ActivityEnrollment>)completedActivities
            };

            return View("EnrollmentsHistory", enrollmentHistory);
        }


        [Authorize(Roles = "Child")]
        [HttpGet("MyConfirmations")]
        public async Task<IActionResult> MyConfirmations()
        {
            // Get the currently logged-in user
            Core.Models.User user = await _userManager.GetUserAsync(User);
            var child = await _childService.GetByIdAsync(user.Id);

            var scheduledCoursesToConfirm = await _courseEnrollmentService.GetScheduledSessionsToConfirmByChildAsync(child.ChildID);

            var courseSchedulesToConfirmList = new List<CourseSchedulesViewModel>();

            foreach (var group in scheduledCoursesToConfirm.GroupBy(e => e.Course))
            {
                var fee = await _feeService.GetByChildIdCourseIdAsync(child.ChildID, group.Key.CourseID);

                courseSchedulesToConfirmList.Add(new CourseSchedulesViewModel
                {
                    Course = group.Key,
                    CourseID = group.Key.CourseID,
                    Schedules = group.ToList(),
                    Fee = fee
                });
            }

            //private courses
            var privatecourses = await _courseEnrollmentService.GetPrivateEnrollmentsViewByChildAsync(child.ChildID, "Registered");

            var activities = await _activityEnrollmentService.GetEnrollmentsViewByChildAsync(child.ChildID, "Registered");


            var viewModel = new ChildSchedulesToConfirmViewModel
            {
                Child = child,
                ChildID = child.ChildID,
                PrivateCoursesToConfirm = privatecourses.ToList(),  // Private courses to confirm
                CoursesSchedulesToConfirm = courseSchedulesToConfirmList,  // Group course sessions to confirm
                ActivitiesToConfirm = activities.ToList()  // Activities to confirm
            };

            return View("MyConfirmations", viewModel);
        }


        [Authorize(Roles = "Child")]
        [HttpGet("MySchedules")]
        public async Task<IActionResult> MySchedules()
        {
            // Get the currently logged-in user
            Core.Models.User user = await _userManager.GetUserAsync(User);
            var child = await _childService.GetByIdAsync(user.Id);


            //var scheduledCourses = await _courseEnrollmentService.GetScheduledSessionsByChildAsync(child.ChildID);
            var scheduledCourses = await _courseEnrollmentService.GetUpcomingEnrollmentsByChildAsync(child.ChildID);

            var courseSchedulesList = scheduledCourses
                .GroupBy(e => e.Course)
                .Select(group => new CourseSchedulesViewModel
                {
                    Course = group.Key,
                    CourseID = group.Key.CourseID,
                    Schedules = group.ToList()
                }).ToList();


            var activityEnrollments = await _activityEnrollmentService.GetUpcomingEnrollmentsByChildAsync(child.ChildID);


            var viewModel = new ChildSchedulesViewModel
            {
                Child = child,
                ChildID = child.ChildID,
                CoursesSchedules = courseSchedulesList,
                ActivitySchedules = activityEnrollments
            };

            return View("MySchedules", viewModel);
        }





        [Authorize(Roles = "Child")]
        [HttpGet("MyEnrollmentsHistory")]
        public async Task<IActionResult> MyEnrollmentsHistory(string sortOrder)
        {
            ViewData["CurrentSort"] = sortOrder;

            ViewData["CourseSortParm"] = String.IsNullOrEmpty(sortOrder) ? "course_desc" : "course";
            ViewData["ScheduledAtSortParm"] = sortOrder == "scheduledAt" ? "scheduledAt_desc" : "scheduledAt";


            Core.Models.User user = await _userManager.GetUserAsync(User);
            var child = await _childService.GetByIdAsync(user.Id);


            var completedCourses = await _courseEnrollmentService.GetFinishedEnrollmentsByChildAsync(child.ChildID);
            var completedActivities = await _activityEnrollmentService.GetFinishedEnrollmentsByChildAsync(child.ChildID);

            switch (sortOrder)
            {
                case "course":
                    completedCourses = completedCourses.OrderBy(e => e.Course.Title);
                    break;

                case "course_desc":
                    completedCourses = completedCourses.OrderByDescending(e => e.Course.Title);
                    break;

                case "scheduledAt":
                    completedCourses = completedCourses.OrderBy(e => e.ScheduledAt);
                    break;

                case "scheduledAt_desc":
                    completedCourses = completedCourses.OrderByDescending(e => e.ScheduledAt);
                    break;

                default:
                    completedCourses = completedCourses.OrderBy(e => e.Course.Title);
                    break;
            }


            EnrollmentsHistoryViewModel enrollmentHistory = new EnrollmentsHistoryViewModel
            {
                Child = child,
                //CompletedCourses = (List<CourseEnrollment>)completedCourses,
                CompletedCourses = completedCourses.ToList(),
                CompletedActivities = (List<ActivityEnrollment>)completedActivities
            };

            return View("MyEnrollmentsHistory", enrollmentHistory);
        }



        [Authorize(Roles = "Child")]
        [HttpGet("MyBalance")]
        public async Task<IActionResult> MyBalance()
        {
            Core.Models.User user = await _userManager.GetUserAsync(User);
            var child = await _childService.GetByIdAsync(user.Id);
            var balances = await _balanceService.GetBalanceHistoryAsync(child.ChildID);
            var finalBalance = await _balanceService.GetFinalBalanceAsync(child.ChildID);
            ViewBag.FinalBalance = finalBalance;
            return View(balances);
        }


        [Authorize(Roles = "Child")]
        [HttpGet("MyPayments")]
        public async Task<IActionResult> MyPayments()
        {
            Core.Models.User user = await _userManager.GetUserAsync(User);
            var child = await _childService.GetByIdAsync(user.Id);
            //var balances = await _balanceService.GetBalanceHistoryAsync(child.ChildID);
            //var finalBalance = await _balanceService.GetFinalBalanceAsync(child.ChildID);

            var payments = await _paymentService.GetByChildAsync(child.ChildID);
            var paymentViewModel = new ManagePaymentsViewModel
            {
                Payments = payments,
                Child = child
            };
            
            return View(paymentViewModel);
        }





        [Authorize(Roles = "Staff")]
        [HttpGet("ManageBalance/{childId}")]
        public async Task<IActionResult> ManageBalance(int childId)
        {
            var balances = await _balanceService.GetBalanceHistoryAsync(childId);
            var finalBalance = await _balanceService.GetFinalBalanceAsync(childId);
            //ViewBag.FinalBalance = finalBalance;
            //return View(balances);
            var model = new ManageChildBalanceViewModel
            {
                ChildID = childId,
                CurrentBalance = finalBalance,
                BalanceHistory = balances
            };

            return View(model);

        }





        [Authorize(Roles = "Staff")]
        [HttpPost("FixBalance")]
        public async Task<IActionResult> FixBalance(int childId, string actionType, decimal amount, string remarks, IFormFile file)
        {

            //string calculationPath = null;

            // ✅ Save the receipt file
            //if (calculationFile != null && calculationFile.Length > 0)
            //{
            //    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/calculations");
            //    Directory.CreateDirectory(uploadsFolder);

            //    string uniqueFileName = $"{Guid.NewGuid()}_{calculationFile.FileName}";
            //    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            //    using (var fileStream = new FileStream(filePath, FileMode.Create))
            //    {
            //        await calculationFile.CopyToAsync(fileStream);
            //    }

            //    calculationPath = $"/calculations/{uniqueFileName}";
            //}

            string fileUrl = null;

            if (file != null)
            {
                // Upload to R2
                string fileName = String.Concat(childId, "-", DateTimeHelper.GetTorontoTime().ToString("yyyyMMdd-HHmmss"));
                fileUrl = await _r2UploadService.UploadAsync(file, "balance", fileName);
            }

            Core.Models.User user = await _userManager.GetUserAsync(User);
            //var userId = int.Parse(User.FindFirst("UserId").Value); // assuming you store UserId in claims

            var result = await _balanceService.AddBalanceFixAsync(
                childId,
                actionType,
                amount,
                remarks,
                fileUrl,
                user.Id
            );

            TempData["SuccessMessage"] = $"{actionType} of {amount:C} completed successfully.";

           // return RedirectToAction(nameof(ManageBalance), new { childId = model.ChildID });
            return RedirectToAction("Participation", new { childId, tab = "ManageBalance" });
        }




        //Add course sessions to a child who has registered to a group course 
        [Authorize(Roles = "Staff")]
        [HttpGet("ManageSessionRegistrations")]
        public async Task<IActionResult> ManageSessionRegistrations(int childId, int courseId)
        {

            ViewBag.ChildID = childId;
            ViewBag.CourseID = courseId;

            var sessionOptions = new List<SessionOption>();

            // Sessions available to register
            var sessions = await _courseEnrollmentService.GetOpenSessionsByCourseAsync(courseId);

            // Sessions the child already registered to
            //var registeredSessions = await _courseEnrollmentService.GetRegisteredByCourseChildAsync(courseId, childId);
            var allEnrolledSessions = await _courseEnrollmentService.GetEnrollmentsByCourseChildAsync(courseId, childId);
            var allEnrolledSessions2 = await _courseEnrollmentService.GetEnrollments2ByCourseChildAsync(courseId, childId);
            if (sessions != null)
            {
                foreach (var session in sessions)
                {

                    if (allEnrolledSessions.Any(e => e.ScheduledAt == session.ScheduledAt))
                    {
                        continue;
                    }

                    var sessionOption = new ManageSessionRegistrationsViewModel.SessionOption
                    {
                        EnrollmentID = session.EnrollmentID,
                        ScheduledAt = session.ScheduledAt ?? DateTime.MinValue,
                        ScheduledHours = session.ScheduledHours ?? 0,
                        Location = session.Location ?? string.Empty,
                        IsSelected = false
                    };
                    sessionOptions.Add(sessionOption);
                    
                }
            }


            //var allSessions = await _courseEnrollmentService.GetEnrollmentsByCourseChildAsync(courseId, childId);

            var all_sessions = allEnrolledSessions.Select(e => new SessionViewModel
            {
                EnrollmentID = e.EnrollmentID,
                ScheduledAt = e.ScheduledAt ?? DateTime.MinValue,
                ScheduledHours = e.ScheduledHours ?? 0,
                Status = e.Status,
                ParentNote = e.ParentNote,
                StaffNote = e.StaffNote
            }).ToList();

            //var scheduledSessions = await _courseEnrollmentService.GetSchedulesByCourseChildAsync(courseId, childId);

            //var scheduled_sessions = scheduledSessions.Select(e => new SessionViewModel
            //{
            //    EnrollmentID = e.EnrollmentID,
            //    ScheduledAt = e.ScheduledAt ?? DateTime.MinValue,
            //    ScheduledHours = e.ScheduledHours ?? 0,
            //    Status = e.Status,
            //    ParentNote = e.ParentNote,
            //    StaffNote = e.StaffNote
            //}).ToList();

            Child child = await _childService.GetAsync(childId);
            Course course = await _courseService.GetAsync(courseId);

            var viewModel = new ManageSessionRegistrationsViewModel
            {
                Child = child,
                Course = course,
                ChildID = childId,
                CourseID = courseId,
                AvailableSessions = sessionOptions,
                AllSessions = all_sessions,
                //ScheduledSessions = scheduled_sessions,
                CourseSessionsCount = (int)course.SessionCount,
                EnrolledSessionsCount = allEnrolledSessions2.Count()

            };

            return View(viewModel);
        }

        //Add course sessions to a child who has registered to a group course 
        [Authorize(Roles = "Staff")]
        [HttpPost("AddRegisteredSessions")]
        public async Task<IActionResult> AddRegisteredSessions(ManageSessionRegistrationsViewModel model)
        {
            try
            {
                Core.Models.User user = await _userManager.GetUserAsync(User);
                var selectedSessions = model.AvailableSessions.Where(s => s.IsSelected).ToList();


                if (selectedSessions.Count + model.EnrolledSessionsCount > model.CourseSessionsCount)
                {
                    TempData["ErrorMessage"] = "You can't register more than " + model.CourseSessionsCount + " sessions";
                    return RedirectToAction("ManageSessionRegistrations", new { childId = model.ChildID, courseId = model.CourseID });

                }


                foreach (var session in selectedSessions)
                {

                    var sessionRef = await _courseEnrollmentService.GetAsync(session.EnrollmentID);

                    if (sessionRef == null) continue;



                    var success = await _courseEnrollmentService.AddSessionRegisteredEnrollmentAsync(model.ChildID, model.CourseID, sessionRef.ScheduledAt, sessionRef.ScheduledHours, sessionRef.Location, sessionRef.EnrollmentID, "Registered", user);

                    if (!success)
                    {
                        TempData["ErrorMessage"] = "Failed to add session.";
                    }
                    else
                    {
                        TempData["SuccessMessage"] = "Session added successfully.";
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"{ex.Message}";
                return RedirectToAction("ManageSessionRegistrations", new { childId = model.ChildID, courseId = model.CourseID });
            }

            return RedirectToAction("ManageSessionRegistrations", new { childId = model.ChildID, courseId = model.CourseID });
        }


        [Authorize(Roles = "Staff")]
        [HttpPost("UpdateAllSessions")]
        public async Task<IActionResult> UpdateAllSessions(UpdateAllSessionsFormModel formModel)
        {
            Core.Models.User user = await _userManager.GetUserAsync(User);
            //var child = await _childService.GetByIdAsync(user.Id);

            var course = await _courseService.GetAsync(formModel.CourseID);
            var child = await _childService.GetAsync(formModel.ChildID);

           

                //if (child == null)
                //    return NotFound("Child not found.");

            if (formModel.AllSessions == null || !formModel.AllSessions.Any())
            {
                TempData["ErrorMessage"] = "No sessions submitted.";
                return RedirectToAction("ManageSessionRegistrations", new { courseId = formModel.CourseID, childId = formModel.ChildID });
            }

            bool hasConfirmed = true;

            foreach (var session in formModel.AllSessions)
            {
                if (session.Status == "Registered")
                {
                    hasConfirmed = false;
                    break;
                }
            }

            foreach (var session in formModel.AllSessions)
            {
                // Example pseudo-code for updating session in database
                //var existingSession = _dbContext.CourseEnrollments.FirstOrDefault(e => e.EnrollmentID == session.EnrollmentID);
                var existingSession = await _courseEnrollmentService.GetAsync(session.EnrollmentID);
                if (existingSession != null)
                {
                    existingSession.Status = session.Status;
                    existingSession.StaffNote = session.StaffNote;
                    var result = await _courseEnrollmentService.UpdateSessionAsync(existingSession);


                }

            }


            //_dbContext.SaveChanges();
            var subject = "";
            var htmlMessage = "";

            if (hasConfirmed)
            {
                subject = "Please Review Your Child’s Updated Course Schedule";

                htmlMessage =
            "<p>Hello,</p>" +
            $"<p>We’ve updated the course schedule for your child <strong>{child.Name}</strong> in " +
            $"<strong>\"{WebUtility.HtmlEncode(course.Title)}\"</strong>.</p>" +
            "<p>Please log in to your parent portal to review the changes:</p>" +
            "<p><a href=\"https://me.nsns.ca/Child/MySchedules\">https://me.nsns.ca/Child/MySchedules</a></p>" +
    
            "<p>If you have any questions or need assistance, please feel free to contact us.</p>" +
            "<p>Thank you,<br/>NSNS Support Team</p>";
            }
            else
            {
                subject = "Please Confirm Your Child’s Course";
                htmlMessage =
            "<p>Hello,</p>" +
            $"<p>We’ve added the course schedule for your child <strong>{child.Name}</strong> in " +
            $"<strong>\"{WebUtility.HtmlEncode(course.Title)}\"</strong>.</p>" +
            "<p>Please log in to your parent portal to confirm the course:</p>" +
            "<p><a href=\"https://me.nsns.ca/Child/MyConfirmations\">https://me.nsns.ca/Child/MyConfirmations</a></p>" +
            "<p>If you have any questions or need assistance, please feel free to contact us.</p>" +
            "<p>Thank you,<br/>NSNS Support Team</p>";
            }

                

            


            await _emailService.SendEmailAsync(child.User.Email, subject, htmlMessage);

            TempData["SuccessMessage"] = "Session updates saved successfully.";
            return RedirectToAction("ManageSessionRegistrations", new { childId = formModel.ChildID, courseId = formModel.CourseID });

            //try
            //{
            //    foreach (var session in model.AllSessions)
            //    {
            //        var enrollment = await _courseEnrollmentService.GetAsync(session.EnrollmentID);

            //        if (enrollment != null)
            //        {
            //            enrollment.Status = session.Status;
            //            enrollment.StaffNote = session.StaffNote;
            //            enrollment.UpdatedDate = DateTime.UtcNow;
            //            var result = await _courseEnrollmentService.UpdateSessionAsync(enrollment);
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    TempData["ErrorMessage"] = $"{ex.Message}";
            //    return RedirectToAction("ManageSessionRegistrations", new { childId = model.ChildID, courseId = model.CourseID });
            //}

            //return RedirectToAction("ManageSessionRegistrations", new { childId = model.ChildID, courseId = model.CourseID });
        }



        [Authorize(Roles = "Child")]
        [HttpPost("UpdateSchedules")]
        public async Task<IActionResult> UpdateSchedules(UpdateSchedulesFormModel model)
        {
            Core.Models.User user = await _userManager.GetUserAsync(User);
            var child = await _childService.GetByIdAsync(user.Id);

            if (child == null)
                return NotFound("Child not found.");


            //model?.S.Course.CourseType



            if (model?.Schedules != null && model.Schedules.Any())
            {
                try
                {


                    foreach (var schedule in model.Schedules)
                    {
                        var existing = await _courseEnrollmentService.GetAsync(schedule.EnrollmentID);
                        if (existing != null)
                        {
                            if (existing.Status != "Canceled" && existing.Status != "Deleted" && schedule.Status != null)  //schedule.Status is not null means Status dropdown list is enabled
                            {
                                existing.Status = schedule.Status;
                                existing.ParentNote = schedule.ParentNote;
                            }

                            else if (existing.Status != "Canceled" && existing.Status != "Deleted" && schedule.Status == null)  //schedule.Status is null means Status dropdown list is disabled
                            {
                                //existing.Status = schedule.Status;
                                existing.ParentNote = schedule.ParentNote;
                            }

                            await _courseEnrollmentService.UpdateSessionAsync(existing);
                        }
                    }



                    var course = await _courseService.GetAsync(model.CourseID);
                    var sendTo = "";
                    if (course != null && course.CourseType == "Group")
                    {
                        sendTo = "customer.nsns@gmail.com";
                    }

                    else if (course != null && course.CourseType == "Private")
                    {
                        sendTo = course.Coach.User.Email;
                    }


                    var subject = child.MemberID + ":" + " Course schedules change has been requested";

                    var message = "The course schedules change has been requested for the child: " + child.Name + ". Please review it ASAP.";

                    //await _emailService.SendEmailAsync("customer.nsns@gmail.com", subject, message);  //send to staff, how about send to coach?

                    TempData["SuccessMessage1"] = "Schedules updated successfully.";
                    TempData["CourseID"] = model.CourseID;
                }

                catch (Exception ex)
                {
                    TempData["ErrorMessage1"] = ex.Message;   //This seems to be strange
                    TempData["CourseID"] = model.CourseID;
                }

            }


            return RedirectToAction("MySchedules");
        }





        [Authorize(Roles = "Child")]
        [HttpPost("UpdateSchedulesToConform")]

        public async Task<IActionResult> UpdateSchedulesToConform(UpdateSchedulesToConfirmFormModel model, string actionType)
        {
            Core.Models.User user = await _userManager.GetUserAsync(User);
            var child = await _childService.GetByIdAsync(user.Id);

            if (child == null)
                return NotFound("Child not found.");

            

            //else if (actionType == "Confirm")
            if (actionType == "Confirm")
            {
                // Handle Confirm logic
                bool result1 = true;
                if (model.Fee.PaymentModel == "Token" && !model.Fee.IsPaid)
                { 
                     result1 = await _balanceService.DeductGroupCourseCostAsync(child.ChildID, model.CourseID, (decimal)model.Fee.TotalCost, user.Id);
                }

                //public async Task<IEnumerable<CourseEnrollment>> GetEnrollmentsByCourseChildAsync(int courseId, int childId)
                int? enrollmentId = await _courseEnrollmentService.GetEnrollmentIdByChildAndCourseAsync(model.CourseID, child.ChildID, "Registered");
                if (enrollmentId == null)
                    enrollmentId = await _courseEnrollmentService.GetEnrollmentIdByChildAndCourseAsync(model.CourseID, child.ChildID, "Confirmed");

                bool result2 = false;
                if (enrollmentId!=null)
                {
                    result2 = await _courseEnrollmentService.UpdateCourseEnrollmentStatusToConfirmedAsync(enrollmentId.Value);
                }
                
                bool result3 = true;


                if (model.Fee.PaymentModel == "Token" && !model.Fee.IsPaid)
                {
                    result3 = await _feeService.UpdateCourseIsPaidAsync((int)model.Fee.CourseEnrollmentID, user.Id);
                }

                if (result1 == true && result2 == true && result3 == true)
                {
                    if (model?.Schedules != null && model.Schedules.Any())
                    {
                        foreach (var schedule in model.Schedules)
                        {
                            var existing = await _courseEnrollmentService.GetAsync(schedule.EnrollmentID);
                            if (existing != null)
                            {
                                if (existing.Status == "Registered")
                                {
                                    existing.Status = "Scheduled";
                                }

                                await _courseEnrollmentService.UpdateSessionAsync(existing);
                            }
                        }



                        //var course = await _courseService.GetAsync(model.CourseID);
                        //var subject = child.MemberID + ":" + " Course schedules has been confirmed";
                        //var message = "The course schedules have been confirmed for the child: " + child.Name + ":\n" +
                        //             "Course: " + course.Title;

                        //await _emailService.SendEmailAsync("customer.nsns@gmail.com", subject, message);  //send to staff

                        TempData["SuccessMessage2"] = "The course schedules have been confirmed successfully. Please check your <a href=\"/Child/MySchedules\">Schedules</a>.";
                    }
                }
            }


            return RedirectToAction("MyConfirmations");
        }




        [Authorize(Roles = "Child")]
        [HttpPost("UpdateCourseToConform")]

        public async Task<IActionResult> UpdateCourseToConform(PrivateCourseEnrollmentViewModel model, string actionType)
        {
            Core.Models.User user = await _userManager.GetUserAsync(User);
            var child = await _childService.GetByIdAsync(user.Id);

            if (child == null)
                return NotFound("Child not found.");


            if (actionType == "Confirm")
            {
                // Handle Confirm logic

                //Upon confirmation, deduct the cost from child's balance
                //if(model.Fee.PaymentModel == "Token")

                bool result1 = true;

                if (model.PaymentModel == "Token")
                {
                   //result1 = await _balanceService.DeductCourseCostAsync(child.ChildID, model.CourseID, (decimal)model.TotalCost, user.Id);
                }
                bool result2 = await _courseEnrollmentService.UpdateCourseEnrollmentStatusToConfirmedAsync(model.EnrollmentID);

                bool result3 = true;


                if (model.PaymentModel == "Token")
                {
                    result3 = await _feeService.UpdateCourseIsPaidAsync(model.EnrollmentID, user.Id);
                }

                if (result1 && result2 && result3)
                {
                    // TempData["SuccessMessage3"] = "Activity schedules confirmed successfully. Please check the schedules in " + <a href=\"/Child/MySchedules\">Schedules</a>;
                    TempData["SuccessMessage2"] = "The course has been confirmed successfully. Once sessions have been scheduled by the coach, they can be viewed in <a href=\"/Child/MySchedules\">Schedules</a>.";

                    //var course = await _courseService.GetAsync(model.CourseID);
                    //var subject = child.MemberID + ":" + " Course has been confirmed";
                    //var message = "The course have been confirmed for the child: " + child.Name + ":\n" +
                    //                "Course: " + course.Title;

                    //await _emailService.SendEmailAsync("customer.nsns@gmail.com", subject, message);  //send to staff
                }




               


            }

            return RedirectToAction("MyConfirmations");
        }






        [Authorize(Roles = "Child")]
        [HttpPost("UpdateActivityToConform")]

        public async Task<IActionResult> UpdateActivityToConform(ActivityEnrollmentViewModel model, string actionType)
        {
            Core.Models.User user = await _userManager.GetUserAsync(User);
            var child = await _childService.GetByIdAsync(user.Id);

            if (child == null)
                return NotFound("Child not found.");

           
            if (actionType == "Confirm")
            {
                // Handle Confirm logic

                //Upon confirmation, deduct the cost from child's balance
                //if(model.Fee.PaymentModel == "Token")

                bool result1 = true;

                if (model.PaymentModel == "Token")
                { 
                    result1 = await _balanceService.DeductActivityCostAsync(child.ChildID, model.ActivityID, (decimal)model.TotalCost, user.Id);
                }
                bool result2 = await _activityEnrollmentService.UpdateActivityEnrollmentStatusToConfirmedAsync(model.EnrollmentID);

                bool result3 = true;


                if (model.PaymentModel == "Token")
                {
                    result3 = await _feeService.UpdateActivityIsPaidAsync(model.EnrollmentID, user.Id);
                }

                if (result1 && result2 && result3)
                {
                   // TempData["SuccessMessage3"] = "Activity schedules confirmed successfully. Please check the schedules in " + <a href=\"/Child/MySchedules\">Schedules</a>;
                    TempData["SuccessMessage3"] = "The activity has been confirmed successfully. Please check your <a href=\"/Child/MySchedules\">Schedules</a>.";

                    //var activity = await _activityService.GetAsync(model.ActivityID);
                    //var subject = child.MemberID + ":" + " Activity has been confirmed";
                    //var message = "The activity have been confirmed for the child: " + child.Name + ":\n" +
                    //                "Activity: " + activity.Title;

                    //await _emailService.SendEmailAsync("customer.nsns@gmail.com", subject, message);  //send to staff
                }

                    
            }

            return RedirectToAction("MyConfirmations");
        }

        [HttpGet("MyCalendar")]
        public async Task<IActionResult> MyCalendar()
        {
            return View();
        }

        [HttpGet("GetChildSchedules")]
        // API endpoint for AJAX (recommended)
        public async Task<IActionResult> GetChildSchedules()
        {
            var user = await _userManager.GetUserAsync(User);
            var child = await _childService.GetByIdAsync(user.Id);
            var schedules = await _calendarService.GetChildCalendar(child.ChildID);
           
            return Json(schedules);
        }


        //[Authorize(Roles = "Staff")]
        //[HttpPost]
        //public async Task<IActionResult> UpdateScheduledSessions(ManageSessionRegistrationsViewModel model)
        //{
        //    try
        //    {
        //        foreach (var session in model.AllSessions)
        //        {
        //            var enrollment = await _courseEnrollmentService.GetAsync(session.EnrollmentID);

        //            if (enrollment != null)
        //            {
        //                enrollment.Status = session.Status;
        //                enrollment.StaffNote = session.StaffNote;
        //                enrollment.UpdatedDate = DateTime.UtcNow;
        //                var result = await _courseEnrollmentService.UpdateSessionAsync(enrollment);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["ErrorMessage2"] = $"{ex.Message}";
        //        return RedirectToAction("ManageSessionRegistrations", new { childId = model.ChildID, courseId = model.CourseID });
        //    }

        //    return RedirectToAction("ManageSessionRegistrations", new { childId = model.ChildID, courseId = model.CourseID });
        //}


    }
}




