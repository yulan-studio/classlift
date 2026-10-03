namespace Core.ViewModels;

public class CoachNotificationsViewModel
{
    public List<CoachNotificationItem> RescheduleRequests { get; set; } = new();
    public List<CoachNotificationItem> SessionsToComplete { get; set; } = new();
}

public class CoachNotificationItem
{
    public string ParticipantName { get; set; } = string.Empty;
    public string CourseTitle { get; set; } = string.Empty;
    public DateTime? ScheduledAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Link { get; set; } = string.Empty;
}
