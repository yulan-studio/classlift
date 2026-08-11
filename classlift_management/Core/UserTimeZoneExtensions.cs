using System.Security.Claims;
using Core.Interfaces;
using Core.Services;

namespace Core;

public static class UserTimeZoneExtensions
{
    public static string GetTimeZoneId(this ClaimsPrincipal user) =>
        user.FindFirstValue(TimeZoneClaimsPrincipalFactory.TimeZoneClaimType)
        ?? TimeZoneService.DefaultTimeZoneId;

    public static DateTime ToUserTime(this DateTime utc, ClaimsPrincipal user, ITimeZoneService service) =>
        service.ConvertUtcToLocal(utc, user.GetTimeZoneId());

    public static DateTime? ToUserTime(this DateTime? utc, ClaimsPrincipal user, ITimeZoneService service) =>
        utc.HasValue ? utc.Value.ToUserTime(user, service) : null;
}
