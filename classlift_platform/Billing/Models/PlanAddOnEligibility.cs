namespace Billing.Models;

public partial class PlanAddOnEligibility
{
    public int PlanAddOnEligibilityId { get; set; }
    public int PlanId { get; set; }
    public int AddOnId { get; set; }
    public bool IsEligible { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public virtual Subscriptionplan Plan { get; set; } = null!;
    public virtual AddOn AddOn { get; set; } = null!;
}
