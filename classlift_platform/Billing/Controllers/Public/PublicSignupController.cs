using Billing.Interfaces;
using Billing.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Billing.Controllers.Public
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/public/signup")]

    


    public class PublicSignupController : ControllerBase
    {
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
