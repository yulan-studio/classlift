namespace Billing.Interfaces
{
    public interface ITenantIdentitySeeder
    {
        Task SeedAdminAsync(
            string connectionString,
            string adminEmail,
            string adminPassword);
    }
}
