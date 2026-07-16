using Amazon.S3;
using Core.BackendService;
using Core.Contexts;
using Core.Interfaces;
using Core.Middleware;
using Core.Models;
using Core.Repositories;
using Core.Services;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
//using System.IdentityModel.Tokens.Jwt;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.IdentityModel.Tokens;
//using System.Text;

//using Pomelo.EntityFrameworkCore.MySql;
//using Microsoft.AspNetCore.Mvc;









var builder = WebApplication.CreateBuilder(args);



var baseConnectionString =
    builder.Configuration.GetConnectionString("ServerConnection")
    ?? throw new InvalidOperationException(
        "ConnectionStrings:ServerConnection is missing.");

var platformConnectionBuilder =
    new MySqlConnectionStringBuilder(baseConnectionString)
    {
        Database = "classlift_platform"
    };


var platformConnectionString =
    platformConnectionBuilder.ConnectionString;

//Register the fixed platform BillingDbContext
builder.Services.AddDbContext<BillingDbContext>(options =>
{
    options.UseMySql(
        platformConnectionString,
        ServerVersion.AutoDetect(platformConnectionString));
});

builder.Services.AddScoped<CurrentTenant>();

builder.Services.AddScoped<ITenantConnectionStringFactory, TenantConnectionStringFactory>();

builder.Services.AddDbContext<AppDbContext>(
    (serviceProvider, options) =>
    {
        var currentTenant =
            serviceProvider.GetRequiredService<CurrentTenant>();

        if (!currentTenant.IsResolved)
        {
            throw new InvalidOperationException(
                "Tenant has not been resolved.");
        }

        options.UseMySql(
            currentTenant.ConnectionString!,
            ServerVersion.AutoDetect(currentTenant.ConnectionString));
    });

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

//Make RememberMe working
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.ExpireTimeSpan = TimeSpan.FromDays(14);
    options.SlidingExpiration = true;
});


// 绑定配置
builder.Services.Configure<SmtpSettings>(
builder.Configuration.GetSection("SmtpSettings"));

// 注册 EmailService
builder.Services.AddTransient<EmailService>();




// Add services to the container.
// Full MVC with Views (HTML pages using Razor).
// Controllers that return both Views & JSON (e.g., hybrid APIs).
builder.Services.AddControllersWithViews();

// ? Add session services
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Session timeout (adjust as needed)
    options.Cookie.HttpOnly = true; // Security: Prevents client-side script from accessing cookies
    options.Cookie.IsEssential = true; // Required for session to work
});




// ? Add distributed memory cache (required for session to work)
builder.Services.AddDistributedMemoryCache();


// Add JWT configuration to the dependency injection container
//builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));


builder.Services.AddScoped<IUserRepository<User>, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

// Add UserService
builder.Services.AddScoped<IStaffRepository, StaffRepository>();
builder.Services.AddScoped<IStaffService, StaffService>();



builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IAdminService, AdminService>();


builder.Services.AddScoped<ICoachRepository, CoachRepository>();
builder.Services.AddScoped<ICoachService, CoachService>();

builder.Services.AddScoped<ICoachIncomeRepository, CoachIncomeRepository>();
builder.Services.AddScoped<ICoachIncomeService, CoachIncomeService>();

builder.Services.AddScoped<IChildBalanceRepository, ChildBalanceRepository>();
builder.Services.AddScoped<IChildBalanceService, ChildBalanceService>();


builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<ICourseService, CourseService>();

builder.Services.AddScoped<IActivityRepository, ActivityRepository>();
builder.Services.AddScoped<IActivityService, ActivityService>();

builder.Services.AddScoped<IChildService, ChildService>();
builder.Services.AddScoped<IChildRepository, ChildRepository>();

builder.Services.AddScoped<IEmergencyContactService, EmergencyContactService>();
builder.Services.AddScoped<IEmergencyContactRepository, EmergencyContactRepository>();

builder.Services.AddScoped<IParentService, ParentService>();
builder.Services.AddScoped<IParentRepository, ParentRepository>();

builder.Services.AddScoped<IParentChildRepository, ParentChildRepository>();
builder.Services.AddScoped<IParentChildService, ParentChildService>();

builder.Services.AddScoped<ICourseEnrollmentRepository, CourseEnrollmentRepository>();
builder.Services.AddScoped<ICourseEnrollmentService, CourseEnrollmentService>();

builder.Services.AddScoped<IActivityEnrollmentService, ActivityEnrollmentService>();
builder.Services.AddScoped<IActivityEnrollmentRepository, ActivityEnrollmentRepository>();

builder.Services.AddScoped<IPaymentPackageRepository, PaymentPackageRepository>();
builder.Services.AddScoped<IPaymentPackageService, PaymentPackageService>();

builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

builder.Services.AddScoped<IFeeRepository, FeeRepository>();
builder.Services.AddScoped<IFeeService, FeeService>();

