using Billing.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Billing.Controllers
{
    [Route("[controller]")]
    public class SubscriptionEventsController : Controller
    {
        private readonly BillingDbContext _context;

        public SubscriptionEventsController(BillingDbContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var eventsList = await _context.SubscriptionEvents
                .Include(e => e.Organization)
                .Include(e => e.OrganizationSubscription)
                .Include(e => e.OldPlan)
                .Include(e => e.NewPlan)
                .OrderByDescending(e => e.EffectiveAt)
                .ToListAsync();

            return View(eventsList);
        }

        [HttpGet("Details/{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            var subscriptionEvent = await _context.SubscriptionEvents
                .Include(e => e.Organization)
                .Include(e => e.OrganizationSubscription)
                .Include(e => e.OldPlan)
                .Include(e => e.NewPlan)
                .FirstOrDefaultAsync(e => e.SubscriptionEventId == id);

            if (subscriptionEvent == null)
                return NotFound();

            return View(subscriptionEvent);
        }
    }
}