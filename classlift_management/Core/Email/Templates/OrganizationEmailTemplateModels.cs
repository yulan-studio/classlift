namespace Core.Email.Templates;

public sealed record CourseScheduleEmailData(
    string ParticipantName,
    string CourseName,
    string ProviderName,
    DateTime ScheduledAtUtc,
    string TimeZoneId,
    string ActionPath,
    decimal? ScheduledHours = null,
    string? Location = null);

public sealed record CourseSessionCompletedEmailData(
    string ParticipantName,
    string CourseName,
    string ProviderName,
    DateTime ScheduledAtUtc,
    string TimeZoneId,
    decimal ActualHours,
    string ActionPath);

public sealed record ScheduleChangeRequestedEmailData(
    string ParticipantName,
    string CourseName,
    string RequestedBy,
    string ActionPath,
    string? RequestNote = null);

public sealed record CourseConfirmedEmailData(
    string ParticipantName,
    string CourseName,
    string CourseType,
    string ActionPath,
    string? ProviderName = null);

public sealed record ActivityConfirmedEmailData(
    string ParticipantName,
    string ActivityName,
    string ActionPath,
    DateTime? ScheduledAtUtc = null,
    string? TimeZoneId = null);
