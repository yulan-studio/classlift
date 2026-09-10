namespace Core.DTOs;

public sealed record CourseScheduleNotificationRecipient(
    string ParticipantName,
    string? Email);
