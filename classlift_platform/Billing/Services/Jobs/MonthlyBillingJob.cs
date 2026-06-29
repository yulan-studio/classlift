using Billing.Services.Billing;

namespace Billing.Services.Jobs
{
    public class MonthlyBillingJob
    {
        private readonly InvoiceService _invoiceService;
        private readonly ILogger<MonthlyBillingJob> _logger;

        public MonthlyBillingJob(
            InvoiceService invoiceService,
            ILogger<MonthlyBillingJob> logger)
        {
            _invoiceService = invoiceService;
            _logger = logger;
        }

        public async Task RunAsync()
        {
            _logger.LogInformation(
                "Monthly Billing Job started at {Time}.",
                DateTime.UtcNow);

            try
            {
                await _invoiceService.GenerateRecurringInvoicesAsync();

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