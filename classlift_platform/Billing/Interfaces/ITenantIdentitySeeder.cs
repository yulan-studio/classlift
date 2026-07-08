namespace Billing.Interfaces
{
    public interface ITenantIdentitySeeder
    {
        Task SeedAdminAsync(
        string connectionString,
        string adminName,
        string adminEmail,
        string adminPassword);
    }
}
