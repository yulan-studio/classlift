namespace Core.Email;

public sealed class NullEmailService : IEmailService
{
    public Task<EmailSendResult> SendAsync(
        EmailMessage message,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var errorCode = EmailMessageValidation.GetErrorCode(message);
        return Task.FromResult(errorCode is null
            ? EmailSendResult.Disabled()
            : EmailSendResult.Failed(errorCode));
    }
}
