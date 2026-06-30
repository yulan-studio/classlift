using Billing.Services.Billing;
using System.Text;

namespace Billing.Services.Jobs
{
    public class DailyBillingJob
    {
        private readonly InvoiceService _invoiceService;
        private readonly DunningService _dunningService;
        private readonly BillingRunService _billingRunService;
        
        private readonly ILogger<DailyBillingJob> _logger;

        public DailyBillingJob(
            InvoiceService invoiceService,
            DunningService dunningService,
            BillingRunService billingRunService,
            ILogger<DailyBillingJob> logger)
        {
            _invoiceService = invoiceService;
            _dunningService = dunningService;
            _billingRunService = billingRunService;
            _logger = logger;
        }

        public async Task RunAsync()
        {
            var run = await _billingRunService.StartRunAsync("DailyBilling");

            _logger.LogInformation(
                "Daily Billing Job started at {Time}.",
                DateTime.UtcNow);

            try
            {
                var activated = await _invoiceService.ActivateExpiredTrialsAsync();

                var overDued = await _dunningService.MarkOverdueInvoicesAsync();

                await _billingRunService.CompleteRunAsync(run, activated, 0, overDued);
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