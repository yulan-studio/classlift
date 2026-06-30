using Billing.Constants;
using Billing.Services.Billing;
using Microsoft.AspNetCore.Mvc;

namespace Billing.Controllers
{
    [Route("[controller]")]
    public class FeatureAccessController : Controller
    {
        private readonly FeatureAccessService _featureAccessService;

        public FeatureAccessController(FeatureAccessService featureAccessService)
        {
            _featureAccessService = featureAccessService;
        }

        [HttpGet("Test")]
        public async Task<IActionResult> Test(int organizationId)
        {
            var context = await _featureAccessService.GetFeatureContextAsync(organizationId);



            bool hasAccess = context.Features.Contains(FeatureKeys.AiEnhancements);
  

            return Content(
                hasAccess
                    ? $"Organization {organizationId} HAS access to {FeatureKeys.AiEnhancements}"
                    : $"Organization {organizationId} does NOT have access to {FeatureKeys.AiEnhancements}"
            );
        }
    }
}