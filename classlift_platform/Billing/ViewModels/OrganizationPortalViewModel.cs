using Billing.Models;

namespace Billing.ViewModels;

public sealed class OrganizationPortalViewModel
{
    public required Organization Organization { get; init; }
    public required IReadOnlyList<Invoice> Invoices { get; init; }
    public required IReadOnlyList<Payment> Payments { get; init; }
    public OrganizationSubscription? CurrentSubscription { get; init; }
    public decimal SuccessfulPayments => Payments
        .Where(p => p.PaymentStatus == Constants.PaymentStatus.Succeeded)
        .Sum(p => p.Amount);
}
