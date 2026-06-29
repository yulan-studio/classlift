using Billing.Services.Billing;

namespace Billing.Services.Jobs
{
    public class DailyBillingJob
    {
        private readonly InvoiceService _invoiceService;
        private readonly DunningService _dunningService;
        private readonly ILogger<DailyBillingJob> _logger;

        public DailyBillingJob(
            InvoiceService invoiceService,
            DunningService dunningService,
            ILogger<DailyBillingJob> logger)
        {
            _invoiceService = invoiceService;
            _dunningService = dunningService;
            _logger = logger;
        }

        public async Task RunAsync()
        {
            _logger.LogInformation(
                "Daily Billing Job started at {Time}.",
                DateTime.UtcNow);

            try
            {
                await _invoiceService.ActivateExpiredTrialsAsync();

                await _dunningService.MarkOverdueInvoicesAsync();

                _logger.LogInformation(
                    "Daily Billing Job completed successfully at {Time}.",
                    DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Daily Billing Job failed at {Time}.",
                    DateTime.UtcNow);

                throw;
            }
        }
    }
}