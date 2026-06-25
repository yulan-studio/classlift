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

            if (subscription.Status != "Active")
                throw new Exception("Subscription is not active.");

            var existingInvoice = await _context.Invoices.AnyAsync(i =>
                i.OrganizationSubscriptionId == organizationSubscriptionId &&
                i.BillingPeriodStart == billingPeriodStart &&
                i.BillingPeriodEnd == billingPeriodEnd);

            if (existingInvoice)
                throw new Exception("Invoice already exists for this billing period.");

            var subtotal = coachCount * subscription.MonthlyPricePerCoach;
            var total = Math.Max(subtotal, subscription.MinimumMonthlyPrice);

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
                Subtotal = subtotal,
                DiscountAmount = 0,
                TotalAmount = total,

                InvoiceStatus = "Pending",
                GeneratedAt = DateTime.Now
            };

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            return invoice;
        }
    }
}