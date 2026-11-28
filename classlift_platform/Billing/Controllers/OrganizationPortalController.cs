using Billing.Configuration;
using Billing.Constants;
using Billing.Data;
using Billing.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Billing.Services.Integrations;
using Billing.Services.Billing;
using PlatformInvoiceService = Billing.Services.Billing.InvoiceService;
using QRCoder;
using Stripe;

namespace Billing.Controllers;

[Authorize(Policy = OrganizationAuthorization.Policy)]
[Route("organization")]
public sealed class OrganizationPortalController : Controller
{
    private readonly BillingDbContext _context;
    private readonly StripeService _stripe;
    private readonly PlatformInvoiceService _invoices;

    public OrganizationPortalController(BillingDbContext context, StripeService stripe, PlatformInvoiceService invoices)
    { _context = context; _stripe = stripe; _invoices = invoices; }

    [HttpGet("plan")]
    public async Task<IActionResult> Plan(CancellationToken cancellationToken)
    {
        var organizationId = await GetOrganizationIdAsync(cancellationToken);
        if (organizationId == null) return Forbid();
        var organization = await _context.Organizations.Include(o => o.CurrentPlan)
            .SingleOrDefaultAsync(o => o.OrganizationId == organizationId, cancellationToken);
        if (organization == null) return NotFound();
        var current = await _context.OrganizationSubscriptions.Include(s => s.Plan)
            .Where(s => s.OrganizationId == organizationId && (s.Status == SubscriptionStatus.Active || s.Status == SubscriptionStatus.Trial))
            .OrderByDescending(s => s.CreatedAt).FirstOrDefaultAsync(cancellationToken);
        var coachCount = await _invoices.GetCoachCountAsync(organizationId.Value);
        var pending = await _context.PendingPlanChanges.Include(p => p.RequestedPlan)
            .Where(p => p.OrganizationId == organizationId && p.Status == "PendingPayment")
            .OrderByDescending(p => p.CreatedAt).FirstOrDefaultAsync(cancellationToken);
        return View(new OrganizationPlanViewModel
        {
            Organization = organization,
            CurrentSubscription = current,
            Plans = await _context.Subscriptionplans.Where(p => p.IsActive).OrderBy(p => p.PricePerCoach).ToListAsync(cancellationToken),
            CoachCount = coachCount,
            PendingChange = pending
        });
    }

