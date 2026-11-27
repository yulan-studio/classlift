using Billing.Models;
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
				if (session.Metadata.ContainsKey("addon_key"))
					await _stripe.ProcessActivityManagementCheckoutCompletedAsync(session, cancellationToken);
				else
				//找到对应的发票
				//确认发票金额和 Stripe 付款对应
				//创建 Payment 记录
                //把发票状态改成 Paid
                //保存 Stripe 的交易编号
				await _stripe.ProcessCheckoutCompletedAsync(session, cancellationToken);
			}
			else if (stripeEvent.Type == "invoice.paid" && stripeEvent.Data.Object is Stripe.Invoice paidInvoice)
			{
				await _stripe.ProcessRecurringInvoicePaidAsync(paidInvoice, cancellationToken);
			}
			else if (stripeEvent.Type == "invoice.payment_failed" && stripeEvent.Data.Object is Stripe.Invoice failedInvoice)
			{
				await _stripe.ProcessRecurringInvoicePaymentFailedAsync(failedInvoice, cancellationToken);
			}
			else if (stripeEvent.Type == "customer.subscription.updated" && stripeEvent.Data.Object is Subscription updatedSubscription)
			{
				await _stripe.ProcessSubscriptionUpdatedAsync(updatedSubscription, cancellationToken);
			}
			else if (stripeEvent.Type == "customer.subscription.deleted" && stripeEvent.Data.Object is Subscription deletedSubscription)
			{
				await _stripe.ProcessSubscriptionDeletedAsync(deletedSubscription, cancellationToken);
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
}
