using Billing.Data;
using Microsoft.EntityFrameworkCore;

namespace Billing.Services
{
    public class FeatureAccessService
    {
        private readonly BillingDbContext _context;

        public FeatureAccessService(BillingDbContext context)
        {
            _context = context;
        }

        public async Task<bool> HasFeatureAsync(int organizationId, string featureKey)
        {
            return await _context.OrganizationSubscriptions
                .Where(s => s.OrganizationId == organizationId && s.Status == "Active")
                .Join(
                    _context.Planfeatures,
                    subscription => subscription.PlanId,
                    planFeature => planFeature.PlanId,
                    (subscription, planFeature) => planFeature
                )
                .Join(
                    _context.Features,
                    planFeature => planFeature.FeatureId,
                    feature => feature.FeatureId,
                    (planFeature, feature) => feature
                )
                .AnyAsync(feature => feature.FeatureKey == featureKey);
        }
    }
}