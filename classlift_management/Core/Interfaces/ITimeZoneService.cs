using Core.Models;

namespace Core.Interfaces;

public interface ITimeZoneService
{
    IReadOnlyList<TimeZoneOption> GetTimeZones();
    bool IsValidTimeZone(string? timeZoneId);
    DateTime ConvertLocalToUtc(DateTime localTime, string timeZoneId);
    DateTime ConvertUtcToLocal(DateTime utcTime, string timeZoneId);
}
