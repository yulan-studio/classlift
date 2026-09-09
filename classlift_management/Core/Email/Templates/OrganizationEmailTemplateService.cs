using System.Globalization;
using System.Text;
using Core.Interfaces;

namespace Core.Email.Templates;

public sealed class OrganizationEmailTemplateService : IOrganizationEmailTemplateService
{
    private readonly ITimeZoneService _timeZoneService;

    public OrganizationEmailTemplateService(ITimeZoneService timeZoneService)
    {
        _timeZoneService = timeZoneService;
    }

    public TemplatedEmail CourseScheduleCreated(
        OrganizationEmailTemplateContext context,
        string recipientEmail,
        CourseScheduleEmailData data) =>
        BuildScheduleEmail(
            EmailNotificationType.CourseScheduleCreated,
            context,
            recipientEmail,
            data,
            "New course schedule",
            "A new course session has been scheduled.",
            "A new course session has been scheduled");

    public TemplatedEmail CourseScheduleUpdated(
        OrganizationEmailTemplateContext context,
        string recipientEmail,
        CourseScheduleEmailData data) =>
        BuildScheduleEmail(
            EmailNotificationType.CourseScheduleUpdated,
            context,
            recipientEmail,
            data,
            "Course schedule updated",
            "A course session schedule has been updated.",
            "A course session schedule has been updated");

    public TemplatedEmail CourseScheduleDeleted(
        OrganizationEmailTemplateContext context,
        string recipientEmail,
        CourseScheduleEmailData data) =>
        BuildScheduleEmail(
            EmailNotificationType.CourseScheduleDeleted,
            context,
            recipientEmail,
            data,
            "Course schedule deleted",
            "A course session has been removed from the schedule.",
            "A course session has been removed from the schedule");

    public TemplatedEmail CourseSessionCompleted(
        OrganizationEmailTemplateContext context,
        string recipientEmail,
        CourseSessionCompletedEmailData data)
    {
        ValidateContext(context);
        ValidateRequired(recipientEmail, nameof(recipientEmail));
        ValidateRequired(data.ParticipantName, nameof(data.ParticipantName));
        ValidateRequired(data.CourseName, nameof(data.CourseName));
        ValidateRequired(data.ProviderName, nameof(data.ProviderName));
        if (data.ActualHours <= 0)
            throw new ArgumentOutOfRangeException(nameof(data.ActualHours), "Actual hours must be greater than zero.");

        var schedule = FormatSchedule(data.ScheduledAtUtc, data.TimeZoneId);
        var details = new[]
        {
            ("Course", data.CourseName),
            (context.Terminology.ParticipantSingular, data.ParticipantName),
            (context.Terminology.ProviderSingular, data.ProviderName),
            ("Scheduled time", schedule),
            ("Actual hours", data.ActualHours.ToString("0.##", CultureInfo.InvariantCulture))
        };

        return Build(
            EmailNotificationType.CourseSessionCompleted,
            context,
            recipientEmail,
            $"{SubjectValue(data.CourseName)}: session completed",
            "Course session completed",
            "A course session has been marked as completed.",
            "A course session has been marked as completed",
            details,
            data.ActionPath);
    }

    public TemplatedEmail ScheduleChangeRequested(
        OrganizationEmailTemplateContext context,
        string recipientEmail,
        ScheduleChangeRequestedEmailData data)
    {
        ValidateContext(context);
        ValidateRequired(recipientEmail, nameof(recipientEmail));
        ValidateRequired(data.ParticipantName, nameof(data.ParticipantName));
        ValidateRequired(data.CourseName, nameof(data.CourseName));
        ValidateRequired(data.RequestedBy, nameof(data.RequestedBy));

        var details = new List<(string Label, string Value)>
        {
            ("Course", data.CourseName),
            (context.Terminology.ParticipantSingular, data.ParticipantName),
            ("Requested by", data.RequestedBy)
        };
        if (!string.IsNullOrWhiteSpace(data.RequestNote))
            details.Add(("Request note", data.RequestNote.Trim()));

        return Build(
            EmailNotificationType.ScheduleChangeRequested,
            context,
            recipientEmail,
            $"{SubjectValue(data.CourseName)}: schedule change requested",
            "Schedule change requested",
            "A course schedule change has been requested.",
            "A course schedule change has been requested",
            details,
            data.ActionPath);
    }

