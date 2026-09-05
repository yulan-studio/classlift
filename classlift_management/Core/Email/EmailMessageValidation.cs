namespace Core.Email;

internal static class EmailMessageValidation
{
    internal static string? GetErrorCode(EmailMessage message)
    {
        if (!EmailAddressValidation.IsValid(message.To))
        {
            return "invalid_recipient";
        }

        if (string.IsNullOrWhiteSpace(message.Subject))
        {
            return "missing_subject";
        }

        if (string.IsNullOrWhiteSpace(message.HtmlBody)
            && string.IsNullOrWhiteSpace(message.TextBody))
        {
            return "missing_body";
        }

        return null;
    }
}
