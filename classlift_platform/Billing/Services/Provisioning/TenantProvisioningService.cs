using Billing.Data;
using Billing.Interfaces;
using Billing.Models;
using Billing.ViewModels;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using System.Text.RegularExpressions;

namespace Billing.Services.Provisioning
{
    public class TenantProvisioningService
    {
        private readonly BillingDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IDatabaseProvisioner _databaseProvisioner;
        private readonly ITenantSchemaService _tenantSchemaService;
        private readonly ITenantSeedService _tenantSeedService;
        private readonly ITenantConnectionStringFactory _tenantConnectionFactory;

        public TenantProvisioningService(
            BillingDbContext context,
            IConfiguration configuration,
            IDatabaseProvisioner databaseProvisioner,
            ITenantSchemaService tenantSchemaService,
            ITenantSeedService tenantSeedService,
            ITenantConnectionStringFactory tenantConnectionFactory)
        {
            _context = context;
            _configuration = configuration;
            _databaseProvisioner = databaseProvisioner;
            _tenantSchemaService = tenantSchemaService;
            _tenantSeedService = tenantSeedService;
            _tenantConnectionFactory = tenantConnectionFactory;
        }


        //private async Task CreateTenantDatabaseAsync(string databaseName)
        //{
        //    var serverConnectionString =
        //        _configuration.GetConnectionString("TenantServerConnection");

        //    await using var connection = new MySqlConnector.MySqlConnection(serverConnectionString);
        //    await connection.OpenAsync();

        //    var safeDatabaseName = databaseName.Replace("`", "");

        //    await using var command = connection.CreateCommand();
        //    command.CommandText = $"CREATE DATABASE `{safeDatabaseName}` CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;";
        //    await command.ExecuteNonQueryAsync();
        //}

        //private string BuildServerConnectionString(string databaseName)
        //{
        //    var host = _configuration["TenantDatabase:Host"];
        //    var port = _configuration["TenantDatabase:Port"];

        //    var user = _configuration["TenantDatabase:User"];
        //    var password = _configuration["TenantDatabase:Password"];

        //    return $"Server={host};Port={port};Database={databaseName};User={user};Password={password};";
        //}


        public async Task<Organization> CreateOrganizationAsync(
            CreateOrganizationViewModel model,
            string createdBy = "admin")
        {

            // 1. Create Organization (billing database)
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
                    CreatedAt = DateTime.UtcNow
                };

                _context.Organizations.Add(organization);
                await _context.SaveChangesAsync();

                // 2. Generate database name

                var databaseName = GenerateDatabaseName(model.Subdomain);

                await _databaseProvisioner.CreateDatabaseAsync(databaseName);

                // 3. Build tenant connection string

                //var connectionString = BuildServerConnectionString(databaseName);
                var connectionString = _tenantConnectionFactory.BuildConnectionString(databaseName);


                await _tenantSchemaService.InitializeSchemaAsync(connectionString);

                await _tenantSeedService.SeedAsync(connectionString);


                // 4. Save TenantRegistry
                var tenant = new Tenantregistry
                {
                    OrganizationId = organization.OrganizationId,
                    DatabaseName = databaseName,
                    //ConnectionString = connectionString,
                    Subdomain = model.Subdomain,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Tenantregistries.Add(tenant);

                // 5. Create Subscription
                var subscription = new OrganizationSubscription
                {
                    OrganizationId = organization.OrganizationId,
                    PlanId = plan.PlanId,
                    StartDate = DateTime.UtcNow,
                    EndDate = null,
                    Status = "Trial",
                    IsTrial = 1,
                    TrialStartDate = DateTime.UtcNow,
                    TrialEndDate = DateTime.UtcNow.AddDays(30),
                    ActivatedAt = null,
                    CancelledAt = null,
                    AutoRenew = 1,
                    MonthlyPricePerCoach = plan.PricePerCoach,
                    MinimumMonthlyPrice = plan.MinimumMonthlyPrice,
                    CreatedAt = DateTime.UtcNow
                };

                _context.OrganizationSubscriptions.Add(subscription);
                await _context.SaveChangesAsync();

                // 6. Create SubscriptionEvent
                var subscriptionEvent = new SubscriptionEvent
                {
                    OrganizationId = organization.OrganizationId,
                    OrganizationSubscriptionId = subscription.OrganizationSubscriptionId,
                    EventType = "TrialStarted",
                    OldPlanId = null,
                    NewPlanId = plan.PlanId,
                    OldStatus = null,
                    NewStatus = "Trial",
                    EffectiveAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = createdBy,
                    Reason = "30-day free trial started"
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

        private static string GenerateDatabaseName(string subdomain)
        {
            
            var safe = subdomain.Trim().ToLower();

            safe = Regex.Replace(safe, @"[^a-z0-9]+", "_");
            safe = Regex.Replace(safe, @"_+", "_");
            safe = safe.Trim('_');

            return $"classlift_{safe}";
        }
    }
}