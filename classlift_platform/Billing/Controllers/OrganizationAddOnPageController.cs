using Billing.Configuration;
using Billing.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Billing.Controllers;

[Authorize(Policy = OrganizationAuthorization.Policy)]
[Route("organization/addons")]
public sealed class OrganizationAddOnPageController : Controller
{
    private readonly BillingDbContext _context;

    public OrganizationAddOnPageController(BillingDbContext context) => _context = context;

    [HttpGet("activity-management")]
    public async Task<IActionResult> ActivityManagement(CancellationToken cancellationToken)
    {
        var userId = OrganizationAuthorization.GetUserId(User);
        if (userId == null || !await _context.OrganizationAdmins.AnyAsync(
                a => a.UserId == userId && a.IsActive, cancellationToken))
            return Forbid();

        return View();
    }
}
