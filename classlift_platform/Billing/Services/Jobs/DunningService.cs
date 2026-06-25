using Billing.Data;
using Microsoft.EntityFrameworkCore;

namespace Billing.Services.Jobs
{
    public class DunningService
    {
        private readonly BillingDbContext _context;

        public DunningService(BillingDbContext context)
        {
            _context = context;
        }

        public async Task MarkOverdueInvoicesAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            var overdueInvoices = await _context.Invoices
                .Where(i =>
                    i.InvoiceStatus == "Pending" &&
                    i.DueDate < today)
                .ToListAsync();

            foreach (var invoice in overdueInvoices)
            {
                invoice.InvoiceStatus = "Overdue";
            }

            await _context.SaveChangesAsync();
        }
    }
}