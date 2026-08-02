using Billing.Configuration;
using Billing.Data;
using Billing.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

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
        await SeedTenantAdminsAsync();
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

    private async Task SeedTenantAdminsAsync()
    {

        //var email = builder.Configuration["TenantAdmin:Email"];
        var email = _configuration["TenantAdmin:Email"];
        var password = _configuration["TenantAdmin:Password"];

        //var password = _configuration["TenantAdmin:Password"];
        //if (string.IsNullOrWhiteSpace(password))
        //{
        //    password = Environment.GetEnvironmentVariable("TENANT_ADMIN_PASSWORD");
        //}

        if (string.IsNullOrWhiteSpace(email) &&
            string.IsNullOrWhiteSpace(password))
        {
            _logger.LogInformation("Tenant startup admin configuration is not set; tenant seeding is skipped.");
            return;
        }

        EnsureComplete("tenant startup admin configuration", email, password);

        var databaseNames = await _billingDbContext.Tenantregistries
            .AsNoTracking()
            .Where(tenant => tenant.IsActive)
            .Select(tenant => tenant.DatabaseName)
            .ToListAsync();

        foreach (var databaseName in databaseNames)
        {
            var connectionString = _connectionStringFactory.BuildConnectionString(databaseName);
            await _tenantIdentitySeeder.SeedAdminAsync(connectionString, email!, password!);
            _logger.LogInformation("Tenant startup admin {Email} is ready in {DatabaseName}.", email, databaseName);
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