    public TemplatedEmail CourseConfirmed(
        OrganizationEmailTemplateContext context,
        string recipientEmail,
        CourseConfirmedEmailData data)
    {
        ValidateContext(context);
        ValidateRequired(recipientEmail, nameof(recipientEmail));
        ValidateRequired(data.ParticipantName, nameof(data.ParticipantName));
        ValidateRequired(data.CourseName, nameof(data.CourseName));
        ValidateRequired(data.CourseType, nameof(data.CourseType));

        var details = new List<(string Label, string Value)>
        {
            ("Course", data.CourseName),
            ("Course type", data.CourseType),
            (context.Terminology.ParticipantSingular, data.ParticipantName)
        };
        if (!string.IsNullOrWhiteSpace(data.ProviderName))
            details.Add((context.Terminology.ProviderSingular, data.ProviderName.Trim()));

        return Build(
            EmailNotificationType.CourseConfirmed,
            context,
            recipientEmail,
            $"{SubjectValue(data.CourseName)}: course confirmed",
            "Course confirmed",
            "A course registration has been confirmed.",
            "A course registration has been confirmed",
            details,
            data.ActionPath);
    }

    public TemplatedEmail ActivityConfirmed(
        OrganizationEmailTemplateContext context,
        string recipientEmail,
        ActivityConfirmedEmailData data)
    {
        ValidateContext(context);
        ValidateRequired(recipientEmail, nameof(recipientEmail));
        ValidateRequired(data.ParticipantName, nameof(data.ParticipantName));
        ValidateRequired(data.ActivityName, nameof(data.ActivityName));

        var details = new List<(string Label, string Value)>
        {
            ("Activity", data.ActivityName),
            (context.Terminology.ParticipantSingular, data.ParticipantName)
        };
        if (data.ScheduledAtUtc.HasValue)
        {
            ValidateRequired(data.TimeZoneId, nameof(data.TimeZoneId));
            details.Add(("Scheduled time", FormatSchedule(data.ScheduledAtUtc.Value, data.TimeZoneId!)));
        }

        return Build(
            EmailNotificationType.ActivityConfirmed,
            context,
            recipientEmail,
            $"{SubjectValue(data.ActivityName)}: activity confirmed",
            "Activity confirmed",
            "An activity registration has been confirmed.",
            "An activity registration has been confirmed",
            details,
            data.ActionPath);
    }

    private TemplatedEmail BuildScheduleEmail(
        EmailNotificationType notificationType,
        OrganizationEmailTemplateContext context,
        string recipientEmail,
        CourseScheduleEmailData data,
        string heading,
        string htmlIntroduction,
        string textIntroduction)
    {
        ValidateContext(context);
        ValidateRequired(recipientEmail, nameof(recipientEmail));
        ValidateRequired(data.ParticipantName, nameof(data.ParticipantName));
        ValidateRequired(data.CourseName, nameof(data.CourseName));
        ValidateRequired(data.ProviderName, nameof(data.ProviderName));

        var details = new List<(string Label, string Value)>
        {
            ("Course", data.CourseName),
            (context.Terminology.ParticipantSingular, data.ParticipantName),
            (context.Terminology.ProviderSingular, data.ProviderName),
            ("Scheduled time", FormatSchedule(data.ScheduledAtUtc, data.TimeZoneId))
        };
        if (data.ScheduledHours.HasValue)
            details.Add(("Scheduled hours", data.ScheduledHours.Value.ToString("0.##", CultureInfo.InvariantCulture)));
        if (!string.IsNullOrWhiteSpace(data.Location))
            details.Add(("Location", data.Location.Trim()));

        return Build(
            notificationType,
            context,
            recipientEmail,
            $"{SubjectValue(data.CourseName)}: {heading.ToLowerInvariant()}",
            heading,
            htmlIntroduction,
            textIntroduction,
            details,
            data.ActionPath);
    }

