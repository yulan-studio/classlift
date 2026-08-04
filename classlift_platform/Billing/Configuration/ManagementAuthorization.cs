using Microsoft.AspNetCore.Authorization;

namespace Billing.Configuration;

public static class ManagementAuthorization
{
    public static AuthorizationPolicy AuthenticatedUserPolicy { get; } =
        new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();
}
