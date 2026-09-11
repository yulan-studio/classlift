using Core.Email.Templates;
using Core.Models;
using Core.Services;

namespace Test;

public class OrganizationEmailTemplateServiceTests
{
    private readonly OrganizationEmailTemplateService _service = new(new TimeZoneService());

    [TestCase(EmailNotificationType.CourseScheduleCreated)]
    [TestCase(EmailNotificationType.CourseScheduleUpdated)]
    [TestCase(EmailNotificationType.CourseScheduleDeleted)]
    [TestCase(EmailNotificationType.CourseSessionCompleted)]
    [TestCase(EmailNotificationType.ScheduleChangeRequested)]
    [TestCase(EmailNotificationType.CourseConfirmationRequested)]
    [TestCase(EmailNotificationType.CourseConfirmed)]
    [TestCase(EmailNotificationType.ActivityConfirmed)]
    public void BuildsEverySupportedTemplate(EmailNotificationType notificationType)
    {
        var email = Build(notificationType);
        var expectedPortalUrl = $"https://northstar.example{GetActionPath(notificationType)}";

        Assert.Multiple(() =>
        {
            Assert.That(email.NotificationType, Is.EqualTo(notificationType));
            Assert.That(email.Message.ReplyTo, Is.EqualTo("support@northstar.example"));
            Assert.That(email.Message.To, Is.EqualTo("recipient@example.com"));
            Assert.That(email.Message.Subject, Is.Not.Empty);
            Assert.That(email.Message.HtmlBody, Does.Contain("North Star Academy"));
            Assert.That(email.Message.TextBody, Does.Contain("North Star Academy"));
            Assert.That(email.Message.HtmlBody, Does.Contain(expectedPortalUrl));
            Assert.That(email.Message.TextBody, Does.Contain(expectedPortalUrl));
        });
    }

    [Test]
    public void EncodesDynamicHtmlButPreservesReadablePlainText()
    {
        var context = Context() with
        {
            OrganizationName = "North <script>alert('org')</script>",
            Terminology = new OrganizationTerminology
            {
                ParticipantSingular = "Student <img>",
                ProviderSingular = "Instructor <b>"
            }
        };
        var data = ScheduleData() with
        {
            ParticipantName = "Alex <script>alert('name')</script>",
            CourseName = "Robotics <img src=x>",
            Location = "Room <b>One</b>"
        };

        var email = _service.CourseScheduleCreated(context, "recipient@example.com", data);

        Assert.Multiple(() =>
        {
            Assert.That(email.Message.HtmlBody, Does.Not.Contain("<script>alert('name')</script>"));
            Assert.That(email.Message.HtmlBody, Does.Contain("Alex &lt;script&gt;alert(&#39;name&#39;)&lt;/script&gt;"));
            Assert.That(email.Message.HtmlBody, Does.Contain("Room &lt;b&gt;One&lt;/b&gt;"));
            Assert.That(email.Message.TextBody, Does.Contain("Alex <script>alert('name')</script>"));
        });
    }

    [Test]
    public void ConvertsUtcScheduleToTheSuppliedCourseTimeZone()
    {
        var data = ScheduleData() with
        {
            ScheduledAtUtc = new DateTime(2026, 7, 15, 13, 30, 0, DateTimeKind.Utc),
            TimeZoneId = "America/Toronto"
        };

        var email = _service.CourseScheduleUpdated(Context(), "recipient@example.com", data);

        Assert.Multiple(() =>
        {
            Assert.That(email.Message.HtmlBody, Does.Contain("July 15, 2026 at 9:30 AM (America/Toronto)"));
            Assert.That(email.Message.TextBody, Does.Contain("July 15, 2026 at 9:30 AM (America/Toronto)"));
        });
    }

    [Test]
    public void RemovesLineBreaksFromUserDerivedSubjectText()
    {
        var email = _service.CourseConfirmed(
            Context(),
            "recipient@example.com",
            new CourseConfirmedEmailData(
                "Alex",
                "Robotics\r\nBcc: attacker@example.com",
                "Private",
                "/Course/Manage",
                "Morgan"));

        Assert.Multiple(() =>
        {
            Assert.That(email.Message.Subject, Does.Not.Contain("\r"));
            Assert.That(email.Message.Subject, Does.Not.Contain("\n"));
        });
    }

