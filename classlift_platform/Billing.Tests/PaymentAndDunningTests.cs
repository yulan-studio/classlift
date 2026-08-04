using Billing.Constants;
using Billing.Services.Billing;

namespace Billing.Tests;

public class PaymentAndDunningTests
{
    [Fact]
    public async Task Successful_payment_records_transaction_and_pays_invoice()
    {
        await using var db = TestDb.Create();
        var sub = InvoiceServiceTests.SeedActiveSubscription(db, 50m, 0m);
        var invoice = InvoiceServiceTests.NewInvoice(sub, new(2026, 7, 1), new(2026, 7, 31), 50m);
        db.Add(invoice);
        await db.SaveChangesAsync();

        var payment = await new PaymentService(db).RecordPaymentAsync(invoice.InvoiceId, "Stripe", "txn-1", 50m, notes: "ok");

        Assert.Equal(PaymentStatus.Succeeded, payment.PaymentStatus);
        Assert.Equal("Paid", invoice.InvoiceStatus);
        Assert.NotNull(invoice.PaidAt);
        Assert.Single(db.Payments);
    }

    [Theory]
    [InlineData("Paid", 50, "already paid")]
    [InlineData("Cancelled", 50, "cannot be paid")]
    [InlineData("Pending", 49, "must equal")]
    public async Task Invalid_payments_are_rejected(string status, decimal amount, string message)
    {
        await using var db = TestDb.Create();
        var sub = InvoiceServiceTests.SeedActiveSubscription(db, 50m, 0m);
        var invoice = InvoiceServiceTests.NewInvoice(sub, new(2026, 7, 1), new(2026, 7, 31), 50m);
        invoice.InvoiceStatus = status;
        db.Add(invoice);
        await db.SaveChangesAsync();

        var error = await Assert.ThrowsAsync<Exception>(() => new PaymentService(db).RecordPaymentAsync(invoice.InvoiceId, "Test", "x", amount));
        Assert.Contains(message, error.Message);
        Assert.Empty(db.Payments);
    }

    [Fact]
    public async Task Missing_invoice_payment_is_rejected()
    {
        await using var db = TestDb.Create();
        var error = await Assert.ThrowsAsync<Exception>(() => new PaymentService(db).RecordPaymentAsync(404, "Test", "x", 1));
        Assert.Contains("not found", error.Message);
    }

    [Fact]
    public async Task Dunning_only_marks_past_due_pending_invoices()
    {
        await using var db = TestDb.Create();
        var sub = InvoiceServiceTests.SeedActiveSubscription(db, 10m, 0m);
        var overdue = InvoiceServiceTests.NewInvoice(sub, new(2026, 1, 1), new(2026, 1, 31), 10m);
        overdue.DueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1));
        var future = InvoiceServiceTests.NewInvoice(sub, new(2026, 2, 1), new(2026, 2, 28), 10m);
        future.DueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var paid = InvoiceServiceTests.NewInvoice(sub, new(2026, 3, 1), new(2026, 3, 31), 10m);
        paid.DueDate = overdue.DueDate; paid.InvoiceStatus = InvoiceStatus.Paid;
        db.AddRange(overdue, future, paid);
        await db.SaveChangesAsync();

        var count = await new DunningService(db).MarkOverdueInvoicesAsync();

        Assert.Equal(1, count);
        Assert.Equal(InvoiceStatus.Overdue, overdue.InvoiceStatus);
        Assert.Equal(InvoiceStatus.Pending, future.InvoiceStatus);
        Assert.Equal(InvoiceStatus.Paid, paid.InvoiceStatus);
    }
}
