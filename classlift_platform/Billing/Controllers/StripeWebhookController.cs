using Billing.Models;
using Billing.Constants;
using Billing.Services.Integrations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;

namespace Billing.Controllers;


//当顾客完成付款后，Stripe 会向"webhooks/stripe"发送支付结果
[AllowAnonymous]
[Route("webhooks/stripe")]
public sealed class StripeWebhookController : Controller
{
    private readonly StripeService _stripe;
    private readonly ILogger<StripeWebhookController> _logger;

    public StripeWebhookController(
        StripeService stripe,
        ILogger<StripeWebhookController> logger)
    {
        _stripe = stripe;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        using var reader = new StreamReader(Request.Body);
        var payload = await reader.ReadToEndAsync(cancellationToken);
        var signature = Request.Headers["Stripe-Signature"].ToString();
        var traceId = HttpContext.TraceIdentifier;

        try
        {
			//验证请求是否真的来自 Stripe。
			//Stripe 会发送 Stripe - Signature，系统使用 WebhookSecret 验证它，防止别人伪造“付款成功”通知。
            var stripeEvent = _stripe.VerifyWebhook(payload, signature);

            _logger.LogInformation(
                "Stripe webhook received. TraceId={TraceId}, EventId={EventId}, EventType={EventType}, PayloadLength={PayloadLength}",
                traceId,
                stripeEvent.Id,
                stripeEvent.Type,
                payload.Length);

            if (stripeEvent.Type == "checkout.session.completed" && stripeEvent.Data.Object is Session session)
			{
				var scope = GetScope(session.Metadata);
				if (scope == BillingScope.AddOn)
					await _stripe.ProcessActivityManagementCheckoutCompletedAsync(session, cancellationToken);
				else if (scope == BillingScope.Plan && session.Metadata.ContainsKey("plan_id"))
					await _stripe.ProcessPlanCheckoutCompletedAsync(session, cancellationToken);
				else if (scope == BillingScope.Plan ||
				         (scope == null && session.Metadata.ContainsKey("invoice_id")))
				//找到对应的发票
				//确认发票金额和 Stripe 付款对应
				//创建 Payment 记录
                //把发票状态改成 Paid
                //保存 Stripe 的交易编号
				await _stripe.ProcessPlanInvoiceCheckoutCompletedAsync(session, cancellationToken);
				else
					_logger.LogWarning("Ignoring checkout.session.completed without a supported billing scope. EventId={EventId}", stripeEvent.Id);
			}
			else if (stripeEvent.Type == "invoice.paid" && stripeEvent.Data.Object is Stripe.Invoice paidInvoice)
			{
				var scope = GetScope(paidInvoice.Parent?.SubscriptionDetails?.Metadata);
				if (scope == BillingScope.AddOn)
					await _stripe.ProcessAddOnRecurringInvoicePaidAsync(paidInvoice, cancellationToken);
				else if (scope == BillingScope.Plan)
					await _stripe.ProcessPlanRecurringInvoicePaidAsync(paidInvoice, cancellationToken);
				else
					_logger.LogInformation("Ignoring invoice.paid outside the add-on billing scope. EventId={EventId}", stripeEvent.Id);
			}
			else if (stripeEvent.Type == "invoice.payment_failed" && stripeEvent.Data.Object is Stripe.Invoice failedInvoice)
			{
				var scope = GetScope(failedInvoice.Parent?.SubscriptionDetails?.Metadata);
				if (scope == BillingScope.AddOn)
					await _stripe.ProcessAddOnRecurringInvoicePaymentFailedAsync(failedInvoice, cancellationToken);
				else if (scope == BillingScope.Plan)
					await _stripe.ProcessPlanRecurringInvoicePaymentFailedAsync(failedInvoice, cancellationToken);
				else
					_logger.LogInformation("Ignoring invoice.payment_failed outside the add-on billing scope. EventId={EventId}", stripeEvent.Id);
			}
			else if (stripeEvent.Type == "customer.subscription.updated" && stripeEvent.Data.Object is Subscription updatedSubscription)
			{
				if (GetScope(updatedSubscription.Metadata) == BillingScope.AddOn)
					await _stripe.ProcessAddOnSubscriptionUpdatedAsync(updatedSubscription, cancellationToken);
				else if (GetScope(updatedSubscription.Metadata) == BillingScope.Plan)
					await _stripe.ProcessPlanSubscriptionUpdatedAsync(updatedSubscription, cancellationToken);
				else
					_logger.LogInformation("Ignoring customer.subscription.updated outside the add-on billing scope. EventId={EventId}", stripeEvent.Id);
			}
			else if (stripeEvent.Type == "customer.subscription.deleted" && stripeEvent.Data.Object is Subscription deletedSubscription)
			{
				if (GetScope(deletedSubscription.Metadata) == BillingScope.AddOn)
					await _stripe.ProcessAddOnSubscriptionDeletedAsync(deletedSubscription, cancellationToken);
				else if (GetScope(deletedSubscription.Metadata) == BillingScope.Plan)
					await _stripe.ProcessPlanSubscriptionDeletedAsync(deletedSubscription, cancellationToken);
				else
					_logger.LogInformation("Ignoring customer.subscription.deleted outside the add-on billing scope. EventId={EventId}", stripeEvent.Id);
			}
            return Ok();
        }
        catch (StripeException ex)
        {
            _logger.LogError(
                ex,
                "Stripe webhook failed with StripeException. TraceId={TraceId}, PayloadLength={PayloadLength}, HasSignature={HasSignature}, StripeErrorCode={StripeErrorCode}",
                traceId,
                payload.Length,
                !string.IsNullOrWhiteSpace(signature),
                ex.StripeError?.Code);

            return BadRequest(new
            {
                error = "stripe_webhook_error",
                traceId
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(
                ex,
                "Stripe webhook failed with business or configuration error. TraceId={TraceId}, PayloadLength={PayloadLength}, HasSignature={HasSignature}",
                traceId,
                payload.Length,
                !string.IsNullOrWhiteSpace(signature));

            return BadRequest(new
            {
                error = "stripe_webhook_configuration_or_business_error",
                traceId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unexpected Stripe webhook failure. TraceId={TraceId}, PayloadLength={PayloadLength}, HasSignature={HasSignature}",
                traceId,
                payload.Length,
                !string.IsNullOrWhiteSpace(signature));

            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                error = "unexpected_stripe_webhook_error",
                traceId
            });
        }
    }

    private static string? GetScope(IReadOnlyDictionary<string, string>? metadata)
        => metadata != null && metadata.TryGetValue("billing_scope", out var scope)
            ? scope.Trim().ToLowerInvariant()
            : null;
}
