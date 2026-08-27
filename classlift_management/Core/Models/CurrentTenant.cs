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

        public string? Subdomain { get; set; }

        public string? DatabaseName { get; set; }

        public string? ConnectionString { get; set; }

        public int? PlanId { get; set; }

        public string? PlanName { get; set; }

        public IReadOnlySet<string> EnabledFeatures { get; set; }
            = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        public OrganizationTerminology Terminology { get; set; } = new();

        /// <summary>
        /// Checks the feature set loaded during tenant resolution.
        /// Feature keys are compared case-insensitively.
        /// </summary>
        public bool HasFeature(string featureKey) =>
            !string.IsNullOrWhiteSpace(featureKey) &&
            EnabledFeatures.Contains(featureKey);

        public bool IsResolved =>
            !string.IsNullOrWhiteSpace(DatabaseName) &&
            !string.IsNullOrWhiteSpace(ConnectionString);
    }
}
