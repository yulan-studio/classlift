namespace Core.Models;

public sealed class OrganizationPaymentSettings
{
    public int OrganizationPaymentSettingsId { get; set; } = 1;
    public string DirectPaymentInstructionTemplate { get; set; } = "Please pay {amount} for {payment_scope} using the payment method selected by the organization.";
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}
