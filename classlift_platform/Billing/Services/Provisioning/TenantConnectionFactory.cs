using Billing.Interfaces;
using MySqlConnector;

namespace Billing.Services.Provisioning;

public class TenantConnectionFactory : ITenantConnectionStringFactory
{
    private const uint TenantMaximumPoolSize = 5;
    private const uint ServerMaximumPoolSize = 5;
    private readonly IConfiguration _configuration;

    public TenantConnectionFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string BuildConnectionString(string databaseName)
    {
        var builder = new MySqlConnectionStringBuilder
        {
            Server = _configuration["TenantDatabase:Host"],
            Port = uint.Parse(_configuration["TenantDatabase:Port"] ?? "3306"),
            UserID = _configuration["TenantDatabase:User"],
            Password = _configuration["TenantDatabase:Password"],
            Database = databaseName,

            // Each database name creates a distinct MySqlConnector pool. Keep every
            // tenant pool small so their combined size stays below MySQL's limit.
            Pooling = true,
            MinimumPoolSize = 0,
            MaximumPoolSize = TenantMaximumPoolSize,
            ConnectionIdleTimeout = 60,
            ConnectionLifeTime = 300,
            ConnectionTimeout = 15
        };

        return builder.ConnectionString;
    }

    public string BuildServerConnectionString()
    {
        var builder = new MySqlConnectionStringBuilder
        {
            Server = _configuration["TenantDatabase:Host"],
            Port = uint.Parse(_configuration["TenantDatabase:Port"] ?? "3306"),
            UserID = _configuration["TenantDatabase:User"],
            Password = _configuration["TenantDatabase:Password"],
            Pooling = true,
            MinimumPoolSize = 0,
            MaximumPoolSize = ServerMaximumPoolSize,
            ConnectionIdleTimeout = 60,
            ConnectionLifeTime = 300,
            ConnectionTimeout = 15
        };

        return builder.ConnectionString;
    }
}
