
using Core.Contexts;
using Core.Interfaces;
using Core.Models;
using Core.Repositories;
using Core.Services;
using Core.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using System.Diagnostics;



namespace Web.Controllers.Setting
{
    [Route("Specialty")]
    public class SpecialtyController : Controller
    {
        //private readonly AppDbContext _context;
        private ISpecialtyService _specialtyService;
        private ICoachSpecialtyService _coachSpecialtyService;
        private readonly UserManager<Core.Models.User> _userManager;
        public SpecialtyController(ISpecialtyService specialtyService, ICoachSpecialtyService coachSpecialtyService, UserManager<Core.Models.User> userManager)
        {
            _specialtyService = specialtyService;
            _coachSpecialtyService = coachSpecialtyService;
            _userManager = userManager;
        }

        // ✅ Load City List
        [HttpGet("List")]
        public async Task<IActionResult> List(string sortOrder)
        {

            ViewData["SpecialtyNameParm"] = sortOrder == "name" ? "name_desc" : "name";

     
            var specialties = await _specialtyService.GetAllAsync();

            List<SpecialtyWithDeleteViewModel> specialtiesWithDelete = new List<SpecialtyWithDeleteViewModel>();

            switch (sortOrder)
            {
                case "name_desc":
                    specialties = specialties.OrderByDescending(s => s.Title);
                    break;
                case "name":
                    specialties = specialties.OrderBy(s => s.Title);
                    break;
            }


            foreach (Specialty specialty in specialties)
            {
                SpecialtyWithDeleteViewModel specialtyWithDelete = new SpecialtyWithDeleteViewModel();
                specialtyWithDelete.Specialty = specialty;
                bool canDelete = !(await _coachSpecialtyService.GetCoachesBySpecialtyAsync(specialty.SpecialtyID)).Any();
                specialtyWithDelete.CanDelete = canDelete;
                specialtiesWithDelete.Add(specialtyWithDelete);
            }
            return View(specialtiesWithDelete); // Ensure there is a corresponding List.cshtml in Views/Staff

        }

        // ✅ Load Partial View for Add/Edit Form
        [HttpGet("Add")]
        public async Task<IActionResult> Add()
        {

            return PartialView("_Add", new Specialty { Title = string.Empty, Description = string.Empty });
        }


        [HttpGet("Edit/{specialtyId}")]
        public async Task<IActionResult> Edit(int specialtyId)
        {


            //var city = await _context.Cities.FindAsync(cityId);
            var specialty = await _specialtyService.GetAsync(specialtyId);
            if (specialty == null) return NotFound();

            return PartialView("_Edit", specialty);
        }





        // ✅ Save Specialty (Add / Edit)
        [HttpPost("Save")]
        public async Task<IActionResult> Save(Specialty specialty)
        {

            if (!ModelState.IsValid)
                return BadRequest("Invalid data");
            var user = await _userManager.GetUserAsync(User);
             
            try
            {
                var result = false;
                bool isNewSpecialty = false;
                if (specialty.SpecialtyID == 0)
                {
                    specialty.CreatedBy = user.Id;
                    result = await _specialtyService.AddAsync(specialty);
                }
                else
                {
                    specialty.UpdatedBy = user.Id;
                    result = await _specialtyService.UpdateAsync(specialty);
                }

                if (result)
                {
                    if (isNewSpecialty)
                        TempData["SuccessMessage"] = "The specialty has been added";
                    else
                        TempData["SuccessMessage"] = "The specialty has been updated";
                    return RedirectToAction("List");
                }
                else
                //return BadRequest("Something is wrong.");
                //return Json(new { success = false}); // ✅ Return error message
                {
                    TempData["ErrorMessage"] = "Something went wrong.";
                    return RedirectToAction("List");
                }


            }

            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("List");
            }


        }


        // ✅ Load Partial View for Add/Edit Form
        [HttpGet("DeleteConfirm/{specialtyId}")]
        public async Task<IActionResult> DeleteConfirm(int specialtyId)
        {
           
            var specialty = await _specialtyService.GetAsync(specialtyId);
            if (specialty == null) return NotFound();

            return PartialView("_DeleteConfirm", specialty);
        }

        // ✅ Delete Specialty
        [HttpPost("Delete/{specialtyId}")]
        public async Task<IActionResult> Delete(int specialtyId)
        {
            //var city = await _context.Cities.FindAsync(cityId);
            var result = await _specialtyService.DeleteAsync(specialtyId);
            //if (city == null) return NotFound();

            //_context.Cities.Remove(city);
            //await _context.SaveChangesAsync();
            if (result)
                return Json(new { success = true });
            else
                return Json(new { success = false });

        }
    }

}
