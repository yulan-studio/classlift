using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Feature
    {
        public int FeatureID { get; set; }

        public string FeatureKey { get; set; } = null!;

        public string FeatureName { get; set; } = null!;

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; }

        // Navigation Properties

        public ICollection<PlanFeature> PlanFeatures { get; set; }
            = new List<PlanFeature>();
    }
}
