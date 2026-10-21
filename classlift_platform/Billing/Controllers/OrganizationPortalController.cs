using Billing.Configuration;
using Billing.Data;
using Billing.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Billing.Services.Integrations;
using QRCoder;
using Stripe;

namespace Billing.Controllers;

[Authorize(Policy = OrganizationAuthorization.Policy)]
[Route("organization")]
public sealed class OrganizationPortalController : Controller
{
    private readonly BillingDbContext _context;
    private readonly StripeService _stripe;

    public OrganizationPortalController(BillingDbContext context, StripeService stripe) { _context = context; _stripe = stripe; }

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

        return View(new OrganizationPortalViewModel
        {
            Organization = organization,
            Invoices = invoices,
            Payments = payments,
            CurrentSubscription = subscription
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
            await _stripe.CreateCheckoutSessionAsync(invoice, cancellationToken);
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
