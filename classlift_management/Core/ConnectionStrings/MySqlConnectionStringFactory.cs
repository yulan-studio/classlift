using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace Core.ConnectionStrings
{
    public static class MySqlConnectionStringFactory
    {
        private const string PoolSectionName = "ConnectionPool";

        public static MySqlConnectionStringBuilder CreateBuilder(
            IConfiguration configuration,
            string? databaseName = null)
        {
            var baseConnectionString =
                configuration["ServerConnection"]
                ?? configuration.GetConnectionString("ServerConnection")
                ?? throw new InvalidOperationException(
                    "ServerConnection is missing. Configure the Railway environment "
                    + "variable 'ServerConnection' or the configuration key "
                    + "'ConnectionStrings:ServerConnection'.");

            var poolSection = configuration.GetSection(PoolSectionName);
            var builder = new MySqlConnectionStringBuilder(baseConnectionString)
            {
                Pooling = poolSection.GetValue("Enabled", true),
                MinimumPoolSize = poolSection.GetValue<uint>("MinimumPoolSize", 0),
                MaximumPoolSize = poolSection.GetValue<uint>("MaximumPoolSize", 30),
                ConnectionIdleTimeout = poolSection.GetValue<uint>("ConnectionIdleTimeoutSeconds", 180),
                ConnectionLifeTime = poolSection.GetValue<uint>("ConnectionLifeTimeSeconds", 0),
                ConnectionTimeout = poolSection.GetValue<uint>("ConnectionTimeoutSeconds", 15)
            };

            if (builder.MaximumPoolSize == 0)
            {
                throw new InvalidOperationException(
                    "ConnectionPool:MaximumPoolSize must be greater than zero.");
            }

            if (builder.MinimumPoolSize > builder.MaximumPoolSize)
            {
                throw new InvalidOperationException(
                    "ConnectionPool:MinimumPoolSize cannot exceed MaximumPoolSize.");
            }

            if (!string.IsNullOrWhiteSpace(databaseName))
            {
                builder.Database = databaseName;
            }

            return builder;
        }
    }
}
