using System;
using System.Collections.Generic;

namespace Billing.Models;

public partial class Tenantregistry
{
    public int TenantRegistryId { get; set; }

    public int OrganizationId { get; set; }

    public string DatabaseName { get; set; } = null!;

    public string? Subdomain { get; set; }

    public string? CustomDomain { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Organization Organization { get; set; } = null!;
}
