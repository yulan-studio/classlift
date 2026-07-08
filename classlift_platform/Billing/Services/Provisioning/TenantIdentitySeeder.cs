
using Billing.Data;
using Billing.Interfaces;
using Billing.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;


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

            services.AddLogging();

            services.AddDbContext<BillingDbContext>(options =>
                options.UseMySql(
                    connectionString,
                    ServerVersion.AutoDetect(connectionString)));

            services.AddIdentity<User, IdentityRole<int>>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = false;
            })
            .AddEntityFrameworkStores<BillingDbContext>()
            .AddDefaultTokenProviders();

            using var provider = services.BuildServiceProvider();
            using var scope = provider.CreateScope();

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            string[] roles =
            {
                "Admin",
                "Staff",
                "Coach",
                "Parent",
                "Child"
            };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var roleResult = await roleManager.CreateAsync(new IdentityRole<int> { Name = role });

                    if (!roleResult.Succeeded)
                    {
                        throw new InvalidOperationException(
                            string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                    }
                }
            }

            var existingAdmin = await userManager.FindByEmailAsync(adminEmail);

            if (existingAdmin != null)
            {
                return;
            }

            var adminUser = new User
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                Role = "Admin"
            };

            var createResult = await userManager.CreateAsync(adminUser, adminPassword);

            if (!createResult.Succeeded)
            {
                throw new InvalidOperationException(
                    string.Join(", ", createResult.Errors.Select(e => e.Description)));
            }

            var addRoleResult = await userManager.AddToRoleAsync(adminUser, "Admin");

            if (!addRoleResult.Succeeded)
            {
                throw new InvalidOperationException(
                    string.Join(", ", addRoleResult.Errors.Select(e => e.Description)));
            }
        }
    }
}
