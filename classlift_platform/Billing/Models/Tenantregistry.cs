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

    // Public-signup tenants remain inactive until this one-time token is verified.
    public string? EmailVerificationTokenHash { get; set; }

    public DateTime? EmailVerificationExpiresAt { get; set; }

    public DateTime? ActivatedAt { get; set; }

    public virtual Organization Organization { get; set; } = null!;
}
