using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ClassLift.Diagnostic.Data;

public sealed class DiagnosticDbContextFactory : IDesignTimeDbContextFactory<DiagnosticDbContext>
{
    public DiagnosticDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();
        var connection = DatabaseConnection.Build(configuration)
            ?? throw new InvalidOperationException("未找到开发环境数据库连接，请配置 ConnectionStrings:MySql。");
        var options = new DbContextOptionsBuilder<DiagnosticDbContext>()
            .UseMySql(connection, ServerVersion.AutoDetect(connection))
            .Options;
        return new DiagnosticDbContext(options);
    }
}
