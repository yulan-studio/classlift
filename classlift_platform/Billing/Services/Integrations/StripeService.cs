using Billing.Configuration;
using Billing.Constants;
using Billing.Data;
using Billing.Models;
using Billing.Services.Billing;
using Microsoft.AspNetCore;
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
            Metadata = new Dictionary<string, string>
            {
                ["billing_scope"] = BillingScope.Plan,
                ["invoice_id"] = invoice.InvoiceId.ToString()
            },
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

//	负责“创建支付页面”。

//它会：
//- 检查 Stripe 配置；
//- 检查当前 Plan 是否 eligible；
//- 检查是否已经购买 Add-on；
//- 创建数据库中配置的月度 Stripe Subscription Checkout Session；
//- 返回 Stripe Checkout URL。
//它不会授予 Activity Management 权限。

	public async Task<string> CreateActivityManagementCheckoutSessionAsync(
        int organizationId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_settings.SecretKey))
            throw new InvalidOperationException("Stripe payments are not configured. Please contact support.");
        if (!Uri.TryCreate(_settings.PublicBaseUrl, UriKind.Absolute, out var baseUri) ||
            (baseUri.Scheme != Uri.UriSchemeHttp && baseUri.Scheme != Uri.UriSchemeHttps))
            throw new InvalidOperationException("Stripe payment URL is not configured correctly. Please contact support.");

        var addOn = await _db.AddOns
            .Include(a => a.PlanEligibilities)
            .SingleOrDefaultAsync(a =>
                a.AddOnKey == FeatureKeys.ActivityManagement && a.IsActive,
                cancellationToken);
        if (addOn == null)
            throw new InvalidOperationException("Activity Management add-on is not available.");

        var planSubscription = await _db.OrganizationSubscriptions
            .Where(s => s.OrganizationId == organizationId &&
                        (s.Status == SubscriptionStatus.Active || s.Status == SubscriptionStatus.Trial))
            .OrderByDescending(s => s.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
        if (planSubscription == null || !addOn.PlanEligibilities.Any(e =>
                e.PlanId == planSubscription.PlanId && e.IsEligible))
            throw new InvalidOperationException("The current plan is not eligible for Activity Management.");

        var alreadySubscribed = await _db.OrganizationAddOnSubscriptions.AnyAsync(s =>
            s.OrganizationId == organizationId &&
            s.AddOnId == addOn.AddOnId &&
            (s.Status == AddOnStatus.Active || s.Status == AddOnStatus.Trial),
            cancellationToken);
        if (alreadySubscribed)
            throw new InvalidOperationException("Activity Management is already active for this organization.");

        StripeConfiguration.ApiKey = _settings.SecretKey;
        var baseUrl = baseUri.ToString().TrimEnd('/');
        var session = await new SessionService().CreateAsync(new SessionCreateOptions
        {
            Mode = "subscription",
            SuccessUrl = $"{baseUrl}/organization/addons/activity-management?payment=success",
            CancelUrl = $"{baseUrl}/organization/addons/activity-management?payment=cancelled",
            ClientReferenceId = $"organization:{organizationId}:addon:{addOn.AddOnKey}",
            Metadata = new Dictionary<string, string>
            {
                ["billing_scope"] = BillingScope.AddOn,
                ["organization_id"] = organizationId.ToString(),
                ["addon_key"] = addOn.AddOnKey
            },
            SubscriptionData = new SessionSubscriptionDataOptions
            {
                Metadata = new Dictionary<string, string>
                {
                    ["billing_scope"] = BillingScope.AddOn,
                    ["organization_id"] = organizationId.ToString(),
                    ["addon_key"] = addOn.AddOnKey
                }
            },
            LineItems = new List<SessionLineItemOptions>
            {
                new()
                {
                    Quantity = 1,
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = addOn.Currency.ToLowerInvariant(),
                        UnitAmount = checked((long)Math.Round(addOn.MonthlyPrice * 100m, 0)),
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = addOn.Name
                        },
                        Recurring = new SessionLineItemPriceDataRecurringOptions { Interval = "month" }
                    }
                }
            }
        }, cancellationToken: cancellationToken);

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

//	负责“支付成功后的处理”。

