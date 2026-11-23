using System.ComponentModel.DataAnnotations;

namespace Core.ViewModels;

public sealed class OrganizationPaymentSettingsViewModel
{
    [Required]
    [StringLength(2000)]
    public string DirectPaymentInstructionTemplate { get; set; } = "Please pay {amount} for {payment_scope} using the payment method selected by the organization.";
}
