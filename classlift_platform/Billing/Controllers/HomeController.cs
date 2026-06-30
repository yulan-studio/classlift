using Microsoft.AspNetCore.Mvc;

namespace Billing.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
