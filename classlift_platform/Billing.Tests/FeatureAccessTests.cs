using Billing.Constants;
using Billing.Models;
using Billing.Services.Billing;
using Microsoft.Extensions.Caching.Memory;

namespace Billing.Tests;

public class FeatureAccessTests
{
    [Fact]
    public async Task Active_plan_features_are_returned_without_duplicates()
    {
        await using var db = TestDb.Create();
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var sub = InvoiceServiceTests.SeedActiveSubscription(db, 10m, 0m);
        var feature = new Feature { FeatureKey = "ai", FeatureName = "AI" };
        db.Planfeatures.Add(new Planfeature { Plan = sub.Plan, Feature = feature });
        await db.SaveChangesAsync();

        var context = await new FeatureAccessService(db, cache).GetFeatureContextAsync(sub.OrganizationId);

        Assert.NotNull(context);
        Assert.Equal(sub.PlanId, context.PlanId);
        Assert.Contains("ai", context.Features);
    }

    [Fact]
    public async Task Missing_subscription_has_no_features()
    {
        await using var db = TestDb.Create();
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var service = new FeatureAccessService(db, cache);
        Assert.Null(await service.GetFeatureContextAsync(999));
        Assert.False(await service.HasFeatureAsync(999, "ai"));
    }

    [Fact]
    public async Task Cache_is_used_until_explicitly_cleared()
    {
        await using var db = TestDb.Create();
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var sub = InvoiceServiceTests.SeedActiveSubscription(db, 10m, 0m);
        var feature = new Feature { FeatureKey = "reports", FeatureName = "Reports" };
        db.Planfeatures.Add(new Planfeature { Plan = sub.Plan, Feature = feature });
        await db.SaveChangesAsync();
        var service = new FeatureAccessService(db, cache);
        Assert.True(await service.HasFeatureAsync(sub.OrganizationId, "reports"));
        db.Planfeatures.RemoveRange(db.Planfeatures);
        await db.SaveChangesAsync();
        Assert.True(await service.HasFeatureAsync(sub.OrganizationId, "reports"));
        service.ClearFeatureCache(sub.OrganizationId);
        Assert.False(await service.HasFeatureAsync(sub.OrganizationId, "reports"));
    }
}
