using Core.Email.Templates;
using Core.Interfaces;
using Core.Models;
using Microsoft.Extensions.Logging;

namespace Core.Email.Notifications;

public sealed class OrganizationEmailNotificationService : IOrganizationEmailNotificationService
{
    private readonly IOrganizationEmailSettingsService _settingsService;
    private readonly IOrganizationEmailTemplateService _templateService;
    private readonly IEmailService _emailService;
    private readonly CurrentTenant _currentTenant;
    private readonly ILogger<OrganizationEmailNotificationService> _logger;

    public OrganizationEmailNotificationService(
        IOrganizationEmailSettingsService settingsService,
        IOrganizationEmailTemplateService templateService,
        IEmailService emailService,
        CurrentTenant currentTenant,
        ILogger<OrganizationEmailNotificationService> logger)
    {
        _settingsService = settingsService;
        _templateService = templateService;
        _emailService = emailService;
        _currentTenant = currentTenant;
        _logger = logger;
    }

    public Task<OrganizationNotificationResult> SendCourseScheduleCreatedAsync(
        string? familyEmail,
        CourseScheduleEmailData data,
        CancellationToken cancellationToken = default) =>
        SendDirectAsync(
            EmailNotificationType.CourseScheduleCreated,
            NotificationRecipientKind.Family,
            familyEmail,
            (context, recipient) => _templateService.CourseScheduleCreated(context, recipient, data),
            cancellationToken);

    public Task<OrganizationNotificationResult> SendCourseSchedulesCreatedAsync(
        string? familyEmail,
        CourseScheduleSummaryEmailData data,
        CancellationToken cancellationToken = default) =>
        SendDirectAsync(
            EmailNotificationType.CourseScheduleCreated,
            NotificationRecipientKind.Family,
            familyEmail,
            (context, recipient) => _templateService.CourseSchedulesCreated(context, recipient, data),
            cancellationToken);

    public Task<OrganizationNotificationResult> SendCourseScheduleUpdatedAsync(
        string? familyEmail,
        CourseScheduleEmailData data,
        CancellationToken cancellationToken = default) =>
        SendDirectAsync(
            EmailNotificationType.CourseScheduleUpdated,
            NotificationRecipientKind.Family,
            familyEmail,
            (context, recipient) => _templateService.CourseScheduleUpdated(context, recipient, data),
            cancellationToken);

    public Task<OrganizationNotificationResult> SendCourseScheduleDeletedAsync(
        string? familyEmail,
        CourseScheduleEmailData data,
        CancellationToken cancellationToken = default) =>
        SendDirectAsync(
            EmailNotificationType.CourseScheduleDeleted,
            NotificationRecipientKind.Family,
            familyEmail,
            (context, recipient) => _templateService.CourseScheduleDeleted(context, recipient, data),
            cancellationToken);

    public Task<OrganizationNotificationResult> SendCourseSessionCompletedAsync(
        string? familyEmail,
        CourseSessionCompletedEmailData data,
        CancellationToken cancellationToken = default) =>
        SendDirectAsync(
            EmailNotificationType.CourseSessionCompleted,
            NotificationRecipientKind.Family,
            familyEmail,
            (context, recipient) => _templateService.CourseSessionCompleted(context, recipient, data),
            cancellationToken);

    public Task<OrganizationNotificationResult> SendScheduleChangeRequestedToCoachAsync(
        string? coachEmail,
        ScheduleChangeRequestedEmailData data,
        CancellationToken cancellationToken = default) =>
        SendDirectAsync(
            EmailNotificationType.ScheduleChangeRequested,
            NotificationRecipientKind.Coach,
            coachEmail,
            (context, recipient) => _templateService.ScheduleChangeRequested(context, recipient, data),
            cancellationToken);

    public async Task<OrganizationNotificationResult> SendScheduleChangeRequestedToOrganizationAsync(
        ScheduleChangeRequestedEmailData data,
        CancellationToken cancellationToken = default)
    {
        var bundle = await LoadContextAsync(
            EmailNotificationType.ScheduleChangeRequested,
            NotificationRecipientKind.Organization,
            cancellationToken);
        if (bundle.Failure is not null)
            return bundle.Failure;

        return await SendWithContextAsync(
            EmailNotificationType.ScheduleChangeRequested,
            NotificationRecipientKind.Organization,
            bundle.Settings!.ReceiverEmail,
            context => _templateService.ScheduleChangeRequested(context, bundle.Settings.ReceiverEmail, data),
            bundle.Context!,
            cancellationToken);
    }

