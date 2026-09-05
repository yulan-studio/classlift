using Microsoft.Extensions.Logging;

namespace Core.Email;

public sealed class DevelopmentEmailService : IEmailService
{
    private readonly DevelopmentEmailStore _store;
    private readonly ILogger<DevelopmentEmailService> _logger;

    public DevelopmentEmailService(
        DevelopmentEmailStore store,
        ILogger<DevelopmentEmailService> logger)
    {
        _store = store;
        _logger = logger;
    }

    public Task<EmailSendResult> SendAsync(
        EmailMessage message,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var errorCode = EmailMessageValidation.GetErrorCode(message);
        if (errorCode is not null)
        {
            _logger.LogWarning("Development email was rejected. Error code: {ErrorCode}", errorCode);
            return Task.FromResult(EmailSendResult.Failed(errorCode));
        }

        _store.Add(message);
        _logger.LogInformation("Development email captured for {RecipientDomain}", GetDomain(message.To));
        return Task.FromResult(EmailSendResult.Captured());
    }

    private static string GetDomain(string address)
    {
        var separator = address.LastIndexOf('@');
        return separator >= 0 ? address[(separator + 1)..] : "unknown";
    }
}
