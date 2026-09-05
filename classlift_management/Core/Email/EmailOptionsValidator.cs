using Microsoft.Extensions.Options;

namespace Core.Email;

public sealed class EmailOptionsValidator : IValidateOptions<EmailOptions>
{
    private readonly bool _requireSmtpSettings;

    public EmailOptionsValidator(bool requireSmtpSettings = true)
    {
        _requireSmtpSettings = requireSmtpSettings;
    }

    public ValidateOptionsResult Validate(string? name, EmailOptions options)
    {
        if (!options.Enabled || !_requireSmtpSettings)
        {
            return ValidateOptionsResult.Success;
        }

        var failures = new List<string>();

        if (string.IsNullOrWhiteSpace(options.Host))
        {
            failures.Add("Email:Host is required when email is enabled.");
        }

        if (options.Port is <= 0 or > 65535)
        {
            failures.Add("Email:Port must be a valid TCP port.");
        }

        if (string.IsNullOrWhiteSpace(options.Username))
        {
            failures.Add("Email:Username is required when email is enabled.");
        }

        if (string.IsNullOrWhiteSpace(options.Password))
        {
            failures.Add("Email:Password is required when email is enabled.");
        }

        if (!EmailAddressValidation.IsValid(options.SenderEmail))
        {
            failures.Add("Email:SenderEmail must be a valid email address.");
        }

        if (string.IsNullOrWhiteSpace(options.SenderName))
        {
            failures.Add("Email:SenderName is required when email is enabled.");
        }

        if (options.TimeoutSeconds is < 1 or > 300)
        {
            failures.Add("Email:TimeoutSeconds must be between 1 and 300.");
        }

        return failures.Count == 0
            ? ValidateOptionsResult.Success
            : ValidateOptionsResult.Fail(failures);
    }
}
