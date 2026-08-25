using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Core.Services;

using Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Core.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Core.Contexts;
using Microsoft.EntityFrameworkCore;
using Core.Models;
using Core.R2;

namespace Web.Controllers.Account
{
    [Route("Account")]
    public class AccountController : Controller
    {
        private readonly IUserRegistrationService _userRegistrationService;
        private readonly SignInManager<Core.Models.User> _signInManager;
        private readonly UserManager<Core.Models.User> _userManager;
        private readonly ITimeZoneService _timeZoneService;
        private readonly AppDbContext _dbContext;
        private readonly R2StorageService _storageService;
        private readonly CurrentTenant _currentTenant;
        private readonly OrganizationTerminologyService _terminologyService;
        private const string AdminResetPassword = "hello123!";
        private const long MaxLogoSize = 2 * 1024 * 1024;
        private const string DefaultHomePageUrl = "https://courses.roboturtle.ca/";

        public AccountController(IUserRegistrationService userRegistrationService, SignInManager<Core.Models.User> signInManager, UserManager<Core.Models.User> userManager, ITimeZoneService timeZoneService, AppDbContext dbContext, R2StorageService storageService, CurrentTenant currentTenant, OrganizationTerminologyService terminologyService)
        {
            _userRegistrationService = userRegistrationService;
            _signInManager = signInManager;
            _userManager = userManager;
            _timeZoneService = timeZoneService;
            _dbContext = dbContext;
            _storageService = storageService;
            _currentTenant = currentTenant;
            _terminologyService = terminologyService;
        }

        // Show Registration Form (GET)
        [HttpGet("Register")]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        // Handle Form Submission (POST)
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model); // Return form with validation errors
            var user = await _userManager.GetUserAsync(User);

            var result = await _userRegistrationService.RegisterUserAsync(model.Email, model.Password, model.Role, user);

            if (result)
                return RedirectToAction("Login", "Account"); // Redirect to login page

