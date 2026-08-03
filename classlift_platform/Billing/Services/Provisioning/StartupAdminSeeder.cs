using Billing.Configuration;
using Billing.Data;
using Billing.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MySqlConnector;
using System.Data.SqlClient;

namespace Billing.Services.Provisioning;

public sealed class StartupAdminSeeder
{
    private const string AdminRole = "Admin";
    private readonly UserManager<IdentityUser> _localUserManager;
    private readonly RoleManager<IdentityRole> _localRoleManager;
    private readonly BillingDbContext _billingDbContext;
    private readonly ITenantConnectionStringFactory _connectionStringFactory;
    private readonly ITenantIdentitySeeder _tenantIdentitySeeder;
    private readonly PlatformAdminOptions _platformAdminOptions;
    private readonly IConfiguration _configuration;
    private readonly ILogger<StartupAdminSeeder> _logger;

    public StartupAdminSeeder(
        UserManager<IdentityUser> localUserManager,
        RoleManager<IdentityRole> localRoleManager,
        BillingDbContext billingDbContext,
        ITenantConnectionStringFactory connectionStringFactory,
        ITenantIdentitySeeder tenantIdentitySeeder,
        IOptions<PlatformAdminOptions> platformAdminOptions,
        IConfiguration configuration,
        ILogger<StartupAdminSeeder> logger)
    {
        _localUserManager = localUserManager;
        _localRoleManager = localRoleManager;
        _billingDbContext = billingDbContext;
        _connectionStringFactory = connectionStringFactory;
        _tenantIdentitySeeder = tenantIdentitySeeder;
        _platformAdminOptions = platformAdminOptions.Value;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        await SeedPlatformAdminAsync();
        await SeedTenantAdminStaffAsync();
    }

    private async Task SeedPlatformAdminAsync()
    {
        

        EnsureComplete(
            "Platform Admin configuration",
            _platformAdminOptions.Email,
            _platformAdminOptions.Password);

        if (!await _localRoleManager.RoleExistsAsync(AdminRole))
        {
            EnsureSucceeded(await _localRoleManager.CreateAsync(new IdentityRole(AdminRole)));
        }

        var user = await _localUserManager.FindByEmailAsync(_platformAdminOptions.Email);
        if (user == null)
        {
            user = new IdentityUser
            {
                UserName = _platformAdminOptions.Email,
                Email = _platformAdminOptions.Email,
                EmailConfirmed = true
            };

            EnsureSucceeded(await _localUserManager.CreateAsync(user, _platformAdminOptions.Password));
        }

        if (!await _localUserManager.IsInRoleAsync(user, AdminRole))
        {
            EnsureSucceeded(await _localUserManager.AddToRoleAsync(user, AdminRole));
        }

        _logger.LogInformation("Local startup admin {Email} is ready.", _platformAdminOptions.Email);
    }

    private async Task SeedTenantAdminStaffAsync()
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
            _logger.LogInformation("Tenant startup account configuration is not set; tenant seeding is skipped.");
            return;
        }

        EnsureComplete("tenant startup admin configuration", adminEmail, adminPassword);
        EnsureComplete("tenant startup staff configuration", staffEmail, staffPassword);

        var databaseNames = await _billingDbContext.Tenantregistries
            .AsNoTracking()
            .Where(tenant => tenant.IsActive)
            .Select(tenant => tenant.DatabaseName)
            .ToListAsync();

        foreach (var databaseName in databaseNames)
        {
            var connectionString = _connectionStringFactory.BuildConnectionString(databaseName);



            try
            {
                await _tenantIdentitySeeder.SeedUserAsync(
                    connectionString,
                    adminEmail!,
                    adminPassword!,
                    "Admin");

                await _tenantIdentitySeeder.SeedUserAsync(
                    connectionString,
                    staffEmail!,
                    staffPassword!,
                    "Staff");

                _logger.LogInformation(
                    "Tenant startup admin {AdminEmail} and staff {StaffEmail} are ready in {DatabaseName}.",
                    adminEmail,
                    staffEmail,
                    databaseName);
            }
            

            catch (MySqlException ex) when (ex.ErrorCode == MySqlErrorCode.UnknownDatabase)
            {
                _logger.LogWarning(
                    "Tenant database {DatabaseName} does not exist; account seeding is skipped.",
                    databaseName);
            }
        }
    }

    private static void EnsureComplete(string source, params string?[] values)
    {
        if (values.Any(string.IsNullOrWhiteSpace))
        {
            throw new InvalidOperationException($"Both email and password must be provided in {source}.");
        }
    }

    private static void EnsureSucceeded(IdentityResult result)
    {
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                string.Join(", ", result.Errors.Select(error => error.Description)));
        }
    }
}
