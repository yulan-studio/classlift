using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Core.Services;

using Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Core.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Core.Contexts;
using Microsoft.EntityFrameworkCore;

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
        private const string StaffResetPassword = "hello123!";

        public AccountController(IUserRegistrationService userRegistrationService, SignInManager<Core.Models.User> signInManager, UserManager<Core.Models.User> userManager, ITimeZoneService timeZoneService, AppDbContext dbContext)
        {
            _userRegistrationService = userRegistrationService;
            _signInManager = signInManager;
            _userManager = userManager;
            _timeZoneService = timeZoneService;
            _dbContext = dbContext;
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
                    return Redirect("/Coach/ManageCourse"); 
                  
                else if (user.Role == "Child")
                    return Redirect("/Child/MyRegistrations"); 

                else if (user.Role == "Staff")
                    return Redirect("/Child/List");
                
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

        [Authorize(Roles = "Child,Coach,Staff")]
        [HttpGet("ChangePassword")]
        public IActionResult ChangePassword()
        {
            return PartialView("_ChangePassword", new ChangePasswordViewModel());
        }

        [Authorize(Roles = "Child,Coach,Staff")]
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

        [Authorize(Roles = "Staff")]
        [HttpGet("ResetPassword")]
        public IActionResult ResetPassword()
        {
            return PartialView("_ResetPassword", new ResetPasswordViewModel());
        }

        [Authorize(Roles = "Staff")]
        [HttpPost("FindResetPasswordUser")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FindResetPasswordUser(ResetPasswordViewModel model)
        {
            ModelState.Remove(nameof(model.TargetUserId));
            if (!ModelState.IsValid)
                return PartialView("_ResetPassword", model);

            var targetUser = await _userManager.FindByNameAsync(model.SearchUsername.Trim());
            if (targetUser == null || !CanStaffReset(targetUser.Role))
            {
                ModelState.AddModelError(nameof(model.SearchUsername), "No Staff, Child, or Coach user was found with that username.");
                return PartialView("_ResetPassword", model);
            }

            await PopulateResetProfileAsync(model, targetUser);
            return PartialView("_ResetPassword", model);
        }

        [Authorize(Roles = "Staff")]
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
            if (targetUser == null || !CanStaffReset(targetUser.Role))
                return NotFound();

            var staffUser = await _userManager.GetUserAsync(User);
            if (staffUser == null) return Challenge();

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(targetUser);
            targetUser.UpdatedBy = staffUser.Id;
            targetUser.UpdatedDate = DateTime.UtcNow;
            var result = await _userManager.ResetPasswordAsync(targetUser, resetToken, StaffResetPassword);

            await PopulateResetProfileAsync(model, targetUser);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return PartialView("_ResetPassword", model);
            }

            ViewBag.SuccessMessage = $"The password for {targetUser.UserName} was reset to {StaffResetPassword}";
            return PartialView("_ResetPassword", model);
        }

        private static bool CanStaffReset(string? role) =>
            role is not null
            && (role.Equals("Staff", StringComparison.OrdinalIgnoreCase)
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
