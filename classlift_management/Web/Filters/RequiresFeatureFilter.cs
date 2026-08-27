using Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Net.Http.Headers;

namespace Web.Filters
{
    /// <summary>
    /// Enforces plan features already loaded into CurrentTenant by
    /// TenantResolutionMiddleware. No additional database query is made here.
    /// </summary>
    public sealed class RequiresFeatureFilter : IAsyncActionFilter
    {
        private readonly string _featureKey;
        private readonly CurrentTenant _currentTenant;

        public RequiresFeatureFilter(
            string featureKey,
            CurrentTenant currentTenant)
        {
            _featureKey = featureKey;
            _currentTenant = currentTenant;
        }

        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            if (_currentTenant.HasFeature(_featureKey))
            {
                await next();
                return;
            }

            var acceptsHtml = context.HttpContext.Request
                .GetTypedHeaders()
                .Accept?
                .Any(mediaType => string.Equals(
                    mediaType.MediaType.Value,
                    "text/html",
                    StringComparison.OrdinalIgnoreCase)) == true;

            // Browser page navigation receives a useful explanation. Fetch/API
            // calls receive a plain 403 so clients never try to parse HTML as JSON.
            context.Result = acceptsHtml
                ? new RedirectToActionResult(
                    "FeatureUnavailable",
                    "Account",
                    new
                    {
                        featureKey = _featureKey,
                        planName = _currentTenant.PlanName
                    })
                : new StatusCodeResult(StatusCodes.Status403Forbidden);
        }
    }
}
