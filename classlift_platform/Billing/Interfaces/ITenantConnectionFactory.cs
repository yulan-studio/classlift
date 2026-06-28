namespace Billing.Interfaces;

public interface ITenantConnectionFactory
{
    string BuildConnectionString(string databaseName);
    string BuildServerConnectionString();
}