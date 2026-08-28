
using Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Core.Interfaces;
using Core.Models;
using Core.Contexts;
using System.Diagnostics;
using Core.Repositories;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Core.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Core;
using Web.Filters;


namespace Web.Controllers.Payment
{
    [Route("PaymentPackage")]
    [RequiresFeature(FeatureCodes.CreditTracking)]
    public class PaymentPackageController: Controller
    {
        private readonly IPaymentPackageService _paymentPackageService;
        private readonly IPaymentService _paymentService;

        public PaymentPackageController(IPaymentPackageService paymentPackageService, IPaymentService paymentService)
        {
            _paymentPackageService = paymentPackageService;
            _paymentService = paymentService;
        }

        [HttpGet("List")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> List()
        {
            var packages = await _paymentPackageService.GetAllAsync();
            List<PackageWithDeleteViewModel> packagesWithDelete = new List<PackageWithDeleteViewModel>();

            foreach (var package in packages)
            {
                bool canDelete = !(await _paymentService.GetByPackageAsync(package.PackageID)).Any();
                packagesWithDelete.Add(new PackageWithDeleteViewModel { Package = package, CanDelete = canDelete});
            }
            return View(packagesWithDelete);
        }

        [HttpGet("AddEdit/{packageId?}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddEdit(int? packageId)
        {
            var package = packageId.HasValue ? await _paymentPackageService.GetByIdAsync(packageId.Value) : new PaymentPackage();
            return View("AddEdit", package);
        }

        [HttpPost("Save")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Save(PaymentPackage package)
        {
            if (!ModelState.IsValid) return View("AddEdit", package);
            try
            {
                if (package.PackageID == 0)
                {
                    var result = await _paymentPackageService.AddAsync(package);
                    if(result)
                        TempData["SuccessMessage"] = "Payment Package info has been added successfully.";
                    else
                        TempData["ErrorMessage"] = "Payment Package info is not being added.";
                }
                else
                {
                    var result = await _paymentPackageService.UpdateAsync(package);
                    if(result)
                        TempData["SuccessMessage"] = "Payment Package info has been updated successfully.";
                    else
                        TempData["ErrorMessage"] = "Payment Package info is not being updated.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("List");
        }

        [HttpGet("ConfirmDelete/{packageId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ConfirmDelete(int packageId)
        {
            var package = await _paymentPackageService.GetByIdAsync(packageId);
            return View("ConfirmDelete", package);
        }

        [HttpPost("DeleteConfirmed")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int packageId)
        {
            try
            {
                var result = await _paymentPackageService.RemoveAsync(packageId);
                if (result)
                {
                    TempData["SuccessMessage"] = "Payment Package info has been deleted successfully.";
                    return RedirectToAction("List");
                }
                else
                {
                    TempData["ErrorMessage"] = "Payment Package info can't be deleted.";
                    return RedirectToAction("List");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("List");
            }
        }


        [HttpGet("GetPackageAmount")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> GetPackageAmount(int packageId)
        {
            if (packageId <= 0)
                return BadRequest(new { success = false, message = "Invalid package ID" });

            var package = await _paymentPackageService.GetByIdAsync(packageId);
            if (package == null)
                return NotFound(new { success = false, message = "Payment package not found" });

            return Ok(new { success = true, amount = package.Amount });
        }
    }
}
