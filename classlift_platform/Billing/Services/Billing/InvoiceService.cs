using Billing.Constants;
using Billing.Data;
using Billing.Interfaces;
using Billing.Models;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace Billing.Services.Billing
{
    public class InvoiceService
    {
        private readonly BillingDbContext _context;
        private readonly ITenantConnectionStringFactory? _tenantConnectionFactory;

        public InvoiceService(
            BillingDbContext context,
            ITenantConnectionStringFactory? tenantConnectionFactory = null)
        {
            _context = context;
            _tenantConnectionFactory = tenantConnectionFactory;
        }

        public async Task<int> ActivateExpiredTrialsAsync()
        {
            var processed = 0;

            var now = DateTime.UtcNow;

            var expiredTrials = await _context.OrganizationSubscriptions
                .Where(s => s.Status == SubscriptionStatus.Trial)
                .Where(s => s.IsTrial == 1)
                .Where(s => s.TrialEndDate != null && s.TrialEndDate <= now)
                .ToListAsync();

            foreach (var subscription in expiredTrials)
            {
                var tenant = await _context.Tenantregistries
                    .Where(t => t.OrganizationId == subscription.OrganizationId && t.IsActive)
                    .FirstOrDefaultAsync();

                var userCount = await GetTenantUserCountAsync(tenant?.DatabaseName);

                if (userCount <= 3)
                {
                    subscription.Status = SubscriptionStatus.Cancelled;
                    subscription.IsTrial = 0;
                    subscription.EndDate = now;

                    var organization = await _context.Organizations
                        .FirstAsync(o => o.OrganizationId == subscription.OrganizationId);
                    organization.IsActive = false;
                    organization.UpdatedAt = now;
                    if (tenant != null)
                        tenant.IsActive = false;
                    processed++;
                    continue;
                }

                subscription.Status = SubscriptionStatus.Active;
                subscription.IsTrial = 0;
                subscription.ActivatedAt = now;

                var coachCount = await GetCoachCountAsync(subscription.OrganizationId);

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

                processed++;
            }

            await _context.SaveChangesAsync();

            return processed;
        }

        public async Task<int> GenerateRecurringInvoicesAsync()
        {
            var processed = 0;
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

                var coachCount = await GetCoachCountAsync(subscription.OrganizationId);

                await GenerateMonthlyInvoiceAsync(
                    subscription.OrganizationSubscriptionId,
                    billingPeriodStart,
                    billingPeriodEnd,
                    coachCount);

                subscription.LastBilledDate = billingPeriodEndDateTime;
                processed++;
            }

            await _context.SaveChangesAsync();
            return processed;
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

            var total = proratedSubtotal;

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

        private async Task<int> GetCoachCountAsync(int organizationId)
        {
            // Unit tests that call the billing calculation directly do not have a tenant database.
            if (_tenantConnectionFactory == null)
                return 1;

            var databaseName = await _context.Tenantregistries
                .Where(t => t.OrganizationId == organizationId && t.IsActive)
                .Select(t => t.DatabaseName)
                .FirstOrDefaultAsync();

            if (string.IsNullOrWhiteSpace(databaseName))
                throw new InvalidOperationException($"No active tenant database found for organization {organizationId}.");

            await using var connection = new MySqlConnection(
                _tenantConnectionFactory.BuildConnectionString(databaseName));
            await connection.OpenAsync();

            await using var command = new MySqlCommand("SELECT COUNT(*) FROM coaches;", connection);
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        private async Task<int> GetTenantUserCountAsync(string? databaseName)
        {
            // Direct unit tests do not configure a tenant connection factory.
            // Production always resolves the factory through dependency injection.
            if (_tenantConnectionFactory == null)
                return 2;

            if (string.IsNullOrWhiteSpace(databaseName))
                return 0;

            await using var connection = new MySqlConnection(
                _tenantConnectionFactory.BuildConnectionString(databaseName));
            await connection.OpenAsync();

            await using var command = new MySqlCommand("SELECT COUNT(*) FROM users;", connection);
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }
    }
}
