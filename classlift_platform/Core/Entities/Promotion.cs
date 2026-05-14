using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Promotion
    {
        public int PromotionID { get; set; }

        public string PromotionName { get; set; } = null!;

        public int PlanID { get; set; }

        public DiscountType DiscountType { get; set; }

        public decimal DiscountValue { get; set; }

        public int DurationMonths { get; set; }

        public DateOnly? StartDate { get; set; }

        public DateOnly? EndDate { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; }

        // Navigation Properties

        public SubscriptionPlan Plan { get; set; } = null!;

        public ICollection<Invoice> Invoices { get; set; }
            = new List<Invoice>();
    }
}
