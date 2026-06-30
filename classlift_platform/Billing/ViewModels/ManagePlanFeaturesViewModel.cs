namespace Billing.ViewModels
{
    public class ManagePlanFeaturesViewModel
    {
        public int PlanId { get; set; }
        public string PlanName { get; set; } = "";

        public List<FeatureCheckboxItem> Features { get; set; } = new();
    }

    public class FeatureCheckboxItem
    {
        public int FeatureId { get; set; }
        public string FeatureKey { get; set; } = "";
        public string FeatureName { get; set; } = "";

        public bool IsLocked { get; set; }
        public bool IsSelected { get; set; }
    }
}