    [TestCase("ftp://northstar.example/")]
    [TestCase("mailto:support@northstar.example")]
    public void RejectsUntrustedPortalScheme(string portalUrl)
    {
        var context = Context() with { TrustedPortalBaseUri = new Uri(portalUrl) };

        Assert.Throws<ArgumentException>(() =>
            _service.CourseScheduleCreated(context, "recipient@example.com", ScheduleData()));
    }

    [Test]
    public void RejectsInvalidOrganizationReplyToAddress()
    {
        var context = Context() with { ReplyToEmail = "invalid" };

        Assert.Throws<ArgumentException>(() =>
            _service.CourseScheduleCreated(context, "recipient@example.com", ScheduleData()));
    }

    [Test]
    public void RejectsMissingRequiredTemplateData()
    {
        var data = ScheduleData() with { ParticipantName = "" };

        Assert.Throws<ArgumentException>(() =>
            _service.CourseScheduleCreated(Context(), "recipient@example.com", data));
    }

    [TestCase("https://attacker.example/path")]
    [TestCase("//attacker.example/path")]
    [TestCase("relative/path")]
    [TestCase("/\\attacker.example/path")]
    [TestCase("/Child/MySchedules\nInjected")]
    public void RejectsExternalOrRelativeActionPath(string actionPath)
    {
        var data = ScheduleData() with { ActionPath = actionPath };

        Assert.Throws<ArgumentException>(() =>
            _service.CourseScheduleCreated(Context(), "recipient@example.com", data));
    }

    [Test]
    public void AcceptsRootedLocalActionPathWithoutPlatformDependentUriClassification()
    {
        var data = ScheduleData() with { ActionPath = "/Child/MySchedules" };

        var email = _service.CourseScheduleUpdated(Context(), "recipient@example.com", data);

        Assert.That(
            email.Message.TextBody,
            Does.Contain("https://northstar.example/Child/MySchedules"));
    }

    [Test]
    public void CanceledScheduleIsExplicitInSubjectAndBothBodies()
    {
        var data = ScheduleData() with { Status = "Canceled" };

        var email = _service.CourseScheduleUpdated(Context(), "recipient@example.com", data);

        Assert.Multiple(() =>
        {
            Assert.That(email.Message.Subject, Does.Contain("canceled").IgnoreCase);
            Assert.That(email.Message.HtmlBody, Does.Contain("has been canceled"));
            Assert.That(email.Message.HtmlBody, Does.Contain("<strong>Status:</strong> Canceled"));
            Assert.That(email.Message.TextBody, Does.Contain("has been canceled"));
            Assert.That(email.Message.TextBody, Does.Contain("Status: Canceled"));
        });
    }

    [Test]
    public void RegistrationConfirmationRequestLinksDirectlyToConfirmationsPage()
    {
        var email = _service.CourseConfirmationRequested(
            Context(),
            "family@example.com",
            new CourseConfirmationRequestedEmailData(
                "Jamie", "Piano", "Group", "/Child/MyConfirmations", "Taylor"));

        Assert.Multiple(() =>
        {
            Assert.That(email.Message.Subject, Does.Contain("confirmation required"));
            Assert.That(email.Message.HtmlBody, Does.Contain("Review and confirm course"));
            Assert.That(email.Message.HtmlBody,
                Does.Contain("https://northstar.example/Child/MyConfirmations"));
            Assert.That(email.Message.TextBody,
                Does.Contain("https://northstar.example/Child/MyConfirmations"));
        });
    }

    [Test]
    public void BuildsOneSummaryEmailContainingEveryCreatedSession()
    {
        var data = new CourseScheduleSummaryEmailData(
            "Jamie",
            "Piano",
            "Taylor",
            [
                new CourseScheduleSummaryItem(
                    new DateTime(2026, 9, 14, 18, 0, 0, DateTimeKind.Utc),
                    "America/Toronto",
                    1m,
                    "Room A"),
                new CourseScheduleSummaryItem(
                    new DateTime(2026, 9, 21, 18, 0, 0, DateTimeKind.Utc),
                    "America/Toronto",
                    1m,
                    "Room A")
            ],
            "/Child/MySchedules");

        var email = _service.CourseSchedulesCreated(Context(), "recipient@example.com", data);

        Assert.Multiple(() =>
        {
            Assert.That(email.NotificationType, Is.EqualTo(EmailNotificationType.CourseScheduleCreated));
            Assert.That(email.Message.Subject, Does.Contain("2 new course sessions"));
            Assert.That(email.Message.HtmlBody, Does.Contain("Session 1"));
            Assert.That(email.Message.HtmlBody, Does.Contain("Session 2"));
            Assert.That(email.Message.HtmlBody, Does.Contain("Location: Room A"));
            Assert.That(email.Message.TextBody, Does.Contain("September 14, 2026"));
            Assert.That(email.Message.TextBody, Does.Contain("September 21, 2026"));
        });
    }

