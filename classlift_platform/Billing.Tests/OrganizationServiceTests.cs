using Billing.Constants;
using Billing.Interfaces;
using Billing.Models;
using Billing.Services.Billing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Billing.Tests;

public class OrganizationServiceTests
{
    [Fact]
    public async Task Cancel_disables_organization_tenant_and_current_subscriptions()
    {
        await using var db = TestDb.Create();
        var plan = new Subscriptionplan { PlanName = "Basic", PricePerCoach = 10 };
        var organization = new Organization
        {
            OrganizationName = "Org",
            CurrentPlan = plan,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        var subscription = new OrganizationSubscription
        {
            Organization = organization,
            Plan = plan,
            Status = SubscriptionStatus.Active,
            StartDate = DateTime.UtcNow,
            AutoRenew = 1
        };
        var tenant = new Tenantregistry
        {
            Organization = organization,
            DatabaseName = "classlift_org",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        db.AddRange(organization, subscription, tenant);
        await db.SaveChangesAsync();

        using var cache = new MemoryCache(new MemoryCacheOptions());
        var service = new OrganizationService(db, new FakeDatabaseProvisioner(), cache);
        await service.CancelOrganizationAsync(organization.OrganizationId, "tester", "requested");

        Assert.False(organization.IsActive);
        Assert.Null(organization.CurrentPlanId);
        Assert.False(tenant.IsActive);
        Assert.Equal(SubscriptionStatus.Cancelled, subscription.Status);
        Assert.Equal(0, subscription.AutoRenew);
        Assert.NotNull(subscription.EndDate);
        Assert.NotNull(subscription.CancelledAt);
        var cancellation = await db.SubscriptionEvents.SingleAsync();
        Assert.Equal(SubscriptionEventTypes.Cancelled, cancellation.EventType);
        Assert.Equal("tester", cancellation.CreatedBy);
        Assert.Equal("requested", cancellation.Reason);
    }

    [Fact]
    public async Task Delete_removes_complete_aggregate_and_tenant_database()
    {
        await using var db = TestDb.Create();
        var plan = new Subscriptionplan { PlanName = "Basic", PricePerCoach = 10 };
        var organization = new Organization
        {
            OrganizationName = "Org",
            CurrentPlan = plan,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        var subscription = new OrganizationSubscription
        {
            Organization = organization,
            Plan = plan,
            Status = SubscriptionStatus.Active,
            StartDate = DateTime.UtcNow
        };
        var invoice = new Invoice
        {
            Organization = organization,
            OrganizationSubscription = subscription,
            Plan = plan,
            InvoiceStatus = InvoiceStatus.Paid
        };
        var payment = new Payment
        {
            Invoice = invoice,
            PaymentProvider = "test",
            ProviderTransactionId = "tx-1",
            Currency = "CAD",
            PaymentStatus = PaymentStatus.Succeeded
        };
        var tenant = new Tenantregistry
        {
            Organization = organization,
            DatabaseName = "classlift_org",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        db.AddRange(organization, subscription, invoice, payment, tenant);
        await db.SaveChangesAsync();

        var provisioner = new FakeDatabaseProvisioner();
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var service = new OrganizationService(db, provisioner, cache);
        await service.CancelOrganizationAsync(organization.OrganizationId);
        await service.DeleteOrganizationAsync(organization.OrganizationId);

        Assert.Empty(await db.Organizations.ToListAsync());
        Assert.Empty(await db.OrganizationSubscriptions.ToListAsync());
        Assert.Empty(await db.Invoices.ToListAsync());
        Assert.Empty(await db.Payments.ToListAsync());
        Assert.Empty(await db.Tenantregistries.ToListAsync());
        Assert.Equal(new[] { "classlift_org" }, provisioner.DeletedDatabases);
    }

    [Fact]
    public async Task Delete_rejects_organization_that_has_not_been_cancelled()
    {
        await using var db = TestDb.Create();
        var organization = new Organization
        {
            OrganizationName = "Active Org",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        db.Organizations.Add(organization);
        await db.SaveChangesAsync();

        var provisioner = new FakeDatabaseProvisioner();
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var service = new OrganizationService(db, provisioner, cache);

        var error = await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.DeleteOrganizationAsync(organization.OrganizationId));

        Assert.Contains("cancelled", error.Message, StringComparison.OrdinalIgnoreCase);
        Assert.NotNull(await db.Organizations.FindAsync(organization.OrganizationId));
        Assert.Empty(provisioner.DeletedDatabases);
    }

    [Fact]
    public async Task Operations_reject_unknown_organization()
    {
        await using var db = TestDb.Create();
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var service = new OrganizationService(db, new FakeDatabaseProvisioner(), cache);

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.CancelOrganizationAsync(42));
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.DeleteOrganizationAsync(42));
    }

    private sealed class FakeDatabaseProvisioner : IDatabaseProvisioner
    {
        public List<string> DeletedDatabases { get; } = [];

        public Task CreateDatabaseAsync(string databaseName) => Task.CompletedTask;

        public Task DeleteDatabaseAsync(string databaseName)
        {
            DeletedDatabases.Add(databaseName);
            return Task.CompletedTask;
        }
    }
}
