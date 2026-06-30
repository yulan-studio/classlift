using Microsoft.AspNetCore.Mvc;

namespace Billing.Controllers
{
    public class RailwayWebhookController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
