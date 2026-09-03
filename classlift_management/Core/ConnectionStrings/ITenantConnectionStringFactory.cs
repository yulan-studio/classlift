namespace Core.ConnectionStrings
{
    public interface ITenantConnectionStringFactory
    {
        string BuildConnectionString(string databaseName);
        string BuildServerConnectionString();
    }
}
