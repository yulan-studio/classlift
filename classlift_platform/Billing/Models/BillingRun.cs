namespace Billing.Models;

public partial class BillingRun
{
    public int BillingRunId { get; set; }

    public string? RunType { get; set; }

    public DateTime StartedAt { get; set; }

    public DateTime FinishedAt { get; set; }

    public string? Status { get; set; }

    public int? DurationMilliseconds { get; set; }

    public string? ErrorMessage { get; set; }

    public int TrialActivated { get; set; }

    public int InvoicesGenerated { get; set; }

    public int InvoicesMarkedOverdue { get; set; }
}