    public Task<OrganizationNotificationResult> SendCourseConfirmationRequestedAsync(
        string? familyEmail,
        CourseConfirmationRequestedEmailData data,
        CancellationToken cancellationToken = default) =>
        SendDirectAsync(
            EmailNotificationType.CourseConfirmationRequested,
            NotificationRecipientKind.Family,
            familyEmail,
            (context, recipient) => _templateService.CourseConfirmationRequested(context, recipient, data),
            cancellationToken);

    public async Task<OrganizationNotificationBatchResult> SendPrivateCourseConfirmedAsync(
        string? coachEmail,
        CourseConfirmedEmailData data,
        CancellationToken cancellationToken = default)
    {
        var bundle = await LoadContextAsync(
            EmailNotificationType.CourseConfirmed,
            NotificationRecipientKind.Organization,
            cancellationToken);
        if (bundle.Failure is not null)
        {
            return new OrganizationNotificationBatchResult(
                [
                    bundle.Failure,
                    MissingOrSettingsFailureForCoach(bundle.Failure, coachEmail)
                ]);
        }

        var organizationDelivery = await SendWithContextAsync(
            EmailNotificationType.CourseConfirmed,
            NotificationRecipientKind.Organization,
            bundle.Settings!.ReceiverEmail,
            context => _templateService.CourseConfirmed(context, bundle.Settings.ReceiverEmail, data),
            bundle.Context!,
            cancellationToken);
        var coachDelivery = await SendWithContextAsync(
            EmailNotificationType.CourseConfirmed,
            NotificationRecipientKind.Coach,
            coachEmail,
            context => _templateService.CourseConfirmed(
                context,
                coachEmail!,
                data with { ActionPath = data.ProviderActionPath ?? data.ActionPath }),
            bundle.Context!,
            cancellationToken);

        return new OrganizationNotificationBatchResult([organizationDelivery, coachDelivery]);
    }

    public async Task<OrganizationNotificationResult> SendGroupCourseConfirmedAsync(
        CourseConfirmedEmailData data,
        CancellationToken cancellationToken = default)
    {
        var bundle = await LoadContextAsync(
            EmailNotificationType.CourseConfirmed,
            NotificationRecipientKind.Organization,
            cancellationToken);
        if (bundle.Failure is not null)
            return bundle.Failure;

        return await SendWithContextAsync(
            EmailNotificationType.CourseConfirmed,
            NotificationRecipientKind.Organization,
            bundle.Settings!.ReceiverEmail,
            context => _templateService.CourseConfirmed(context, bundle.Settings.ReceiverEmail, data),
            bundle.Context!,
            cancellationToken);
    }

    public async Task<OrganizationNotificationResult> SendActivityConfirmedAsync(
        ActivityConfirmedEmailData data,
        CancellationToken cancellationToken = default)
    {
        var bundle = await LoadContextAsync(
            EmailNotificationType.ActivityConfirmed,
            NotificationRecipientKind.Organization,
            cancellationToken);
        if (bundle.Failure is not null)
            return bundle.Failure;

        return await SendWithContextAsync(
            EmailNotificationType.ActivityConfirmed,
            NotificationRecipientKind.Organization,
            bundle.Settings!.ReceiverEmail,
            context => _templateService.ActivityConfirmed(context, bundle.Settings.ReceiverEmail, data),
            bundle.Context!,
            cancellationToken);
    }

    private async Task<OrganizationNotificationResult> SendDirectAsync(
        EmailNotificationType notificationType,
        NotificationRecipientKind recipientKind,
        string? recipientEmail,
        Func<OrganizationEmailTemplateContext, string, TemplatedEmail> buildTemplate,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(recipientEmail))
            return Result(notificationType, recipientKind, OrganizationNotificationStatus.SkippedMissingRecipient);

        var bundle = await LoadContextAsync(notificationType, recipientKind, cancellationToken);
        if (bundle.Failure is not null)
            return bundle.Failure;

