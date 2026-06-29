using System;
using System.Collections.Generic;

namespace Billing.Models;

public partial class OrganizationSubscription
{
    public int OrganizationSubscriptionId { get; set; }

    public int OrganizationId { get; set; }

    public int PlanId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string Status { get; set; } = null!;

    public sbyte IsTrial { get; set; }

    public DateTime? TrialStartDate { get; set; }

    public DateTime? TrialEndDate { get; set; }

    public DateTime? ActivatedAt { get; set; }

    public DateTime? CancelledAt { get; set; }

    public DateTime? LastBilledDate { get; set; }

    public sbyte AutoRenew { get; set; }

    public decimal MonthlyPricePerCoach { get; set; }

    public decimal MinimumMonthlyPrice { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? OrganizationSubscriptionscol { get; set; }

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual Organization Organization { get; set; } = null!;

    public virtual Subscriptionplan Plan { get; set; } = null!;

    public virtual ICollection<SubscriptionEvent> SubscriptionEvents { get; set; } = new List<SubscriptionEvent>();
}
