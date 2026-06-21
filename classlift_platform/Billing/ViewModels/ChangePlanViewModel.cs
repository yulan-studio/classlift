using Microsoft.AspNetCore.Mvc.Rendering;

namespace Billing.ViewModels
{
    public class ChangePlanViewModel
    {
        public int OrganizationId { get; set; }

        public string OrganizationName { get; set; } = "";

        public int? CurrentPlanId { get; set; }

        public string? CurrentPlanName { get; set; }

        public int NewPlanId { get; set; }

        public string? Reason { get; set; }

        public List<SelectListItem> Plans { get; set; } = new();
    }
}
