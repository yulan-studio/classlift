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

        //public async Task<bool> HasFeatureAsync(int organizationId, string featureKey)
        //{
        //    return await _context.OrganizationSubscriptions
        //        .Where(s => s.OrganizationId == organizationId && s.Status == "Active")
        //        .Join(
        //            _context.Planfeatures,
        //            subscription => subscription.PlanId,
        //            planFeature => planFeature.PlanId,
        //            (subscription, planFeature) => planFeature
        //        )
        //        .Join(
        //            _context.Features,
        //            planFeature => planFeature.FeatureId,
        //            feature => feature.FeatureId,
        //            (planFeature, feature) => feature
        //        )
        //        .AnyAsync(feature => feature.FeatureKey == featureKey);
        //}


        public async Task<bool> HasFeatureAsync(int organizationId, string featureKey)
        {
            var subscription = await _context.OrganizationSubscriptions
                .FirstOrDefaultAsync(s =>
                    s.OrganizationId == organizationId &&
                    s.Status == "Active");

            if (subscription == null)
                return false;

            return await _context.Planfeatures
                .Include(pf => pf.Feature)
                .AnyAsync(pf =>
                    pf.PlanId == subscription.PlanId &&
                    pf.Feature.FeatureKey == featureKey);
        }
    }
}