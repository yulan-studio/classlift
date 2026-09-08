namespace Core.Email;

public sealed record EmailMessage(

    string ReplyTo,
    string To,
    string Subject,
    string HtmlBody,
    string? TextBody = null);
