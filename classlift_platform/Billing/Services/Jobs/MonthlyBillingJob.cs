using Billing.Constants;
using Billing.Data;
using Billing.Models;
using Billing.Services.Billing;
using Microsoft.EntityFrameworkCore;

namespace Billing.Services.Jobs
{
    public class MonthlyBillingJob
    {
        private readonly BillingDbContext _context;
        private readonly InvoiceService _invoiceService;
        private readonly DunningService _dunningService;

        public MonthlyBillingJob(
            BillingDbContext context,
            InvoiceService invoiceService,
            DunningService dunningService)
        {
            _context = context;
            _invoiceService = invoiceService;
            _dunningService = dunningService;
        }

        public async Task RunAsync()
        {
            await ActivateExpiredTrialsAsync();
            await GenerateMonthlyInvoicesAsync();
            await _dunningService.MarkOverdueInvoicesAsync();
        }

        private async Task ActivateExpiredTrialsAsync()
        {
            var now = DateTime.UtcNow;

            var expiredTrials = await _context.OrganizationSubscriptions
                .Where(s => s.Status == SubscriptionStatus.Trial)
                .Where(s => s.IsTrial == 1)
                .Where(s => s.TrialEndDate != null && s.TrialEndDate <= now)
                .ToListAsync();

            foreach (var subscription in expiredTrials)
            {
                subscription.Status = SubscriptionStatus.Active;
                subscription.IsTrial = 0;
                subscription.ActivatedAt = now;

                _context.SubscriptionEvents.Add(new SubscriptionEvent
                {
                    OrganizationId = subscription.OrganizationId,
                    OrganizationSubscriptionId = subscription.OrganizationSubscriptionId,

                    EventType = SubscriptionEventTypes.TrialEnded,

                    OldPlanId = subscription.PlanId,
                    NewPlanId = subscription.PlanId,

                    OldStatus = SubscriptionStatus.Trial,
                    NewStatus = SubscriptionStatus.Active,

                    EffectiveAt = now,
                    CreatedAt = now,

                    CreatedBy = "System",

                    Reason = "30-day trial completed"
                });

            }

            if (expiredTrials.Any())
            {
                await _context.SaveChangesAsync();
            }
        }

        private async Task GenerateMonthlyInvoicesAsync()
        {
            var today = DateTime.UtcNow.Date;

            var billingPeriodStart = new DateOnly(today.Year, today.Month, 1);
            var billingPeriodEnd = billingPeriodStart.AddMonths(1).AddDays(-1);

            var activeSubscriptions = await _context.OrganizationSubscriptions
                .Where(s => s.Status == SubscriptionStatus.Active)
                .Where(s => s.IsTrial== 0)
                .Where(s => s.StartDate <= billingPeriodEnd.ToDateTime(TimeOnly.MaxValue))
                .Where(s => s.EndDate == null ||
                            s.EndDate >= billingPeriodStart.ToDateTime(TimeOnly.MinValue))
                .ToListAsync();

            foreach (var subscription in activeSubscriptions)
            {
                var alreadyExists = await _context.Invoices.AnyAsync(i =>
                    i.OrganizationSubscriptionId == subscription.OrganizationSubscriptionId &&
                    i.BillingPeriodStart == billingPeriodStart &&
                    i.BillingPeriodEnd == billingPeriodEnd);

                if (alreadyExists)
                    continue;
                //Temparary set to 1
                var coachCount = 1;

                await _invoiceService.GenerateMonthlyInvoiceAsync(
                    subscription.OrganizationSubscriptionId,
                    billingPeriodStart,
                    billingPeriodEnd,
                    coachCount);
            }
        }
    }
}