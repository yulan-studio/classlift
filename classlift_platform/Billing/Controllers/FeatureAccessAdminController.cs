using Billing.Data;
using Billing.Services.Billing;
using Billing.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Billing.Controllers
{
    [Route("[controller]")]
    public class FeatureAccessAdminController : Controller
    {
        private readonly BillingDbContext _context;
        private readonly FeatureAccessService _featureAccessService;

        public FeatureAccessAdminController(
            BillingDbContext context,
            FeatureAccessService featureAccessService)
        {
            _context = context;
            _featureAccessService = featureAccessService;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(int? organizationId)
        {
            var organizations = await _context.Organizations
                .OrderBy(o => o.OrganizationName)
                .ToListAsync();

            var model = new FeatureAccessAdminViewModel
            {
                SelectedOrganizationId = organizationId,
                Organizations = organizations.Select(o => new SelectListItem
                {
                    Value = o.OrganizationId.ToString(),
                    Text = o.OrganizationName,
                    Selected = organizationId == o.OrganizationId
                }).ToList()
            };

            if (organizationId.HasValue)
            {
                var organization = await _context.Organizations
                    .FirstOrDefaultAsync(o => o.OrganizationId == organizationId.Value);

                var featureContext = await _featureAccessService
                    .GetFeatureContextAsync(organizationId.Value);

                var allFeatures = await _context.Features
                    .OrderBy(f => f.FeatureName)
                    .ToListAsync();

                model.OrganizationName = organization?.OrganizationName;
                model.PlanName = featureContext?.PlanName ?? "No active subscription";

                model.Features = allFeatures.Select(f => new FeatureAccessItem
                {
                    FeatureKey = f.FeatureKey,
                    FeatureName = f.FeatureName,
                    HasAccess = featureContext != null &&
                                featureContext.Features.Contains(f.FeatureKey)
                }).ToList();
            }

            return View(model);
        }
    }
}
