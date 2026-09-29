using Billing.Data;
using Billing.Services.Provisioning;
using Billing.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Billing.Constants;
using Billing.Services.Billing;

namespace Billing.Controllers
{
    [Route("[controller]")]
    public class OrganizationsController : Controller
    {
        private readonly BillingDbContext _context;
        private readonly TenantProvisioningService _tenantProvisioningService;
        private readonly OrganizationService _organizationService;

        public OrganizationsController(BillingDbContext context,
                                       TenantProvisioningService tenantProvisioningService,
                                       OrganizationService organizationService)
        {
            _context = context;
            _tenantProvisioningService = tenantProvisioningService;
            _organizationService = organizationService;
        }

        [HttpPost("Cancel/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            try
            {
                await _organizationService.CancelOrganizationAsync(id);
                TempData["Success"] = "Organization cancelled successfully.";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost("Delete/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var eligible = await _context.Organizations
                    .Where(o => o.OrganizationId == id && o.IsActive == false)
                    .Where(o => o.Tenantregistries.Any(t => !t.IsActive))
                    .Where(o => o.OrganizationSubscriptions.Any(s => s.Status == SubscriptionStatus.Cancelled))
                    .AnyAsync();

                if (!eligible)
                    throw new InvalidOperationException(
                        "Organization must be inactive, have an inactive tenant registry, and have a cancelled subscription before it can be deleted.");

                await _organizationService.DeleteOrganizationAsync(id);
                TempData["Success"] = "Organization deleted successfully.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost("BulkDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkDelete(List<int> organizationIds)
        {
            if (organizationIds.Count == 0)
            {
                TempData["Error"] = "Select at least one cancelled registration.";
                return RedirectToAction(nameof(Index));
            }

            var deleted = 0;
            try
            {
                foreach (var organizationId in organizationIds.Distinct())
                {
                    var eligible = await _context.Organizations
                        .Where(o => o.OrganizationId == organizationId && o.IsActive == false)
                        .Where(o => o.Tenantregistries.Any(t => !t.IsActive))
                        .Where(o => o.OrganizationSubscriptions.Any(s => s.Status == SubscriptionStatus.Cancelled))
                        .AnyAsync();

                    if (!eligible) continue;

                    await _organizationService.DeleteOrganizationAsync(organizationId);
                    deleted++;
                }

                TempData["Success"] = $"Deleted {deleted} cancelled registration(s).";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var organizations = await _context.Organizations
                .Include(o => o.CurrentPlan)
                .Include(o => o.OrganizationSubscriptions
                                .OrderByDescending(s => s.CreatedAt)
                                .Take(1))
                .OrderBy(o => o.OrganizationName)
                .ToListAsync();

            var cancelledRegistrations = await _context.Organizations
                .Include(o => o.OrganizationSubscriptions.OrderByDescending(s => s.CreatedAt).Take(1))
                .Include(o => o.Tenantregistries)
                .Where(o => o.IsActive == false)
                .Where(o => o.Tenantregistries.Any(t => !t.IsActive))
                .Where(o => o.OrganizationSubscriptions.Any(s => s.Status == SubscriptionStatus.Cancelled))
                .OrderBy(o => o.OrganizationName)
                .ToListAsync();

            return View(new OrganizationsIndexViewModel
            {
                Organizations = organizations,
                CancelledRegistrations = cancelledRegistrations
            });
        }

        [HttpGet("Details/{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            var organization = await _context.Organizations
                .Include(o => o.CurrentPlan)
                .FirstOrDefaultAsync(o => o.OrganizationId == id);

            if (organization == null)
                return NotFound();

            var subscriptions = await _context.OrganizationSubscriptions
                .Include(s => s.Plan)
                .Where(s => s.OrganizationId == id)
                .OrderByDescending(s => s.StartDate)
                .ToListAsync();

            var invoices = await _context.Invoices
                .Include(i => i.Plan)
                .Where(i => i.OrganizationId == id)
                .OrderByDescending(i => i.GeneratedAt)
                .ToListAsync();

            var payments = await _context.Payments
                .Include(p => p.Invoice)
                .Where(p => p.Invoice.OrganizationId == id)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();

            var tenant = await _context.Tenantregistries
                .FirstOrDefaultAsync(t => t.OrganizationId == id);

            var totalRevenue = payments
                .Where(p => p.PaymentStatus == PaymentStatus.Succeeded)
                .Sum(p => p.Amount);

            var model = new OrganizationDetailsViewModel
            {
                Organization = organization,
                Subscriptions = subscriptions,
                Invoices = invoices,
                Payments = payments,
                Tenant = tenant,
                TotalRevenue = totalRevenue
            };

            return View(model);
        }


        [HttpGet("Create")]
        public async Task<IActionResult> Create()
        {
            var plans = await _context.Subscriptionplans
                .Where(p => p.IsActive)
                .OrderBy(p => p.PlanName)
                .ToListAsync();

            var model = new CreateOrganizationViewModel
            {
                Plans = plans.Select(p => new SelectListItem
                {
                    Value = p.PlanId.ToString(),
                    Text = $"{p.PlanName} - {p.PricePerCoach:C}/coach"
                }).ToList()
            };

            return View(model);
        }



        [HttpPost("Create")]
        public async Task<IActionResult> Create(CreateOrganizationViewModel model)
        {
            if (model.PlanId <= 0)
                ModelState.AddModelError(nameof(model.PlanId), "Please select a plan.");

            if (!ModelState.IsValid)
            {
                var plans = await _context.Subscriptionplans
                    .Where(p => p.IsActive)
                    .OrderBy(p => p.PlanName)
                    .ToListAsync();

                model.Plans = plans.Select(p => new SelectListItem
                {
                    Value = p.PlanId.ToString(),
                    Text = $"{p.PlanName} - {p.PricePerCoach:C}/coach"
                }).ToList();

                return View(model);
            }

            try
            {
                var organization = await _tenantProvisioningService
                    .CreateOrganizationAsync(model);

                await _tenantProvisioningService.SeedSharedAccountsAsync(organization);

                TempData["Success"] = "Organization created successfully.";

                return RedirectToAction(nameof(Details), new { id = organization.OrganizationId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;

                var plans = await _context.Subscriptionplans
                    .Where(p => p.IsActive)
                    .OrderBy(p => p.PlanName)
                    .ToListAsync();

                model.Plans = plans.Select(p => new SelectListItem
                {
                    Value = p.PlanId.ToString(),
                    Text = $"{p.PlanName} - {p.PricePerCoach:C}/coach"
                }).ToList();

                return View(model);
            }
        }



    }
}
