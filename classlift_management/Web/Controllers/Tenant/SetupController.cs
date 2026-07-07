using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Core.Models;

namespace Web.Controllers.Tenant
{
    [Route("Setup")]
    public class TenantSetupController : Controller
    {
        private readonly UserManager<Core.Models.User> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly IConfiguration _configuration;

        public TenantSetupController(
            UserManager<Core.Models.User> userManager,
            RoleManager<IdentityRole<int>> roleManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        [HttpGet("CreateAdmin")]
        public async Task<IActionResult> CreateAdmin()
        {
            if (await AdminExists())
            {
                return NotFound();
                // or: return Forbid();
            }

            return View();
        }

        [HttpPost("CreateAdmin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAdmin(
            string setupToken,
            string email,
            string password)
        {
            if (await AdminExists())
            {
                return NotFound();
            }

            var expectedToken = _configuration["TenantSetup:Token"];

            if (string.IsNullOrWhiteSpace(expectedToken) || setupToken != expectedToken)
            {
                TempData["Error"] = "Invalid setup token.";
                return View();
            }

            string[] roles = { "Admin", "Staff", "Coach", "Parent", "Child" };

            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    var roleResult = await _roleManager.CreateAsync(
                        new IdentityRole<int> { Name = role });

                    if (!roleResult.Succeeded)
                    {
                        TempData["Error"] = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                        return View();
                    }
                }
            }

            var existingUser = await _userManager.FindByEmailAsync(email);

            if (existingUser != null)
            {
                TempData["Error"] = "User already exists.";
                return View();
            }

            var adminUser = new Core.Models.User
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                Role = "Admin"
            };

            var result = await _userManager.CreateAsync(adminUser, password);

            if (!result.Succeeded)
            {
                TempData["Error"] = string.Join(", ", result.Errors.Select(e => e.Description));
                return View();
            }

            await _userManager.AddToRoleAsync(adminUser, "Admin");

            TempData["Message"] = "Admin created successfully. You can now log in.";
            return RedirectToAction("Login", "Account");
        }

        private async Task<bool> AdminExists()
        {
            var admins = await _userManager.GetUsersInRoleAsync("Admin");
            return admins.Any();
        }
    }
}
