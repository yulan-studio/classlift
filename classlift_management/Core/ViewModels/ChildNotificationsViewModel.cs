namespace Core.ViewModels;

public class ChildNotificationsViewModel
{
    public List<ChildNotificationItem> Items { get; set; } = new();
}

public class ChildNotificationItem
{
    public int CourseId { get; set; }
    public string CourseTitle { get; set; } = string.Empty;
    public DateTime ScheduledAt { get; set; }
    public string Status { get; set; } = string.Empty;
}
