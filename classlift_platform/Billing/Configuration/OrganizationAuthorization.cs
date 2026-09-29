using System.Security.Claims;
using Billing.Data;
using Microsoft.EntityFrameworkCore;

namespace Billing.Configuration;

public static class OrganizationAuthorization
{
    public const string Policy = "OrganizationAdmin";

    public static string? GetUserId(ClaimsPrincipal principal) =>
        principal.FindFirstValue(ClaimTypes.NameIdentifier);

    public static async Task<bool> HasAccessAsync(
        BillingDbContext context,
        ClaimsPrincipal principal,
        int organizationId,
        CancellationToken cancellationToken = default)
    {
        if (principal.IsInRole("PlatformAdmin") || principal.IsInRole("Admin"))
            return true;

        var userId = GetUserId(principal);
        return userId != null && await context.OrganizationAdmins
            .AnyAsync(a => a.UserId == userId &&
                          a.OrganizationId == organizationId &&
                          a.IsActive,
                cancellationToken);
    }
}
