using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public sealed class CurrentTenant
    {
        public int? OrganizationId { get; set; }

        public string? Subdomain { get; set; }

        public string? DatabaseName { get; set; }

        public string? ConnectionString { get; set; }

        public OrganizationTerminology Terminology { get; set; } = new();

        public bool IsResolved =>
            !string.IsNullOrWhiteSpace(DatabaseName) &&
            !string.IsNullOrWhiteSpace(ConnectionString);
    }
}
