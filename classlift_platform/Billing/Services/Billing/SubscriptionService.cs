using Billing.Data;
using Billing.Models;
using Microsoft.EntityFrameworkCore;

namespace Billing.Services.Billing
{
    public class SubscriptionService
    {
        private readonly BillingDbContext _context;

        public SubscriptionService(BillingDbContext context)
        {
            _context = context;
        }

        public async Task<OrganizationSubscription> ChangePlanAsync(
            int organizationId,
            int newPlanId,
            string changedBy = "admin",
            string? reason = null)
        {
            var organization = await _context.Organizations
                .FirstOrDefaultAsync(o => o.OrganizationId == organizationId);

            if (organization == null)
                throw new Exception("Organization not found.");

            var newPlan = await _context.Subscriptionplans
                .FirstOrDefaultAsync(p => p.PlanId == newPlanId);

            if (newPlan == null)
                throw new Exception("Plan not found.");

            var oldSubscription = await _context.OrganizationSubscriptions
                .Where(s => s.OrganizationId == organizationId && s.Status == "Active")
                .OrderByDescending(s => s.StartDate)
                .FirstOrDefaultAsync();

            if (oldSubscription != null && oldSubscription.PlanId == newPlanId)
                throw new Exception("Organization is already on this plan.");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                int? oldPlanId = null;
                string? oldStatus = null;

                if (oldSubscription != null)
                {
                    oldPlanId = oldSubscription.PlanId;
                    oldStatus = oldSubscription.Status;

                    oldSubscription.Status = "Cancelled";
                    oldSubscription.EndDate = DateTime.Now;
                }

                var newSubscription = new OrganizationSubscription
                {
                    OrganizationId = organizationId,
                    PlanId = newPlan.PlanId,
                    StartDate = DateTime.Now,
                    EndDate = null,
                    Status = "Active",
                    MonthlyPricePerCoach = newPlan.PricePerCoach,
                    MinimumMonthlyPrice = newPlan.MinimumMonthlyPrice
                };

                _context.OrganizationSubscriptions.Add(newSubscription);

                organization.CurrentPlanId = newPlan.PlanId;
                organization.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                var subscriptionEvent = new SubscriptionEvent
                {
                    OrganizationId = organizationId,
                    OrganizationSubscriptionId = newSubscription.OrganizationSubscriptionId,
                    EventType = oldSubscription == null ? "Created" : "PlanChanged",
                    OldPlanId = oldPlanId,
                    NewPlanId = newPlan.PlanId,
                    OldStatus = oldStatus,
                    NewStatus = "Active",
                    EffectiveAt = DateTime.Now,
                    CreatedAt = DateTime.Now,
                    CreatedBy = changedBy,
                    Reason = reason ?? "Plan changed from admin portal"
                };

                _context.SubscriptionEvents.Add(subscriptionEvent);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return newSubscription;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}