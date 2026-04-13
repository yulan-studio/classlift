using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Core.Services;

using Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Core.ViewModels;

namespace Web.Controllers.Account
{
    [Route("Account")]
    public class AccountController : Controller
    {
        private readonly IUserRegistrationService _userRegistrationService;
        private readonly SignInManager<Core.Models.User> _signInManager;
        private readonly UserManager<Core.Models.User> _userManager;

        public AccountController(IUserRegistrationService userRegistrationService, SignInManager<Core.Models.User> signInManager, UserManager<Core.Models.User> userManager)
        {
            _userRegistrationService = userRegistrationService;
            _signInManager = signInManager;
            _userManager = userManager;
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
        
    }
}
