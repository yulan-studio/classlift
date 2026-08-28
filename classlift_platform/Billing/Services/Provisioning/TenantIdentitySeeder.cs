
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
        public async Task SeedUserAsync(
        string connectionString,
        string email,
        string password,
        string role,
        string? name = null,
        bool addStaffRoleAndProfile = false)
        {
            var services = new ServiceCollection();

            services.AddLogging();

            services.AddDbContext<ManagementDBContext>(options =>
                options.UseMySql(
                connectionString,
                ServerVersion.AutoDetect(connectionString)));

            services
                .AddIdentityCore<User>(options =>
                {
                    options.Password.RequiredLength = 6;
                    options.Password.RequireDigit = true;
                    options.Password.RequireUppercase = false;
                })
                .AddRoles<IdentityRole<int>>()
                .AddEntityFrameworkStores<ManagementDBContext>();
                //.AddDefaultTokenProviders();

            using var provider = services.BuildServiceProvider();
            using var scope = provider.CreateScope();

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            var dbContext = scope.ServiceProvider.GetRequiredService<ManagementDBContext>();

            string[] roles =
            {
                "Admin",
                "Staff",
                "Coach",
                "Parent",
                "Child"
            };

            if (!roles.Contains(role, StringComparer.OrdinalIgnoreCase))
            {
                throw new ArgumentException($"Unsupported tenant role '{role}'.", nameof(role));
            }

            foreach (var roleName in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    var roleResult = await roleManager.CreateAsync(new IdentityRole<int> { Name = roleName });

                    if (!roleResult.Succeeded)
                    {
                        throw new InvalidOperationException(
                            string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                    }
                }
            }

            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new User
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    Role = role
                };

                EnsureSucceeded(await userManager.CreateAsync(user, password));
            }
            else if (!string.Equals(user.Role, role, StringComparison.OrdinalIgnoreCase))
            {
                user.Role = role;
                EnsureSucceeded(await userManager.UpdateAsync(user));
            }

            if (!await userManager.IsInRoleAsync(user, role))
            {
                EnsureSucceeded(await userManager.AddToRoleAsync(user, role));
            }

            if (string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                var admin = await dbContext.Admins
                    .SingleOrDefaultAsync(existingAdmin => existingAdmin.UserId == user.Id);

                if (admin == null)
                {
                    dbContext.Admins.Add(new Admin
                    {
                        UserId = user.Id,
                        Name = string.IsNullOrWhiteSpace(name) ? email : name.Trim()
                    });
                }
                else if (!string.IsNullOrWhiteSpace(name) &&
                         !string.Equals(admin.Name, name.Trim(), StringComparison.Ordinal))
                {
                    admin.Name = name.Trim();
                }

                await dbContext.SaveChangesAsync();
            }

            if (addStaffRoleAndProfile)
            {
                if (!await userManager.IsInRoleAsync(user, "Staff"))
                {
                    EnsureSucceeded(await userManager.AddToRoleAsync(user, "Staff"));
                }

                var staff = await dbContext.Staff
                    .SingleOrDefaultAsync(existingStaff => existingStaff.UserId == user.Id);

                if (staff == null)
                {
                    dbContext.Staff.Add(new Staff
                    {
                        UserId = user.Id,
                        Name = string.IsNullOrWhiteSpace(name) ? email : name.Trim()
                    });
                }
                else if (!string.IsNullOrWhiteSpace(name) &&
                         !string.Equals(staff.Name, name.Trim(), StringComparison.Ordinal))
                {
                    staff.Name = name.Trim();
                }

                await dbContext.SaveChangesAsync();
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
}