            ModelState.AddModelError("", "Registration failed. Please try again.");
            return View(model);
        }

        [HttpGet("Login")]
        public IActionResult Login()
        {
            
            ViewData["Title"] = "Login";
            return View(new LoginViewModel()); // Ensure a model instance is passed
        }


        // Handle login form submission
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {

                //if(user.Role == "Admin")
                //    return Redirect("/Dashboard/Admin"); // Redirect to the requested page
                if (user.Role == "Coach")
                    return Redirect("/Home/Index"); 
                  
                else if (user.Role == "Child")
                    return Redirect("/Home/Index");

                else if (user.Role == "Staff")
                    return Redirect("/Home/Index");
                
                else
                    return Redirect("/Home/Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }
        }


        // Logout action
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        [HttpGet("AccessDenied")]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [Authorize]
        [HttpGet("Settings")]
        public IActionResult Settings(string tab = "TimeZone")
        {
            ViewBag.ActiveTab = tab;
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("Branding")]
        public async Task<IActionResult> Branding()
        {
            ViewBag.HomePageUrl = await GetHomePageUrlAsync();
            ViewBag.Terminology = ToTerminologyViewModel(_currentTenant.Terminology);
            return PartialView("_Branding", new BrandingSettingsViewModel
            {
                CurrentLogoUrl = GetTenantLogoUrl()
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("Branding")]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(MaxLogoSize + 64 * 1024)]
        public async Task<IActionResult> Branding(BrandingSettingsViewModel model)
        {
            model.CurrentLogoUrl = GetTenantLogoUrl();
            ViewBag.HomePageUrl = await GetHomePageUrlAsync();
            ViewBag.Terminology = ToTerminologyViewModel(_currentTenant.Terminology);

            if (model.Logo == null || model.Logo.Length == 0)
            {
                ModelState.AddModelError(nameof(model.Logo), "Please choose a logo image.");
                return PartialView("_Branding", model);
            }

            if (model.Logo.Length > MaxLogoSize)
                ModelState.AddModelError(nameof(model.Logo), "The logo must be 2 MB or smaller.");

            var contentType = await DetectImageContentTypeAsync(model.Logo);
            if (contentType == null)
                ModelState.AddModelError(nameof(model.Logo), "Choose a valid PNG, JPEG, or WebP image.");

            if (!ModelState.IsValid)
                return PartialView("_Branding", model);

            try
            {
                var logoUrl = await _storageService.UploadToKeyAsync(
                    model.Logo,
                    GetTenantLogoKey(),
                    contentType!);

                model.CurrentLogoUrl = $"{logoUrl}?v={DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";
                model.Logo = null;
                ModelState.Clear();
                ViewBag.SuccessMessage = "Your logo has been updated.";
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "The logo could not be uploaded. Please try again.");
            }

            return PartialView("_Branding", model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("Terminology")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Terminology(OrganizationTerminologyViewModel model)
        {
            NormalizeTerminology(model);

            if (!ModelState.IsValid)
                return PartialView("_Terminology", model);

            try
            {
                var terminology = new OrganizationTerminology
                {
                    OrganizationType = model.OrganizationType,
                    ProviderSingular = model.ProviderSingular,
                    ProviderPlural = model.ProviderPlural,
                    ParticipantSingular = model.ParticipantSingular,
                    ParticipantPlural = model.ParticipantPlural,
                    ParticipantsRequireParentSupport = model.ParticipantsRequireParentSupport
                };

                await _terminologyService.SaveAsync(GetTenantDatabaseName(), terminology);
                _currentTenant.Terminology = terminology;
                ViewBag.SuccessMessage = "Organization settings have been updated.";
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "The terminology could not be saved. Please try again.");
            }

            return PartialView("_Terminology", model);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("HomePage")]
        public async Task<IActionResult> HomePage()
        {
            return PartialView("_HomePage", new HomePageSettingsViewModel
            {
                PageUrl = await GetHomePageUrlAsync()
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("HomePage")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HomePage(HomePageSettingsViewModel model)
        {
            model.PageUrl = model.PageUrl?.Trim() ?? string.Empty;

            if (!Uri.TryCreate(model.PageUrl, UriKind.Absolute, out var pageUri)
                || (pageUri.Scheme != Uri.UriSchemeHttps && pageUri.Scheme != Uri.UriSchemeHttp))
            {
                ModelState.AddModelError(nameof(model.PageUrl), "Enter a complete URL beginning with https:// or http://.");
            }

            if (!ModelState.IsValid)
                return PartialView("_HomePage", model);

            try
            {
                await _storageService.UploadTextAsync(GetTenantHomePageUrlKey(), model.PageUrl);
                ViewBag.SuccessMessage = "The Home page URL has been updated.";
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "The page URL could not be saved. Please try again.");
            }

            return PartialView("_HomePage", model);
        }

        private string GetTenantLogoUrl() =>
            _storageService.GetPublicUrl(GetTenantLogoKey());

        private string GetTenantLogoKey()
        {
            if (string.IsNullOrWhiteSpace(_currentTenant.DatabaseName))
                throw new InvalidOperationException("A tenant must be resolved before its branding can be changed.");

            return $"branding/{_currentTenant.DatabaseName}/logo";
        }

        private string GetTenantHomePageUrlKey()
        {
            if (string.IsNullOrWhiteSpace(_currentTenant.DatabaseName))
                throw new InvalidOperationException("A tenant must be resolved before its Home page can be changed.");

            return $"branding/{_currentTenant.DatabaseName}/home-page-url.txt";
        }

        private string GetTenantDatabaseName()
        {
            if (string.IsNullOrWhiteSpace(_currentTenant.DatabaseName))
                throw new InvalidOperationException("A tenant must be resolved before its terminology can be changed.");

            return _currentTenant.DatabaseName;
        }

        private static OrganizationTerminologyViewModel ToTerminologyViewModel(
            OrganizationTerminology terminology) => new()
        {
            OrganizationType = terminology.OrganizationType,
            ProviderSingular = terminology.ProviderSingular,
            ProviderPlural = terminology.ProviderPlural,
            ParticipantSingular = terminology.ParticipantSingular,
            ParticipantPlural = terminology.ParticipantPlural,
            ParticipantsRequireParentSupport = terminology.ParticipantsRequireParentSupport
        };

        private static void NormalizeTerminology(OrganizationTerminologyViewModel model)
        {
            model.OrganizationType = model.OrganizationType?.Trim() ?? string.Empty;
            model.ProviderSingular = model.ProviderSingular?.Trim() ?? string.Empty;
            model.ProviderPlural = model.ProviderPlural?.Trim() ?? string.Empty;
            model.ParticipantSingular = model.ParticipantSingular?.Trim() ?? string.Empty;
            model.ParticipantPlural = model.ParticipantPlural?.Trim() ?? string.Empty;
        }

        private async Task<string> GetHomePageUrlAsync()
        {
            var savedUrl = await _storageService.GetTextAsync(GetTenantHomePageUrlKey());
            return string.IsNullOrWhiteSpace(savedUrl)
                ? DefaultHomePageUrl
                : savedUrl.Trim();
        }

        private static async Task<string?> DetectImageContentTypeAsync(IFormFile file)
        {
            var header = new byte[12];
            await using var stream = file.OpenReadStream();
            var bytesRead = await stream.ReadAsync(header.AsMemory(0, header.Length));

            if (bytesRead >= 8 && header.AsSpan(0, 8).SequenceEqual(
                    new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }))
                return "image/png";

            if (bytesRead >= 3 && header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF)
                return "image/jpeg";

            if (bytesRead >= 12
                && header.AsSpan(0, 4).SequenceEqual("RIFF"u8)
                && header.AsSpan(8, 4).SequenceEqual("WEBP"u8))
                return "image/webp";

            return null;
        }

        [Authorize]
        [HttpGet("TimeZone")]
        public async Task<IActionResult> TimeZone(string? returnUrl = null)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();
            ViewBag.TimeZones = _timeZoneService.GetTimeZones();
            return PartialView("_TimeZone", new TimeZonePreferenceViewModel
            {
                TimeZoneId = user.TimeZoneId,
                ReturnUrl = returnUrl
            });
        }

        [Authorize]
        [HttpPost("TimeZone")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TimeZone(TimeZonePreferenceViewModel model)
        {
            if (!_timeZoneService.IsValidTimeZone(model.TimeZoneId))
                ModelState.AddModelError(nameof(model.TimeZoneId), "Please select a valid time zone.");
            ViewBag.TimeZones = _timeZoneService.GetTimeZones();
            if (!ModelState.IsValid) return PartialView("_TimeZone", model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();
            user.TimeZoneId = model.TimeZoneId;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return PartialView("_TimeZone", model);
            }

            await _signInManager.RefreshSignInAsync(user);
            ViewBag.SuccessMessage = "Your time zone has been updated.";
            return PartialView("_TimeZone", model);
        }

        [Authorize(Roles = "Admin,Child,Coach,Staff")]
        [HttpGet("ChangePassword")]
        public IActionResult ChangePassword()
        {
            return PartialView("_ChangePassword", new ChangePasswordViewModel());
        }

        [Authorize(Roles = "Admin,Child,Coach,Staff")]
        [HttpPost("ChangePassword")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("_ChangePassword", model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var result = await _userManager.ChangePasswordAsync(
                user,
                model.CurrentPassword,
                model.NewPassword);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return PartialView("_ChangePassword", model);
            }

            await _signInManager.RefreshSignInAsync(user);
            ModelState.Clear();
            ViewBag.SuccessMessage = "Your password has been changed.";
            return PartialView("_ChangePassword", new ChangePasswordViewModel());
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("ResetPassword")]
        public IActionResult ResetPassword()
        {
            return PartialView("_ResetPassword", new ResetPasswordViewModel());
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("FindResetPasswordUser")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FindResetPasswordUser(ResetPasswordViewModel model)
        {
            ModelState.Remove(nameof(model.TargetUserId));
            if (!ModelState.IsValid)
                return PartialView("_ResetPassword", model);

            var targetUser = await _userManager.FindByNameAsync(model.SearchUsername.Trim());
            if (targetUser == null || !CanAdminReset(targetUser.Role))
            {
                var participantTerm = _currentTenant.Terminology.ParticipantSingular;
                var providerTerm = _currentTenant.Terminology.ProviderSingular;
                ModelState.AddModelError(
                    nameof(model.SearchUsername),
                    $"No Admin, Staff, {participantTerm}, or {providerTerm} account was found with that exact username.");
                return PartialView("_ResetPassword", model);
            }

            await PopulateResetProfileAsync(model, targetUser);
            return PartialView("_ResetPassword", model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("ResetUserPassword")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetUserPassword(ResetPasswordViewModel model)
        {
            if (!model.TargetUserId.HasValue)
            {
                ModelState.AddModelError(string.Empty, "Please locate a user before resetting a password.");
                return PartialView("_ResetPassword", model);
            }

            var targetUser = await _userManager.FindByIdAsync(model.TargetUserId.Value.ToString());
            if (targetUser == null || !CanAdminReset(targetUser.Role))
                return NotFound();

            var adminUser = await _userManager.GetUserAsync(User);
            if (adminUser == null) return Challenge();

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(targetUser);
            targetUser.UpdatedBy = adminUser.Id;
            targetUser.UpdatedDate = DateTime.UtcNow;
            var result = await _userManager.ResetPasswordAsync(targetUser, resetToken, AdminResetPassword);

            await PopulateResetProfileAsync(model, targetUser);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return PartialView("_ResetPassword", model);
            }

            ViewBag.SuccessMessage = $"The password for {targetUser.UserName} was reset to {AdminResetPassword}";
            return PartialView("_ResetPassword", model);
        }

        private static bool CanAdminReset(string? role) =>
            role is not null
            && (role.Equals("Admin", StringComparison.OrdinalIgnoreCase)
                || role.Equals("Staff", StringComparison.OrdinalIgnoreCase)
                || role.Equals("Child", StringComparison.OrdinalIgnoreCase)
                || role.Equals("Coach", StringComparison.OrdinalIgnoreCase));

        private async Task PopulateResetProfileAsync(
            ResetPasswordViewModel model,
            Core.Models.User targetUser)
        {
            model.TargetUserId = targetUser.Id;
            model.SearchUsername = targetUser.UserName ?? model.SearchUsername;
            model.Username = targetUser.UserName;
            model.Email = targetUser.Email;
            model.Role = targetUser.Role;
            model.DisplayName = targetUser.Role switch
            {
                "Admin" => await _dbContext.Admins.AsNoTracking()
                    .Where(item => item.UserID == targetUser.Id)
                    .Select(item => item.Name)
                    .FirstOrDefaultAsync(),
                "Staff" => await _dbContext.Staff.AsNoTracking()
                    .Where(item => item.UserID == targetUser.Id)
                    .Select(item => item.Name)
                    .FirstOrDefaultAsync(),
                "Child" => await _dbContext.Children.AsNoTracking()
                    .Where(item => item.UserID == targetUser.Id)
                    .Select(item => item.Name)
                    .FirstOrDefaultAsync(),
                "Coach" => await _dbContext.Coaches.AsNoTracking()
                    .Where(item => item.UserID == targetUser.Id)
                    .Select(item => item.Name)
                    .FirstOrDefaultAsync(),
                _ => null
            };
        }
        
    }
}
