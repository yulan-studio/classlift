
using Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Core.Interfaces;
using Core.Models;

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
        private readonly UserManager<Core.Models.User> _userManager;


        public StaffController(IStaffService staffService, UserManager<Core.Models.User> userManager)
        {
            _staffService = staffService;
            _userManager = userManager;
          
        }



        [Authorize(Roles = "Admin")]
        // POST: Add Staff Action
        [HttpPost("Add")]
        //[HttpPost]
        public async Task<IActionResult> Add(string name, string email, string password, string phone, string wechat)
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
        public async Task<IActionResult> Edit(int staffId, string name, string email, /*string password,*/ string phone, string wechat)
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
