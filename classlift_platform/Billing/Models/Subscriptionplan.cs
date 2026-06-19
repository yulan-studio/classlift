using System;
using System.Collections.Generic;

namespace Billing.Models;

public partial class Subscriptionplan
{
    public int PlanId { get; set; }

    public string PlanName { get; set; } = null!;

    public string? Description { get; set; }

    public decimal PricePerCoach { get; set; }

    public decimal MinimumMonthlyPrice { get; set; }

    public bool? IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual ICollection<OrganizationSubscription> OrganizationSubscriptions { get; set; } = new List<OrganizationSubscription>();

    public virtual ICollection<Organization> Organizations { get; set; } = new List<Organization>();

    public virtual ICollection<Planfeature> Planfeatures { get; set; } = new List<Planfeature>();

    public virtual ICollection<Promotion> Promotions { get; set; } = new List<Promotion>();

    public virtual ICollection<SubscriptionEvent> SubscriptionEventNewPlans { get; set; } = new List<SubscriptionEvent>();

    public virtual ICollection<SubscriptionEvent> SubscriptionEventOldPlans { get; set; } = new List<SubscriptionEvent>();
}
