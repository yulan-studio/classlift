using Billing.Data;
using Billing.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Billing.Controllers
{
    public class DashboardController : Controller
    {
        private readonly BillingDbContext _context;

        public DashboardController(BillingDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var model = new BillingDashboardViewModel
            {
                TotalRevenue = await _context.Payments
                    .Where(p => p.PaymentStatus == "Succeeded")
                    .SumAsync(p => p.Amount),

                PendingAmount = await _context.Invoices
                    .Where(i => i.InvoiceStatus == "Pending")
                    .SumAsync(i => i.TotalAmount),

                OverdueAmount = await _context.Invoices
                    .Where(i => i.InvoiceStatus == "Overdue")
                    .SumAsync(i => i.TotalAmount),

                TotalOrganizations = await _context.Organizations.CountAsync(),

                ActiveSubscriptions = await _context.OrganizationSubscriptions
                    .CountAsync(s => s.Status == "Active"),

                PendingInvoices = await _context.Invoices
                    .CountAsync(i => i.InvoiceStatus == "Pending"),

                PaidInvoices = await _context.Invoices
                    .CountAsync(i => i.InvoiceStatus == "Paid"),

                OverdueInvoices = await _context.Invoices
                    .CountAsync(i => i.InvoiceStatus == "Overdue"),

                PlanCounts = await _context.OrganizationSubscriptions
                    .Include(s => s.Plan)
                    .Where(s => s.Status == "Active")
                    .GroupBy(s => s.Plan.PlanName)
                    .Select(g => new PlanCountItem
                    {
                        PlanName = g.Key,
                        Count = g.Count()
                    })
                    .ToListAsync()
            };

            return View(model);
        }
    }
}