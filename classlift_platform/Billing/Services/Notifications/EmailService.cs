using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Net.Mail;
using Billing.Configuration;

namespace Billing.Services.Notifications
{
    public class EmailService
    {
        private readonly SmtpSettings _smtpSettings;

        public EmailService(IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;
        }

        public async Task SendWelcomeEmailAsync(
        string adminName,
        string adminEmail,
        string organizationName,
        string tenantUrl)
        {
            var subject = $"Welcome to ClassLift";

            var html = $@"
                <h2>Welcome, {adminName}!</h2>

                <p>Your organization <strong>{organizationName}</strong> has been successfully created.</p>

                <p>Your login account:</p>

                <ul>
                    <li><strong>Email:</strong> {adminEmail}</li>
                </ul>

                <p>Please use the password you created during signup.</p>

                <p>
                <a href='{tenantUrl}'>Sign in to your organization</a>
                </p>

                <p>If the button doesn't work, use this URL:</p>

                <p>{tenantUrl}</p>

                <hr/>

                <p>Need help? Contact support@classlift.ca.</p>
                ";

            await SendEmailAsync(adminEmail, subject, html);
        }


        public async Task SendEmailAsync(string to, string subject, string htmlBody)
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress(
                _smtpSettings.FromName,
                _smtpSettings.FromEmail));

            message.To.Add(MailboxAddress.Parse(to));

            message.Subject = subject;

            message.Body = new BodyBuilder
            {
                HtmlBody = htmlBody
            }.ToMessageBody();

            using var smtp = new MailKit.Net.Smtp.SmtpClient();

            await smtp.ConnectAsync(
                _smtpSettings.Host,
                _smtpSettings.Port,
                SecureSocketOptions.StartTls);

            await smtp.AuthenticateAsync(
                _smtpSettings.Username,
                _smtpSettings.Password);

            await smtp.SendAsync(message);

            await smtp.DisconnectAsync(true);
        }

    }
}
