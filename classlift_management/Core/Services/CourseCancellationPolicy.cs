using Core.Models;

namespace Core.Services;

/// <summary>
/// Centralized rules for actions available after a course session is canceled.
/// Keep plan capability checks here so the UI and server enforce the same rule.
/// </summary>
public static class CourseCancellationPolicy
{
    public static bool CanRefundSessionCost(CurrentTenant tenant) =>
        tenant.HasFeature(Core.FeatureCodes.CreditTracking);
}