    [Test]
    public void RejectsEmptyCreatedSessionSummary()
    {
        var data = new CourseScheduleSummaryEmailData(
            "Jamie", "Piano", "Taylor", [], "/Child/MySchedules");

        Assert.Throws<ArgumentException>(() =>
            _service.CourseSchedulesCreated(Context(), "recipient@example.com", data));
    }

    [Test]
    public void RejectsInvalidRecipientAddress()
    {
        Assert.Throws<ArgumentException>(() =>
            _service.CourseScheduleCreated(Context(), "invalid", ScheduleData()));
    }

    private TemplatedEmail Build(EmailNotificationType notificationType) => notificationType switch
    {
        EmailNotificationType.CourseScheduleCreated =>
            _service.CourseScheduleCreated(Context(), "recipient@example.com", ScheduleData()),
        EmailNotificationType.CourseScheduleUpdated =>
            _service.CourseScheduleUpdated(Context(), "recipient@example.com", ScheduleData()),
        EmailNotificationType.CourseScheduleDeleted =>
            _service.CourseScheduleDeleted(Context(), "recipient@example.com", ScheduleData()),
        EmailNotificationType.CourseSessionCompleted =>
            _service.CourseSessionCompleted(
                Context(),
                "recipient@example.com",
                new CourseSessionCompletedEmailData(
                    "Alex",
                    "Robotics",
                    "Morgan",
                    new DateTime(2026, 7, 15, 13, 30, 0, DateTimeKind.Utc),
                    "America/Toronto",
                    1.5m,
                    "/Child/MySchedules")),
        EmailNotificationType.ScheduleChangeRequested =>
            _service.ScheduleChangeRequested(
                Context(),
                "recipient@example.com",
                new ScheduleChangeRequestedEmailData(
                    "Alex",
                    "Robotics",
                    "Alex's family",
                    "/Staff/ScheduleRequests",
                    "Please move this session.")),
        EmailNotificationType.CourseConfirmationRequested =>
            _service.CourseConfirmationRequested(
                Context(),
                "recipient@example.com",
                new CourseConfirmationRequestedEmailData(
                    "Alex",
                    "Robotics",
                    "Group",
                    "/Child/MyConfirmations",
                    "Morgan")),
        EmailNotificationType.CourseConfirmed =>
            _service.CourseConfirmed(
                Context(),
                "recipient@example.com",
                new CourseConfirmedEmailData(
                    "Alex",
                    "Robotics",
                    "Private",
                    "/Course/Manage",
                    "Morgan")),
        EmailNotificationType.ActivityConfirmed =>
            _service.ActivityConfirmed(
                Context(),
                "recipient@example.com",
                new ActivityConfirmedEmailData(
                    "Alex",
                    "Robotics Camp",
                    "/Activity/Manage",
                    new DateTime(2026, 7, 15, 13, 30, 0, DateTimeKind.Utc),
                    "America/Toronto")),
        _ => throw new ArgumentOutOfRangeException(nameof(notificationType), notificationType, null)
    };

    private static string GetActionPath(EmailNotificationType notificationType) => notificationType switch
    {
        EmailNotificationType.ScheduleChangeRequested => "/Staff/ScheduleRequests",
        EmailNotificationType.CourseConfirmationRequested => "/Child/MyConfirmations",
        EmailNotificationType.CourseConfirmed => "/Course/Manage",
        EmailNotificationType.ActivityConfirmed => "/Activity/Manage",
        _ => "/Child/MySchedules"
    };

    private static OrganizationEmailTemplateContext Context() => new(
        "North Star Academy",
        "support@northstar.example",
        new Uri("https://northstar.example/"),
        new OrganizationTerminology
        {
            OrganizationType = "Academy",
            ParticipantSingular = "Student",
            ParticipantPlural = "Students",
            ProviderSingular = "Instructor",
            ProviderPlural = "Instructors"
        });

    private static CourseScheduleEmailData ScheduleData() => new(
        "Alex",
        "Robotics",
        "Morgan",
        new DateTime(2026, 7, 15, 13, 30, 0, DateTimeKind.Utc),
        "America/Toronto",
        "/Child/MySchedules",
        1.5m,
        "Room 2");
}
