
﻿using Billing.Services.Billing;

namespace Billing.Services.Jobs
{
    public class DunningJob
    {
        private readonly DunningService _dunningService;

        public DunningJob(DunningService dunningService)
        {
            _dunningService = dunningService;
        }

        public async Task RunAsync()
        {
            await _dunningService.MarkOverdueInvoicesAsync();
        }
    }
}