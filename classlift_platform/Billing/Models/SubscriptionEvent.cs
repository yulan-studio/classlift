using System;
using System.Collections.Generic;

namespace Billing.Models;

public partial class SubscriptionEvent
{
    public int SubscriptionEventId { get; set; }

    public int OrganizationId { get; set; }

    public int? OrganizationSubscriptionId { get; set; }

    public string EventType { get; set; } = null!;

    public int? OldPlanId { get; set; }

    public int? NewPlanId { get; set; }

    public string? OldStatus { get; set; }

    public string? NewStatus { get; set; }

    public DateTime EffectiveAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public string? Reason { get; set; }

    public virtual Subscriptionplan? NewPlan { get; set; }

    public virtual Subscriptionplan? OldPlan { get; set; }

    public virtual Organization Organization { get; set; } = null!;

    public virtual OrganizationSubscription? OrganizationSubscription { get; set; }
}
