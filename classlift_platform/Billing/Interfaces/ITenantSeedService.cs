namespace Billing.Interfaces
{
    public interface ITenantSeedService
    {
        Task SeedAsync(string connectionString);
    }
}