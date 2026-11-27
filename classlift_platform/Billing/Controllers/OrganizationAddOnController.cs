using Billing.Configuration;
using Billing.Constants;
using Billing.Data;
using Billing.Services.Integrations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace Billing.Controllers;

[Authorize(Policy = OrganizationAuthorization.Policy)]
[ApiController]
[Route("api/organization/addons")]
public sealed class OrganizationAddOnController : ControllerBase
{
    private readonly BillingDbContext _context;
    private readonly StripeService _stripe;

    public OrganizationAddOnController(BillingDbContext context, StripeService stripe)
    {
        _context = context;
        _stripe = stripe;
    }

    [HttpGet("activity-management")]
    public async Task<IActionResult> ActivityManagement(CancellationToken cancellationToken)
    {
        var organizationId = await GetOrganizationIdAsync(cancellationToken);
        if (organizationId == null) return Forbid();

        var addOn = await _context.AddOns.AsNoTracking().SingleOrDefaultAsync(
            a => a.AddOnKey == FeatureKeys.ActivityManagement && a.IsActive,
            cancellationToken);
        if (addOn == null) return NotFound();

		//检查当前 Organization 的 Plan；
		var plan = await _context.OrganizationSubscriptions.AsNoTracking()
            .Where(s => s.OrganizationId == organizationId &&
                        (s.Status == SubscriptionStatus.Active || s.Status == SubscriptionStatus.Trial))
            .OrderByDescending(s => s.CreatedAt)
            .Select(s => new { s.PlanId, s.Plan.PlanName })
            .FirstOrDefaultAsync(cancellationToken);
		//检查 Plan 是否对activity-management eligible；
		var eligible = plan != null && await _context.PlanAddOnEligibilities.AnyAsync(e =>
            e.PlanId == plan.PlanId && e.AddOnId == addOn.AddOnId && e.IsEligible, cancellationToken);
		//检查是否已有 Active activity-management；
		var active = await _context.OrganizationAddOnSubscriptions.AnyAsync(s =>
            s.OrganizationId == organizationId && s.AddOnId == addOn.AddOnId &&
            (s.Status == AddOnStatus.Active || s.Status == AddOnStatus.Trial), cancellationToken);

        return Ok(new
        {
            addOnKey = addOn.AddOnKey,
            name = addOn.Name,
            monthlyPrice = addOn.MonthlyPrice,
            currency = addOn.Currency,
            planName = plan?.PlanName,
            eligible,
            active,
            canPurchase = eligible && !active
        });
    }

    [HttpPost("activity-management/checkout")]
    public async Task<IActionResult> CreateActivityManagementCheckout(CancellationToken cancellationToken)
    {
        var organizationId = await GetOrganizationIdAsync(cancellationToken);
        if (organizationId == null) return Forbid();

        try
        {
            var url = await _stripe.CreateActivityManagementCheckoutSessionAsync(
                organizationId.Value, cancellationToken);
            return Ok(new { checkoutUrl = url });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (StripeException)
        {
            return Problem(
                title: "Stripe checkout could not be created",
                statusCode: StatusCodes.Status502BadGateway);
        }
    }

    private async Task<int?> GetOrganizationIdAsync(CancellationToken cancellationToken)
    {
        var userId = OrganizationAuthorization.GetUserId(User);
        if (userId == null) return null;

        return await _context.OrganizationAdmins
            .Where(a => a.UserId == userId && a.IsActive)
            .Select(a => (int?)a.OrganizationId)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
