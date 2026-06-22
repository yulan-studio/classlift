using System;
using System.Collections.Generic;

namespace Billing.Models;

public partial class Planfeature
{
    public int PlanFeatureId { get; set; }

    public int PlanId { get; set; }

    public int FeatureId { get; set; }

    public bool IsLocked { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Feature Feature { get; set; } = null!;

    public virtual Subscriptionplan Plan { get; set; } = null!;
}
