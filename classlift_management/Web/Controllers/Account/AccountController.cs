using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Core.Services;

using Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Core.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace Web.Controllers.Account
{
    [Route("Account")]
    public class AccountController : Controller
    {
        private readonly IUserRegistrationService _userRegistrationService;
        private readonly SignInManager<Core.Models.User> _signInManager;
        private readonly UserManager<Core.Models.User> _userManager;
        private readonly ITimeZoneService _timeZoneService;

        public AccountController(IUserRegistrationService userRegistrationService, SignInManager<Core.Models.User> signInManager, UserManager<Core.Models.User> userManager, ITimeZoneService timeZoneService)
        {
            _userRegistrationService = userRegistrationService;
            _signInManager = signInManager;
            _userManager = userManager;
            _timeZoneService = timeZoneService;
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
        [HttpGet("TimeZone")]
        public async Task<IActionResult> TimeZone(string? returnUrl = null)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();
            ViewBag.TimeZones = _timeZoneService.GetTimeZones();
            return View(new TimeZonePreferenceViewModel { TimeZoneId = user.TimeZoneId, ReturnUrl = returnUrl });
        }

        [Authorize]
        [HttpPost("TimeZone")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TimeZone(TimeZonePreferenceViewModel model)
        {
            if (!_timeZoneService.IsValidTimeZone(model.TimeZoneId))
                ModelState.AddModelError(nameof(model.TimeZoneId), "Please select a valid time zone.");
            ViewBag.TimeZones = _timeZoneService.GetTimeZones();
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();
            user.TimeZoneId = model.TimeZoneId;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return View(model);
            }

            await _signInManager.RefreshSignInAsync(user);
            if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                return LocalRedirect(model.ReturnUrl);
            return RedirectToAction("Index", "Home");
        }
        
    }
}
