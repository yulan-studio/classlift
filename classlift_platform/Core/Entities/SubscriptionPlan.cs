using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class SubscriptionPlan
    {
        public int PlanID { get; set; }

        public string PlanName { get; set; } = null!;

        public string? Description { get; set; }

        public decimal PricePerCoach { get; set; }

        public decimal MinimumMonthlyPrice { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; }

        // Navigation Properties

        public ICollection<Organization> Organizations { get; set; }
            = new List<Organization>();

        public ICollection<PlanFeature> PlanFeatures { get; set; }
            = new List<PlanFeature>();

        public ICollection<Promotion> Promotions { get; set; }
            = new List<Promotion>();

        public ICollection<Invoice> Invoices { get; set; }
            = new List<Invoice>();
    }
}
