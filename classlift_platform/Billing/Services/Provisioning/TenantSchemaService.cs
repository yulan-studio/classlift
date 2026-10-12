using Billing.Data;
using Billing.Interfaces;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace Billing.Services.Provisioning;

public class TenantSchemaService : ITenantSchemaService
{
    private readonly IWebHostEnvironment _environment;
    private readonly BillingDbContext _billingDbContext;
    private readonly ITenantConnectionStringFactory _connectionStringFactory;
    private readonly ILogger<TenantSchemaService> _logger;

    public TenantSchemaService(IWebHostEnvironment environment, BillingDbContext billingDbContext,
        ITenantConnectionStringFactory connectionStringFactory, ILogger<TenantSchemaService> logger)
    {
        _environment = environment;
        _billingDbContext = billingDbContext;
        _connectionStringFactory = connectionStringFactory;
        _logger = logger;
    }

    public async Task MigrateAllTenantsAsync(CancellationToken cancellationToken = default)
    {
        var tenants = await _billingDbContext.Tenantregistries.AsNoTracking()
            .Where(t => t.DatabaseName != null && t.DatabaseName != "")
            .OrderBy(t => t.TenantRegistryId)
            .Select(t => new { t.TenantRegistryId, t.DatabaseName, t.IsActive })
            .ToListAsync(cancellationToken);
        var failures = new List<Exception>();
        foreach (var tenant in tenants)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                await InitializeSchemaAsync(_connectionStringFactory.BuildConnectionString(tenant.DatabaseName));
                _logger.LogInformation("Tenant migration completed for {DatabaseName} ({Status}).",
                    tenant.DatabaseName, tenant.IsActive ? "Active" : "Inactive");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Tenant migration failed for {TenantRegistryId} ({DatabaseName}).",
                    tenant.TenantRegistryId, tenant.DatabaseName);
                failures.Add(new InvalidOperationException(
                    $"Tenant registry {tenant.TenantRegistryId} ({tenant.DatabaseName}) failed.", ex));
            }
        }
        if (failures.Count > 0)
            throw new AggregateException($"{failures.Count} of {tenants.Count} tenant migrations failed.", failures);
    }

    public async Task InitializeSchemaAsync(string connectionString)
    {
        var folder = Path.Combine(_environment.ContentRootPath, "TenantScripts");
        if (!Directory.Exists(folder)) throw new DirectoryNotFoundException($"TenantScripts folder not found: {folder}");
        var files = Directory.GetFiles(folder, "*.sql").OrderBy(f => f).ToList();
        if (files.Count == 0) throw new Exception("No tenant SQL scripts found.");
        await using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();
        await EnsureMigrationTableAsync(connection);
        foreach (var file in files)
        {
            var name = Path.GetFileName(file);
            if (await ScriptAlreadyAppliedAsync(connection, name)) continue;
            await ExecuteSqlScriptAsync(connection, await File.ReadAllTextAsync(file));
            await MarkScriptAppliedAsync(connection, name);
        }
    }

    private static async Task EnsureMigrationTableAsync(MySqlConnection c)
    {
        await using var cmd = c.CreateCommand();
        cmd.CommandText = "CREATE TABLE IF NOT EXISTS __TenantSchemaMigrations (MigrationID INT NOT NULL AUTO_INCREMENT, ScriptName VARCHAR(255) NOT NULL, AppliedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP, PRIMARY KEY (MigrationID), UNIQUE KEY UX_TenantSchemaMigrations_ScriptName (ScriptName));";
        await cmd.ExecuteNonQueryAsync();
    }
    private static async Task<bool> ScriptAlreadyAppliedAsync(MySqlConnection c, string name)
    {
        await using var cmd = c.CreateCommand();
        cmd.CommandText = "SELECT COUNT(*) FROM __TenantSchemaMigrations WHERE ScriptName = @ScriptName;";
        cmd.Parameters.AddWithValue("@ScriptName", name);
        return Convert.ToInt32(await cmd.ExecuteScalarAsync()) > 0;
    }
    private static async Task MarkScriptAppliedAsync(MySqlConnection c, string name)
    {
        await using var cmd = c.CreateCommand();
        cmd.CommandText = "INSERT INTO __TenantSchemaMigrations (ScriptName) VALUES (@ScriptName);";
        cmd.Parameters.AddWithValue("@ScriptName", name);
        await cmd.ExecuteNonQueryAsync();
    }
    private static async Task ExecuteSqlScriptAsync(MySqlConnection c, string sql)
    {
        foreach (var statement in sql.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            if (string.IsNullOrWhiteSpace(statement)) continue;
            await using var cmd = c.CreateCommand();
            cmd.CommandText = statement;
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
