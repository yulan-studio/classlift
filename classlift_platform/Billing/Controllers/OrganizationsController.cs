using Billing.Data;
using Billing.Services;
using Billing.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Billing.Controllers
{
    [Route("[controller]")]
    public class OrganizationsController : Controller
    {
        private readonly BillingDbContext _context;
        private readonly TenantProvisioningService _tenantProvisioningService;

        public OrganizationsController(BillingDbContext context,
                                       TenantProvisioningService tenantProvisioningService)
        {
            _context = context;
            _tenantProvisioningService = tenantProvisioningService;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var organizations = await _context.Organizations
                .Include(o => o.CurrentPlan)
                .OrderBy(o => o.OrganizationName)
                .ToListAsync();

            return View(organizations);
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
                .Where(p => p.PaymentStatus == "Succeeded")
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
                    Text = $"{p.PlanName} - {p.PricePerCoach:C}/coach, min {p.MinimumMonthlyPrice:C}"
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
                    Text = $"{p.PlanName} - {p.PricePerCoach:C}/coach, min {p.MinimumMonthlyPrice:C}"
                }).ToList();

                return View(model);
            }

            try
            {
                var organization = await _tenantProvisioningService
                    .CreateOrganizationAsync(model);

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
                    Text = $"{p.PlanName} - {p.PricePerCoach:C}/coach, min {p.MinimumMonthlyPrice:C}"
                }).ToList();

                return View(model);
            }
        }



    }
}