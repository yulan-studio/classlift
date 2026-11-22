using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public sealed class CurrentTenant
    {
        public int? OrganizationId { get; set; }

        public string OrganizationName { get; set; } = "ClassLift";

        public string? Subdomain { get; set; }

        public string? DatabaseName { get; set; }

        public string? ConnectionString { get; set; }

        public Uri? TrustedPortalBaseUri { get; set; }

        public int? PlanId { get; set; }

        public string? PlanName { get; set; }

        public IReadOnlySet<string> EnabledFeatures { get; set; }
            = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Bypasses plan-based feature checks for the local development tenant.
        /// This must only be enabled while resolving a loopback host.
        /// </summary>
        public bool AreAllFeaturesEnabled { get; set; }

        public OrganizationTerminology Terminology { get; set; } = new();

        /// <summary>
        /// Checks the feature set loaded during tenant resolution.
        /// Feature keys are compared case-insensitively.
        /// </summary>
        public bool HasFeature(string featureKey)
        {
            if (string.IsNullOrWhiteSpace(featureKey))
                return false;

            var enabled = AreAllFeaturesEnabled || EnabledFeatures.Contains(featureKey);
            return string.Equals(featureKey, Core.FeatureCodes.StudentScheduleRequest, StringComparison.OrdinalIgnoreCase)
                ? enabled && HasFeatureWithoutDependency(Core.FeatureCodes.CoachPrivateCourseManagement)
                : enabled;
        }

        private bool HasFeatureWithoutDependency(string featureKey) =>
            AreAllFeaturesEnabled || EnabledFeatures.Contains(featureKey);

        public bool IsResolved =>
            !string.IsNullOrWhiteSpace(DatabaseName) &&
            !string.IsNullOrWhiteSpace(ConnectionString);
    }
}
