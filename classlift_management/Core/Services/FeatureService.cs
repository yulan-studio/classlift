using Core.Contexts;
using Core.Interfaces;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Core.Services
{
    /// <summary>
    /// Reads plan-based feature entitlements from the platform database.
    /// A PlanFeature row means that the feature is included in the plan;
    /// PlanFeature.IsLocked is not used as an enabled/disabled flag.
    /// </summary>
    public sealed class FeatureService : IFeatureService
    {
        private readonly BillingDbContext _billingDbContext;

        public FeatureService(BillingDbContext billingDbContext)
        {
            _billingDbContext = billingDbContext;
        }

        public async Task<TenantFeatures> GetFeaturesAsync(
            int organizationId,
            CancellationToken cancellationToken = default)
        {
            // CurrentPlanId is the source of truth for the plan currently
            // applied to the organization. Inactive organizations and plans
            // intentionally fall through to an empty feature set.
            var plan = await _billingDbContext.Organizations
                .AsNoTracking()
                .Where(organization =>
                    organization.OrganizationId == organizationId &&
                    organization.IsActive &&
                    organization.CurrentPlanId != null &&
                    organization.CurrentPlan != null &&
                    organization.CurrentPlan.IsActive)
                .Select(organization => new
                {
                    PlanId = organization.CurrentPlanId!.Value,
                    organization.CurrentPlan!.PlanName
                })
                .SingleOrDefaultAsync(cancellationToken);

            if (plan is null)
            {
                return TenantFeatures.Empty;
            }

            var featureKeys = await _billingDbContext.PlanFeatures
                .AsNoTracking()
                .Where(planFeature => planFeature.PlanId == plan.PlanId)
                .Select(planFeature => planFeature.Feature.FeatureKey)
                .ToListAsync(cancellationToken);

            return new TenantFeatures
            {
                PlanId = plan.PlanId,
                PlanName = plan.PlanName,
                EnabledFeatures = new HashSet<string>(
                    featureKeys,
                    StringComparer.OrdinalIgnoreCase)
            };
        }

        public async Task<bool> IsEnabledAsync(
            int organizationId,
            string featureKey,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(featureKey))
            {
                return false;
            }

            // Keep this as a single EXISTS query for callers that only need
            // to check one feature and do not need the full entitlement set.
            return await _billingDbContext.PlanFeatures
                .AsNoTracking()
                .AnyAsync(planFeature =>
                    planFeature.Plan.CurrentPlanOrganizations.Any(organization =>
                        organization.OrganizationId == organizationId &&
                        organization.IsActive) &&
                    planFeature.Plan.IsActive &&
                    planFeature.Feature.FeatureKey == featureKey,
                    cancellationToken);
        }
    }
}
