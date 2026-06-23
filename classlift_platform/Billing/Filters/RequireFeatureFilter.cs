using Billing.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Billing.Filters
{
    public class RequireFeatureFilter : IAsyncAuthorizationFilter
    {
        private readonly FeatureAccessService _featureAccessService;
        private readonly string _featureKey;

        public RequireFeatureFilter(
            FeatureAccessService featureAccessService,
            string featureKey)
        {
            _featureAccessService = featureAccessService;
            _featureKey = featureKey;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var orgIdValue = context.HttpContext.Request.Query["organizationId"].FirstOrDefault();

            if (!int.TryParse(orgIdValue, out var organizationId))
            {
                context.Result = new BadRequestObjectResult(
                    "Missing or invalid organizationId.");
                return;
            }

            var hasFeature = await _featureAccessService.HasFeatureAsync(
                organizationId,
                _featureKey);

            if (!hasFeature)
            {
                context.Result = new ContentResult
                {
                    StatusCode = StatusCodes.Status403Forbidden,
                    Content = $"Feature not allowed: {_featureKey}"
                };
            }
        }
    }
}