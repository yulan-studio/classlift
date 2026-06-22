using Billing.Services;
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
        public async Task<IActionResult> Test(int organizationId, string featureKey)
        {
            var hasAccess = await _featureAccessService.HasFeatureAsync(
                organizationId,
                featureKey
            );

            return Content(
                hasAccess
                    ? $"Organization {organizationId} HAS access to {featureKey}"
                    : $"Organization {organizationId} does NOT have access to {featureKey}"
            );
        }
    }
}