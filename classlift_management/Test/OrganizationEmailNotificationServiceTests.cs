using Core.Email;
using Core.Email.Notifications;
using Core.Email.Templates;
using Core.Interfaces;
using Core.Models;
using Core.Services;
using Microsoft.Extensions.Logging.Abstractions;

namespace Test;

public class OrganizationEmailNotificationServiceTests
{
    [Test]
    public async Task MissingDirectRecipientSkipsBeforeLoadingOrganizationSettings()
    {
        var settings = new FakeSettingsService(ValidSettings());
        var email = new FakeEmailService(EmailSendResult.Captured());
        var service = CreateService(settings, email);

        var result = await service.SendCourseScheduleUpdatedAsync(null, ScheduleData());

        Assert.Multiple(() =>
        {
            Assert.That(result.NotificationType, Is.EqualTo(EmailNotificationType.CourseScheduleUpdated));
            Assert.That(result.Status, Is.EqualTo(OrganizationNotificationStatus.SkippedMissingRecipient));
            Assert.That(settings.GetCallCount, Is.Zero);
            Assert.That(email.Messages, Is.Empty);
        });
    }

    [Test]
    public async Task MissingOrganizationSettingsSkipsWithoutSending()
    {
        var email = new FakeEmailService(EmailSendResult.Captured());
        var service = CreateService(new FakeSettingsService(null), email);

        var result = await service.SendCourseScheduleDeletedAsync("family@example.com", ScheduleData());

        Assert.Multiple(() =>
        {
            Assert.That(result.NotificationType, Is.EqualTo(EmailNotificationType.CourseScheduleDeleted));
            Assert.That(result.Status, Is.EqualTo(OrganizationNotificationStatus.SkippedMissingOrganizationSettings));
            Assert.That(email.Messages, Is.Empty);
        });
    }

    [Test]
    public async Task OrganizationNotificationUsesConfiguredReceiverAndReplyTo()
    {
        var email = new FakeEmailService(EmailSendResult.Captured());
        var service = CreateService(new FakeSettingsService(ValidSettings()), email);

        var result = await service.SendActivityConfirmedAsync(ActivityData());

        Assert.Multiple(() =>
        {
            Assert.That(result.NotificationType, Is.EqualTo(EmailNotificationType.ActivityConfirmed));
            Assert.That(result.RecipientKind, Is.EqualTo(NotificationRecipientKind.Organization));
            Assert.That(result.Status, Is.EqualTo(OrganizationNotificationStatus.Captured));
            Assert.That(email.Messages, Has.Count.EqualTo(1));
            Assert.That(email.Messages[0].To, Is.EqualTo("office@northstar.example"));
            Assert.That(email.Messages[0].ReplyTo, Is.EqualTo("support@northstar.example"));
            Assert.That(email.Messages[0].HtmlBody, Does.Contain("North Star Academy"));
        });
    }

