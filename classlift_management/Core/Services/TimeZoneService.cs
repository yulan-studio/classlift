using Core.Interfaces;
using Core.Models;

namespace Core.Services;

public sealed class TimeZoneService : ITimeZoneService
{
    public const string DefaultTimeZoneId = "America/Toronto";

    private static readonly IReadOnlyList<TimeZoneOption> TimeZones =
    [
        new("Pacific/Honolulu", "Hawaii"),
        new("America/Anchorage", "Alaska"),
        new("America/Vancouver", "Vancouver / Pacific Time"),
        new("America/Edmonton", "Edmonton / Mountain Time"),
        new("America/Phoenix", "Arizona"),
        new("America/Winnipeg", "Winnipeg / Central Time"),
        new("America/Chicago", "Chicago / Central Time"),
        new("America/Toronto", "Toronto / Eastern Time"),
        new("America/New_York", "New York / Eastern Time"),
        new("America/Halifax", "Halifax / Atlantic Time"),
        new("America/St_Johns", "St. John's / Newfoundland Time"),
        new("UTC", "UTC"),
        new("Europe/London", "London"),
        new("Europe/Paris", "Paris / Central European Time"),
        new("Europe/Berlin", "Berlin / Central European Time"),
        new("Asia/Dubai", "Dubai"),
        new("Asia/Kolkata", "India"),
        new("Asia/Shanghai", "China"),
        new("Asia/Hong_Kong", "Hong Kong"),
        new("Asia/Tokyo", "Tokyo"),
        new("Asia/Seoul", "Seoul"),
        new("Australia/Perth", "Perth"),
        new("Australia/Sydney", "Sydney"),
        new("Pacific/Auckland", "Auckland")
    ];

    public IReadOnlyList<TimeZoneOption> GetTimeZones() => TimeZones;

    public bool IsValidTimeZone(string? timeZoneId) =>
        !string.IsNullOrWhiteSpace(timeZoneId)
        && TimeZoneInfo.TryFindSystemTimeZoneById(timeZoneId, out _);

    public DateTime ConvertLocalToUtc(DateTime localTime, string timeZoneId)
    {
        var zone = GetTimeZone(timeZoneId);
        var local = DateTime.SpecifyKind(localTime, DateTimeKind.Unspecified);

        if (zone.IsInvalidTime(local))
            throw new ArgumentException("The selected local time does not exist because of a daylight-saving transition.");
        if (zone.IsAmbiguousTime(local))
            throw new ArgumentException("The selected local time occurs twice because of a daylight-saving transition. Please choose another time.");

        return TimeZoneInfo.ConvertTimeToUtc(local, zone);
    }

    public DateTime ConvertUtcToLocal(DateTime utcTime, string timeZoneId) =>
        TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utcTime, DateTimeKind.Utc), GetTimeZone(timeZoneId));

    private static TimeZoneInfo GetTimeZone(string timeZoneId)
    {
        if (!TimeZoneInfo.TryFindSystemTimeZoneById(timeZoneId, out var zone))
            throw new ArgumentException($"Unsupported time zone '{timeZoneId}'.", nameof(timeZoneId));
        return zone;
    }
}
