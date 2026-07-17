using System;
using System.Collections.Generic;

namespace Billing.Models;

public partial class Organization
{
    public int OrganizationId { get; set; }

    public string OrganizationName { get; set; } = null!;

    public string? ContactName { get; set; }

    public string? ContactEmail { get; set; }

    public string? ContactPhone { get; set; }

    public int? CurrentPlanId { get; set; }

    public bool? IsActive { get; set; }

    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Subscriptionplan? CurrentPlan { get; set; }

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual ICollection<OrganizationSubscription> OrganizationSubscriptions { get; set; } = new List<OrganizationSubscription>();

    public virtual ICollection<SubscriptionEvent> SubscriptionEvents { get; set; } = new List<SubscriptionEvent>();

    public virtual ICollection<Tenantregistry> Tenantregistries { get; set; } = new List<Tenantregistry>();
}
