namespace Billing.Models;

public partial class OrganizationAddOnSubscription
{
    public int OrganizationAddOnSubscriptionId { get; set; }
    public int OrganizationId { get; set; }
    public int AddOnId { get; set; }
    public string Status { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? CurrentPeriodEnd { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? StripeSubscriptionId { get; set; }
    public bool CancelAtPeriodEnd { get; set; }
    public decimal MonthlyPrice { get; set; }
    public string Currency { get; set; } = "CAD";
    public string BillingCycle { get; set; } = "Monthly";
    public bool AutoRenew { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public virtual Organization Organization { get; set; } = null!;
    public virtual AddOn AddOn { get; set; } = null!;
}
