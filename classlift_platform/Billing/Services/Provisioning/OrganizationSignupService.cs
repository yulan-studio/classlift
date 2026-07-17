using Billing.Controllers.Public;
using Billing.Data;
using Billing.Interfaces;
using Billing.Models;
using Billing.Services.Notifications;
using Billing.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace Billing.Services.Provisioning
{
    public class OrganizationSignupService : IOrganizationSignupService
    {
        private readonly BillingDbContext _context;
        private readonly TenantProvisioningService _tenantProvisioningService;
        private readonly ITenantConnectionStringFactory _connectionFactory;
        private readonly ITenantIdentitySeeder _tenantIdentitySeeder;
        private readonly EmailService _emailService;
        private readonly ILogger<OrganizationSignupService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrganizationSignupService(
        TenantProvisioningService tenantProvisioningService,
        ITenantConnectionStringFactory connectionFactory,
        BillingDbContext context,
        ITenantIdentitySeeder tenantIdentitySeeder,
        EmailService emailService,
        ILogger<OrganizationSignupService> logger,
        IHttpContextAccessor httpContextAccessor)
        {
            _tenantProvisioningService = tenantProvisioningService;
            _connectionFactory = connectionFactory;
            _context = context;
            _tenantIdentitySeeder = tenantIdentitySeeder;
            _emailService = emailService;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
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

            //make TenantUrl to differenciate between dev, staging and production environment
            //If current domain is dev.classlift.ca, then TenantUrl is "https://{request.Subdomain}.dev.classlift.ca/Account/Login"
            //If current domain is staging.classlift.ca, then TenantUrl is "https://{request.Subdomain}.staging.classlift.ca/Account/Login"
            //If current domain is classlift.ca, then TenantUrl is "https://{request.Subdomain}.classlift.ca/Account/Login"
            var host = _httpContextAccessor.HttpContext?.Request.Host.Host?.ToLower() ?? "";

            string suffix = host switch
            {
                var h when h.StartsWith("dev.") => ".dev",
                var h when h.StartsWith("staging.") => ".staging",
                _ => ""
            };

            var tenantUrl =
                $"https://{request.Subdomain}{suffix}.classlift.ca/Account/Login";



            //try
            //{
            //    await _emailService.SendWelcomeEmailAsync(
            //    request.AdminName,
            //    request.AdminEmail,
            //    request.OrganizationName,
            //    tenantUrl);
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(ex, "Failed to send welcome email.");
            //}
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
