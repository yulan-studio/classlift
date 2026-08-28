namespace Core.Models
{
    public class SubscriptionPlan
    {
        public int PlanId { get; set; }

        public string PlanName { get; set; } = null!;

        public string? Description { get; set; }

        public decimal PricePerCoach { get; set; }

        public decimal MinimumMonthlyPrice { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public virtual ICollection<PlanFeature> PlanFeatures { get; set; } = new List<PlanFeature>();

        public virtual ICollection<Organization> CurrentPlanOrganizations { get; set; } = new List<Organization>();

        public virtual ICollection<OrganizationSubscription> OrganizationSubscriptions { get; set; } = new List<OrganizationSubscription>();
    }
}
