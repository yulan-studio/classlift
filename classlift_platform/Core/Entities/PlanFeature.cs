using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class PlanFeature
    {
        public int PlanFeatureID { get; set; }

        public int PlanID { get; set; }

        public int FeatureID { get; set; }

        public DateTime CreatedAt { get; set; }

        // Navigation Properties

        public SubscriptionPlan Plan { get; set; } = null!;

        public Feature Feature { get; set; } = null!;
    }

}
