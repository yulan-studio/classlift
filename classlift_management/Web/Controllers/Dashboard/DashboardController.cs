


using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Core.Services;

using Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Core.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace Web.Controllers.Dashboard
{
    [Route("Dashboard")]
    public class DashboardController : Controller
    {


        public DashboardController()
        {

        }

        [Authorize(Roles = "Admin")]
        [HttpGet("Admin")]
        public IActionResult Admin()
        {
            return View();
        }

        [Authorize(Roles = "Staff")]
        [HttpGet("Staff")]
        public IActionResult Staff()
        {
            return View();
        }

        [Authorize(Roles = "Coach")]
        [HttpGet("Coach")]
        public IActionResult Coach()
        {
            return View();
        }


        [Authorize(Roles = "Child")]
        [HttpGet("Child")]
        public IActionResult Child()
        {
            return View();
        }

    }
}
