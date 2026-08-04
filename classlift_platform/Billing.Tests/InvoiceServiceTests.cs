using Billing.Constants;
using Billing.Models;
using Billing.Services.Billing;
using Microsoft.EntityFrameworkCore;

namespace Billing.Tests;

public class InvoiceServiceTests
{
    [Fact]
    public async Task Monthly_invoice_uses_coach_price_when_above_minimum()
    {
        await using var db = TestDb.Create();
        var subscription = SeedActiveSubscription(db, 25m, 10m);
        await db.SaveChangesAsync();

        var invoice = await new InvoiceService(db).GenerateMonthlyInvoiceAsync(
            subscription.OrganizationSubscriptionId, new DateOnly(2026, 7, 1), new DateOnly(2026, 7, 31), 3);

        Assert.Equal(75m, invoice.Subtotal);
        Assert.Equal(75m, invoice.TotalAmount);
        Assert.Equal(new DateOnly(2026, 8, 15), invoice.DueDate);
        Assert.Equal(InvoiceStatus.Pending, invoice.InvoiceStatus);
    }

    [Fact]
    public async Task Monthly_invoice_enforces_minimum_price()
    {
        await using var db = TestDb.Create();
        var subscription = SeedActiveSubscription(db, 10m, 100m);
        await db.SaveChangesAsync();

        var invoice = await new InvoiceService(db).GenerateMonthlyInvoiceAsync(
            subscription.OrganizationSubscriptionId, new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 30), 2);

        Assert.Equal(20m, invoice.Subtotal);
        Assert.Equal(100m, invoice.TotalAmount);
    }

    [Fact]
    public async Task Prorated_invoice_calculates_partial_month()
    {
        await using var db = TestDb.Create();
        var subscription = SeedActiveSubscription(db, 310m, 0m);
        await db.SaveChangesAsync();

        var invoice = await new InvoiceService(db).GenerateProratedInvoiceAsync(
            subscription.OrganizationSubscriptionId, new DateTime(2026, 7, 16), 1);

        Assert.Equal(160m, invoice.TotalAmount);
        Assert.Equal(new DateOnly(2026, 7, 16), invoice.BillingPeriodStart);
    }

    [Fact]
    public async Task Duplicate_billing_period_is_rejected()
    {
        await using var db = TestDb.Create();
        var subscription = SeedActiveSubscription(db, 10m, 0m);
        await db.SaveChangesAsync();
        var service = new InvoiceService(db);
        var start = new DateOnly(2026, 7, 1);
        var end = new DateOnly(2026, 7, 31);
        await service.GenerateMonthlyInvoiceAsync(subscription.OrganizationSubscriptionId, start, end, 1);

        var error = await Assert.ThrowsAsync<Exception>(() =>
            service.GenerateMonthlyInvoiceAsync(subscription.OrganizationSubscriptionId, start, end, 1));

        Assert.Contains("already exists", error.Message);
    }

    [Fact]
    public async Task Missing_or_inactive_subscription_is_rejected()
    {
        await using var db = TestDb.Create();
        var service = new InvoiceService(db);
        await Assert.ThrowsAsync<Exception>(() => service.GenerateMonthlyInvoiceAsync(99, new(2026, 1, 1), new(2026, 1, 31), 1));
        var subscription = SeedActiveSubscription(db, 10m, 0m);
        subscription.Status = SubscriptionStatus.Cancelled;
        await db.SaveChangesAsync();
        var error = await Assert.ThrowsAsync<Exception>(() => service.GenerateMonthlyInvoiceAsync(subscription.OrganizationSubscriptionId, new(2026, 1, 1), new(2026, 1, 31), 1));
        Assert.Contains("not active", error.Message);
    }

    [Fact]
    public async Task Expired_trials_are_activated_invoiced_and_audited()
    {
        await using var db = TestDb.Create();
        var subscription = SeedActiveSubscription(db, 31m, 0m);
        subscription.Status = SubscriptionStatus.Trial;
        subscription.IsTrial = 1;
        subscription.TrialEndDate = DateTime.UtcNow.AddMinutes(-1);
        await db.SaveChangesAsync();

        var count = await new InvoiceService(db).ActivateExpiredTrialsAsync();

        Assert.Equal(1, count);
        Assert.Equal(SubscriptionStatus.Active, subscription.Status);
        Assert.Equal(0, subscription.IsTrial);
        Assert.Single(db.Invoices);
        Assert.Single(db.SubscriptionEvents);
    }

    [Fact]
    public async Task Recurring_generation_skips_existing_invoice_and_updates_last_billed()
    {
        await using var db = TestDb.Create();
        var subscription = SeedActiveSubscription(db, 20m, 0m);
        subscription.StartDate = DateTime.UtcNow.AddYears(-1);
        var today = DateTime.UtcNow.Date;
        var start = new DateOnly(today.Year, today.Month, 1);
        var end = start.AddMonths(1).AddDays(-1);
        db.Invoices.Add(NewInvoice(subscription, start, end, 20m));
        await db.SaveChangesAsync();

        var count = await new InvoiceService(db).GenerateRecurringInvoicesAsync();

        Assert.Equal(0, count);
        Assert.Single(db.Invoices);
        Assert.NotNull(subscription.LastBilledDate);
    }

    internal static OrganizationSubscription SeedActiveSubscription(Billing.Data.BillingDbContext db, decimal unitPrice, decimal minimum)
    {
        var plan = new Subscriptionplan { PlanName = Guid.NewGuid().ToString(), PricePerCoach = unitPrice, MinimumMonthlyPrice = minimum };
        var organization = new Organization { OrganizationName = "Test Org", IsActive = true };
        var subscription = new OrganizationSubscription { Organization = organization, Plan = plan, Status = SubscriptionStatus.Active, MonthlyPricePerCoach = unitPrice, MinimumMonthlyPrice = minimum, StartDate = DateTime.UtcNow.AddMonths(-2) };
        db.Add(subscription);
        return subscription;
    }

    internal static Invoice NewInvoice(OrganizationSubscription s, DateOnly start, DateOnly end, decimal total) => new()
    {
        Organization = s.Organization, OrganizationSubscription = s, Plan = s.Plan,
        BillingPeriodStart = start, BillingPeriodEnd = end, DueDate = end.AddDays(15),
        CoachCount = 1, PricePerCoach = total, Subtotal = total, TotalAmount = total,
        InvoiceStatus = InvoiceStatus.Pending, GeneratedAt = DateTime.UtcNow
    };
}
