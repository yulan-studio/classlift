using System.Security.Claims;
using Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Core.Services;

public sealed class TimeZoneClaimsPrincipalFactory : UserClaimsPrincipalFactory<User, IdentityRole<int>>
{
    public const string TimeZoneClaimType = "time_zone";

    public TimeZoneClaimsPrincipalFactory(
        UserManager<User> userManager,
        RoleManager<IdentityRole<int>> roleManager,
        IOptions<IdentityOptions> optionsAccessor)
        : base(userManager, roleManager, optionsAccessor) { }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(User user)
    {
        var identity = await base.GenerateClaimsAsync(user);
        identity.AddClaim(new Claim(TimeZoneClaimType, user.TimeZoneId));
        return identity;
    }
}
