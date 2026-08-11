using Core.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using MySqlConnector;

namespace Web;

/// <summary>
/// Creates the tenant application context for local EF CLI commands without
/// requiring tenant-resolution middleware to run.
/// </summary>
public sealed class DesignTimeAppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var serverConnection = configuration["ServerConnection"]
            ?? configuration.GetConnectionString("ServerConnection")
            ?? throw new InvalidOperationException("ServerConnection is not configured.");

        var connectionBuilder = new MySqlConnectionStringBuilder(serverConnection)
        {
            Database = "classlift"
        };

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseMySql(
                connectionBuilder.ConnectionString,
                new MySqlServerVersion(new Version(8, 0, 0)))
            .Options;

        return new AppDbContext(options);
    }
}
