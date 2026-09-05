using Billing.Controllers.Public;
using Billing.Data;
using Billing.Interfaces;
using Billing.Models;
using Billing.Services.Notifications;
using Billing.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Cryptography;
using System.Text;

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
                createdBy: "public-signup",
                tenantIsActive: false);

            // 8. Create Admin user
            var tenant = await _context.Tenantregistries
                .FirstAsync(t => t.OrganizationId == organization.OrganizationId);

            var tenantConnectionString =
                _connectionFactory.BuildConnectionString(tenant.DatabaseName);

            var verificationToken = WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(32));
            tenant.EmailVerificationTokenHash = HashToken(verificationToken);
            tenant.EmailVerificationExpiresAt = DateTime.UtcNow.AddHours(24);
            await _context.SaveChangesAsync();

            await _tenantIdentitySeeder.SeedUserAsync(
                tenantConnectionString,
                request.AdminEmail,
                request.AdminPassword,
                "Admin",
                request.AdminName,
                addStaffRoleAndProfile: true,
                emailConfirmed: false);

            // Shared support accounts are provisioned once, together with this new
            // tenant. Application startup must not enumerate and connect to every
            // existing tenant database.
            await SeedSharedAccountsAsync(tenantConnectionString, tenant.DatabaseName);

            // Verification links must use this application's configured public URL.
            // Never build them from the requester-controlled Host header.
            var verificationUrl = BuildVerificationUrl(verificationToken);

            await _emailService.SendSignupVerificationEmailAsync(
                request.AdminName,
                request.AdminEmail,
                request.OrganizationName,
                verificationUrl);
            return new OrganizationSignupResult
            {
                Success = true,
                Message = "Organization created. Check your email to activate it."
            };
        }

        public async Task<OrganizationSignupResult> ConfirmEmailAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return Failure("Verification link is invalid or expired.");

            var tenant = await _context.Tenantregistries
                .Include(item => item.Organization)
                .SingleOrDefaultAsync(item =>
                    item.EmailVerificationTokenHash == HashToken(token));

            if (tenant == null ||
                tenant.IsActive ||
                tenant.EmailVerificationExpiresAt <= DateTime.UtcNow)
                return Failure("Verification link is invalid or expired.");

            var connectionString = _connectionFactory.BuildConnectionString(tenant.DatabaseName);
            await _tenantIdentitySeeder.ConfirmEmailAsync(
                connectionString,
                tenant.Organization.ContactEmail!);

            tenant.IsActive = true;
            tenant.ActivatedAt = DateTime.UtcNow;
            tenant.EmailVerificationTokenHash = null;
            tenant.EmailVerificationExpiresAt = null;
            await _context.SaveChangesAsync();

            var requestHost = _httpContextAccessor.HttpContext?.Request.Host.Host;
            return new OrganizationSignupResult
            {
                Success = true,
                Message = "Email confirmed. Tenant activated.",
                TenantUrl = BuildTenantUrl(tenant.Subdomain!, requestHost)
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

        private static string HashToken(string token) =>
            Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));

        private string BuildVerificationUrl(string verificationToken)
        {
            var configuredBaseUrl = _configuration["Platform:PublicBaseUrl"]?.TrimEnd('/');
            if (!Uri.TryCreate(configuredBaseUrl, UriKind.Absolute, out var baseUri) ||
                (baseUri.Scheme != Uri.UriSchemeHttp && baseUri.Scheme != Uri.UriSchemeHttps))
            {
                throw new InvalidOperationException(
                    "Platform:PublicBaseUrl must be configured as an absolute HTTP or HTTPS URL.");
            }

            return $"{configuredBaseUrl}/api/public/signup/verify?token={Uri.EscapeDataString(verificationToken)}";
        }

        private static string BuildTenantUrl(string subdomain, string? requestHost)
        {
            var host = requestHost?.ToLowerInvariant() ?? string.Empty;
            var suffix = host.StartsWith("dev.") ? ".dev" :
                host.StartsWith("staging.") ? ".staging" : string.Empty;
            return $"https://{subdomain}{suffix}.classlift.ca/Account/Login";
        }

        private static OrganizationSignupResult Failure(string message) =>
            new() { Success = false, Message = message };


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
