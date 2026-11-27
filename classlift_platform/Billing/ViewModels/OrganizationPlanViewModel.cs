using Billing.Models;

namespace Billing.ViewModels;

public sealed class OrganizationPlanViewModel
{
    public required Organization Organization { get; init; }
    public OrganizationSubscription? CurrentSubscription { get; init; }
    public required IReadOnlyList<Subscriptionplan> Plans { get; init; }
    public int CoachCount { get; init; }
    public PendingPlanChange? PendingChange { get; init; }
}
