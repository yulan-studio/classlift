namespace Billing.Interfaces;

public interface ITenantConnectionStringFactory
{
    string BuildConnectionString(string databaseName);
    string BuildServerConnectionString();
}