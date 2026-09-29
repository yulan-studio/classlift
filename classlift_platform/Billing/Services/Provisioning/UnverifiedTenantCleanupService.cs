using Billing.Data;
using Billing.Services.Billing;
using Microsoft.EntityFrameworkCore;

namespace Billing.Services.Provisioning;

public sealed class UnverifiedTenantCleanupService
{
    private readonly BillingDbContext _context;
    private readonly OrganizationService _organizationService;
    private readonly ILogger<UnverifiedTenantCleanupService> _logger;

    public UnverifiedTenantCleanupService(BillingDbContext context, OrganizationService organizationService, ILogger<UnverifiedTenantCleanupService> logger)
    {
        _context = context;
        _organizationService = organizationService;
        _logger = logger;
    }

    public async Task<int> DeleteExpiredUnverifiedTenantsAsync()
    {
        var now = DateTime.UtcNow;
        var tenants = await _context.Tenantregistries
            .Where(t => !t.IsActive && t.EmailVerificationTokenHash != null)
            .Where(t => t.EmailVerificationExpiresAt != null && t.EmailVerificationExpiresAt <= now)
            .ToListAsync();

        foreach (var tenant in tenants)
        {
            await _organizationService.DeleteOrganizationAsync(tenant.OrganizationId);
            _logger.LogInformation("Deleted unverified organization {OrganizationId} and tenant database {DatabaseName}.", tenant.OrganizationId, tenant.DatabaseName);
        }

        return tenants.Count;
    }
}
