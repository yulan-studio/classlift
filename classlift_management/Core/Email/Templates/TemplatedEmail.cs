namespace Core.Email.Templates;

public sealed record TemplatedEmail(
    EmailNotificationType NotificationType,
    EmailMessage Message);
