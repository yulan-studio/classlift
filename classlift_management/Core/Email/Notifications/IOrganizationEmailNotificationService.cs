using Core.Email.Templates;

namespace Core.Email.Notifications;

public interface IOrganizationEmailNotificationService
{
    Task<OrganizationNotificationResult> SendCourseScheduleCreatedAsync(
        string? familyEmail,
        CourseScheduleEmailData data,
        CancellationToken cancellationToken = default);

    Task<OrganizationNotificationResult> SendCourseSchedulesCreatedAsync(
        string? familyEmail,
        CourseScheduleSummaryEmailData data,
        CancellationToken cancellationToken = default);

    Task<OrganizationNotificationResult> SendCourseScheduleUpdatedAsync(
        string? familyEmail,
        CourseScheduleEmailData data,
        CancellationToken cancellationToken = default);

    Task<OrganizationNotificationResult> SendCourseScheduleDeletedAsync(
        string? familyEmail,
        CourseScheduleEmailData data,
        CancellationToken cancellationToken = default);

    Task<OrganizationNotificationResult> SendCourseSessionCompletedAsync(
        string? familyEmail,
        CourseSessionCompletedEmailData data,
        CancellationToken cancellationToken = default);

    Task<OrganizationNotificationResult> SendScheduleChangeRequestedToCoachAsync(
        string? coachEmail,
        ScheduleChangeRequestedEmailData data,
        CancellationToken cancellationToken = default);

    Task<OrganizationNotificationResult> SendScheduleChangeRequestedToOrganizationAsync(
        ScheduleChangeRequestedEmailData data,
        CancellationToken cancellationToken = default);

    Task<OrganizationNotificationResult> SendCourseConfirmationRequestedAsync(
        string? familyEmail,
        CourseConfirmationRequestedEmailData data,
        CancellationToken cancellationToken = default);

    Task<OrganizationNotificationBatchResult> SendPrivateCourseConfirmedAsync(
        string? coachEmail,
        CourseConfirmedEmailData data,
        CancellationToken cancellationToken = default);

    Task<OrganizationNotificationResult> SendGroupCourseConfirmedAsync(
        CourseConfirmedEmailData data,
        CancellationToken cancellationToken = default);

    Task<OrganizationNotificationResult> SendActivityConfirmedAsync(
        ActivityConfirmedEmailData data,
        CancellationToken cancellationToken = default);
}
