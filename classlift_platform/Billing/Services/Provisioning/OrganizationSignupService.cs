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
        private readonly IConfiguration _configuration;

        public OrganizationSignupService(
        TenantProvisioningService tenantProvisioningService,
        ITenantConnectionStringFactory connectionFactory,
        BillingDbContext context,
        ITenantIdentitySeeder tenantIdentitySeeder,
        EmailService emailService,
        ILogger<OrganizationSignupService> logger,
        IHttpContextAccessor httpContextAccessor,
        IConfiguration configuration)
        {
            _tenantProvisioningService = tenantProvisioningService;
            _connectionFactory = connectionFactory;
            _context = context;
            _tenantIdentitySeeder = tenantIdentitySeeder;
            _emailService = emailService;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }


        public async Task<OrganizationSignupResult> CreateOrganizationAsync(PublicSignupRequest request)
        {
            var organizationNameExists = await _context.Organizations
                .AnyAsync(o => o.OrganizationName == request.OrganizationName);

            if (organizationNameExists)
            {
                return new OrganizationSignupResult
                {
                    Success = false,
                    Message = "Organization name already exists."
                };
            }

            var subdomainExists = await _context.Tenantregistries
                .AnyAsync(t => t.Subdomain == request.Subdomain);

            if (subdomainExists)
            {
                return new OrganizationSignupResult
                {
                    Success = false,
                    Message = "Subdomain already exists."
                };
            }

            var model = new CreateOrganizationViewModel
            {
                OrganizationName = request.OrganizationName,
                Subdomain = request.Subdomain,
                ContactName = request.AdminName,
                ContactEmail = request.AdminEmail,
                PlanId = request.PlanId
            };

            // 1. Validate subdomain  2. Create Organization 3. Create TenantRegistry 4. Create tenant database  5. Run migrations  6. Create Subscription   7. Create SubscriptionEvent  
            var organization = await _tenantProvisioningService.CreateOrganizationAsync(
                model,
                createdBy: "public-signup");

            // 8. Create Admin user
            var tenant = await _context.Tenantregistries
                .FirstAsync(t => t.OrganizationId == organization.OrganizationId);

            var tenantConnectionString =
                _connectionFactory.BuildConnectionString(tenant.DatabaseName);

            await _tenantIdentitySeeder.SeedUserAsync(
                tenantConnectionString,
                request.AdminEmail,
                request.AdminPassword,
                "Admin",
                request.AdminName,
                addStaffRoleAndProfile: true);

            // Shared support accounts are provisioned once, together with this new
            // tenant. Application startup must not enumerate and connect to every
            // existing tenant database.
            await SeedSharedAccountsAsync(tenantConnectionString, tenant.DatabaseName);

            // 9. Return tenant URL

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

        private async Task SeedSharedAccountsAsync(
            string tenantConnectionString,
            string databaseName)
        {
            var adminEmail = _configuration["TenantAdmin:Email"];
            var adminPassword = _configuration["TenantAdmin:Password"];
            var staffEmail = _configuration["TenantStaff:Email"];
            var staffPassword = _configuration["TenantStaff:Password"];

            if (string.IsNullOrWhiteSpace(adminEmail) &&
                string.IsNullOrWhiteSpace(adminPassword) &&
                string.IsNullOrWhiteSpace(staffEmail) &&
                string.IsNullOrWhiteSpace(staffPassword))
            {
                _logger.LogInformation(
                    "Shared tenant account configuration is not set; account provisioning is skipped for {DatabaseName}.",
                    databaseName);
                return;
            }

            EnsureComplete("shared tenant admin configuration", adminEmail, adminPassword);
            EnsureComplete("shared tenant staff configuration", staffEmail, staffPassword);

            await _tenantIdentitySeeder.SeedUserAsync(
                tenantConnectionString,
                adminEmail!,
                adminPassword!,
                "Admin");

            await _tenantIdentitySeeder.SeedUserAsync(
                tenantConnectionString,
                staffEmail!,
                staffPassword!,
                "Staff");

            _logger.LogInformation(
                "Shared tenant admin {AdminEmail} and staff {StaffEmail} are ready in {DatabaseName}.",
                adminEmail,
                staffEmail,
                databaseName);
        }

        private static void EnsureComplete(string source, params string?[] values)
        {
            if (values.Any(string.IsNullOrWhiteSpace))
            {
                throw new InvalidOperationException(
                    $"Both email and password must be provided in {source}.");
            }
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
