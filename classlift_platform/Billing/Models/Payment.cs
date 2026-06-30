using System;
using System.Collections.Generic;

namespace Billing.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int InvoiceId { get; set; }

    public string PaymentProvider { get; set; } = null!;

    public string ProviderTransactionId { get; set; } = null!;

    public decimal Amount { get; set; }

    public string Currency { get; set; } = null!;

    public string PaymentStatus { get; set; } = null!;

    public DateTime PaymentDate { get; set; }

    public string? Notes { get; set; }

    public virtual Invoice Invoice { get; set; } = null!;
}
