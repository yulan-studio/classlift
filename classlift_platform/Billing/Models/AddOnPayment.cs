namespace Billing.Models;

public sealed class AddOnPayment
{
    public int AddOnPaymentId { get; set; }
    public int AddOnInvoiceId { get; set; }
    public string PaymentProvider { get; set; } = null!;
    public string ProviderTransactionId { get; set; } = null!;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = null!;
    public string PaymentStatus { get; set; } = null!;
    public DateTime PaymentDate { get; set; }
    public string? Notes { get; set; }

    public AddOnInvoice AddOnInvoice { get; set; } = null!;
}
