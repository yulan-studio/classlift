using Billing.Data;
using Billing.ViewModels;
using Microsoft.EntityFrameworkCore;
using Billing.Constants;

namespace Billing.Services
{
    /*
     未来主系统：

    var context =  await _featureAccessService.GetFeatureContextAsync(organizationId);

   然后：

    if (context.Features.Contains(FeatureKeys.AiEnhancements)))
    {
        // allow AI
    }
     */
    public class FeatureAccessService
    {
        private readonly BillingDbContext _context;

        public FeatureAccessService(BillingDbContext context)
        {
            _context = context;
        }

        public async Task<OrganizationFeatureContext?>
            GetFeatureContextAsync(int organizationId)
        {
            var subscription =
                await _context.OrganizationSubscriptions
                    .Include(s => s.Plan)
                    .FirstOrDefaultAsync(s =>
                        s.OrganizationId == organizationId &&
                        s.Status == "Active");

            if (subscription == null)
                return null;

            var features =
                await _context.Planfeatures
                    .Include(pf => pf.Feature)
                    .Where(pf => pf.PlanId == subscription.PlanId)
                    .Select(pf => pf.Feature.FeatureKey)
                    .ToListAsync();

            return new OrganizationFeatureContext
            {
                OrganizationId = organizationId,
                PlanId = subscription.PlanId,
                PlanName = subscription.Plan.PlanName,
                Features = features.ToHashSet()
            };
        }

        public async Task<bool> HasFeatureAsync(
            int organizationId,
            string featureKey)
        {
            var context =
                await GetFeatureContextAsync(organizationId);

            if (context == null)
                return false;

            return context.Features.Contains(featureKey);
        }
    }
}