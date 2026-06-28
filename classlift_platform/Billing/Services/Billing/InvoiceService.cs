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

        public async Task<Invoice> GenerateMonthlyInvoiceAsync(
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

            //Count how many days in the month
            var fullMonthStart = new DateOnly(
                    billingPeriodStart.Year,
                    billingPeriodStart.Month,
                    1);

            var fullMonthEnd = fullMonthStart
                .AddMonths(1)
                .AddDays(-1);

            var daysInMonth = fullMonthEnd.Day;

            var daysUsed =
                   billingPeriodEnd.DayNumber -
                   billingPeriodStart.DayNumber +
                   1;

            if (daysUsed <= 0)
                throw new Exception("Invalid billing period.");

            var monthlySubtotal =
                coachCount * subscription.MonthlyPricePerCoach;

            var monthlyTotal =
                Math.Max(monthlySubtotal, subscription.MinimumMonthlyPrice);

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
            await _context.SaveChangesAsync();

            return invoice;
        }
    }
}