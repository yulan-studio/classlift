using Billing.Data;
using Microsoft.EntityFrameworkCore;
using Billing.Constants;

namespace Billing.Services.Billing
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
                    i.InvoiceStatus == InvoiceStatus.Pending &&
                    i.DueDate < today)
                .ToListAsync();

            foreach (var invoice in overdueInvoices)
            {
                invoice.InvoiceStatus = InvoiceStatus.Overdue;
            }

            await _context.SaveChangesAsync();
        }
    }
}