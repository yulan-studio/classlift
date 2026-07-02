using Billing.Data;
using Billing.Interfaces;
using Billing.Middleware;
using Billing.Services.Billing;
using Billing.Services.Jobs;
using Billing.Services.Provisioning;
using Microsoft.AspNetCore.Connections;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;




var builder = WebApplication.CreateBuilder(args);

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

builder.Services.AddControllersWithViews();


var dbHost = Environment.GetEnvironmentVariable("TenantDatabase__Host");

string masterConnectionString;

if (!string.IsNullOrWhiteSpace(dbHost))
{
    // Running on Railway
    var dbPort = Environment.GetEnvironmentVariable("TenantDatabase__Port");
    var dbUser = Environment.GetEnvironmentVariable("TenantDatabase__User");
    var dbPassword = Environment.GetEnvironmentVariable("TenantDatabase__Password");

    masterConnectionString =
        $"Server={dbHost};" +
        $"Port={dbPort};" +
        $"Database=classlift_platform;" +
        $"User={dbUser};" +
        $"Password={dbPassword};";
}
else
{
    // Running locally
    masterConnectionString =
        builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("DefaultConnection not found.");
}
;

builder.Services.AddDbContext<BillingDbContext>(options =>
    options.UseMySql(
        masterConnectionString,
        ServerVersion.AutoDetect(masterConnectionString)
    ));


// Add services to the container.
//builder.Services.AddRazorPages();

builder.Services.AddScoped<InvoiceService>();
builder.Services.AddScoped<BillingRunService>();
builder.Services.AddScoped<MonthlyBillingJob>();
builder.Services.AddScoped<DailyBillingJob>();
builder.Services.AddScoped<PaymentService>();
builder.Services.AddScoped<DunningService>();
builder.Services.AddScoped<DunningJob>();
builder.Services.AddScoped<SubscriptionService>();
//Register cache
builder.Services.AddMemoryCache();
builder.Services.AddScoped<FeatureAccessService>();
builder.Services.AddScoped<TenantProvisioningService>();
builder.Services.AddScoped<IDatabaseProvisioner, RailwayDatabaseService>();
builder.Services.AddScoped<ITenantSchemaService, TenantSchemaService>();
builder.Services.AddScoped<ITenantSeedService, TenantSeedService>();
builder.Services.AddScoped<ITenantConnectionStringFactory, TenantConnectionFactory>();








var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

//Enable to find subdomain, customDomain, so we can find database associated with the tenant
app.UseMiddleware<TenantResolutionMiddleware>();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

//app.UseAuthorization();

//app.MapRazorPages();

app.Run();
