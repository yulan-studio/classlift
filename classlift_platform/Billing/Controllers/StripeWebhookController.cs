using Microsoft.AspNetCore.Mvc;

namespace Billing.Controllers
{
    public class StripeWebhookController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
