namespace Billing.Models
{
    public class OrganizationSignupResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string TenantUrl { get; set; } = string.Empty;
    }
}
