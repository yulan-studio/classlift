using ClassLift.Diagnostic.Data;
using ClassLift.Diagnostic.Models;
using ClassLift.Diagnostic.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var railwayPort = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrWhiteSpace(railwayPort))
    builder.WebHost.UseUrls($"http://0.0.0.0:{railwayPort}");

var connectionString = DatabaseConnection.Build(builder.Configuration);
if (!string.IsNullOrWhiteSpace(connectionString))
{
    builder.Services.AddDbContext<DiagnosticDbContext>(options =>
        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
    builder.Services.AddScoped<IDiagnosticRepository, MySqlDiagnosticRepository>();
}
else
{
    builder.Services.AddSingleton<IDiagnosticRepository, InMemoryDiagnosticRepository>();
}

builder.Services.AddSingleton<ScoringService>();
builder.Services.AddProblemDetails();

var app = builder.Build();

if (!string.IsNullOrWhiteSpace(connectionString))
{
    using var scope = app.Services.CreateScope();
    var database = scope.ServiceProvider.GetRequiredService<DiagnosticDbContext>();
    await database.Database.MigrateAsync();
}

app.UseExceptionHandler();
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/api/health", () => Results.Ok(new
{
    status = "healthy",
    database = string.IsNullOrWhiteSpace(connectionString) ? "in-memory" : "mysql",
    timestamp = DateTimeOffset.UtcNow
}));

app.MapPost("/api/diagnostics", async (
    CreateDiagnosticRequest request,
    ScoringService scoring,
    IDiagnosticRepository repository,
    CancellationToken cancellationToken) =>
{
    var errors = request.Validate();
    if (errors.Count > 0) return Results.ValidationProblem(errors);

    var result = scoring.Calculate(request);
    var diagnostic = DiagnosticLead.From(request, result);
    await repository.AddAsync(diagnostic, cancellationToken);

    return Results.Created($"/api/diagnostics/{diagnostic.Id}", new DiagnosticResponse(
        diagnostic.Id, diagnostic.CreatedAt, result, diagnostic.LeadIntent));
});

app.MapGet("/api/diagnostics/{id:guid}", async (
    Guid id,
    IDiagnosticRepository repository,
    CancellationToken cancellationToken) =>
{
    var diagnostic = await repository.FindAsync(id, cancellationToken);
    return diagnostic is null ? Results.NotFound() : Results.Ok(diagnostic.ToResponse());
});

app.MapFallbackToFile("index.html");
app.Run();

public partial class Program;
