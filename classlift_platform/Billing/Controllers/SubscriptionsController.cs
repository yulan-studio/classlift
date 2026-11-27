using Billing.Data;
using Billing.Services.Billing;
using Billing.Services.Integrations;
using Stripe;
using Billing.ViewModels;
using PlatformSubscriptionService = Billing.Services.Billing.SubscriptionService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Billing.Controllers
{
    [Route("[controller]")]
    public class SubscriptionsController : Controller
    {
        private readonly BillingDbContext _context;
        private readonly PlatformSubscriptionService _subscriptionService;
        private readonly StripeService _stripe;

        public SubscriptionsController(
            BillingDbContext context,
            PlatformSubscriptionService subscriptionService,
            StripeService stripe)
        {
            _context = context;
            _subscriptionService = subscriptionService;
            _stripe = stripe;
        }

        [HttpGet("ChangePlan/{organizationId:int}")]
        public async Task<IActionResult> ChangePlan(int organizationId)
        {
            var organization = await _context.Organizations
                .Include(o => o.CurrentPlan)
                .FirstOrDefaultAsync(o => o.OrganizationId == organizationId);

            if (organization == null)
                return NotFound();

            var plans = await _context.Subscriptionplans
                .Where(p => p.IsActive)
                .OrderBy(p => p.PricePerCoach)
                .ThenBy(p => p.PlanName)
                .ToListAsync();

            var model = new ChangePlanViewModel
            {
                OrganizationId = organization.OrganizationId,
                OrganizationName = organization.OrganizationName,
                CurrentPlanId = organization.CurrentPlanId,
                CurrentPlanName = organization.CurrentPlan?.PlanName,
                Plans = plans.Select(p => new SelectListItem
                {
                    Value = p.PlanId.ToString(),
                    Text = $"{p.PlanName} - {p.PricePerCoach:C}/coach"
                }).ToList()
            };

            return View(model);
        }

        [HttpPost("ChangePlan")]
        public async Task<IActionResult> ChangePlan(ChangePlanViewModel model)
        {
            if (model.NewPlanId <= 0)
            {
                ModelState.AddModelError(nameof(model.NewPlanId), "Please select a plan.");
            }

            if (!ModelState.IsValid)
            {
                var plans = await _context.Subscriptionplans
                    .Where(p => p.IsActive)
                    .OrderBy(p => p.PricePerCoach)
                    .ThenBy(p => p.PlanName)
                    .ToListAsync();

                model.Plans = plans.Select(p => new SelectListItem
                {
                    Value = p.PlanId.ToString(),
                    Text = $"{p.PlanName} - {p.PricePerCoach:C}/coach"
                }).ToList();

                return View(model);
            }

            await _subscriptionService.ChangePlanAsync(
                model.OrganizationId,
                model.NewPlanId,
                "admin",
                model.Reason
            );

            TempData["Success"] = "Subscription plan changed successfully.";

            return RedirectToAction("Details", "Organizations", new { id = model.OrganizationId });
        }

        [HttpPost("CheckoutPlan")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckoutPlan(ChangePlanViewModel model, CancellationToken cancellationToken)
        {
            try
            {
                var url = await _stripe.CreatePlanCheckoutSessionAsync(
                    model.OrganizationId, model.NewPlanId, cancellationToken);
                return Redirect(url);
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(ChangePlan), new { organizationId = model.OrganizationId });
            }
            catch (StripeException)
            {
                TempData["Error"] = "Stripe could not create the subscription checkout. Please try again.";
                return RedirectToAction(nameof(ChangePlan), new { organizationId = model.OrganizationId });
            }
        }
    }
}
