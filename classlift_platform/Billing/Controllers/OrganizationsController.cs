using Billing.Data;
using Billing.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Billing.Controllers
{
    [Route("[controller]")]
    public class OrganizationsController : Controller
    {
        private readonly BillingDbContext _context;

        public OrganizationsController(BillingDbContext context)
        {
            _context = context;
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
    }
}