using Billing.Constants;
using Billing.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Billing.Controllers
{
    // try /AiTest?organizationId = 1      -> Feature not allowed: ai_enhancements
    // try /AiTest?organizationId = 3      -> AI Enhancements page is accessible.


    [Route("[controller]")]
    [RequireFeature(FeatureKeys.AiEnhancements)]
    public class AiTestController : Controller
    {
        [HttpGet("")]
        public IActionResult Index()
        {
            return Content("AI Enhancements page is accessible.");
        }
    }
}