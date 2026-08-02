using Billing.Constants;
using Billing.Models;
using Billing.Services.Billing;

namespace Billing.Tests;

public class BillingRunAndSubscriptionTests
{
    [Fact]
    public async Task Billing_run_tracks_success_counters()
    {
        await using var db = TestDb.Create();
        var service = new BillingRunService(db);
        var run = await service.StartRunAsync("Daily");
        Assert.Equal(BillingRunStatus.Running, run.Status);
        await service.CompleteRunAsync(run, 2, 3, 4);
        Assert.Equal(BillingRunStatus.Success, run.Status);
        Assert.Equal(2, run.TrialActivated);
        Assert.Equal(3, run.InvoicesGenerated);
        Assert.Equal(4, run.InvoicesMarkedOverdue);
        Assert.True(run.FinishedAt >= run.StartedAt);
    }

    [Fact]
    public async Task Billing_run_preserves_failure_details()
    {
        await using var db = TestDb.Create();
        var service = new BillingRunService(db);
        var run = await service.StartRunAsync("Monthly");
        await service.FailRunAsync(run, new InvalidOperationException("provider unavailable"));
        Assert.Equal(BillingRunStatus.Failed, run.Status);
        Assert.Contains("provider unavailable", run.ErrorMessage);
    }

    [Fact]
    public async Task Change_plan_validates_organization_plan_and_same_plan()
    {
        await using var db = TestDb.Create();
        var service = new SubscriptionService(db);
        Assert.Contains("Organization", (await Assert.ThrowsAsync<Exception>(() => service.ChangePlanAsync(99, 1))).Message);
        var org = new Organization { OrganizationName = "Org", IsActive = true };
        db.Organizations.Add(org);
        await db.SaveChangesAsync();
        Assert.Contains("Plan", (await Assert.ThrowsAsync<Exception>(() => service.ChangePlanAsync(org.OrganizationId, 99))).Message);
        var plan = new Subscriptionplan { PlanName = "Basic", PricePerCoach = 10 };
        db.Subscriptionplans.Add(plan);
        await db.SaveChangesAsync();
        db.OrganizationSubscriptions.Add(new OrganizationSubscription { OrganizationId = org.OrganizationId, PlanId = plan.PlanId, Status = "Active", StartDate = DateTime.UtcNow });
        await db.SaveChangesAsync();
        Assert.Contains("already", (await Assert.ThrowsAsync<Exception>(() => service.ChangePlanAsync(org.OrganizationId, plan.PlanId))).Message);
    }
}
