using Billing.Data;
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

            return View(organization);
        }
    }
}