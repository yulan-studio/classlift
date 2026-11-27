namespace Billing.Constants;

/// <summary>
/// Identifies which billing workflow owns a Stripe object or event.
/// </summary>
public static class BillingScope
{
    public const string Plan = "plan";
    public const string AddOn = "addon";
}
