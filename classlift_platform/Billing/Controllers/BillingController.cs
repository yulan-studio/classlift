using Billing.Jobs;
using Microsoft.AspNetCore.Mvc;

namespace Billing.Controllers
{
    //[Route("[controller]")]
    [Route("Billing")]
    public class BillingController : Controller
    {
        private readonly MonthlyBillingJob _monthlyBillingJob;

        public BillingController(MonthlyBillingJob monthlyBillingJob)
        {
            _monthlyBillingJob = monthlyBillingJob;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("GenerateInvoices")]
        public async Task<IActionResult> GenerateInvoices()
        {
            await _monthlyBillingJob.RunAsync();

            TempData["Success"] = "Monthly billing completed successfully.";

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