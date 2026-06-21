namespace Billing.ViewModels
{
    public class BillingDashboardViewModel
    {
        public decimal TotalRevenue { get; set; }
        public decimal PendingAmount { get; set; }
        public decimal OverdueAmount { get; set; }

        public int TotalOrganizations { get; set; }
        public int ActiveSubscriptions { get; set; }

        public int PendingInvoices { get; set; }
        public int PaidInvoices { get; set; }
        public int OverdueInvoices { get; set; }

        public List<PlanCountItem> PlanCounts { get; set; } = new();
    }

    public class PlanCountItem
    {
        public string PlanName { get; set; } = "";
        public int Count { get; set; }
    }
}