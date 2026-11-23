using System.Globalization;

namespace Core.Services;

public static class DirectPaymentInstructionRenderer
{
    public static string Render(string? template, decimal amount, string paymentScope) =>
        (string.IsNullOrWhiteSpace(template)
            ? OrganizationPaymentSettingsService.DefaultDirectPaymentInstructionTemplate
            : template)
        .Replace("{amount}", amount.ToString("C", CultureInfo.CurrentCulture), StringComparison.OrdinalIgnoreCase)
        .Replace("{payment_scope}", paymentScope, StringComparison.OrdinalIgnoreCase);
}
