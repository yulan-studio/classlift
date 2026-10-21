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
    public StripeWebhookController(StripeService stripe) => _stripe = stripe;

    [HttpPost]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        using var reader = new StreamReader(Request.Body);
        var payload = await reader.ReadToEndAsync(cancellationToken);
        try
        {
			//验证请求是否真的来自 Stripe。
			//Stripe 会发送 Stripe - Signature，系统使用 WebhookSecret 验证它，防止别人伪造“付款成功”通知。
            var stripeEvent = _stripe.VerifyWebhook(payload, Request.Headers["Stripe-Signature"].ToString());
            if (stripeEvent.Type == "checkout.session.completed" && stripeEvent.Data.Object is Session session)
				//找到对应的发票
				//确认发票金额和 Stripe 付款对应
				//创建 Payment 记录
                //把发票状态改成 Paid
                //保存 Stripe 的交易编号
				await _stripe.ProcessCheckoutCompletedAsync(session, cancellationToken);
            return Ok();
        }
        catch (StripeException) { return BadRequest(); }
        catch (InvalidOperationException) { return BadRequest(); }
    }
}
