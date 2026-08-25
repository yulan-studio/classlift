
using Core;
using Core.Contexts;
using Core.Interfaces;
using Core.Models;
using Core.Repositories;
using Core.Services;
using Core.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;



namespace Web.Controllers.Setting
{
    [Route("City")]
    public class CityController : Controller
    {
        //private readonly AppDbContext _context;
        private ICityService _cityService;
        private readonly IProvinceService _provinceService;
        private readonly UserManager<Core.Models.User> _userManager;
        public CityController(
            ICityService cityService,
            IProvinceService provinceService,
            UserManager<Core.Models.User> userManager)
        {
            _cityService = cityService;
            _provinceService = provinceService;
            _userManager = userManager;
        }

        // ✅ Load City List
        [HttpGet("List")]
        public async Task<IActionResult> List(string sortOrder)
        {
            ViewData["CityNameParm"] = sortOrder == "name" ? "name_desc" : "name";
            ViewData["ProvinceNameParm"] = sortOrder == "province" ? "province_desc" : "province";
            //var cities =await _cityService.GetAllAsync();
            //return View(cities);

            var allCities = await _cityService.GetAllAsync();
            var usedCities = (await _cityService.GetAllUsedAsync()).Select(c => c.CityID).ToHashSet(); // Get used city IDs as HashSet for fast lookup

            switch (sortOrder)
            {
                case "name_desc":
                    allCities = allCities.OrderByDescending(s => s.Name);
                    break;
                case "name":
                    allCities = allCities.OrderBy(s => s.Name);
                    break;
                case "province_desc":
                    allCities = allCities.OrderByDescending(s => s.Province != null ? s.Province.Name : string.Empty)
                        .ThenBy(s => s.Name);
                    break;
                case "province":
                    allCities = allCities.OrderBy(s => s.Province != null ? s.Province.Name : string.Empty)
                        .ThenBy(s => s.Name);
                    break;
            }

            var viewModel = new ManageCitiesViewModel
            {
                Cities = allCities,
                UsedCityIds = usedCities
            };

            return View(viewModel);
        }

        

        // ✅ Load Partial View for Add/Edit Form
        [HttpGet("Add")]
        public async Task<IActionResult> Add()
        {
            await PopulateProvincesAsync();
            return PartialView("_Add", new City{ Name = string.Empty });
        }

        [HttpGet("AddProvince")]
        public IActionResult AddProvince()
        {
            return PartialView("_AddProvince", new Province { Name = string.Empty });
        }


        [HttpGet("Edit/{cityId}")]
        public async Task<IActionResult> Edit(int cityId)
        {


            //var city = await _context.Cities.FindAsync(cityId);
            var city = await _cityService.GetAsync(cityId);
            if (city == null) return NotFound();

            await PopulateProvincesAsync(city.ProvinceID);
            return PartialView("_Edit", city);
        }





        // ✅ Save City (Add / Edit)
        [HttpPost("Save")]
        public async Task<IActionResult> Save(City city)
        {
            if (!city.ProvinceID.HasValue)
                ModelState.AddModelError(nameof(city.ProvinceID), "Please select a province.");

            if (!ModelState.IsValid)
            {
                await PopulateProvincesAsync(city.ProvinceID);
                return PartialView(city.CityID == 0 ? "_Add" : "_Edit", city);
            }
            //city.CreatedBy = 1;  //temparaly set it to 1
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();
            try
            {
                var result = false;
                bool isNewCity = false;

                if (city.CityID == 0)
                {
                    isNewCity = true;
                    city.CreatedBy = user.Id;
                    city.CreatedDate = DateTime.UtcNow;
                    result = await _cityService.AddAsync(city);
                }
                else
                {
                    city.UpdatedBy = user.Id;
                    city.UpdatedDate = DateTime.UtcNow;
                    result = await _cityService.UpdateAsync(city);
                }


                if (result)
                {
                    if (isNewCity)
                        TempData["SuccessMessage"] = "The city has been added";
                    else
                        TempData["SuccessMessage"] = "The city has been updated";
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
                //return Json(new { success = false, message = ex.Message }); // ✅ Return error message
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("List");
            }

            
        }

        [HttpPost("SaveProvince")]
        public async Task<IActionResult> SaveProvince(Province province)
        {
            if (!ModelState.IsValid)
                return PartialView("_AddProvince", province);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            try
            {
                province.CreatedBy = user.Id;
                province.CreatedDate = DateTime.UtcNow;
                if (await _provinceService.AddAsync(province))
                    TempData["SuccessMessage"] = "The province has been added.";
                else
                    TempData["ErrorMessage"] = "The province could not be added.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("List");
        }

        private async Task PopulateProvincesAsync(int? selectedProvinceId = null)
        {
            ViewBag.Provinces = new SelectList(
                await _provinceService.GetAllAsync(),
                nameof(Province.ProvinceID),
                nameof(Province.Name),
                selectedProvinceId);
        }


        // ✅ Load Partial View for Add/Edit Form
        [HttpGet("DeleteConfirm/{cityId}")]
        public async Task<IActionResult> DeleteConfirm(int cityId)
        {
            //if (cityId == 0) return PartialView("_DeleteConfirm", new City { Name = string.Empty });

            //var city = await _context.Cities.FindAsync(cityId);
            var city = await _cityService.GetAsync(cityId);
            if (city == null) return NotFound();

            return PartialView("_DeleteConfirm", city);
        }

        // ✅ Delete City
        [HttpPost("Delete/{cityId}")]
        public async Task<IActionResult> Delete(int cityId)
        {
            //var city = await _context.Cities.FindAsync(cityId);
            var result = await _cityService.DeleteAsync(cityId);
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
