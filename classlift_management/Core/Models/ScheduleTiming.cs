namespace Core.Models;

public sealed class ScheduleTiming
{
    public required DateTime ScheduledAtUtc { get; init; }
    public required DateTime ScheduledLocalTime { get; init; }
    public required string TimeZoneId { get; init; }
}
