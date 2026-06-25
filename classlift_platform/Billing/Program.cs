using Billing.Data;
using Billing.Jobs;
using Billing.Services.Billing;
using Billing.Services.Jobs;
using Billing.Services.Provisioning;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<BillingDbContext>(options =>
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString)
    ));


// Add services to the container.
//builder.Services.AddRazorPages();

builder.Services.AddScoped<InvoiceService>();
builder.Services.AddScoped<MonthlyBillingJob>();
builder.Services.AddScoped<PaymentService>();
builder.Services.AddScoped<DunningService>();
builder.Services.AddScoped<DunningJob>();
builder.Services.AddScoped<SubscriptionService>();
//Register cache
builder.Services.AddMemoryCache();
builder.Services.AddScoped<FeatureAccessService>();
builder.Services.AddScoped<TenantProvisioningService>();








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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

//app.UseAuthorization();

//app.MapRazorPages();

app.Run();
