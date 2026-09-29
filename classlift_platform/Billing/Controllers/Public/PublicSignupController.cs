using Billing.Interfaces;
using Billing.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Text;

namespace Billing.Controllers.Public
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/public/signup")]

    


    public class PublicSignupController : ControllerBase
    {
        private static readonly SemaphoreSlim SignupIpLogLock = new(1, 1);
        private readonly IOrganizationSignupService _signupService;
        private readonly ILogger<PublicSignupController> _logger;

        public PublicSignupController(IOrganizationSignupService signupService, ILogger<PublicSignupController> logger)
        {
            _signupService = signupService;
            _logger = logger;
        }

        [HttpPost]
        [EnableRateLimiting("public-signup")]
        public async Task<IActionResult> Signup([FromBody] PublicSignupRequest request)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            _logger.LogInformation(
                "Signup request from IP {IpAddress} for organization: {OrganizationName}",
                ipAddress,
                request.OrganizationName);

            await WriteSignupIpLogAsync(ipAddress, request.OrganizationName);

            _logger.LogInformation("Received signup request for organization: {OrganizationName}", request.OrganizationName);

            try
            {
                var result = await _signupService.CreateOrganizationAsync(request);

                if (!result.Success)
                {
                    _logger.LogWarning(
                       "Signup failed for organization: {OrganizationName}. Reason: {Message}",
                       request.OrganizationName,
                       result.Message);

                    return BadRequest(new
                    {
                        message = result.Message
                    });
                }

                return Ok(new
                {
                    message = result.Message
                });
            }
            catch(Exception ex)
            {
                _logger.LogError(ex,
                   "Unexpected error occurred while processing signup for organization: {OrganizationName}",
                   request.OrganizationName);

                return StatusCode(500, new
                {
                    message = "An unexpected error occurred."
                });
            }
        }

        private async Task WriteSignupIpLogAsync(string ipAddress, string organizationName)
        {
            var logDirectory = Path.Combine(AppContext.BaseDirectory, "logs");
            var logFile = Path.Combine(logDirectory, "signup-ip.log");

            try
            {
                Directory.CreateDirectory(logDirectory);

                var line = $"{DateTime.UtcNow:O}\tIP={ipAddress}\tOrganization={organizationName}{Environment.NewLine}";

                await SignupIpLogLock.WaitAsync();
                try
                {
                    await System.IO.File.AppendAllTextAsync(logFile, line, Encoding.UTF8);
                }
                finally
                {
                    SignupIpLogLock.Release();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to write signup IP log file.");
            }
        }

        [HttpGet("verify")]
        public async Task<IActionResult> Verify([FromQuery] string token)
        {
            var result = await _signupService.ConfirmEmailAsync(token);
            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Redirect(result.TenantUrl);
        }
    }
}
