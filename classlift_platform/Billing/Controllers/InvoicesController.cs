using Billing.Data;
using Billing.Services.Billing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Billing.Controllers
{
    [Route("[controller]")]
    public class InvoicesController : Controller
    {
        private readonly BillingDbContext _context;
        private readonly PaymentService _paymentService;

        public InvoicesController(
                BillingDbContext context,
                PaymentService paymentService)
        {
            _context = context;
            _paymentService = paymentService;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var invoices = await _context.Invoices
                .Include(i => i.Organization)
                .Include(i => i.Plan)
                .Include(i => i.OrganizationSubscription)
                .OrderByDescending(i => i.GeneratedAt)
                .ToListAsync();

            return View(invoices);
        }

        [HttpGet("Details/{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            var invoice = await _context.Invoices
                .Include(i => i.Organization)
                .Include(i => i.Plan)
                .Include(i => i.OrganizationSubscription)
                .Include(i => i.Payments)
                .FirstOrDefaultAsync(i => i.InvoiceId == id);

            if (invoice == null)
                return NotFound();

            return View(invoice);
        }

        [HttpPost("MarkPaid/{id:int}")]
        public async Task<IActionResult> MarkPaid(int id)
        {
            var invoice = await _context.Invoices
                .FirstOrDefaultAsync(i => i.InvoiceId == id);

            if (invoice == null)
                return NotFound();

            await _paymentService.RecordPaymentAsync(
                invoice.InvoiceId,
                "Manual",
                $"MANUAL-{DateTime.Now:yyyyMMddHHmmss}",
                invoice.TotalAmount,
                "CAD",
                "Manual admin payment"
            );

            TempData["Success"] = "Payment recorded and invoice marked as paid.";

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost("Cancel/{id:int}")]
        public async Task<IActionResult> Cancel(int id)
        {
            var invoice = await _context.Invoices
                .FirstOrDefaultAsync(i => i.InvoiceId == id);

            if (invoice == null)
                return NotFound();

            invoice.InvoiceStatus = "Cancelled";

            await _context.SaveChangesAsync();

            TempData["Success"] = "Invoice cancelled.";

            return RedirectToAction(nameof(Details), new { id });
        }
    }
}