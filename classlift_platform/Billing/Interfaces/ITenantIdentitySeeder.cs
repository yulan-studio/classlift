namespace Billing.Interfaces
{
    public interface ITenantIdentitySeeder
    {
        Task SeedUserAsync(
            string connectionString,
            string email,
            string password,
            string role,
            string? name = null,
            bool createStaffProfile = false);
    }
}
