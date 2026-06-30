using Billing.Services.Jobs;
using Microsoft.AspNetCore.Mvc;

namespace Billing.Controllers
{
    //[Route("[controller]")]
    [Route("Billing")]
    public class BillingController : Controller
    {
        private readonly MonthlyBillingJob _monthlyBillingJob;
        private readonly DailyBillingJob _dailyBillingJob;

        public BillingController(MonthlyBillingJob monthlyBillingJob, DailyBillingJob dailyBillingJob)
        {
            _monthlyBillingJob = monthlyBillingJob;
            _dailyBillingJob = dailyBillingJob;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("RunMonthlyJob")]
        public async Task<IActionResult> RunMonthlyJob()
        {
            await _monthlyBillingJob.RunAsync();

            TempData["Success"] = "Monthly billing completed successfully.";

            return RedirectToAction(nameof(Index));
        }


        [HttpPost("RunDailyJob")]
        public async Task<IActionResult> RunDailyJob()
        {
            await _dailyBillingJob.RunAsync();

            TempData["Success"] = "Daily billing completed successfully.";

            return RedirectToAction(nameof(Index));
        }


        //[HttpPost("GenerateInvoices")]
        //public async Task<IActionResult> GenerateInvoices()
        //{
        //    await _monthlyBillingJob.RunAsync();

        //    TempData["Success"] = "Monthly billing completed successfully.";

        //    return RedirectToAction(nameof(Index));
        //}
    }
}