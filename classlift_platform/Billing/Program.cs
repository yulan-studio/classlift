using Billing.Configuration;
using Billing.Data;
using Billing.Interfaces;
using Billing.Middleware;
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





var builder = WebApplication.CreateBuilder(args);



//Require authentication globally
builder.Services.AddControllersWithViews(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();

    options.Filters.Add(new AuthorizeFilter(policy));
});



builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<BillingDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

builder.Services.AddAuthorization();


var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
//builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

builder.Services.AddControllersWithViews();


var dbHost = builder.Configuration["TenantDatabase:Host"];
var dbPort = builder.Configuration["TenantDatabase:Port"];
var dbUser = builder.Configuration["TenantDatabase:User"];
var dbPassword = builder.Configuration["TenantDatabase:Password"];

Console.WriteLine("DB HOST: " + builder.Configuration["TenantDatabase:Host"]);
Console.WriteLine("DB PORT: " + builder.Configuration["TenantDatabase:Port"]);
Console.WriteLine("DB USER: " + builder.Configuration["TenantDatabase:User"]);

string masterConnectionString =
        $"Server={dbHost};" +
        $"Port={dbPort};" +
        $"Database=classlift_platform;" +
        $"User={dbUser};" +
        $"Password={dbPassword};";



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
builder.Services.AddScoped<IOrganizationSignupService, OrganizationSignupService>();
builder.Services.AddScoped<ITenantIdentitySeeder, TenantIdentitySeeder>();

builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));

builder.Services.AddTransient<EmailService>();



builder.Services.AddHangfire(config =>
    config.UseStorage(
        new MySqlStorage(
            masterConnectionString,
            new MySqlStorageOptions()
        )));

builder.Services.AddHangfireServer();

//Allow CORS from your marketing site only
builder.Services.AddCors(options =>
{
    options.AddPolicy("Classlift", policy =>
    {
        policy.WithOrigins("https://classlift.ca")
              .SetIsOriginAllowedToAllowWildcardSubdomains()
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

app.UseCors("MarketingSite");

app.UseAuthentication();
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    var email = "info@classlift.ca";
    var password = "Class123!";

    if (await userManager.FindByEmailAsync(email) == null)
    {
        var user = new IdentityUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                Console.WriteLine($"USER CREATE ERROR: {error.Code} - {error.Description}");
            }
        }
        else
        {
            Console.WriteLine("Admin user created successfully.");
        }
    }
}

//Enable to find subdomain, customDomain, so we can find database associated with the tenant
app.UseMiddleware<TenantResolutionMiddleware>();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");



//app.MapRazorPages();



app.Run();
