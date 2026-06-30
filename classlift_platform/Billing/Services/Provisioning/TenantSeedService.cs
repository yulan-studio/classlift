using Billing.Interfaces;

namespace Billing.Services.Provisioning;

public class TenantSeedService : ITenantSeedService
{
    public Task SeedAsync(string connectionString)
    {
        return Task.CompletedTask;
    }
}