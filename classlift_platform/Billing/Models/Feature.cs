using System;
using System.Collections.Generic;

namespace Billing.Models;

public partial class Feature
{
    public int FeatureId { get; set; }

    public string FeatureKey { get; set; } = null!;

    public string FeatureName { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Planfeature> Planfeatures { get; set; } = new List<Planfeature>();
}
