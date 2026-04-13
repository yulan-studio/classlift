
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
    [Route("Admin")]
    //[ApiController]
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;
        private readonly UserManager<Core.Models.User> _userManager;


        public AdminController(IAdminService adminService, UserManager<Core.Models.User> userManager)
        {
            _adminService = adminService;
            _userManager = userManager;
            
          
        }



       
        // POST: Add Staff Action
        [Authorize(Roles = "Admin")]
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

                var result = await _adminService.AddAsync( name, email, password, phone,  wechat, user);
                if (!result)
                {
                    ModelState.AddModelError(string.Empty, "Failed in adding the admin info.");
                    return View();
                }
                TempData["SuccessMessage"] = "The admin member has been added.";
                return RedirectToAction("List"); // Redirect to the admin list page


            }
            catch (Exception ex)
            {
                ModelState.AddModelError(String.Empty, $"{ex.Message}");
                return View();
            }
        }


        // GET: Add View
        [Authorize(Roles = "Admin")]
        [HttpGet("Add")]
        //[HttpGet]
        public async Task<IActionResult> Add()
        {
            return View();

        }

        // GET: Admin/Delete/{userId}
        [Authorize(Roles = "Admin")]
        [HttpGet("ConfirmDelete/{adminId}")]
        public async Task<IActionResult> ConfirmDelete(int adminId)
        {
            // Fetch the staff details from the database
            var admin = await _adminService.GetAsync(adminId);
            if (admin == null)
            {
                return NotFound();
            }

            // Pass the staff details to the Delete.cshtml view
            return View(admin);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("DeleteConfirmed")]
        public async Task<IActionResult> DeleteConfirmed(int adminId)
        {
            try
            {
                var result = await _adminService.RemoveAsync(adminId);

                if (!result)
                {
                    TempData["ErrorMessage"] = "The admin member could not be deleted.";
                    return RedirectToAction("List");
                }

                TempData["SuccessMessage"] = "Admin member has been deleted successfully.";
                return RedirectToAction("List"); // Redirect to the staff list page
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"{ex.Message}";
            }

            // If delete fails, reload the confirmation page
            var admin = await _adminService.RemoveAsync(adminId);
            return View(admin);
        }



        // GET: Add View
        [Authorize(Roles = "Admin")]
        [HttpGet("List")]
        //[HttpGet]
        public async Task<IActionResult> List()
        {

            var adminList = await _adminService.GetAllAsync();
            return View(adminList); // Ensure there is a corresponding List.cshtml in Views/Staff

        }


        // GET: Edit View
        [Authorize(Roles = "Admin")]
        [HttpGet("Edit/{adminId}")]
        //[HttpGet]
        public async Task<IActionResult> Edit(int adminId)
        {
            // Fetch the staff details from the database
            var admin = await _adminService.GetAsync(adminId);
            if (admin == null)
            {
                return NotFound();
            }

            // Pass the staff details to the Delete.cshtml view
            return View(admin);

        }

        [Authorize(Roles = "Admin")]
        [HttpPost("Edit/{adminId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int adminId, string name, string email, /*string password,*/ string phone, string wechat)
        {
           

            try
            {
                var user = await _userManager.GetUserAsync(User);
                var result = await _adminService.UpdateAsync(adminId, name, email, /*password,*/ phone, wechat, user);


                if (!result)
                {
                    ModelState.AddModelError(string.Empty, "Failed to update staff information.");
                    var admin = await _adminService.GetAsync(adminId);
                    return View(admin);
                }

                TempData["SuccessMessage"] = "Admin information updated successfully.";
                return RedirectToAction("List");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"{ex.Message}";
                var admin = await _adminService.GetAsync(adminId);
                return View(admin);
            }
        }
    }
}
