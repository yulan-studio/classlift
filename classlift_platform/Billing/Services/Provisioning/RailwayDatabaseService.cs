using Billing.Interfaces;
using MySqlConnector;
using System.Text.RegularExpressions;

namespace Billing.Services.Provisioning;

public class RailwayDatabaseService : IDatabaseProvisioner
{
    private readonly IConfiguration _configuration;

    public RailwayDatabaseService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task CreateDatabaseAsync(string databaseName)
    {
        var connectionString = BuildServerConnectionString();

        await using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        var safeDatabaseName = SanitizeDatabaseName(databaseName);

        await using var command = connection.CreateCommand();
        command.CommandText =
            $"CREATE DATABASE IF NOT EXISTS `{safeDatabaseName}` " +
        "CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;";

        await command.ExecuteNonQueryAsync();
    }

    public async Task DeleteDatabaseAsync(string databaseName)
    {
        var connectionString = BuildServerConnectionString();

        await using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        var safeDatabaseName = SanitizeDatabaseName(databaseName);

        await using var command = connection.CreateCommand();
        command.CommandText = $"DROP DATABASE IF EXISTS `{safeDatabaseName}`;";

        await command.ExecuteNonQueryAsync();
    }

    private string BuildServerConnectionString()
    {
        var host = _configuration["TenantDatabase:Host"];
        var port = _configuration["TenantDatabase:Port"];
        var user = _configuration["TenantDatabase:User"];
        var password = _configuration["TenantDatabase:Password"];

        return $"Server={host};Port={port};User={user};Password={password};";
    }

    private static string SanitizeDatabaseName(string databaseName)
    {
        var safe = databaseName.Trim().ToLower();

        safe = Regex.Replace(safe, @"[^a-z0-9]+", "_");
        safe = Regex.Replace(safe, @"_+", "_");
        safe = safe.Trim('_');

        return $"classlift_{safe}";
    }
}