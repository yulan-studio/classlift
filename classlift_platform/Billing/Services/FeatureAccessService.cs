using Billing.Constants;
using Billing.Data;
using Billing.Models;
using Billing.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

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
                        s.Status == "Active");

            if (subscription == null)
                return null;

            var features =
                await _context.Planfeatures
                    .Include(pf => pf.Feature)
                    .Where(pf => pf.PlanId == subscription.PlanId)
                    .Select(pf => pf.Feature.FeatureKey)
                    .Distinct()
                    .ToListAsync();

            var context = new OrganizationFeatureContext
            {
                OrganizationId = organizationId,
                PlanId = subscription.PlanId,
                PlanName = subscription.Plan.PlanName,
                Features = features.ToHashSet()
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