namespace Billing.Models;

public sealed class AddOnInvoice
{
    public int AddOnInvoiceId { get; set; }
    public int OrganizationId { get; set; }
    public int OrganizationAddOnSubscriptionId { get; set; }
    public int AddOnId { get; set; }
    public string StripeInvoiceId { get; set; } = null!;
    public string? StripeSubscriptionId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "CAD";
    public string Status { get; set; } = "Pending";
    public DateTime? BillingPeriodStart { get; set; }
    public DateTime? BillingPeriodEnd { get; set; }
    public DateTime? PaidAt { get; set; }
    public DateTime? PaymentFailedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public Organization Organization { get; set; } = null!;
    public OrganizationAddOnSubscription OrganizationAddOnSubscription { get; set; } = null!;
    public AddOn AddOn { get; set; } = null!;
}