    [HttpPost("plan/checkout")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PlanCheckout(int planId, CancellationToken cancellationToken)
    {
        var organizationId = await GetOrganizationIdAsync(cancellationToken);
        if (organizationId == null) return Forbid();
        try
        {
            var url = await _stripe.CreatePlanCheckoutSessionAsync(organizationId.Value, planId, cancellationToken);
            return Redirect(url);
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Plan));
        }
        catch (StripeException)
        {
            TempData["Error"] = "Stripe could not create the Plan payment session. Please try again.";
            return RedirectToAction(nameof(Plan));
        }
    }

    [HttpPost("plan/pending-payment")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResumePendingPlanPayment(CancellationToken cancellationToken)
    {
        var organizationId = await GetOrganizationIdAsync(cancellationToken);
        if (organizationId == null) return Forbid();
        var pending = await _context.PendingPlanChanges
            .SingleOrDefaultAsync(p => p.OrganizationId == organizationId && p.Status == "PendingPayment",
                cancellationToken);
        if (pending == null || string.IsNullOrWhiteSpace(pending.StripeCheckoutSessionId))
        {
            TempData["Error"] = "The pending Plan payment could not be found.";
            return RedirectToAction(nameof(Plan));
        }

        var checkoutUrl = await _stripe.GetCheckoutUrlAsync(pending.StripeCheckoutSessionId, cancellationToken);
        if (string.IsNullOrWhiteSpace(checkoutUrl))
        {
            TempData["Error"] = "The pending Stripe Checkout session is no longer available. Please start a new Plan change.";
            pending.Status = "Expired";
            await _context.SaveChangesAsync(cancellationToken);
            return RedirectToAction(nameof(Plan));
        }

        return Redirect(checkoutUrl);
    }

    private async Task<int?> GetOrganizationIdAsync(CancellationToken cancellationToken)
    {
        var userId = OrganizationAuthorization.GetUserId(User);
        if (userId == null) return null;
        return await _context.OrganizationAdmins.Where(a => a.UserId == userId && a.IsActive)
            .Select(a => (int?)a.OrganizationId).FirstOrDefaultAsync(cancellationToken);
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var userId = OrganizationAuthorization.GetUserId(User);
        if (userId == null) return Forbid();

        var organizationIds = await _context.OrganizationAdmins
            .Where(a => a.UserId == userId && a.IsActive)
            .Select(a => a.OrganizationId)
            .ToListAsync(cancellationToken);

        if (organizationIds.Count == 0)
            return Forbid();

        // A billing admin may currently be assigned to one organization. If the
        // model later supports multiple organizations, add an explicit selector.
        var organizationId = organizationIds[0];
        var organization = await _context.Organizations
            .Include(o => o.CurrentPlan)
            .FirstOrDefaultAsync(o => o.OrganizationId == organizationId, cancellationToken);
        if (organization == null) return NotFound();

        var invoices = await _context.Invoices
            .Include(i => i.Plan)
            .Where(i => i.OrganizationId == organizationId)
            .OrderByDescending(i => i.GeneratedAt)
            .ToListAsync(cancellationToken);

        var payments = await _context.Payments
            .Include(p => p.Invoice)
            .Where(p => p.Invoice.OrganizationId == organizationId)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync(cancellationToken);

        var subscription = await _context.OrganizationSubscriptions
            .Include(s => s.Plan)
            .Where(s => s.OrganizationId == organizationId)
            .OrderByDescending(s => s.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        var addOns = await _context.AddOns
            .AsNoTracking()
            .Where(a => a.IsActive)
            .Select(a => new
            {
                AddOn = a,
                IsEligible = subscription != null && _context.PlanAddOnEligibilities.Any(e =>
                    e.PlanId == subscription.PlanId && e.AddOnId == a.AddOnId && e.IsEligible),
                IsActive = _context.OrganizationAddOnSubscriptions.Any(s =>
                    s.OrganizationId == organizationId && s.AddOnId == a.AddOnId &&
                    s.Status == AddOnStatus.Active)
            })
            .ToListAsync(cancellationToken);

        return View(new OrganizationPortalViewModel
        {
            Organization = organization,
            Invoices = invoices,
            Payments = payments,
            CurrentSubscription = subscription,
            AddOns = addOns.Select(item => new OrganizationAddOnViewModel
            {
                AddOnKey = item.AddOn.AddOnKey,
                Name = item.AddOn.Name,
                Status = item.IsActive ? "Enabled" : "Disabled",
                IsEligible = item.IsEligible,
                ManagementUrl = item.AddOn.AddOnKey == FeatureKeys.ActivityManagement
                    ? "/organization/addons/activity-management"
                    : "/organization"
            }).ToList()
        });
    }

    [HttpGet("invoice/{id:int}")]
    public async Task<IActionResult> Invoice(int id, CancellationToken cancellationToken)
    {
        var invoice = await _context.Invoices
            .Include(i => i.Plan)
            .Include(i => i.Payments)
            .FirstOrDefaultAsync(i => i.InvoiceId == id, cancellationToken);
        if (invoice == null) return NotFound();

        if (!await OrganizationAuthorization.HasAccessAsync(
                _context, User, invoice.OrganizationId, cancellationToken))
            return Forbid();

        return View(invoice);
    }

    [HttpPost("invoice/{id:int}/checkout")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout(int id, CancellationToken cancellationToken)
    {
        var invoice = await _context.Invoices.FirstOrDefaultAsync(i => i.InvoiceId == id, cancellationToken);
        if (invoice == null) return NotFound();
        if (!await OrganizationAuthorization.HasAccessAsync(_context, User, invoice.OrganizationId, cancellationToken)) return Forbid();
        try
        {
            await _stripe.CreatePlanInvoiceCheckoutSessionAsync(invoice, cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }
        catch (StripeException)
        {
            TempData["Error"] = "Stripe could not create the payment session. Please try again or contact support.";
        }
        return RedirectToAction(nameof(Invoice), new { id, checkout = "created" });
    }

    [HttpPost("invoice/{id:int}/pay")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PayInvoice(int id, CancellationToken cancellationToken)
    {
        var invoice = await _context.Invoices.FirstOrDefaultAsync(i => i.InvoiceId == id, cancellationToken);
        if (invoice == null) return NotFound();
        if (!await OrganizationAuthorization.HasAccessAsync(_context, User, invoice.OrganizationId, cancellationToken)) return Forbid();

        try
        {
            var checkoutUrl = await _stripe.CreatePlanInvoiceCheckoutSessionAsync(invoice, cancellationToken);
            return Redirect(checkoutUrl);
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }
        catch (StripeException)
        {
            TempData["Error"] = "Stripe could not create the payment session. Please try again or contact support.";
        }

        return RedirectToAction(nameof(Invoice), new { id });
    }

    [HttpGet("invoice/{id:int}/qr")]
    public async Task<IActionResult> QrCode(int id, CancellationToken cancellationToken)
    {
        var invoice = await _context.Invoices.AsNoTracking().FirstOrDefaultAsync(i => i.InvoiceId == id, cancellationToken);
        if (invoice?.StripeCheckoutSessionId == null || !await OrganizationAuthorization.HasAccessAsync(_context, User, invoice.OrganizationId, cancellationToken)) return NotFound();
        var url = await _stripe.GetCheckoutUrlAsync(invoice.StripeCheckoutSessionId, cancellationToken);
        if (url == null) return NotFound();
        using var generator = new QRCodeGenerator();
        using var data = generator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
        using var qr = new PngByteQRCode(data);
        return File(qr.GetGraphic(8), "image/png");
    }
}