        return await SendWithContextAsync(
            notificationType,
            recipientKind,
            recipientEmail,
            context => buildTemplate(context, recipientEmail),
            bundle.Context!,
            cancellationToken);
    }

    private async Task<ContextLoadResult> LoadContextAsync(
        EmailNotificationType notificationType,
        NotificationRecipientKind recipientKind,
        CancellationToken cancellationToken)
    {
        try
        {
            var settings = await _settingsService.GetAsync(cancellationToken);
            if (settings is null
                || string.IsNullOrWhiteSpace(settings.ReplyToEmail)
                || string.IsNullOrWhiteSpace(_currentTenant.OrganizationName)
                || _currentTenant.TrustedPortalBaseUri is null)
            {
                return new ContextLoadResult(
                    null,
                    settings,
                    Result(
                        notificationType,
                        recipientKind,
                        OrganizationNotificationStatus.SkippedMissingOrganizationSettings));
            }

            return new ContextLoadResult(
                new OrganizationEmailTemplateContext(
                    _currentTenant.OrganizationName,
                    settings.ReplyToEmail,
                    _currentTenant.TrustedPortalBaseUri,
                    _currentTenant.Terminology),
                settings,
                null);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Organization email settings could not be loaded");
            return new ContextLoadResult(
                null,
                null,
                Result(notificationType, recipientKind, OrganizationNotificationStatus.Failed, "settings_load_failed"));
        }
    }

    private async Task<OrganizationNotificationResult> SendWithContextAsync(
        EmailNotificationType notificationType,
        NotificationRecipientKind recipientKind,
        string? recipientEmail,
        Func<OrganizationEmailTemplateContext, TemplatedEmail> buildTemplate,
        OrganizationEmailTemplateContext context,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(recipientEmail))
        {
            return Result(
                notificationType,
                recipientKind,
                OrganizationNotificationStatus.SkippedMissingRecipient);
        }

        TemplatedEmail templatedEmail;
        try
        {
            templatedEmail = buildTemplate(context);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Email template creation failed for recipient kind {RecipientKind}", recipientKind);
            return Result(
                notificationType,
                recipientKind,
                OrganizationNotificationStatus.Failed,
                "template_failed");
        }

        try
        {
            var sendResult = await _emailService.SendAsync(templatedEmail.Message, cancellationToken);
            return Result(
                templatedEmail.NotificationType,
                recipientKind,
                MapStatus(sendResult.Status),
                sendResult.ErrorCode);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Email notification delivery threw unexpectedly. Type={NotificationType}, RecipientKind={RecipientKind}",
                templatedEmail.NotificationType,
                recipientKind);
            return Result(
                templatedEmail.NotificationType,
                recipientKind,
                OrganizationNotificationStatus.Failed,
                "delivery_exception");
        }
    }

    private OrganizationNotificationResult Result(
        EmailNotificationType notificationType,
        NotificationRecipientKind recipientKind,
        OrganizationNotificationStatus status,
        string? errorCode = null)
    {
        _logger.LogInformation(
            "Email notification outcome. Type={NotificationType}, RecipientKind={RecipientKind}, Status={Status}",
            notificationType,
            recipientKind,
            status);
        return new OrganizationNotificationResult(notificationType, recipientKind, status, errorCode);
    }

    private static OrganizationNotificationResult MissingOrSettingsFailureForCoach(
        OrganizationNotificationResult settingsFailure,
        string? coachEmail) =>
        string.IsNullOrWhiteSpace(coachEmail)
            ? new OrganizationNotificationResult(
                settingsFailure.NotificationType,
                NotificationRecipientKind.Coach,
                OrganizationNotificationStatus.SkippedMissingRecipient)
            : settingsFailure with { RecipientKind = NotificationRecipientKind.Coach };

    private static OrganizationNotificationStatus MapStatus(EmailSendStatus status) => status switch
    {
        EmailSendStatus.Sent => OrganizationNotificationStatus.Sent,
        EmailSendStatus.Captured => OrganizationNotificationStatus.Captured,
        EmailSendStatus.Disabled => OrganizationNotificationStatus.Disabled,
        EmailSendStatus.Failed => OrganizationNotificationStatus.Failed,
        _ => OrganizationNotificationStatus.Failed
    };

    private sealed record ContextLoadResult(
        OrganizationEmailTemplateContext? Context,
        OrganizationEmailSettings? Settings,
        OrganizationNotificationResult? Failure);
}
