namespace Core.ViewModels;

public class StaffNotificationsViewModel
{
    public List<StaffNotificationItem> UnconfirmedPrivate { get; set; } = new();
    public List<StaffNotificationItem> UnconfirmedGroup { get; set; } = new();
    public List<StaffNotificationItem> PaidUnconfirmedPrivate { get; set; } = new();
    public List<StaffNotificationItem> PaidUnconfirmedGroup { get; set; } = new();
    public List<StaffNotificationItem> UnpaidPrivate { get; set; } = new();
    public List<StaffNotificationItem> UnpaidGroup { get; set; } = new();
    public List<StaffNotificationItem> LeaveRequests { get; set; } = new();
}

public class StaffNotificationItem
{
    public int EnrollmentId { get; set; }
    public int ChildId { get; set; }
    public int CourseId { get; set; }
    public string ParticipantName { get; set; } = string.Empty;
    public string CourseTitle { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime? ScheduledAt { get; set; }
    public string Link { get; set; } = string.Empty;
}
