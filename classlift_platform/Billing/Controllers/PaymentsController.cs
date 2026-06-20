using Billing.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Billing.Controllers
{
    [Route("[controller]")]
    public class PaymentsController : Controller
    {
        private readonly BillingDbContext _context;

        public PaymentsController(BillingDbContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var payments = await _context.Payments
                .Include(p => p.Invoice)
                    .ThenInclude(i => i.Organization)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();

            return View(payments);
        }

        [HttpGet("Details/{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            var payment = await _context.Payments
                .Include(p => p.Invoice)
                    .ThenInclude(i => i.Organization)
                .Include(p => p.Invoice)
                    .ThenInclude(i => i.Plan)
                .FirstOrDefaultAsync(p => p.PaymentId == id);

            if (payment == null)
                return NotFound();

            return View(payment);
        }
    }
}