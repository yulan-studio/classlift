using Core.Contexts;
using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Core.Middleware
{
    public sealed class TenantResolutionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<TenantResolutionMiddleware> _logger;

        public TenantResolutionMiddleware(
            RequestDelegate next,
            ILogger<TenantResolutionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(
            HttpContext context,
            BillingDbContext billingDbContext,
            ITenantConnectionStringFactory connectionFactory,
            CurrentTenant currentTenant)
        {
            var host = context.Request.Host.Host
                .Trim()
                .ToLowerInvariant();

            _logger.LogInformation("Middleware run");
                  

            // Ignore localhost
            if (host is "localhost" or "127.0.0.1")
            {
                await _next(context);
                return;
            }

            // Platform hosts are not tenant hosts.
            if (IsPlatformHost(host))
            {
                await _next(context);
                return;
            }

            // 1. Try exact custom-domain match.
            var tenant = await billingDbContext.TenantRegistries
                .AsNoTracking()
                .FirstOrDefaultAsync(t =>
                    t.IsActive &&
                    t.CustomDomain != null &&
                    t.CustomDomain.ToLower() == host);

            // 2. Try ClassLift-managed tenant subdomain.
            if (tenant is null)
            {
                var subdomain = GetTenantSubdomain(host);

                if (!string.IsNullOrWhiteSpace(subdomain))
                {
                    tenant = await billingDbContext.TenantRegistries
                        .AsNoTracking()
                        .FirstOrDefaultAsync(t =>
                            t.IsActive &&
                            t.Subdomain != null &&
                            t.Subdomain.ToLower() == subdomain);
                }
            }

            // You may choose to return 404 here for tenant-only applications.
            if (tenant is null)
            {
                _logger.LogWarning(
                    "No active tenant was found for host {Host}",
                    host);

                await _next(context);
                return;
            }

            var tenantConnectionString =
                connectionFactory.BuildConnectionString(
                    tenant.DatabaseName);

            currentTenant.OrganizationId = tenant.OrganizationId;
            currentTenant.Subdomain = tenant.Subdomain;
            currentTenant.DatabaseName = tenant.DatabaseName;
            currentTenant.ConnectionString = tenantConnectionString;
            _logger.LogInformation("tenant was found for host {Host}", host);
            // Optional backward compatibility for existing code.
            context.Items["CurrentTenant"] = currentTenant;

            _logger.LogInformation(
                "Tenant resolved. Host={Host}, Subdomain={Subdomain}, Database={Database}",
                host,
                tenant.Subdomain,
                tenant.DatabaseName);

           

            await _next(context);
        }

        private static bool IsPlatformHost(string host)
        {
            return host is
                "classlift.ca" or
                "www.classlift.ca" or
                "dev.classlift.ca" or
                "staging.classlift.ca" or
                "platform.classlift.ca" or
                "dev.platform.classlift.ca" or
                "staging.platform.classlift.ca";
        }

        private static string? GetTenantSubdomain(string host)
        {
            string[] suffixes =
            {
                ".dev.classlift.ca",
                ".staging.classlift.ca",
                ".classlift.ca"
            };

            foreach (var suffix in suffixes)
            {
                if (!host.EndsWith(
                        suffix,
                        StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var subdomain = host[..^suffix.Length];

                // Reject malformed values such as:
                // school.region.dev.classlift.ca
                if (string.IsNullOrWhiteSpace(subdomain) ||
                    subdomain.Contains('.'))
                {
                    return null;
                }

                return subdomain;
            }

            return null;
        }
    }
}