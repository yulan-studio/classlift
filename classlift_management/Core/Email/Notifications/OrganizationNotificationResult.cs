using Core.Email.Templates;

namespace Core.Email.Notifications;

public sealed record OrganizationNotificationResult(
    EmailNotificationType NotificationType,
    NotificationRecipientKind RecipientKind,
    OrganizationNotificationStatus Status,
    string? ErrorCode = null)
{
    public bool IsSuccessful => Status is OrganizationNotificationStatus.Sent
        or OrganizationNotificationStatus.Captured
        or OrganizationNotificationStatus.Disabled;
}

public sealed record OrganizationNotificationBatchResult(
    IReadOnlyList<OrganizationNotificationResult> Deliveries)
{
    public bool IsSuccessful => Deliveries.All(delivery => delivery.IsSuccessful);
}

public enum OrganizationNotificationStatus
{
    Sent,
    Captured,
    Disabled,
    SkippedMissingRecipient,
    SkippedMissingOrganizationSettings,
    Failed
}

public enum NotificationRecipientKind
{
    Family,
    Coach,
    Organization
}
