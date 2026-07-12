using Billing.Data;
using Billing.Interfaces;
using Billing.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Billing.Services.Provisioning
{
    public class TenantIdentitySeeder : ITenantIdentitySeeder
    {
        public async Task SeedAdminAsync(
            string connectionString,
            string adminName,
            string adminEmail,
            string adminPassword)
        {
            var services = new ServiceCollection();

            services.AddLogging(logging =>
            {
                logging.AddConsole();
                logging.SetMinimumLevel(LogLevel.Information);
            });

            services.AddDbContext<BillingDbContext>(options =>
            {
                options.UseMySql(
                    connectionString,
                    ServerVersion.AutoDetect(connectionString));
            });

            services
                .AddIdentityCore<User>(options =>
                {
                    options.Password.RequiredLength = 6;
                    options.Password.RequireDigit = true;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;

                    options.User.RequireUniqueEmail = true;
                })
                .AddRoles<IdentityRole<int>>()
                .AddEntityFrameworkStores<BillingDbContext>()
                .AddDefaultTokenProviders();

            await using var provider = services.BuildServiceProvider();

            await using var scope = provider.CreateAsyncScope();

            var logger = scope.ServiceProvider
                .GetRequiredService<ILogger<TenantIdentitySeeder>>();

            logger.LogInformation(
                "Identity user type: {UserType}",
                typeof(User).AssemblyQualifiedName);

            logger.LogInformation(
                "DbContext base type: {BaseType}",
                typeof(BillingDbContext).BaseType);

            var roleManager = scope.ServiceProvider
                .GetRequiredService<RoleManager<IdentityRole<int>>>();

            var userManager = scope.ServiceProvider
                .GetRequiredService<UserManager<User>>();

            string[] roles =
            {
                "Admin",
                "Staff",
                "Coach",
                "Parent",
                "Child"
            };

            foreach (var roleName in roles)
            {
                if (await roleManager.RoleExistsAsync(roleName))
                {
                    continue;
                }

                var roleResult = await roleManager.CreateAsync(
                    new IdentityRole<int>
                    {
                        Name = roleName
                    });

                if (!roleResult.Succeeded)
                {
                    throw new InvalidOperationException(
                        $"Failed to create role '{roleName}': " +
                        string.Join(
                            ", ",
                            roleResult.Errors.Select(e => e.Description)));
                }
            }

            var existingAdmin =
                await userManager.FindByEmailAsync(adminEmail);

            if (existingAdmin is not null)
            {
                logger.LogInformation(
                    "Admin account already exists for {AdminEmail}",
                    adminEmail);

                return;
            }

            var adminUser = new User
            {
                UserName = adminName,
                Email = adminEmail,
                EmailConfirmed = true,
                Role = "Admin"
            };

            var createResult =
                await userManager.CreateAsync(adminUser, adminPassword);

            if (!createResult.Succeeded)
            {
                throw new InvalidOperationException(
                    "Failed to create admin account: " +
                    string.Join(
                        ", ",
                        createResult.Errors.Select(e => e.Description)));
            }

            var addRoleResult =
                await userManager.AddToRoleAsync(adminUser, "Admin");

            if (!addRoleResult.Succeeded)
            {
                // Avoid leaving a user without the expected role.
                await userManager.DeleteAsync(adminUser);

                throw new InvalidOperationException(
                    "Failed to assign the Admin role: " +
                    string.Join(
                        ", ",
                        addRoleResult.Errors.Select(e => e.Description)));
            }

            logger.LogInformation(
                "Created tenant admin {AdminEmail}",
                adminEmail);
        }
    }
}