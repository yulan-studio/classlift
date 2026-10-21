using Billing.Configuration;
using Billing.Data;
using Billing.Models;
using Billing.Services.Billing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using PlatformInvoice = Billing.Models.Invoice;

namespace Billing.Services.Integrations;

public sealed class StripeService
{
    private readonly BillingDbContext _db;
    private readonly StripeSettings _settings;
    private readonly PaymentService _payments;

    public StripeService(BillingDbContext db, IOptions<StripeSettings> settings, PaymentService payments)
    { _db = db; _settings = settings.Value; _payments = payments; }

    public async Task<string> CreateCheckoutSessionAsync(PlatformInvoice invoice, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_settings.SecretKey))
            throw new InvalidOperationException("Stripe payments are not configured. Please contact support.");
        if (!Uri.TryCreate(_settings.PublicBaseUrl, UriKind.Absolute, out var baseUri) ||
            (baseUri.Scheme != Uri.UriSchemeHttp && baseUri.Scheme != Uri.UriSchemeHttps))
            throw new InvalidOperationException("Stripe payment URL is not configured correctly. Please contact support.");
        if (invoice.InvoiceStatus is "Paid" or "Cancelled") throw new InvalidOperationException("This invoice cannot be paid.");
        StripeConfiguration.ApiKey = _settings.SecretKey;
        var baseUrl = baseUri.ToString().TrimEnd('/');
        var session = await new SessionService().CreateAsync(new SessionCreateOptions
        {
            Mode = "payment",
            SuccessUrl = $"{baseUrl}/organization/invoice/{invoice.InvoiceId}?payment=success",
            CancelUrl = $"{baseUrl}/organization/invoice/{invoice.InvoiceId}?payment=cancelled",
            ClientReferenceId = invoice.InvoiceId.ToString(),
            Metadata = new Dictionary<string, string> { ["invoice_id"] = invoice.InvoiceId.ToString() },
            LineItems = new List<SessionLineItemOptions> { new()
            {
                Quantity = 1,
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = "cad",
                    UnitAmount = checked((long)Math.Round(invoice.TotalAmount * 100m, 0)),
                    ProductData = new SessionLineItemPriceDataProductDataOptions { Name = $"ClassLift invoice #{invoice.InvoiceId}" }
                }
            }}
        }, cancellationToken: cancellationToken);
        invoice.StripeCheckoutSessionId = session.Id;
        await _db.SaveChangesAsync(cancellationToken);
        return session.Url;
    }

    public async Task ProcessCheckoutCompletedAsync(Session session, CancellationToken cancellationToken = default)
    {
        var rawId = session.Metadata.TryGetValue("invoice_id", out var metadataId) ? metadataId : session.ClientReferenceId;
        if (!int.TryParse(rawId, out var invoiceId)) throw new InvalidOperationException("Stripe session is missing invoice metadata.");
        var invoice = await _db.Invoices.Include(i => i.Payments).SingleOrDefaultAsync(i => i.InvoiceId == invoiceId, cancellationToken);
        if (invoice == null) throw new InvalidOperationException("Invoice not found.");
        if (invoice.StripeCheckoutSessionId != null && invoice.StripeCheckoutSessionId != session.Id) throw new InvalidOperationException("Stripe session mismatch.");
        if (session.PaymentStatus != "paid" || invoice.InvoiceStatus == "Paid") return;
        invoice.StripeCheckoutSessionId ??= session.Id;
        invoice.StripePaymentIntentId = session.PaymentIntentId;
        await _payments.RecordPaymentAsync(invoice.InvoiceId, "Stripe", session.PaymentIntentId ?? session.Id, invoice.TotalAmount, "CAD", $"Stripe Checkout session {session.Id}", cancellationToken: cancellationToken);
    }

    public Event VerifyWebhook(string payload, string signature)
    {
        if (string.IsNullOrWhiteSpace(_settings.WebhookSecret)) throw new InvalidOperationException("Stripe webhook is not configured.");
        return EventUtility.ConstructEvent(payload, signature, _settings.WebhookSecret);
    }

    public async Task<string?> GetCheckoutUrlAsync(string sessionId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_settings.SecretKey)) return null;
        StripeConfiguration.ApiKey = _settings.SecretKey;
        return (await new SessionService().GetAsync(sessionId, cancellationToken: cancellationToken)).Url;
    }
}
