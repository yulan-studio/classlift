using Core.Interfaces;
using Core.Repositories;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{
    public class SmtpSettings
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public string SenderName { get; set; }
        public string SenderEmail { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class EmailService
    {
        //public async Task SendEmailAsync(string toEmail, string subject, string body)
        //{
        //    var smtpClient = new SmtpClient("smtp.yourmail.com")
        //    {
        //        Port = 587, // 常用：587 (TLS) 或 465 (SSL)
        //        Credentials = new NetworkCredential("qiangyulan@hotmail.com", "Hlanlan78123!"),
        //        EnableSsl = true,
        //    };

        //    var mailMessage = new MailMessage
        //    {
        //        From = new MailAddress("your-email@domain.com"),
        //        Subject = subject,
        //        Body = body,
        //        IsBodyHtml = true, // 可以发HTML邮件
        //    };
        //    mailMessage.To.Add(toEmail);

        //    await smtpClient.SendMailAsync(mailMessage);
        //}

        private readonly SmtpSettings _smtpSettings;


        //public EmailService(IEmailService emailService)
        //{
        //    _activityRepository = activityRepository;
        //}
        public EmailService(IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            using var smtpClient = new SmtpClient(_smtpSettings.Server)
            {
                Port = _smtpSettings.Port,
                Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password.Substring(0, _smtpSettings.Password.Length -2) ),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_smtpSettings.SenderEmail, _smtpSettings.SenderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
