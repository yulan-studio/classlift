using Core.Models;

namespace Core.Interfaces
{
    /// <summary>
    /// Resolves feature entitlements from an organization's current plan.
    /// </summary>
    public interface IFeatureService
    {
        Task<TenantFeatures> GetFeaturesAsync(
            int organizationId,
            CancellationToken cancellationToken = default);

        Task<bool> IsEnabledAsync(
            int organizationId,
            string featureKey,
            CancellationToken cancellationToken = default);
    }
}
