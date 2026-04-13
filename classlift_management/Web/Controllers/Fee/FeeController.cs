using Core.Interfaces;
using Core.Models;
using Core.Services;
using Core.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;

namespace Web.Controllers.Fee
{
    [Route("Fee")]
    public class FeeController : Controller
    {
        private readonly IFeeService _feeService;
        private readonly UserManager<Core.Models.User> _userManager;

        public FeeController(IFeeService feeService, UserManager<Core.Models.User> userManager)
        {
            _feeService = feeService;
            _userManager = userManager;
        }

        // GET: Fee/Edit/{courseEnrollmentId}
        
        [HttpGet("EditCourseFee/{courseEnrollmentId}")]
        public async Task<IActionResult> EditCourseFee(int courseEnrollmentId)
        {
            var fee = await _feeService.GetFeeForCourseEnrollmentAsync(courseEnrollmentId);

            if (fee == null)
                return NotFound();

            var feeModel = new FeeEditViewModel
            {
                ChildID = fee.CourseEnrollment.Child.ChildID,
                CourseEnrollmentID = fee.CourseEnrollmentID,
                FeeID = fee.FeeID,
                PaymentModel = fee.PaymentModel,
                Description = fee.Description,
                TotalCost = fee.TotalCost,
                //IsPaid = fee.IsPaid
            };

            //return View(model);
            return PartialView("_EditCourse", feeModel);
        }


        [HttpGet("EditActivityFee/{activityEnrollmentId}")]
        public async Task<IActionResult> EditActivityFee(int activityEnrollmentId)
        {
            var fee = await _feeService.GetFeeForActivityEnrollmentAsync(activityEnrollmentId);

            if (fee == null)
                return NotFound();

            var feeModel = new FeeEditViewModel
            {
                ChildID = fee.ActivityEnrollment.Child.ChildID,
                ActivityEnrollmentID = fee.ActivityEnrollmentID,
                FeeID = fee.FeeID,
                PaymentModel = fee.PaymentModel,
                Description = fee.Description,
                TotalCost = fee.TotalCost,
                //IsPaid = fee.IsPaid
            };

            //return View(model);
            return PartialView("_EditActivity", feeModel);
        }






        // POST: Fee/EditCourseFee
        [HttpPost("EditCourseFee/{courseEnrollmentId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCourseFee(FeeEditViewModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("_EditCourse", model);

            var user = await _userManager.GetUserAsync(User);

            if (model.CourseEnrollmentID != null)
            { // Ensure it's a course fee
                var fee = await _feeService.GetFeeForCourseEnrollmentAsync((int)model.CourseEnrollmentID);

                if (fee == null)
                    return NotFound();

                //int userId = 1; // TODO: replace with HttpContext.User info
                int userId = user.Id;

                var success = await _feeService.UpdateFeeAsync(fee, model.Description, model.TotalCost, userId);

                if (!success)
                {
                    ModelState.AddModelError("", "Cannot update a paid fee.");
                    return PartialView("_EditCourse", model);
                }

                //return RedirectToAction("Child", "ManageRegistrations", new { id = model. });
                //return RedirectToAction("ManageRegistrations", "Child", new { childId = model.ChildID });
                //return RedirectToAction("Participation", new { model.ChildID, tab = "ManageRegistrations" });
                return RedirectToAction("Participation", "Child", new { model.ChildID, tab = "ManageRegistrations" });

            }
            else
            {
                return BadRequest("Invalid fee type.");
            }
                
        }


        [HttpPost("EditActivityFee/{activityEnrollmentId}")]
        // POST: Fee/EditActivityFee
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditActivityFee(FeeEditViewModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("_EditActivity", model);

            var user = await _userManager.GetUserAsync(User);

            if (model.ActivityEnrollmentID != null)
            { // Ensure it's a course fee
                var fee = await _feeService.GetFeeForActivityEnrollmentAsync((int)model.ActivityEnrollmentID);

                if (fee == null)
                    return NotFound();

                //int userId = 1; // TODO: replace with HttpContext.User info
                int userId = user.Id;

                var success = await _feeService.UpdateFeeAsync(fee, model.Description, model.TotalCost, userId);

                if (!success)
                {
                    ModelState.AddModelError("", "Cannot update a paid fee.");
                    return PartialView("_EditActivity", model);
                }

                //return RedirectToAction("Child", "ManageRegistrations", new { id = model. });
                //return RedirectToAction("ManageRegistrations", "Child", new { childId = model.ChildID });
                //return RedirectToAction("Participation", new { model.ChildID, tab = "ManageRegistrations" });
                return RedirectToAction("Participation", "Child", new { model.ChildID, tab = "ManageRegistrations" });
            }
            else
            {
                return BadRequest("Invalid fee type.");
            }

        }
    }

}
