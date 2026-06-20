using Billing.Data;
using Billing.Services;
using Microsoft.EntityFrameworkCore;

namespace Billing.Jobs
{
    public class MonthlyBillingJob
    {
        private readonly BillingDbContext _context;
        private readonly InvoiceService _invoiceService;

        public MonthlyBillingJob(
            BillingDbContext context,
            InvoiceService invoiceService)
        {
            _context = context;
            _invoiceService = invoiceService;
        }

        public async Task RunAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            var billingPeriodStart = new DateOnly(today.Year, today.Month, 1);
            var billingPeriodEnd = billingPeriodStart.AddMonths(1).AddDays(-1);

            var activeSubscriptions = await _context.OrganizationSubscriptions
                .Where(s => s.Status == "Active")
                .Where(s => s.StartDate <= billingPeriodEnd.ToDateTime(TimeOnly.MaxValue))
                .Where(s => s.EndDate == null || s.EndDate >= billingPeriodStart.ToDateTime(TimeOnly.MinValue))
                .ToListAsync();

            foreach (var subscription in activeSubscriptions)
            {
                var alreadyExists = await _context.Invoices.AnyAsync(i =>
                    i.OrganizationSubscriptionId == subscription.OrganizationSubscriptionId &&
                    i.BillingPeriodStart == billingPeriodStart &&
                    i.BillingPeriodEnd == billingPeriodEnd);

                if (alreadyExists)
                    continue;

                // TODO: Replace this with real coach count from tenant database later.
                var coachCount = 1;

                await _invoiceService.GenerateMonthlyInvoiceAsync(
                    subscription.OrganizationSubscriptionId,
                    billingPeriodStart,
                    billingPeriodEnd,
                    coachCount
                );
            }
        }
    }
}
