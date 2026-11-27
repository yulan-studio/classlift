namespace Billing.Models;

public sealed class PendingPlanChange
{
    public int PendingPlanChangeId { get; set; }
    public int OrganizationId { get; set; }
    public int? CurrentPlanId { get; set; }
    public int RequestedPlanId { get; set; }
    public int CoachCount { get; set; }
    public decimal MonthlyAmount { get; set; }
    public string Status { get; set; } = "PendingPayment";
    public string? StripeCheckoutSessionId { get; set; }
    public string? StripeSubscriptionId { get; set; }
    public string? CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    public Organization Organization { get; set; } = null!;
    public Subscriptionplan RequestedPlan { get; set; } = null!;
}
