namespace Billing.ViewModels
{
    public class OrganizationFeatureContext
    {
        public int OrganizationId { get; set; }

        public int PlanId { get; set; }

        public string PlanName { get; set; } = "";

        public HashSet<string> Features { get; set; } = new();
    }
}