builder.Services.AddScoped<IChildCalendarRepository, ChildCalendarRepository>();
builder.Services.AddScoped<IChildCalendarService, ChildCalendarService>();



builder.Services.AddScoped<ICityRepository, CityRepository>();
builder.Services.AddScoped<ICityService, CityService>();

builder.Services.AddScoped<ISpecialtyRepository, SpecialtyRepository>();
builder.Services.AddScoped<ISpecialtyService, SpecialtyService>();

builder.Services.AddScoped<ICoachSpecialtyRepository, CoachSpecialtyRepository>();
builder.Services.AddScoped<ICoachSpecialtyService, CoachSpecialtyService>();

builder.Services.AddScoped<IUserRegistrationService, UserRegistrationService>();




//var connectionString1 = Environment.GetEnvironmentVariable("DefaultConnection");




builder.Services.AddHostedService<ActivityStatusUpdater>();

builder.Services.AddHostedService<GroupCourseStatusUpdater>();

builder.Services.AddHostedService<RootCourseStatusUpdater>(); //Set Course to completed if completed number == session Count


// Add Identity
builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});


// To get connectionstring
//try
//{
//    var connectionString = Environment.GetEnvironmentVariable("PlatformConnection")
//   ?? builder.Configuration.GetConnectionString("PlatformConnection");

//    builder.Services.AddDbContext<AppDbContext>(options =>
//            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString),
//                mysqlOptions =>
//                {
//                    mysqlOptions.EnableRetryOnFailure(
//                        maxRetryCount: 5,
//                        maxRetryDelay: TimeSpan.FromSeconds(10),
//                        errorNumbersToAdd: null);
//                }
//            )
//    );
//}

//catch (Exception ex)
//{
//    Console.WriteLine("DB setup failed: " + ex.Message);
//    throw;
//}






//builder.Services.AddHttpContextAccessor();

//builder.Services.AddDbContext<BillingDbContext>(options =>
//    options.UseMySql(
//        builder.Configuration.GetConnectionString("PlatformConnection"),
//        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("PlatformConnection"))));


////Get DefaultConnectionString in Debug mode when TenantConnectionString is not available
//builder.Services.AddDbContext<AppDbContext>((serviceProvider, options) =>
//{
//    var httpContextAccessor =
//        serviceProvider.GetRequiredService<IHttpContextAccessor>();

//    var configuration =
//        serviceProvider.GetRequiredService<IConfiguration>();

//    var connectionString = httpContextAccessor.HttpContext?
//        .Items["TenantConnectionString"]?.ToString();

//    if (string.IsNullOrWhiteSpace(connectionString))
//    {
//        connectionString = configuration.GetConnectionString("DefaultConnection");
//    }

//    if (string.IsNullOrWhiteSpace(connectionString))
//        throw new InvalidOperationException("Tenant connection string not found.");

//    options.UseMySql(
//        connectionString,
//        ServerVersion.AutoDetect(connectionString),
//        mysqlOptions =>
//        {
//            mysqlOptions.EnableRetryOnFailure(
//                maxRetryCount: 5,
//                maxRetryDelay: TimeSpan.FromSeconds(10),
//                errorNumbersToAdd: null);
//        });
//});






var cultureInfo = new System.Globalization.CultureInfo("en-CA");
System.Globalization.CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;


//Register S3 Client
builder.Services.Configure<Core.R2.CloudflareR2Options>(
    builder.Configuration.GetSection("CloudflareR2"));
builder.Services.AddSingleton<Core.R2.R2StorageService>();


var app = builder.Build();




//Add / health endpoint
//app.MapGet("/health", () => "OK");
app.MapGet("/health", () => Results.Ok("Healthy"));



// Automatically create roles and an admin user
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
   // await SeedRolesAndAdmin(services);
}



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

//app.UseForwardedHeaders(new ForwardedHeadersOptions
//{
//    ForwardedHeaders = ForwardedHeaders.XForwardedProto
//});

//app.UseHttpsRedirection();

app.UseStaticFiles();

//This will ensure any request to the root URL redirects to /User/AddAdmin.
//app.Use(async (context, next) =>
//{
//    if (context.Request.Path == "/")
//    {
//        context.Response.Redirect("/Account/Login");
//        //context.Response.Redirect("/Staff/List");
//        return;
//    }
//    await next();
//});

app.UseRouting();

//Get Tenant ConnectionString
app.UseMiddleware<TenantResolutionMiddleware>();
// ? Enable session middleware
app.UseSession();



app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();



app.UseEndpoints(endpoints =>
{
    // Map controller routes
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}" // Default route points to Home/Index
    );
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}");


app.MapControllerRoute(
    name: "childRoute",
    pattern: "{controller=Child}/{action=ManageRegistrations}/{childId?}");




app.Run();