    [Test]
    public async Task PrivateCourseConfirmationSendsSeparateOrganizationAndCoachMessages()
    {
        var email = new FakeEmailService(EmailSendResult.Captured());
        var service = CreateService(new FakeSettingsService(ValidSettings()), email);

        var result = await service.SendPrivateCourseConfirmedAsync(
            "coach@example.com",
            CourseConfirmedData());

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccessful, Is.True);
            Assert.That(result.Deliveries, Has.Count.EqualTo(2));
            Assert.That(result.Deliveries.Select(x => x.RecipientKind),
                Is.EquivalentTo(new[] { NotificationRecipientKind.Organization, NotificationRecipientKind.Coach }));
            Assert.That(email.Messages.Select(x => x.To),
                Is.EquivalentTo(new[] { "office@northstar.example", "coach@example.com" }));
            Assert.That(email.Messages.All(x => x.ReplyTo == "support@northstar.example"), Is.True);
        });
    }

    [Test]
    public async Task PrivateCourseConfirmationStillNotifiesOrganizationWhenCoachEmailIsMissing()
    {
        var email = new FakeEmailService(EmailSendResult.Sent());
        var service = CreateService(new FakeSettingsService(ValidSettings()), email);

        var result = await service.SendPrivateCourseConfirmedAsync(null, CourseConfirmedData());

        Assert.Multiple(() =>
        {
            Assert.That(email.Messages, Has.Count.EqualTo(1));
            Assert.That(email.Messages[0].To, Is.EqualTo("office@northstar.example"));
            Assert.That(result.Deliveries.Single(x => x.RecipientKind == NotificationRecipientKind.Organization).Status,
                Is.EqualTo(OrganizationNotificationStatus.Sent));
            Assert.That(result.Deliveries.Single(x => x.RecipientKind == NotificationRecipientKind.Coach).Status,
                Is.EqualTo(OrganizationNotificationStatus.SkippedMissingRecipient));
            Assert.That(result.IsSuccessful, Is.False);
        });
    }

    [TestCase(EmailSendStatus.Sent, OrganizationNotificationStatus.Sent)]
    [TestCase(EmailSendStatus.Captured, OrganizationNotificationStatus.Captured)]
    [TestCase(EmailSendStatus.Disabled, OrganizationNotificationStatus.Disabled)]
    [TestCase(EmailSendStatus.Failed, OrganizationNotificationStatus.Failed)]
    public async Task MapsEmailDeliveryStatus(
        EmailSendStatus emailStatus,
        OrganizationNotificationStatus expectedStatus)
    {
        var sendResult = emailStatus == EmailSendStatus.Failed
            ? EmailSendResult.Failed("smtp_failed")
            : new EmailSendResult(emailStatus);
        var service = CreateService(
            new FakeSettingsService(ValidSettings()),
            new FakeEmailService(sendResult));

        var result = await service.SendCourseSessionCompletedAsync(
            "family@example.com",
            CompletedData());

        Assert.Multiple(() =>
        {
            Assert.That(result.NotificationType, Is.EqualTo(EmailNotificationType.CourseSessionCompleted));
            Assert.That(result.Status, Is.EqualTo(expectedStatus));
            Assert.That(result.ErrorCode, Is.EqualTo(emailStatus == EmailSendStatus.Failed ? "smtp_failed" : null));
        });
    }

    [Test]
    public async Task UnexpectedDeliveryExceptionReturnsFailureInsteadOfBreakingBusinessFlow()
    {
        var service = CreateService(
            new FakeSettingsService(ValidSettings()),
            new FakeEmailService(new InvalidOperationException("test failure")));

        var result = await service.SendScheduleChangeRequestedToCoachAsync(
            "coach@example.com",
            ChangeRequestData());

        Assert.Multiple(() =>
        {
            Assert.That(result.NotificationType, Is.EqualTo(EmailNotificationType.ScheduleChangeRequested));
            Assert.That(result.Status, Is.EqualTo(OrganizationNotificationStatus.Failed));
            Assert.That(result.ErrorCode, Is.EqualTo("delivery_exception"));
        });
    }

    [Test]
    public async Task CreatedScheduleSummaryIsDeliveredAsOneEmail()
    {
        var email = new FakeEmailService(EmailSendResult.Captured());
        var service = CreateService(new FakeSettingsService(ValidSettings()), email);
        var summary = new CourseScheduleSummaryEmailData(
            "Jamie",
            "Piano",
            "Taylor",
            [
                new CourseScheduleSummaryItem(
                    new DateTime(2026, 9, 14, 18, 0, 0, DateTimeKind.Utc),
                    "America/Toronto",
                    1m),
                new CourseScheduleSummaryItem(
                    new DateTime(2026, 9, 21, 18, 0, 0, DateTimeKind.Utc),
                    "America/Toronto",
                    1m)
            ],
            "/Child/MySchedules");

        var result = await service.SendCourseSchedulesCreatedAsync("family@example.com", summary);

        Assert.Multiple(() =>
        {
            Assert.That(result.Status, Is.EqualTo(OrganizationNotificationStatus.Captured));
            Assert.That(email.Messages, Has.Count.EqualTo(1));
            Assert.That(email.Messages[0].HtmlBody, Does.Contain("Session 1"));
            Assert.That(email.Messages[0].HtmlBody, Does.Contain("Session 2"));
        });
    }

    private static OrganizationEmailNotificationService CreateService(
        IOrganizationEmailSettingsService settingsService,
        IEmailService emailService) =>
        new(
            settingsService,
            new OrganizationEmailTemplateService(new TimeZoneService()),
            emailService,
            new CurrentTenant
            {
                OrganizationName = "North Star Academy",
                TrustedPortalBaseUri = new Uri("https://northstar.example/"),
                Terminology = new OrganizationTerminology
                {
                    ParticipantSingular = "Student",
                    ProviderSingular = "Coach"
                }
            },
            NullLogger<OrganizationEmailNotificationService>.Instance);

    private static OrganizationEmailSettings ValidSettings() => new()
    {
        ReplyToEmail = "support@northstar.example",
        ReceiverEmail = "office@northstar.example"
    };

    private static CourseScheduleEmailData ScheduleData() => new(
        "Jamie", "Piano", "Taylor", new DateTime(2026, 9, 10, 18, 0, 0, DateTimeKind.Utc),
        "America/Toronto", "/courses/1");

    private static CourseSessionCompletedEmailData CompletedData() => new(
        "Jamie", "Piano", "Taylor", new DateTime(2026, 9, 10, 18, 0, 0, DateTimeKind.Utc),
        "America/Toronto", 1.0m, "/courses/1");

    private static ScheduleChangeRequestedEmailData ChangeRequestData() => new(
        "Jamie", "Piano", "Parent", "/requests/1", "Please change the time.");

    private static CourseConfirmedEmailData CourseConfirmedData() => new(
        "Jamie", "Piano", "Private", "/courses/1", "Taylor");

    private static ActivityConfirmedEmailData ActivityData() => new(
        "Jamie", "Field Trip", "/activities/1");

    private sealed class FakeSettingsService(OrganizationEmailSettings? settings)
        : IOrganizationEmailSettingsService
    {
        public int GetCallCount { get; private set; }

        public Task<OrganizationEmailSettings?> GetAsync(CancellationToken cancellationToken = default)
        {
            GetCallCount++;
            return Task.FromResult(settings);
        }

        public Task SaveAsync(
            string replyToEmail,
            string receiverEmail,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();
    }

    private sealed class FakeEmailService : IEmailService
    {
        private readonly EmailSendResult? _result;
        private readonly Exception? _exception;

        public FakeEmailService(EmailSendResult result) => _result = result;

        public FakeEmailService(Exception exception) => _exception = exception;

        public List<EmailMessage> Messages { get; } = [];

        public Task<EmailSendResult> SendAsync(
            EmailMessage message,
            CancellationToken cancellationToken = default)
        {
            Messages.Add(message);
            if (_exception is not null)
                throw _exception;

            return Task.FromResult(_result!);
        }
    }
}
