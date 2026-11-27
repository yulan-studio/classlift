using Billing.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Billing.Controllers;

[Authorize(Policy = OrganizationAuthorization.Policy)]
[Route("organization/addons")]
public sealed class OrganizationAddOnPageController : Controller
{
    [HttpGet("activity-management")]
    public IActionResult ActivityManagement() => View();
}
