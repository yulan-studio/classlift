using Billing.Configuration;
using Billing.Data;
using Billing.Interfaces;
using Billing.Services.Billing;
using Billing.Services.Jobs;
using Billing.Services.Notifications;
using Billing.Services.Provisioning;
using Hangfire;
using Hangfire.MySql;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using System.Globalization;




//For api call, determine the environment from the request's host
var builder = WebApplication.CreateBuilder(args);


builder.Services.AddHttpContextAccessor();

//Require authentication globally
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new AuthorizeFilter(
        ManagementAuthorization.AuthenticatedUserPolicy));
});



builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<BillingDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

builder.Services.AddAuthorization();


var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

var dbHost = builder.Configuration["TenantDatabase:Host"];
var dbPort = builder.Configuration["TenantDatabase:Port"];
var dbUser = builder.Configuration["TenantDatabase:User"];
var dbPassword = builder.Configuration["TenantDatabase:Password"];

Console.WriteLine("DB HOST: " + builder.Configuration["TenantDatabase:Host"]);
Console.WriteLine("DB PORT: " + builder.Configuration["TenantDatabase:Port"]);
Console.WriteLine("DB USER: " + builder.Configuration["TenantDatabase:User"]);

var masterConnectionString = new MySqlConnectionStringBuilder
{
    Server = dbHost,
    Port = uint.Parse(dbPort ?? "3306"),
    Database = "classlift_platform",
    UserID = dbUser,
    Password = dbPassword,
    Pooling = true,
    MinimumPoolSize = 0,
    MaximumPoolSize = 15,
    ConnectionIdleTimeout = 60,
    ConnectionLifeTime = 300,
    ConnectionTimeout = 15
}.ConnectionString;

// Railway currently runs this application against MySQL 8.4.8. Keeping the
// version explicit avoids opening a discovery connection for every DbContext.
var mysqlServerVersion = ServerVersion.Parse(
    builder.Configuration["TenantDatabase:ServerVersion"] ?? "8.4.8-mysql");



builder.Services.AddDbContext<BillingDbContext>(options =>
    options.UseMySql(
        masterConnectionString,
        mysqlServerVersion
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
builder.Services.AddScoped<OrganizationService>();
//Register cache
builder.Services.AddMemoryCache();
builder.Services.AddScoped<FeatureAccessService>();
builder.Services.AddScoped<TenantProvisioningService>();
builder.Services.AddScoped<IDatabaseProvisioner, RailwayDatabaseService>();
builder.Services.AddScoped<ITenantSchemaService, TenantSchemaService>();
builder.Services.AddScoped<ITenantSeedService, TenantSeedService>();
builder.Services.AddScoped<ITenantConnectionStringFactory, TenantConnectionFactory>();
builder.Services.AddScoped<IOrganizationSignupService, OrganizationSignupService>();
builder.Services.AddScoped<ITenantIdentitySeeder, TenantIdentitySeeder>();
builder.Services.AddScoped<StartupAdminSeeder>();

builder.Services.Configure<PlatformAdminOptions>(
    builder.Configuration.GetSection("PlatformAdmin"));

builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));

builder.Services.AddTransient<EmailService>();



builder.Services.AddHangfire(config =>
    config.UseStorage(
        new MySqlStorage(
            masterConnectionString,
            new MySqlStorageOptions()
        )));

builder.Services.AddHangfireServer(options =>
{
    options.WorkerCount = 2;
});

//Allow CORS from your marketing site only
builder.Services.AddCors(options =>
{
    options.AddPolicy("Classlift", policy =>
    {
        policy.WithOrigins("https://dev.classlift.ca", "https://staging.classlift.ca", "https://classlift.ca")
              //.SetIsOriginAllowedToAllowWildcardSubdomains()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});



var app = builder.Build();

var jobOptions = new RecurringJobOptions
{
    TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time")
};

using (var scope = app.Services.CreateScope())
{
    var recurringJobManager =
        scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();

    recurringJobManager.AddOrUpdate<DunningJob>(
        "daily-dunning",
        job => job.RunAsync(),
        "0 2 * * *",
        jobOptions);

    recurringJobManager.AddOrUpdate<DailyBillingJob>(
        "daily-billing",
        job => job.RunAsync(),
        "30 2 * * *",
        jobOptions);

    recurringJobManager.AddOrUpdate<MonthlyBillingJob>(
        "monthly-billing",
        job => job.RunAsync(),
        "0 2 1 * *",
        jobOptions);
}

//Set Culture
var culture = new CultureInfo("en-CA");

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(culture),
    SupportedCultures = new[] { culture },
    SupportedUICultures = new[] { culture }
});

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

app.UseCors("Classlift");

app.UseAuthentication();
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var startupAdminSeeder = scope.ServiceProvider.GetRequiredService<StartupAdminSeeder>();
    await startupAdminSeeder.SeedAsync();
}

//Enable to find subdomain, customDomain, so we can find database associated with the tenant
//Need to create database, create tables
//from posted data in portal website -> platform website /api/public/signup (wrong)
//I can't remember why we need this
//app.UseMiddleware<TenantResolutionMiddleware>();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");



//app.MapRazorPages();



app.Run();
