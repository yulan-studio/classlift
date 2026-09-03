using Microsoft.Extensions.Configuration;

namespace Core.ConnectionStrings
{
    public sealed class TenantConnectionStringFactory
        : ITenantConnectionStringFactory
    {
        private readonly IConfiguration _configuration;

        public TenantConnectionStringFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string BuildConnectionString(string databaseName)
        {
            return MySqlConnectionStringFactory
                .CreateBuilder(_configuration, databaseName)
                .ConnectionString;
        }

        public string BuildServerConnectionString()
        {
            return MySqlConnectionStringFactory
                .CreateBuilder(_configuration)
                .ConnectionString;
        }
    }
}
