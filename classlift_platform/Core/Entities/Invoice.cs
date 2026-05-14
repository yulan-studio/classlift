using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Invoice
    {
        public int InvoiceID { get; set; }

        public int OrganizationID { get; set; }

        public int PlanID { get; set; }

        public int? PromotionID { get; set; }

        public DateOnly BillingPeriodStart { get; set; }

        public DateOnly BillingPeriodEnd { get; set; }

        public int CoachCount { get; set; }

        public decimal PricePerCoach { get; set; }

        public decimal Subtotal { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal TotalAmount { get; set; }

        public InvoiceStatus InvoiceStatus { get; set; }

        public DateTime GeneratedAt { get; set; }

        public DateTime? PaidAt { get; set; }

        // Navigation Properties

        public Organization Organization { get; set; } = null!;

        public SubscriptionPlan Plan { get; set; } = null!;

        public Promotion? Promotion { get; set; }

        public ICollection<Payment> Payments { get; set; }
            = new List<Payment>();
    }
}
