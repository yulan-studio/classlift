using Billing.Services.Billing;
using System.Text;

namespace Billing.Services.Jobs
{
    public class MonthlyBillingJob
    {
        private readonly InvoiceService _invoiceService;
        private readonly BillingRunService _billingRunService;
        private readonly ILogger<MonthlyBillingJob> _logger;

        public MonthlyBillingJob(
            InvoiceService invoiceService,
            BillingRunService billingRunService,
            ILogger<MonthlyBillingJob> logger)
        {
            _invoiceService = invoiceService;
            _billingRunService = billingRunService;
            _logger = logger;
        }

        public async Task RunAsync()
        {
            _logger.LogInformation(
                "Monthly Billing Job started at {Time}.",
                DateTime.UtcNow);

            var run = await _billingRunService.StartRunAsync("MonthlyBilling");

            try
            {
                var processed = await _invoiceService.GenerateRecurringInvoicesAsync();

                await _billingRunService.CompleteRunAsync(run, 0, processed, 0);
                _logger.LogInformation(
                    "Monthly Billing Job completed successfully at {Time}.",
                    DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Monthly Billing Job failed at {Time}.",
                    DateTime.UtcNow);

                throw;
            }
        }
    }
}