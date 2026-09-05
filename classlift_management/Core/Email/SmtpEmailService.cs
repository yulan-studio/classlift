using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Core.Email;

public sealed class SmtpEmailService : IEmailService
{
    private readonly EmailOptions _options;
    private readonly ILogger<SmtpEmailService> _logger;

    public SmtpEmailService(
        IOptions<EmailOptions> options,
        ILogger<SmtpEmailService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task<EmailSendResult> SendAsync(
        EmailMessage message,
        CancellationToken cancellationToken = default)
    {
        var errorCode = EmailMessageValidation.GetErrorCode(message);
        if (errorCode is not null)
        {
            _logger.LogWarning("Email was rejected before sending. Error code: {ErrorCode}", errorCode);
            return EmailSendResult.Failed(errorCode);
        }

        var mimeMessage = CreateMessage(message);

        try
        {
            using var client = new SmtpClient
            {
                Timeout = checked(_options.TimeoutSeconds * 1000)
            };

            await client.ConnectAsync(
                _options.Host,
                _options.Port,
                GetSocketOptions(_options.Security),
                cancellationToken);
            await client.AuthenticateAsync(_options.Username, _options.Password, cancellationToken);
            await client.SendAsync(mimeMessage, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);

            _logger.LogInformation("Email sent to {RecipientDomain}", GetDomain(message.To));
            return EmailSendResult.Sent();
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Email delivery failed for {RecipientDomain}", GetDomain(message.To));
            return EmailSendResult.Failed("delivery_failed");
        }
    }

    private MimeMessage CreateMessage(EmailMessage message)
    {
        var mimeMessage = new MimeMessage();
        mimeMessage.From.Add(new MailboxAddress(_options.SenderName, _options.SenderEmail));
        mimeMessage.To.Add(MailboxAddress.Parse(message.To));
        mimeMessage.Subject = message.Subject;
        mimeMessage.Body = new BodyBuilder
        {
            HtmlBody = message.HtmlBody,
            TextBody = message.TextBody
        }.ToMessageBody();
        return mimeMessage;
    }

    private static SecureSocketOptions GetSocketOptions(EmailSecurity security) => security switch
    {
        EmailSecurity.StartTls => SecureSocketOptions.StartTls,
        EmailSecurity.SslOnConnect => SecureSocketOptions.SslOnConnect,
        _ => throw new ArgumentOutOfRangeException(nameof(security), security, null)
    };

    private static string GetDomain(string address)
    {
        var separator = address.LastIndexOf('@');
        return separator >= 0 ? address[(separator + 1)..] : "unknown";
    }
}
