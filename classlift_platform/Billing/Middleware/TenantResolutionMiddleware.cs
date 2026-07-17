using Billing.Data;
using Billing.Interfaces;
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
        BillingDbContext billingDbContext,
        ITenantConnectionStringFactory connectionFactory)
    {
        var host = context.Request.Host.Host.ToLower();

        // Ignore localhost during development
        if (host.Contains("localhost"))
        {
            await _next(context);
            return;
        }

        // 1. Try Custom Domain
        var tenant = await billingDbContext.Tenantregistries
            .FirstOrDefaultAsync(t =>
                t.IsActive &&
                t.CustomDomain != null &&
                t.CustomDomain.ToLower() == host);

        // 2. Try ClassLift Subdomain
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

        // 3. Tenant found
        if (tenant != null)
        {
            context.Items["OrganizationId"] = tenant.OrganizationId;
            context.Items["DatabaseName"] = tenant.DatabaseName;

            var connectionString =
                connectionFactory.BuildConnectionString(
                    tenant.DatabaseName);

            context.Items["TenantConnectionString"] = connectionString;
        }

        await _next(context);
    }

    private static string? GetSubdomain(string host)
    {
        var parts = host.Split('.');

        if (parts.Length < 3)
            return null;
        if(parts.Length == 3)
            return parts[0];
        if (parts.Length == 4)
            return parts[0] + '.' + parts[1];
        else
            return null;
    }
}