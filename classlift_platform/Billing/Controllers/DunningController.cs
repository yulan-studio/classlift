using Billing.Services.Jobs;
using Microsoft.AspNetCore.Mvc;

namespace Billing.Controllers
{
    public class DunningController : Controller
    {
        private readonly DunningJob _dunningJob;

        public DunningController(DunningJob dunningJob)
        {
            _dunningJob = dunningJob;
        }

        public async Task<IActionResult> Run()
        {
            await _dunningJob.RunAsync();

            return Content("Dunning job completed.");
        }
    }
}