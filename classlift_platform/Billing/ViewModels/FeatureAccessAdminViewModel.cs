using Microsoft.AspNetCore.Mvc.Rendering;

namespace Billing.ViewModels
{
    public class FeatureAccessAdminViewModel
    {
        public int? SelectedOrganizationId { get; set; }

        public string? OrganizationName { get; set; }

        public string? PlanName { get; set; }

        public List<SelectListItem> Organizations { get; set; } = new();

        public List<FeatureAccessItem> Features { get; set; } = new();
    }

    public class FeatureAccessItem
    {
        public string FeatureKey { get; set; } = "";
        public string FeatureName { get; set; } = "";
        public bool HasAccess { get; set; }
    }
}