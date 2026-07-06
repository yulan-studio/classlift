using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class TenantRegistry
    {
        [Key]
        public int TenantRegistryId { get; set; }

        public int OrganizationId { get; set; }

        public string DatabaseName { get; set; }

        public string? Subdomain { get; set; }

        public string? CustomDomain { get; set; }

        public bool IsActive { get; set; }

        public virtual Organization Organization { get; set; } = null!;
    }

}
