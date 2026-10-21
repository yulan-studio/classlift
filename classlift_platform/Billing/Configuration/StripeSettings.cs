namespace Billing.Configuration;

public sealed class StripeSettings
{
	//Stripe 后台获取的服务器密钥
	public string SecretKey { get; set; } = string.Empty;
	//用于验证 Stripe Webhook 确实来自 Stripe
    public string WebhookSecret { get; set; } = string.Empty;
	//填写顾客和 Stripe 可以访问到的网站根地址
	public string PublicBaseUrl { get; set; } = string.Empty;
}
