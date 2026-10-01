using Microsoft.AspNetCore.Mvc.Rendering;

namespace Billing.ViewModels;

public sealed class OrganizationAdminManagementViewModel
{
    public List<OrganizationAdminRow> Administrators { get; set; } = [];
    public List<SelectListItem> Organizations { get; set; } = [];
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public int OrganizationId { get; set; }
}

public sealed record OrganizationAdminRow(
    int BindingId,
    string Email,
    string? DisplayName,
    string OrganizationName,
    bool IsActive,
    DateTime CreatedAt);
