using Billing.Data;
using Billing.Interfaces;
using Billing.Models;
using Billing.Services.Notifications;
using Billing.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Billing.Services.Provisioning
{
    public class OrganizationSignupService : IOrganizationSignupService
    {
        private readonly BillingDbContext _context;
        private readonly TenantProvisioningService _tenantProvisioningService;
        private readonly ITenantConnectionStringFactory _connectionFactory;
        private readonly ITenantIdentitySeeder _tenantIdentitySeeder;
        private readonly EmailService _emailService;


        public OrganizationSignupService(
        TenantProvisioningService tenantProvisioningService,
        ITenantConnectionStringFactory connectionFactory,
        BillingDbContext context,
        ITenantIdentitySeeder tenantIdentitySeeder,
        EmailService emailService)
        {
            _tenantProvisioningService = tenantProvisioningService;
            _connectionFactory = connectionFactory;
            _context = context;
            _tenantIdentitySeeder = tenantIdentitySeeder;
            _emailService = emailService;
        }


        public async Task<OrganizationSignupResult> CreateOrganizationAsync(PublicSignupRequest request)
        {


            var model = new CreateOrganizationViewModel
            {
                OrganizationName = request.OrganizationName,
                Subdomain = request.Subdomain,
                ContactName = request.AdminName,
                ContactEmail = request.AdminEmail,
                PlanId = request.PlanId
            };

            // 1. Validate subdomain  2. Create Organization 3. Create TenantRegistry 4. Create tenant database  5. Run migrations
            var organization = await _tenantProvisioningService.CreateOrganizationAsync(
                model,
                createdBy: "public-signup");

            // 6. Create Admin user
            var tenant = await _context.Tenantregistries
                .FirstAsync(t => t.OrganizationId == organization.OrganizationId);

            var tenantConnectionString =
                _connectionFactory.BuildConnectionString(tenant.DatabaseName);

            await _tenantIdentitySeeder.SeedAdminAsync(
                tenantConnectionString,
                request.AdminName,
                request.AdminEmail,
                request.AdminPassword);

            // 7. Return tenant URL
            var tenantUrl = $"https://{request.Subdomain}.classlift.ca/Account/Login";

            await _emailService.SendWelcomeEmailAsync(
                request.AdminName,
                request.AdminEmail,
                request.OrganizationName,
                tenantUrl);

            return new OrganizationSignupResult
            {
                Success = true,
                Message = "Organization created successfully.",
                TenantUrl = tenantUrl
            };
        }


        //public async Task<OrganizationSignupResult> CreateOrganizationAsync(PublicSignupRequest request)
        //{
            

        //    return new OrganizationSignupResult
        //    {
        //        Success = true,
        //        TenantUrl = $"https://{request.Subdomain}.classlift.ca/Account/Login"
        //    };
        //}
    }
}
