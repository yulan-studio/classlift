using Billing.Constants;
using Billing.Data;
using Billing.Models;
using Microsoft.EntityFrameworkCore;

namespace Billing.Services.Billing
{
    public class InvoiceService
    {
        private readonly BillingDbContext _context;

        public InvoiceService(BillingDbContext context)
        {
            _context = context;
        }

        public async Task ActivateExpiredTrialsAsync()
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

                var coachCount = 1;

                await GenerateProratedInvoiceAsync(
                    subscription.OrganizationSubscriptionId,
                    now,
                    coachCount);

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
                    Reason = "30-day free trial completed."
                });
            }

            await _context.SaveChangesAsync();
        }

        public async Task GenerateRecurringInvoicesAsync()
        {
            var today = DateTime.UtcNow.Date;

            var billingPeriodStart = new DateOnly(today.Year, today.Month, 1);
            var billingPeriodEnd = billingPeriodStart.AddMonths(1).AddDays(-1);

            var billingPeriodEndDateTime =
                billingPeriodEnd.ToDateTime(TimeOnly.MaxValue);

            var subscriptions = await _context.OrganizationSubscriptions
                .Where(s => s.Status == SubscriptionStatus.Active)
                .Where(s => s.IsTrial == 0)
                .Where(s => s.StartDate <= billingPeriodStart.ToDateTime(TimeOnly.MinValue))
                .Where(s => s.EndDate == null || s.EndDate >= billingPeriodEndDateTime)
                .Where(s => s.LastBilledDate == null || s.LastBilledDate < billingPeriodEndDateTime)
                .ToListAsync();

            foreach (var subscription in subscriptions)
            {
                var alreadyExists = await _context.Invoices.AnyAsync(i =>
                    i.OrganizationSubscriptionId == subscription.OrganizationSubscriptionId &&
                    i.BillingPeriodStart == billingPeriodStart &&
                    i.BillingPeriodEnd == billingPeriodEnd);

                if (alreadyExists)
                {
                    subscription.LastBilledDate = billingPeriodEndDateTime;
                    continue;
                }

                var coachCount = 1;

                await GenerateMonthlyInvoiceAsync(
                    subscription.OrganizationSubscriptionId,
                    billingPeriodStart,
                    billingPeriodEnd,
                    coachCount);

                subscription.LastBilledDate = billingPeriodEndDateTime;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<Invoice> GenerateMonthlyInvoiceAsync(
            int organizationSubscriptionId,
            DateOnly billingPeriodStart,
            DateOnly billingPeriodEnd,
            int coachCount)
        {
            return await GenerateInvoiceAsync(
                organizationSubscriptionId,
                billingPeriodStart,
                billingPeriodEnd,
                coachCount);
        }

        public async Task<Invoice> GenerateProratedInvoiceAsync(
            int organizationSubscriptionId,
            DateTime activationDate,
            int coachCount)
        {
            var billingPeriodStart = DateOnly.FromDateTime(activationDate);

            var billingPeriodEnd = new DateOnly(
                activationDate.Year,
                activationDate.Month,
                DateTime.DaysInMonth(activationDate.Year, activationDate.Month));

            return await GenerateInvoiceAsync(
                organizationSubscriptionId,
                billingPeriodStart,
                billingPeriodEnd,
                coachCount);
        }

        private async Task<Invoice> GenerateInvoiceAsync(
            int organizationSubscriptionId,
            DateOnly billingPeriodStart,
            DateOnly billingPeriodEnd,
            int coachCount)
        {
            var subscription = await _context.OrganizationSubscriptions
                .Include(s => s.Plan)
                .FirstOrDefaultAsync(s =>
                    s.OrganizationSubscriptionId == organizationSubscriptionId);

            if (subscription == null)
                throw new Exception("Subscription not found.");

            if (subscription.Status != SubscriptionStatus.Active)
                throw new Exception("Subscription is not active.");

            var existingInvoice = await _context.Invoices.AnyAsync(i =>
                i.OrganizationSubscriptionId == organizationSubscriptionId &&
                i.BillingPeriodStart == billingPeriodStart &&
                i.BillingPeriodEnd == billingPeriodEnd);

            if (existingInvoice)
                throw new Exception("Invoice already exists for this billing period.");

            var fullMonthStart = new DateOnly(
                billingPeriodStart.Year,
                billingPeriodStart.Month,
                1);

            var fullMonthEnd = fullMonthStart.AddMonths(1).AddDays(-1);
            var daysInMonth = fullMonthEnd.Day;

            var daysUsed =
                billingPeriodEnd.DayNumber -
                billingPeriodStart.DayNumber +
                1;

            if (daysUsed <= 0)
                throw new Exception("Invalid billing period.");

            var monthlySubtotal =
                coachCount * subscription.MonthlyPricePerCoach;

            var prorateRatio =
                (decimal)daysUsed / daysInMonth;

            var proratedSubtotal =
                Math.Round(monthlySubtotal * prorateRatio, 2);

            var proratedMinimum =
                Math.Round(subscription.MinimumMonthlyPrice * prorateRatio, 2);

            var total =
                Math.Max(proratedSubtotal, proratedMinimum);

            var invoice = new Invoice
            {
                OrganizationId = subscription.OrganizationId,
                OrganizationSubscriptionId = subscription.OrganizationSubscriptionId,
                PlanId = subscription.PlanId,

                BillingPeriodStart = billingPeriodStart,
                BillingPeriodEnd = billingPeriodEnd,
                DueDate = billingPeriodEnd.AddDays(15),

                CoachCount = coachCount,
                PricePerCoach = subscription.MonthlyPricePerCoach,
                Subtotal = proratedSubtotal,
                DiscountAmount = 0,
                TotalAmount = total,

                InvoiceStatus = InvoiceStatus.Pending,
                GeneratedAt = DateTime.UtcNow
            };

            _context.Invoices.Add(invoice);

            subscription.LastBilledDate =
                billingPeriodEnd.ToDateTime(TimeOnly.MaxValue);

            await _context.SaveChangesAsync();

            return invoice;
        }
    }
}