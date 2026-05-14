using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Organization
    {
        public int OrganizationID { get; set; }

        public string OrganizationName { get; set; } = null!;

        public string? ContactName { get; set; }

        public string? ContactEmail { get; set; }

        public string? ContactPhone { get; set; }

        public int? CurrentPlanID { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        // Navigation Properties

        public SubscriptionPlan? CurrentPlan { get; set; }

        public ICollection<Invoice> Invoices { get; set; }
            = new List<Invoice>();

        public ICollection<TenantRegistry> TenantRegistries { get; set; }
            = new List<TenantRegistry>();
    }
}
