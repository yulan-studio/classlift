namespace Billing.Interfaces
{
    public interface ITenantSchemaService
    {
        Task InitializeSchemaAsync(string connectionString);
    }
}