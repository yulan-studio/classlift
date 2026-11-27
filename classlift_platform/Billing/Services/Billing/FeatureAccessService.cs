using Billing.Constants;
using Billing.Data;
using Billing.Models;
using Billing.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Billing.Services.Billing
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
        private readonly IMemoryCache _cache;
        

        public FeatureAccessService(BillingDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<OrganizationFeatureContext?> GetFeatureContextAsync(int organizationId)
        {
            var cacheKey = $"feature-context-{organizationId}";

            if (_cache.TryGetValue(cacheKey, out OrganizationFeatureContext? cachedContext))
            {
                return cachedContext;
            }

            var subscription =
                await _context.OrganizationSubscriptions
                    .Include(s => s.Plan)
                    .FirstOrDefaultAsync(s =>
                        s.OrganizationId == organizationId &&
                        (s.Status == SubscriptionStatus.Active || s.Status == SubscriptionStatus.Trial));

            if (subscription == null)
                return null;

            var planFeatures =
                await _context.Planfeatures
                    .Include(pf => pf.Feature)
                    .Where(pf => pf.PlanId == subscription.PlanId)
                    .Select(pf => new { pf.FeatureId, pf.Feature.FeatureKey, pf.Feature.IsOrganizationConfigurable })
                    .Distinct()
                    .ToListAsync();

            var featureIds = planFeatures.Select(f => f.FeatureId).ToList();
            var settings = await _context.OrganizationFeatureSettings
                .Where(s => s.OrganizationId == organizationId && featureIds.Contains(s.FeatureId))
                .ToDictionaryAsync(s => s.FeatureId, s => s.IsEnabled);

            var features = planFeatures
                .Where(f => !f.IsOrganizationConfigurable || !settings.TryGetValue(f.FeatureId, out var enabled) || enabled)
                .Select(f => f.FeatureKey)
                .ToHashSet();

            // Add-ons are separate from the features included by a plan.
            // Activity Management is available only when the organization's
            // current plan is eligible and the organization has purchased an
            // active Activity Management add-on subscription.
            var activityManagementPurchased =
                await _context.OrganizationAddOnSubscriptions
                    .AnyAsync(addOnSubscription =>
                        addOnSubscription.OrganizationId == organizationId &&
                        addOnSubscription.AddOn.AddOnKey == FeatureKeys.ActivityManagement &&
                        addOnSubscription.Status == AddOnStatus.Active &&
                        addOnSubscription.AddOn.IsActive &&
                        addOnSubscription.AddOn.PlanEligibilities.Any(eligibility =>
                            eligibility.PlanId == subscription.PlanId &&
                            eligibility.IsEligible), cancellationToken: default);

            if (activityManagementPurchased)
            {
                features.Add(FeatureKeys.ActivityManagement);
            }
            else
            {
                features.Remove(FeatureKeys.ActivityManagement);
            }

            var context = new OrganizationFeatureContext
            {
                OrganizationId = organizationId,
                PlanId = subscription.PlanId,
                PlanName = subscription.Plan.PlanName,
                Features = features
            };

            _cache.Set(
                cacheKey,
                context,
                TimeSpan.FromMinutes(10)
            );

            return context;

        }


        public async Task<bool> HasFeatureAsync(
            int organizationId,
            string featureKey)
        {
            var context = await GetFeatureContextAsync(organizationId);

            if (context == null)
                return false;

            return context.Features.Contains(featureKey);
        }


        //when plans change, clear cache for that organization.
        public void ClearFeatureCache(int organizationId)
        {
            var cacheKey = $"feature-context-{organizationId}";
            _cache.Remove(cacheKey);
        }
    }
}
