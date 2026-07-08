namespace Billing.Models
{
    public class PublicSignupRequest
    {
        public string OrganizationName { get; set; } = string.Empty;
        public string Subdomain { get; set; } = string.Empty;
        public string AdminName { get; set; } = string.Empty;
        public string AdminEmail { get; set; } = string.Empty;
        public string AdminPassword { get; set; } = string.Empty;

        public int PlanId { get; set; } = 1;
    }
}
