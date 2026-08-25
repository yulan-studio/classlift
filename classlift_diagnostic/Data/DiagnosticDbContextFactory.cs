using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ClassLift.Diagnostic.Data;

public sealed class DiagnosticDbContextFactory : IDesignTimeDbContextFactory<DiagnosticDbContext>
{
    public DiagnosticDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<DiagnosticDbContext>()
            .UseMySql(
                "Server=localhost;Port=3306;Database=classlift_diagnostic;User=root;Password=development;",
                new MySqlServerVersion(new Version(8, 0, 36)))
            .Options;
        return new DiagnosticDbContext(options);
    }
}
