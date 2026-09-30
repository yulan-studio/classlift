using Billing.Configuration;
using Billing.Data;
using Billing.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Billing.Controllers;

[Authorize(Policy = OrganizationAuthorization.Policy)]
[Route("organization")]
public sealed class OrganizationPortalController : Controller
{
    private readonly BillingDbContext _context;

    public OrganizationPortalController(BillingDbContext context) => _context = context;

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
}
