using Billing.Interfaces;
using MySqlConnector;


namespace Billing.Services.Provisioning;

public class TenantSchemaService : ITenantSchemaService
{
    private readonly IWebHostEnvironment _environment;


    public TenantSchemaService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task InitializeSchemaAsync(string connectionString)
    {
        var scriptsFolder = Path.Combine(
            _environment.ContentRootPath,
            "TenantScripts"
        );

        if (!Directory.Exists(scriptsFolder))
            throw new DirectoryNotFoundException($"TenantScripts folder not found: {scriptsFolder}");

        var scriptFiles = Directory
            .GetFiles(scriptsFolder, "*.sql")
            .OrderBy(f => f)
            .ToList();

        if (!scriptFiles.Any())
            throw new Exception("No tenant SQL scripts found.");

        await using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        await EnsureMigrationTableAsync(connection);

        foreach (var scriptFile in scriptFiles)
        {
            var scriptName = Path.GetFileName(scriptFile);

            if (await ScriptAlreadyAppliedAsync(connection, scriptName))
                continue;

            var sql = await File.ReadAllTextAsync(scriptFile);

            await ExecuteSqlScriptAsync(connection, sql);

            await MarkScriptAppliedAsync(connection, scriptName);
        }
    }

    private static async Task EnsureMigrationTableAsync(MySqlConnection connection)
    {
        var sql = @"
CREATE TABLE IF NOT EXISTS __TenantSchemaMigrations (
    MigrationID INT NOT NULL AUTO_INCREMENT,
    ScriptName VARCHAR(255) NOT NULL,
    AppliedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (MigrationID),
    UNIQUE KEY UX_TenantSchemaMigrations_ScriptName (ScriptName)
);";

        await using var command = connection.CreateCommand();
        command.CommandText = sql;
        await command.ExecuteNonQueryAsync();
    }

    private static async Task<bool> ScriptAlreadyAppliedAsync(
        MySqlConnection connection,
        string scriptName)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = @"
SELECT COUNT(*)
FROM __TenantSchemaMigrations
WHERE ScriptName = @ScriptName;";

        command.Parameters.AddWithValue("@ScriptName", scriptName);

        var result = Convert.ToInt32(await command.ExecuteScalarAsync());

        return result > 0;
    }

    private static async Task MarkScriptAppliedAsync(
        MySqlConnection connection,
        string scriptName)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = @"
INSERT INTO __TenantSchemaMigrations (ScriptName)
VALUES (@ScriptName);";

        command.Parameters.AddWithValue("@ScriptName", scriptName);

        await command.ExecuteNonQueryAsync();
    }

    private static async Task ExecuteSqlScriptAsync(
        MySqlConnection connection,
        string sql)
    {
        var statements = sql
            .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (var statement in statements)
        {
            if (string.IsNullOrWhiteSpace(statement))
                continue;

            await using var command = connection.CreateCommand();
            command.CommandText = statement;
            await command.ExecuteNonQueryAsync();
        }
    }
}