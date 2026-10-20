using System;
using System.Collections.Generic;

namespace Billing.Models;

public partial class Invoice
{
    public int InvoiceId { get; set; }

    public int OrganizationId { get; set; }

    public int OrganizationSubscriptionId { get; set; }

    public int PlanId { get; set; }

    public int? PromotionId { get; set; }

    public DateOnly BillingPeriodStart { get; set; }

    public DateOnly BillingPeriodEnd { get; set; }

    public DateOnly DueDate { get; set; }

    public int CoachCount { get; set; }

    public decimal PricePerCoach { get; set; }

    public decimal Subtotal { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal TotalAmount { get; set; }

    public string InvoiceStatus { get; set; } = null!;

    public DateTime GeneratedAt { get; set; }

    public DateTime? PaidAt { get; set; }

	//Stripe Checkout 页面会话的 ID。它对应顾客一次打开的支付页面（顾客进入哪个付款页面）
	//主要用于：
    //找回付款页面
    //生成二维码
    //查询 Checkout 状态
    //关联某张发票
	public string? StripeCheckoutSessionId { get; set; }

	//Stripe 实际付款意图的 ID（Stripe 实际处理哪一笔付款）
	//主要用于：
	//确认实际付款
	//查询付款状态
	//退款
	//对账

	public string? StripePaymentIntentId { get; set; }

    public virtual Organization Organization { get; set; } = null!;

    public virtual OrganizationSubscription OrganizationSubscription { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Subscriptionplan Plan { get; set; } = null!;

    public virtual Promotion? Promotion { get; set; }
}
