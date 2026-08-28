namespace Core.Models
{
    /// <summary>
    /// The effective plan and feature set calculated for one tenant.
    /// </summary>
    public sealed class TenantFeatures
    {
        // A missing/inactive organization or plan receives no features by default.
        public static TenantFeatures Empty { get; } = new();

        public int? PlanId { get; init; }

        public string? PlanName { get; init; }

        public IReadOnlySet<string> EnabledFeatures { get; init; }
            = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    }
}
