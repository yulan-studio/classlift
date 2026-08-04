using Billing.Services.Provisioning;
using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace Billing.Tests;

public class TenantConnectionFactoryTests
{
    [Fact]
    public void Builds_tenant_and_server_connection_strings_from_configuration()
    {
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["TenantDatabase:Host"] = "db.internal", ["TenantDatabase:Port"] = "3307",
            ["TenantDatabase:User"] = "tenant_user", ["TenantDatabase:Password"] = "secret"
        }).Build();
        var factory = new TenantConnectionFactory(config);

        var tenant = new MySqlConnectionStringBuilder(factory.BuildConnectionString("tenant_42"));
        var server = new MySqlConnectionStringBuilder(factory.BuildServerConnectionString());

        Assert.Equal("db.internal", tenant.Server);
        Assert.Equal((uint)3307, tenant.Port);
        Assert.Equal("tenant_42", tenant.Database);
        Assert.Equal("", server.Database);
    }

    [Fact]
    public void Defaults_to_mysql_port()
    {
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>()).Build();
        var connection = new MySqlConnectionStringBuilder(new TenantConnectionFactory(config).BuildConnectionString("tenant"));
        Assert.Equal((uint)3306, connection.Port);
    }
}
