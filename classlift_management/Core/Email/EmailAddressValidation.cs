using MimeKit;

namespace Core.Email;

internal static class EmailAddressValidation
{
    internal static bool IsValid(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)
            || !MailboxAddress.TryParse(value, out var mailbox))
        {
            return false;
        }

        var separator = mailbox.Address.LastIndexOf('@');
        return separator > 0
            && separator < mailbox.Address.Length - 1
            && !mailbox.Address[(separator + 1)..].Any(char.IsWhiteSpace);
    }
}
