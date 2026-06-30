using Billing.Data;
using Billing.Models;
using Billing.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Billing.Controllers
{
    [Route("[controller]")]
    public class PlansController : Controller
    {
        private readonly BillingDbContext _context;

        public PlansController(BillingDbContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var plans = await _context.Subscriptionplans
                .OrderBy(p => p.PlanName)
                .ToListAsync();

            return View(plans);
        }

        [HttpGet("Details/{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            var plan = await _context.Subscriptionplans
                .Include(p => p.Planfeatures)
                    .ThenInclude(pf => pf.Feature)
                .FirstOrDefaultAsync(p => p.PlanId == id);

            if (plan == null)
                return NotFound();

            return View(plan);
        }

        [HttpGet("Edit/{id:int}")]
        public async Task<IActionResult> Edit(int id)
        {
            var plan = await _context.Subscriptionplans
                .FirstOrDefaultAsync(p => p.PlanId == id);

            if (plan == null)
                return NotFound();

            return View(plan);
        }

        [HttpPost("Edit/{id:int}")]
        public async Task<IActionResult> Edit(int id, Subscriptionplan model)
        {
            var plan = await _context.Subscriptionplans
                .FirstOrDefaultAsync(p => p.PlanId == id);

            if (plan == null)
                return NotFound();

            plan.PlanName = model.PlanName;
            plan.Description = model.Description;
            plan.PricePerCoach = model.PricePerCoach;
            plan.MinimumMonthlyPrice = model.MinimumMonthlyPrice;
            plan.IsActive = model.IsActive;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Plan updated successfully.";

            return RedirectToAction(nameof(Details), new { id = plan.PlanId });
        }


        [HttpGet("ManageFeatures/{id:int}")]
        public async Task<IActionResult> ManageFeatures(int id)
        {
            var plan = await _context.Subscriptionplans
                .Include(p => p.Planfeatures)
                .FirstOrDefaultAsync(p => p.PlanId == id);

            if (plan == null)
                return NotFound();

            var allFeatures = await _context.Features
                .OrderBy(f => f.FeatureName)
                .ToListAsync();

            var selectedFeatureIds = plan.Planfeatures
                .Select(pf => pf.FeatureId)
                .ToHashSet();

            var model = new ManagePlanFeaturesViewModel
            {
                PlanId = plan.PlanId,
                PlanName = plan.PlanName,
                Features = allFeatures.Select(f => new FeatureCheckboxItem
                {
                    FeatureId = f.FeatureId,
                    FeatureKey = f.FeatureKey,
                    FeatureName = f.FeatureName,
                    IsSelected = selectedFeatureIds.Contains(f.FeatureId),
                    IsLocked = plan.Planfeatures
                                .FirstOrDefault(pf => pf.FeatureId == f.FeatureId)
                                ?.IsLocked ?? false
                }).ToList()
            };

            return View(model);
        }

        [HttpPost("ManageFeatures/{id:int}")]
        public async Task<IActionResult> ManageFeatures(int id, ManagePlanFeaturesViewModel model)
        {
            var plan = await _context.Subscriptionplans
                .Include(p => p.Planfeatures)
                .FirstOrDefaultAsync(p => p.PlanId == id);

            if (plan == null)
                return NotFound();

            var selectedFeatureIds = model.Features
                .Where(f => f.IsSelected)
                .Select(f => f.FeatureId)
                .ToHashSet();

            var existingPlanFeatures = plan.Planfeatures.ToList();

            foreach (var existing in existingPlanFeatures)
            {
                if (!selectedFeatureIds.Contains(existing.FeatureId))
                {
                    _context.Planfeatures.Remove(existing);
                }
            }

            var existingFeatureIds = existingPlanFeatures
                .Select(pf => pf.FeatureId)
                .ToHashSet();

            foreach (var featureId in selectedFeatureIds)
            {
                if (!existingFeatureIds.Contains(featureId))
                {
                    _context.Planfeatures.Add(new Billing.Models.Planfeature
                    {
                        PlanId = id,
                        FeatureId = featureId,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Plan features updated successfully.";

            return RedirectToAction(nameof(Details), new { id });
        }
    }
}