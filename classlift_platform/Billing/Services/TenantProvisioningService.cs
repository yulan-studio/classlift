using Billing.Data;
using Billing.Models;
using Billing.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Billing.Services
{
    public class TenantProvisioningService
    {
        private readonly BillingDbContext _context;
        private readonly IConfiguration _configuration;

        public TenantProvisioningService(
            BillingDbContext context,
            IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<Organization> CreateOrganizationAsync(
            CreateOrganizationViewModel model,
            string createdBy = "admin")
        {
            var plan = await _context.Subscriptionplans
                .FirstOrDefaultAsync(p => p.PlanId == model.PlanId && p.IsActive);

            if (plan == null)
                throw new Exception("Selected plan not found or inactive.");

            var exists = await _context.Organizations
                .AnyAsync(o => o.OrganizationName == model.OrganizationName);

            if (exists)
                throw new Exception("Organization name already exists.");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var organization = new Organization
                {
                    OrganizationName = model.OrganizationName,
                    ContactName = model.ContactName,
                    ContactEmail = model.ContactEmail,
                    ContactPhone = model.ContactPhone,
                    CurrentPlanId = plan.PlanId,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };

                _context.Organizations.Add(organization);
                await _context.SaveChangesAsync();

                var databaseName = GenerateDatabaseName(model.OrganizationName);

                var connectionString =
                    $"server=localhost;database={databaseName};user=root;password=YOUR_PASSWORD;";

                var tenant = new Tenantregistry
                {
                    OrganizationId = organization.OrganizationId,
                    DatabaseName = databaseName,
                    ConnectionString = connectionString,
                    Subdomain = model.Subdomain,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };

                _context.Tenantregistries.Add(tenant);

                var subscription = new OrganizationSubscription
                {
                    OrganizationId = organization.OrganizationId,
                    PlanId = plan.PlanId,
                    StartDate = DateTime.Now,
                    EndDate = null,
                    Status = "Active",
                    MonthlyPricePerCoach = plan.PricePerCoach,
                    MinimumMonthlyPrice = plan.MinimumMonthlyPrice,
                    CreatedAt = DateTime.Now
                };

                _context.OrganizationSubscriptions.Add(subscription);
                await _context.SaveChangesAsync();

                var subscriptionEvent = new SubscriptionEvent
                {
                    OrganizationId = organization.OrganizationId,
                    OrganizationSubscriptionId = subscription.OrganizationSubscriptionId,
                    EventType = "Created",
                    OldPlanId = null,
                    NewPlanId = plan.PlanId,
                    OldStatus = null,
                    NewStatus = "Active",
                    EffectiveAt = DateTime.Now,
                    CreatedAt = DateTime.Now,
                    CreatedBy = createdBy,
                    Reason = "Organization provisioned from admin portal"
                };

                _context.SubscriptionEvents.Add(subscriptionEvent);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return organization;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private static string GenerateDatabaseName(string organizationName)
        {
            var safeName = organizationName
                .ToLower()
                .Replace(" ", "_")
                .Replace("-", "_");

            return $"classlift_{safeName}";
        }
    }
}