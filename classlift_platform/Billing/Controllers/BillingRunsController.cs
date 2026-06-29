using Billing.Data;
using Billing.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Billing.Controllers
{
    public class BillingrunsController : Controller
    {
        private readonly BillingDbContext _context;

        public BillingrunsController(BillingDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var runs = await _context.BillingRuns
                .OrderByDescending(r => r.StartedAt)
                .Take(100)
                .ToListAsync();

            return View(runs);
        }
    }
}
