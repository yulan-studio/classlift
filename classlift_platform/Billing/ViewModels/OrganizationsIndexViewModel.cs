using Billing.Models;

namespace Billing.ViewModels;

public class OrganizationsIndexViewModel
{
    public IReadOnlyList<Organization> Organizations { get; init; } = [];
    public IReadOnlyList<Organization> CancelledRegistrations { get; init; } = [];
}
