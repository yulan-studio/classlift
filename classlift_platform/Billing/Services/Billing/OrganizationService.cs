using Billing.Constants;
using Billing.Data;
using Billing.Interfaces;
using Billing.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Caching.Memory;

namespace Billing.Services.Billing;

public class OrganizationService
{
    private readonly BillingDbContext _context;
    private readonly IDatabaseProvisioner _databaseProvisioner;
    private readonly IMemoryCache _cache;

    public OrganizationService(
        BillingDbContext context,
        IDatabaseProvisioner databaseProvisioner,
        IMemoryCache cache)
    {
        _context = context;
        _databaseProvisioner = databaseProvisioner;
        _cache = cache;
    }

    public async Task<Organization> CancelOrganizationAsync(
        int organizationId,
        string cancelledBy = "admin",
        string? reason = null)
    {
        var organization = await _context.Organizations
            .Include(o => o.OrganizationSubscriptions)
            .Include(o => o.Tenantregistries)
            .FirstOrDefaultAsync(o => o.OrganizationId == organizationId);

        if (organization == null)
            throw new InvalidOperationException("Organization not found.");

        var now = DateTime.UtcNow;
        var cancellableSubscriptions = organization.OrganizationSubscriptions
            .Where(s => s.Status is SubscriptionStatus.Active
                or SubscriptionStatus.Trial
                or SubscriptionStatus.Suspended)
            .ToList();

        organization.IsActive = false;
        organization.CurrentPlanId = null;
        organization.UpdatedAt = now;

        foreach (var tenant in organization.Tenantregistries)
        {
            tenant.IsActive = false;
            tenant.UpdatedAt = now;
        }

        foreach (var subscription in cancellableSubscriptions)
        {
            var oldStatus = subscription.Status;
            subscription.Status = SubscriptionStatus.Cancelled;
            subscription.EndDate = now;
            subscription.CancelledAt = now;
            subscription.AutoRenew = 0;
            subscription.UpdatedAt = now;

            _context.SubscriptionEvents.Add(new SubscriptionEvent
            {
                OrganizationId = organizationId,
                OrganizationSubscriptionId = subscription.OrganizationSubscriptionId,
                EventType = SubscriptionEventTypes.Cancelled,
                OldPlanId = subscription.PlanId,
                NewPlanId = null,
                OldStatus = oldStatus,
                NewStatus = SubscriptionStatus.Cancelled,
                EffectiveAt = now,
                CreatedAt = now,
                CreatedBy = cancelledBy,
                Reason = reason ?? "Organization cancelled from admin portal"
            });
        }

        await _context.SaveChangesAsync();
        _cache.Remove($"feature-context-{organizationId}");

        return organization;
    }

    public async Task DeleteOrganizationAsync(int organizationId)
    {
        var organization = await _context.Organizations
            .Include(o => o.Tenantregistries)
            .FirstOrDefaultAsync(o => o.OrganizationId == organizationId);

        if (organization == null)
            throw new InvalidOperationException("Organization not found.");

        var tenantDatabaseNames = organization.Tenantregistries
            .Select(t => t.DatabaseName)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        IDbContextTransaction? transaction = null;
        if (_context.Database.IsRelational())
            transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var subscriptions = await _context.OrganizationSubscriptions
                .Where(s => s.OrganizationId == organizationId)
                .ToListAsync();
            var subscriptionIds = subscriptions
                .Select(s => s.OrganizationSubscriptionId)
                .ToList();
            var invoices = await _context.Invoices
                .Where(i => i.OrganizationId == organizationId)
                .ToListAsync();
            var invoiceIds = invoices.Select(i => i.InvoiceId).ToList();

            var payments = await _context.Payments
                .Where(p => invoiceIds.Contains(p.InvoiceId))
                .ToListAsync();
            var events = await _context.SubscriptionEvents
                .Where(e => e.OrganizationId == organizationId
                    || (e.OrganizationSubscriptionId != null
                        && subscriptionIds.Contains(e.OrganizationSubscriptionId.Value)))
                .ToListAsync();

            _context.Payments.RemoveRange(payments);
            _context.Invoices.RemoveRange(invoices);
            _context.SubscriptionEvents.RemoveRange(events);
            _context.OrganizationSubscriptions.RemoveRange(subscriptions);
            _context.Tenantregistries.RemoveRange(organization.Tenantregistries);
            _context.Organizations.Remove(organization);

            await _context.SaveChangesAsync();

            foreach (var databaseName in tenantDatabaseNames)
                await _databaseProvisioner.DeleteDatabaseAsync(databaseName);

            if (transaction != null)
                await transaction.CommitAsync();

            _cache.Remove($"feature-context-{organizationId}");
        }
        catch
        {
            if (transaction != null)
                await transaction.RollbackAsync();
            throw;
        }
        finally
        {
            if (transaction != null)
                await transaction.DisposeAsync();
        }
    }
}
