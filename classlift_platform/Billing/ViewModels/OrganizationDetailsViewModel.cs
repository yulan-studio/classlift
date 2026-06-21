using Billing.Models;

namespace Billing.ViewModels
{
    public class OrganizationDetailsViewModel
    {
        public Organization Organization { get; set; } = null!;

        public List<OrganizationSubscription> Subscriptions { get; set; } = new();

        public List<Invoice> Invoices { get; set; } = new();

        public List<Payment> Payments { get; set; } = new();

        public Tenantregistry? Tenant { get; set; }

        public decimal TotalRevenue { get; set; }
    }
}