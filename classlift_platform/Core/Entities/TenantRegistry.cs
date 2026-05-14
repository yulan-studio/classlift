using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class TenantRegistry
    {
        public int TenantRegistryID { get; set; }

        public int OrganizationID { get; set; }

        public string DatabaseName { get; set; } = null!;

        public string ConnectionString { get; set; } = null!;

        public string? Subdomain { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        // Navigation Properties

        public Organization Organization { get; set; } = null!;
    }
}
