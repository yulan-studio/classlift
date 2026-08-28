namespace Core.Models
{
    public class PlanFeature
    {
        public int PlanFeatureId { get; set; }

        public int PlanId { get; set; }

        public int FeatureId { get; set; }

        public bool IsLocked { get; set; }

        public DateTime CreatedAt { get; set; }

        public virtual SubscriptionPlan Plan { get; set; } = null!;

        public virtual Feature Feature { get; set; } = null!;
    }
}