    private static TemplatedEmail Build(
        EmailNotificationType notificationType,
        OrganizationEmailTemplateContext context,
        string recipientEmail,
        string subject,
        string heading,
        string htmlIntroduction,
        string textIntroduction,
        IEnumerable<(string Label, string Value)> details,
        string portalPath)
    {
        if (!EmailAddressValidation.IsValid(recipientEmail))
            throw new ArgumentException("The recipient email address is invalid.", nameof(recipientEmail));
        ValidateActionPath(portalPath);

        var portalUri = new Uri(context.TrustedPortalBaseUri, portalPath);
        var detailList = details.ToList();
        var html = new StringBuilder()
            .Append("<p>Hello,</p>")
            .Append("<h2>").Append(EmailHtml.Encode(heading)).Append("</h2>")
            .Append("<p>").Append(EmailHtml.Encode(htmlIntroduction)).Append("</p>")
            .Append("<ul>");

        foreach (var (label, value) in detailList)
        {
            html.Append("<li><strong>")
                .Append(EmailHtml.Encode(label))
                .Append(":</strong> ")
                .Append(EmailHtml.Encode(value))
                .Append("</li>");
        }

        html.Append("</ul><p><a href=\"")
            .Append(EmailHtml.Encode(portalUri.AbsoluteUri))
            .Append("\">Open your ClassLift portal</a></p><p>Thank you,<br>")
            .Append(EmailHtml.Encode(context.OrganizationName))
            .Append(" Support Team</p>");

        var text = new StringBuilder()
            .AppendLine("Hello,")
            .AppendLine()
            .AppendLine(textIntroduction)
            .AppendLine();
        foreach (var (label, value) in detailList)
            text.Append(label).Append(": ").AppendLine(value);

        text.AppendLine()
            .Append("Open your ClassLift portal: ").AppendLine(portalUri.AbsoluteUri)
            .AppendLine()
            .Append("Thank you,").AppendLine()
            .Append(context.OrganizationName).Append(" Support Team");

        return new TemplatedEmail(
            notificationType,
            new EmailMessage(
                context.ReplyToEmail,
                recipientEmail.Trim(),
                subject,
                html.ToString(),
                text.ToString()));
    }

    private string FormatSchedule(DateTime scheduledAtUtc, string timeZoneId)
    {
        ValidateRequired(timeZoneId, nameof(timeZoneId));
        var local = _timeZoneService.ConvertUtcToLocal(scheduledAtUtc, timeZoneId);
        return $"{local.ToString("MMMM d, yyyy 'at' h:mm tt", CultureInfo.InvariantCulture)} ({timeZoneId})";
    }

    private static void ValidateContext(OrganizationEmailTemplateContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        ValidateRequired(context.OrganizationName, nameof(context.OrganizationName));
        ValidateRequired(context.ReplyToEmail, nameof(context.ReplyToEmail));
        ArgumentNullException.ThrowIfNull(context.Terminology);

        if (!EmailAddressValidation.IsValid(context.ReplyToEmail))
            throw new ArgumentException("The organization Reply-To address is invalid.", nameof(context.ReplyToEmail));
        if (!context.TrustedPortalBaseUri.IsAbsoluteUri
            || (context.TrustedPortalBaseUri.Scheme != Uri.UriSchemeHttps
                && context.TrustedPortalBaseUri.Scheme != Uri.UriSchemeHttp))
            throw new ArgumentException("The trusted portal URL must be an absolute HTTP or HTTPS URL.", nameof(context.TrustedPortalBaseUri));

        ValidateRequired(context.Terminology.ParticipantSingular, nameof(context.Terminology.ParticipantSingular));
        ValidateRequired(context.Terminology.ProviderSingular, nameof(context.Terminology.ProviderSingular));
    }

    private static string SubjectValue(string value) =>
        value.Replace('\r', ' ').Replace('\n', ' ').Trim();

    private static void ValidateActionPath(string actionPath)
    {
        ValidateRequired(actionPath, nameof(actionPath));
        if (!actionPath.StartsWith('/')
            || actionPath.StartsWith("//", StringComparison.Ordinal)
            || Uri.TryCreate(actionPath, UriKind.Absolute, out _))
        {
            throw new ArgumentException("The portal action path must be a local absolute path.", nameof(actionPath));
        }
    }

    private static void ValidateRequired(string? value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("A required email-template value is missing.", parameterName);
    }
}
