using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Billing.ViewModels
{
    public class CreateOrganizationViewModel
    {
        [Required(ErrorMessage = "Organization Name is required.")]
        public string OrganizationName { get; set; } = "";
        [Required(ErrorMessage = "Contact Name is required.")]
        public string? ContactName { get; set; }
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }
        [Required(ErrorMessage = "Please select a subscription plan.")]
        public int PlanId { get; set; }
        [Required(ErrorMessage = "Database Name is required.")]
        [MaxLength(20)]
        //public string DatabaseName { get; set; } = "";
        //[Required(ErrorMessage = "Subdomain is required.")]
        public string? Subdomain { get; set; }

        public List<SelectListItem> Plans { get; set; } = new();
    }
}