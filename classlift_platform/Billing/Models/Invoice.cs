using System;
using System.Collections.Generic;

namespace Billing.Models;

public partial class Invoice
{
    public int InvoiceId { get; set; }

    public int OrganizationId { get; set; }

    public int OrganizationSubscriptionId { get; set; }

    public int PlanId { get; set; }

    public int? PromotionId { get; set; }

    public DateOnly BillingPeriodStart { get; set; }

    public DateOnly BillingPeriodEnd { get; set; }

    public DateOnly DueDate { get; set; }

    public int CoachCount { get; set; }

    public decimal PricePerCoach { get; set; }

    public decimal Subtotal { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal TotalAmount { get; set; }

    public string InvoiceStatus { get; set; } = null!;

    public DateTime GeneratedAt { get; set; }

    public DateTime? PaidAt { get; set; }

    public virtual Organization Organization { get; set; } = null!;

    public virtual OrganizationSubscription OrganizationSubscription { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Subscriptionplan Plan { get; set; } = null!;

    public virtual Promotion? Promotion { get; set; }
}
