using Billing.Data;
using Microsoft.EntityFrameworkCore;

namespace Billing.Middleware;

public class TenantResolutionMiddleware
{
    private readonly RequestDelegate _next;

    public TenantResolutionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        BillingDbContext billingDbContext)
    {
        var host = context.Request.Host.Host.ToLower();

        var tenant = await billingDbContext.Tenantregistries
            .FirstOrDefaultAsync(t =>
                t.IsActive &&
                t.CustomDomain != null &&
                t.CustomDomain.ToLower() == host);

        if (tenant == null)
        {
            var subdomain = GetSubdomain(host);

            if (!string.IsNullOrWhiteSpace(subdomain))
            {
                tenant = await billingDbContext.Tenantregistries
                    .FirstOrDefaultAsync(t =>
                        t.IsActive &&
                        t.Subdomain != null &&
                        t.Subdomain.ToLower() == subdomain);
            }
        }

        if (tenant != null)
        {
            context.Items["OrganizationId"] = tenant.OrganizationId;
            context.Items["TenantDatabaseName"] = tenant.DatabaseName;
            context.Items["TenantConnectionString"] = tenant.ConnectionString;
        }

        await _next(context);
    }

    private static string? GetSubdomain(string host)
    {
        if (host.Contains("localhost"))
            return null;

        var parts = host.Split('.');

        if (parts.Length < 3)
            return null;

        return parts[0];
    }
}