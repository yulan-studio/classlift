using Microsoft.AspNetCore.Mvc;
using Billing.Interfaces;
using Billing.Models;

namespace Billing.Controllers.Public
{
    [ApiController]
    [Route("api/public/signup")]

    


    public class PublicSignupController : ControllerBase
    {
        private readonly IOrganizationSignupService _signupService;

        public PublicSignupController(IOrganizationSignupService signupService)
        {
            _signupService = signupService;
        }

        [HttpPost]
        public async Task<IActionResult> Signup([FromBody] PublicSignupRequest request)
        {
            var result = await _signupService.CreateOrganizationAsync(request);

            if (!result.Success)
            {
                return BadRequest(new
                {
                    message = result.Message
                });
            }

            return Ok(new
            {
                tenantUrl = result.TenantUrl
            });
        }
    }
}
