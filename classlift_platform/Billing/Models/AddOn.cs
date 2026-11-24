namespace Billing.Models;

public partial class AddOn
{
    public int AddOnId { get; set; }
    public string AddOnKey { get; set; } = null!;
    public int FeatureId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal MonthlyPrice { get; set; }
    public string Currency { get; set; } = "CAD";
    public string BillingCycle { get; set; } = "Monthly";
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public virtual Feature Feature { get; set; } = null!;
    public virtual ICollection<PlanAddOnEligibility> PlanEligibilities { get; set; } = new List<PlanAddOnEligibility>();
    public virtual ICollection<OrganizationAddOnSubscription> OrganizationSubscriptions { get; set; } = new List<OrganizationAddOnSubscription>();
}
