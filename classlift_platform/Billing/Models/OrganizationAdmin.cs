namespace Billing.Models;

/// <summary>
/// Grants a platform Identity user access to one organization as its billing administrator.
/// The user id is the ASP.NET Identity string id from BillingDbContext.
/// </summary>
public sealed class OrganizationAdmin
{
    public int OrganizationAdminId { get; set; }
    public string UserId { get; set; } = null!;
    public int OrganizationId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }

    public Organization Organization { get; set; } = null!;
}
