namespace Billing.Interfaces
{
    public interface IDatabaseProvisioner
    {
        Task CreateDatabaseAsync(string databaseName);
        Task DeleteDatabaseAsync(string databaseName);
    }
}