namespace Core.Models
{
    public class Feature
    {
        public int FeatureId { get; set; }

        public string FeatureKey { get; set; } = null!;

        public string FeatureName { get; set; } = null!;

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; }

        public virtual ICollection<PlanFeature> PlanFeatures { get; set; } = new List<PlanFeature>();
    }
}
