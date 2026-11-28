using Billing.Models;

namespace Billing.ViewModels;

public sealed class OrganizationPortalViewModel
{
    public required Organization Organization { get; init; }
    public required IReadOnlyList<Invoice> Invoices { get; init; }
    public required IReadOnlyList<Payment> Payments { get; init; }
    public required IReadOnlyList<AddOnPayment> AddOnPayments { get; init; }
    public OrganizationSubscription? CurrentSubscription { get; init; }
    public required IReadOnlyList<OrganizationAddOnViewModel> AddOns { get; init; }
    public decimal SuccessfulPayments => Payments
        .Where(p => p.PaymentStatus == Constants.PaymentStatus.Succeeded)
        .Sum(p => p.Amount);
}

public sealed class OrganizationAddOnViewModel
{
    public required string AddOnKey { get; init; }
    public required string Name { get; init; }
    public required string Status { get; init; }
    public required string ManagementUrl { get; init; }
    public bool IsEligible { get; init; }
}
