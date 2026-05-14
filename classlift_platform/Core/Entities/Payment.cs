using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Payment
    {
        public int PaymentID { get; set; }

        public int InvoiceID { get; set; }

        public string PaymentProvider { get; set; } = null!;

        public string ProviderTransactionID { get; set; } = null!;

        public decimal Amount { get; set; }

        public string Currency { get; set; } = "CAD";

        public PaymentStatus PaymentStatus { get; set; }

        public DateTime PaymentDate { get; set; }

        public string? Notes { get; set; }

        // Navigation Properties

        public Invoice Invoice { get; set; } = null!;
    }
}
