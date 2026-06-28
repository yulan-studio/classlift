using Billing.Interfaces;
using MySqlConnector;

namespace Billing.Services.Provisioning;

public class TenantConnectionFactory : ITenantConnectionFactory
{
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
            Database = databaseName
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
            Password = _configuration["TenantDatabase:Password"]
        };

        return builder.ConnectionString;
    }
}