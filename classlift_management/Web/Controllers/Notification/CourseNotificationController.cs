using Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.Notification
{
    public class CourseNotificationController : Controller
    {
        //This is a sample controller for sending email notifications about course activities.
        private readonly EmailService _emailService;

        public CourseNotificationController(EmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task<IActionResult> Send(string toEmail, string subject, string body)
        {
            await _emailService.SendEmailAsync(toEmail, subject, body);
            //return Content("Email sent successfully!");
            return Ok();
        }
    }
}