//它由 Stripe Webhook 调用，会：
//- 验证 Stripe 支付事件；
//- 读取 organization_id 和 addon_key；
//- 确认付款已完成；
//- 再次检查 Plan eligibility；
//- 创建 OrganizationAddOnSubscription；
//- 将 Add-on 状态设为 Active。

	public async Task ProcessActivityManagementCheckoutCompletedAsync(
        Session session,
        CancellationToken cancellationToken = default)
    {
        if (!session.Metadata.TryGetValue("organization_id", out var rawOrganizationId) ||
            !int.TryParse(rawOrganizationId, out var organizationId) ||
            !session.Metadata.TryGetValue("addon_key", out var addOnKey))
            throw new InvalidOperationException("Stripe add-on session is missing metadata.");

        if (session.PaymentStatus != "paid" && session.Status != "complete")
            return;

        var addOn = await _db.AddOns.SingleOrDefaultAsync(a =>
            a.AddOnKey == addOnKey && a.IsActive, cancellationToken);
        if (addOn == null)
            throw new InvalidOperationException("Stripe add-on does not exist.");

        var planSubscription = await _db.OrganizationSubscriptions
            .Where(s => s.OrganizationId == organizationId &&
                        (s.Status == SubscriptionStatus.Active || s.Status == SubscriptionStatus.Trial))
            .OrderByDescending(s => s.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
        if (planSubscription == null || !await _db.PlanAddOnEligibilities.AnyAsync(e =>
                e.PlanId == planSubscription.PlanId && e.AddOnId == addOn.AddOnId && e.IsEligible,
                cancellationToken))
            throw new InvalidOperationException("The organization plan is not eligible for this add-on.");

        var existing = await _db.OrganizationAddOnSubscriptions.SingleOrDefaultAsync(s =>
            s.OrganizationId == organizationId && s.AddOnId == addOn.AddOnId &&
            (s.Status == AddOnStatus.Active || s.Status == AddOnStatus.Trial), cancellationToken);
        if (existing != null)
            return;

        if (string.IsNullOrWhiteSpace(session.SubscriptionId))
            throw new InvalidOperationException("Stripe checkout session is missing its subscription ID.");

        var existingStripeSubscription = await _db.OrganizationAddOnSubscriptions.AnyAsync(s =>
            s.StripeSubscriptionId == session.SubscriptionId, cancellationToken);
        if (existingStripeSubscription)
            return;

        var now = DateTime.UtcNow;
        var subscription = new OrganizationAddOnSubscription
        {
            OrganizationId = organizationId,
            AddOnId = addOn.AddOnId,
            Status = AddOnStatus.Active,
            StartDate = now,
            CurrentPeriodEnd = now.AddMonths(1),
            MonthlyPrice = addOn.MonthlyPrice,
            Currency = addOn.Currency,
            BillingCycle = addOn.BillingCycle,
            AutoRenew = true,
            CancelAtPeriodEnd = false,
            StripeSubscriptionId = session.SubscriptionId,
            CreatedAt = now
        };

        _db.OrganizationAddOnSubscriptions.Add(subscription);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task ProcessRecurringInvoicePaidAsync(
        Stripe.Invoice invoice,
        CancellationToken cancellationToken = default)
    {
        var subscriptionId = invoice.Parent?.SubscriptionDetails?.SubscriptionId;
        if (string.IsNullOrWhiteSpace(subscriptionId)) return;

        var local = await _db.OrganizationAddOnSubscriptions.SingleOrDefaultAsync(
            s => s.StripeSubscriptionId == subscriptionId, cancellationToken);
        if (local == null) return;

        local.Status = AddOnStatus.Active;
        local.CancelledAt = null;
        local.EndDate = null;
        local.UpdatedAt = DateTime.UtcNow;
        if (invoice.PeriodEnd != default)
            local.CurrentPeriodEnd = invoice.PeriodEnd;

        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task ProcessRecurringInvoicePaymentFailedAsync(
        Stripe.Invoice invoice,
        CancellationToken cancellationToken = default)
    {
        var subscriptionId = invoice.Parent?.SubscriptionDetails?.SubscriptionId;
        if (string.IsNullOrWhiteSpace(subscriptionId)) return;

        var local = await _db.OrganizationAddOnSubscriptions.SingleOrDefaultAsync(
            s => s.StripeSubscriptionId == subscriptionId, cancellationToken);
        if (local == null) return;

        local.Status = AddOnStatus.PastDue;
        local.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task ProcessSubscriptionUpdatedAsync(
        Subscription stripeSubscription,
        CancellationToken cancellationToken = default)
    {
        var local = await _db.OrganizationAddOnSubscriptions.SingleOrDefaultAsync(
            s => s.StripeSubscriptionId == stripeSubscription.Id, cancellationToken);
        if (local == null) return;

        local.Status = MapStripeSubscriptionStatus(stripeSubscription.Status);
        local.CancelAtPeriodEnd = stripeSubscription.CancelAtPeriodEnd;
        local.UpdatedAt = DateTime.UtcNow;
        if (stripeSubscription.CancelAtPeriodEnd)
            local.EndDate = local.CurrentPeriodEnd;

        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task ProcessSubscriptionDeletedAsync(
        Subscription stripeSubscription,
        CancellationToken cancellationToken = default)
    {
        var local = await _db.OrganizationAddOnSubscriptions.SingleOrDefaultAsync(
            s => s.StripeSubscriptionId == stripeSubscription.Id, cancellationToken);
        if (local == null) return;

        var now = DateTime.UtcNow;
        local.Status = AddOnStatus.Cancelled;
        local.AutoRenew = false;
        local.CancelAtPeriodEnd = false;
        local.CancelledAt ??= now;
        local.EndDate ??= now;
        local.UpdatedAt = now;
        await _db.SaveChangesAsync(cancellationToken);
    }

    private static string MapStripeSubscriptionStatus(string? status) => status switch
    {
        "active" or "trialing" => AddOnStatus.Active,
        "past_due" or "unpaid" => AddOnStatus.PastDue,
        "canceled" or "incomplete_expired" => AddOnStatus.Cancelled,
        _ => AddOnStatus.PastDue
    };

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
