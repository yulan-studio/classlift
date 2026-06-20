using Billing.Data;
using Billing.Models;
using Microsoft.EntityFrameworkCore;

namespace Billing.Services
{
    public class PaymentService
    {
        private readonly BillingDbContext _context;

        public PaymentService(BillingDbContext context)
        {
            _context = context;
        }

        public async Task<Payment> RecordPaymentAsync(
            int invoiceId,
            string paymentProvider,
            string providerTransactionId,
            decimal amount,
            string currency = "CAD",
            string? notes = null)
        {
            var invoice = await _context.Invoices
                .Include(i => i.Payments)
                .FirstOrDefaultAsync(i => i.InvoiceId == invoiceId);

            if (invoice == null)
                throw new Exception("Invoice not found.");

            if (invoice.InvoiceStatus == "Paid")
                throw new Exception("Invoice is already paid.");

            if (invoice.InvoiceStatus == "Cancelled")
                throw new Exception("Cancelled invoice cannot be paid.");

            if (amount != invoice.TotalAmount)
                throw new Exception("Payment amount must equal invoice total. Partial payment is not supported.");

            var payment = new Payment
            {
                InvoiceId = invoice.InvoiceId,
                PaymentProvider = paymentProvider,
                ProviderTransactionId = providerTransactionId,
                Amount = amount,
                Currency = currency,
                PaymentStatus = "Succeeded",
                PaymentDate = DateTime.Now,
                Notes = notes
            };

            _context.Payments.Add(payment);

            invoice.InvoiceStatus = "Paid";
            invoice.PaidAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return payment;
        }
    }
}