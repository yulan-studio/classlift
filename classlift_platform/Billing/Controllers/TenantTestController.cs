using Microsoft.AspNetCore.Mvc;

namespace Billing.Controllers
{
    [Route("[controller]")]
    public class TenantTestController : Controller
    {
        [HttpGet("")]
        public IActionResult Index()
        {
            var organizationId = HttpContext.Items["OrganizationId"];
            var databaseName = HttpContext.Items["TenantDatabaseName"];
            var connectionString = HttpContext.Items["TenantConnectionString"];

            return Content(
                $"OrganizationId: {organizationId}\n" +
                $"DatabaseName: {databaseName}\n" +
                $"ConnectionString: {connectionString}"
            );
        }
    }
}