using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Billing.Configuration;

public static class ManagementAuthorization
{
    public static AuthorizationPolicy AuthenticatedUserPolicy { get; } =
        new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .RequireAssertion(context =>
            {
                if (context.User.IsInRole("Admin"))
                    return true;

                var httpContext = context.Resource as HttpContext
                    ?? (context.Resource as AuthorizationFilterContext)?.HttpContext;
                var controller = httpContext?.Request.RouteValues["controller"]?.ToString();

                var organizationControllers = new[]
                {
                    "OrganizationPortal",
                    "OrganizationAddOnPage",
                    "OrganizationAddOn"
                };

                return context.User.Identity?.IsAuthenticated == true &&
                    organizationControllers.Contains(
                        controller,
                        StringComparer.OrdinalIgnoreCase);
            })
            .Build();
}
