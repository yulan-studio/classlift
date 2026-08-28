namespace Core.Models
{
    public class OrganizationSubscription
    {
        public int OrganizationSubscriptionId { get; set; }

        public int OrganizationId { get; set; }

        public int PlanId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string Status { get; set; } = null!;

        public bool IsTrial { get; set; }

        public DateTime? TrialStartDate { get; set; }

        public DateTime? TrialEndDate { get; set; }

        public DateTime? ActivatedAt { get; set; }

        public DateTime? CancelledAt { get; set; }

        public DateTime? LastBilledDate { get; set; }

        public bool AutoRenew { get; set; }

        public decimal MonthlyPricePerCoach { get; set; }

        public decimal MinimumMonthlyPrice { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string? OrganizationSubscriptionsColumn { get; set; }

        public virtual Organization Organization { get; set; } = null!;

        public virtual SubscriptionPlan Plan { get; set; } = null!;
    }
}
