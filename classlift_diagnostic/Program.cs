using ClassLift.Diagnostic.Data;
using ClassLift.Diagnostic.Models;
using ClassLift.Diagnostic.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.RateLimiting;

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
builder.Services.AddHttpClient<AiReportService>(client => client.Timeout = TimeSpan.FromSeconds(45));
builder.Services.AddProblemDetails();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.Cookie.Name = "classlift_admin";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.SecurePolicy = builder.Environment.IsDevelopment() ? CookieSecurePolicy.SameAsRequest : CookieSecurePolicy.Always;
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
    options.LoginPath = "/admin/login";
    options.Events.OnRedirectToLogin = context =>
    {
        if (context.Request.Path.StartsWithSegments("/api")) context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        else context.Response.Redirect(context.RedirectUri);
        return Task.CompletedTask;
    };
});
builder.Services.AddAuthorization();
builder.Services.AddRateLimiter(options => options.AddPolicy("admin-login", context =>
    RateLimitPartition.GetFixedWindowLimiter(
        context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
        _ => new FixedWindowRateLimiterOptions { PermitLimit = 5, Window = TimeSpan.FromMinutes(1), QueueLimit = 0 })));

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
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/api/health", () => Results.Ok(new
{
    status = "healthy",
    database = string.IsNullOrWhiteSpace(connectionString) ? "in-memory" : "mysql",
    timestamp = DateTimeOffset.UtcNow
}));

app.MapPost("/api/diagnostics", async (
    CreateDiagnosticRequest request,
    ScoringService scoring,
    AiReportService aiReports,
    IDiagnosticRepository repository,
    CancellationToken cancellationToken) =>
{
    var errors = request.Validate();
    if (errors.Count > 0) return Results.ValidationProblem(errors);

    var result = scoring.Calculate(request);
    var diagnostic = DiagnosticLead.From(request, result);
    var report = await aiReports.GenerateAsync(request, result, cancellationToken);
    diagnostic.AiSummary = System.Text.Json.JsonSerializer.Serialize(report);
    diagnostic.RecommendedModulesJson = System.Text.Json.JsonSerializer.Serialize(report.RelevantCapabilities);
    await repository.AddAsync(diagnostic, cancellationToken);

    return Results.Created($"/api/diagnostics/{diagnostic.Id}", new DiagnosticResponse(
        diagnostic.Id, diagnostic.CreatedAt, result, diagnostic.LeadIntent, report));
});

app.MapGet("/api/diagnostics/{id:guid}", async (
    Guid id,
    IDiagnosticRepository repository,
    CancellationToken cancellationToken) =>
{
    var diagnostic = await repository.FindAsync(id, cancellationToken);
    return diagnostic is null ? Results.NotFound() : Results.Ok(diagnostic.ToResponse());
});

app.MapGet("/admin/login", () => Results.File(
    Path.Combine(app.Environment.ContentRootPath, "Admin", "login.html"), "text/html; charset=utf-8"));

app.MapPost("/api/admin/login", async (AdminLoginRequest request, HttpContext context) =>
{
    var expectedUsername = builder.Configuration["ADMIN_USERNAME"];
    var expectedPassword = builder.Configuration["ADMIN_PASSWORD"];
    if (string.IsNullOrWhiteSpace(expectedUsername) || string.IsNullOrWhiteSpace(expectedPassword))
        return Results.Problem("管理员凭据尚未配置。", statusCode: StatusCodes.Status503ServiceUnavailable);
    if (!SecureEquals(request.Username, expectedUsername) || !SecureEquals(request.Password, expectedPassword))
        return Results.Unauthorized();

    var principal = new ClaimsPrincipal(new ClaimsIdentity(
        [new Claim(ClaimTypes.Name, expectedUsername), new Claim(ClaimTypes.Role, "SalesAdmin")],
        CookieAuthenticationDefaults.AuthenticationScheme));
    await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
        new AuthenticationProperties { IsPersistent = false });
    return Results.Ok(new { authenticated = true });
}).RequireRateLimiting("admin-login");

app.MapPost("/api/admin/logout", async (HttpContext context) =>
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Ok();
}).RequireAuthorization();

app.MapGet("/api/admin/session", (ClaimsPrincipal user) => Results.Ok(new
{
    authenticated = user.Identity?.IsAuthenticated == true,
    username = user.Identity?.Name
})).RequireAuthorization();

app.MapGet("/admin", () => Results.File(
    Path.Combine(app.Environment.ContentRootPath, "Admin", "dashboard.html"), "text/html; charset=utf-8"))
    .RequireAuthorization();

app.MapGet("/api/admin/leads", async (
    string? search, string? intent, int? page, int? pageSize,
    IDiagnosticRepository repository, CancellationToken cancellationToken) =>
{
    var safePage = Math.Max(1, page ?? 1);
    var safePageSize = Math.Clamp(pageSize ?? 25, 1, 100);
    return Results.Ok(await repository.ListAsync(search, intent, safePage, safePageSize, cancellationToken));
}).RequireAuthorization();

app.MapGet("/api/admin/leads/{id:guid}", async (
    Guid id, IDiagnosticRepository repository, CancellationToken cancellationToken) =>
{
    var lead = await repository.FindAsync(id, cancellationToken);
    return lead is null ? Results.NotFound() : Results.Ok(AdminLeadDetail.From(lead));
}).RequireAuthorization();

app.MapGet("/api/admin/leads.csv", async (
    string? search, string? intent, IDiagnosticRepository repository, CancellationToken cancellationToken) =>
{
    var page = await repository.ListAsync(search, intent, 1, 10_000, cancellationToken);
    var csv = new StringBuilder("Created At,Name,Email,Organization,Website URL,Business Type,Student Count,Primary Pain,Timeline,Score,Classification,Lead Intent\r\n");
    foreach (var lead in page.Items)
        csv.AppendLine(string.Join(',', new[] { lead.CreatedAt.ToString("O"), lead.Name, lead.Email, lead.Organization, lead.WebsiteUrl,
            lead.BusinessType, lead.StudentCount, lead.PrimaryPain, lead.ImplementationTimeline,
            lead.TotalScore.ToString(), lead.Classification, lead.LeadIntent }.Select(Csv)));
    return Results.File(Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(csv.ToString())).ToArray(),
        "text/csv; charset=utf-8", $"classlift-leads-{DateTime.UtcNow:yyyyMMdd}.csv");
}).RequireAuthorization();

app.MapFallbackToFile("index.html");
app.Run();

static bool SecureEquals(string? provided, string expected)
{
    if (provided is null) return false;
    var left = SHA256.HashData(Encoding.UTF8.GetBytes(provided));
    var right = SHA256.HashData(Encoding.UTF8.GetBytes(expected));
    return CryptographicOperations.FixedTimeEquals(left, right);
}

static string Csv(string? value)
{
    var safe = value ?? "";
    if (safe.Length > 0 && "=+-@".Contains(safe[0])) safe = "'" + safe;
    return $"\"{safe.Replace("\"", "\"\"")}\"";
}

public partial class Program;
