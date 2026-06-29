using Billing.Constants;
using Billing.Data;
using Billing.Models;

namespace Billing.Services.Billing;

public class BillingrunService
{
    private readonly BillingDbContext _context;

    public BillingrunService(BillingDbContext context)
    {
        _context = context;
    }

    public async Task<BillingRun> StartRunAsync(string runType)
    {
        var run = new BillingRun
        {
            RunType = runType,
            StartedAt = DateTime.UtcNow,
            Status = BillingrunStatus.Running
        };

        _context.BillingRuns.Add(run);

        await _context.SaveChangesAsync();

        return run;
    }

    public async Task CompleteRunAsync(BillingRun run, int trialActivated = 0, int invoicesGenerated = 0, int invoicesMarkedOverdue = 0)
    {
        run.Status = BillingrunStatus.Success;
        run.FinishedAt = DateTime.UtcNow;
        run.TrialActivated = trialActivated;
        run.InvoicesGenerated = invoicesGenerated;
        run.InvoicesMarkedOverdue = invoicesMarkedOverdue;

        run.DurationMilliseconds = (int)(run.FinishedAt - run.StartedAt).TotalMilliseconds;

        await _context.SaveChangesAsync();
    }

    public async Task FailRunAsync(
        BillingRun run,
        Exception ex)
    {
        run.Status = BillingrunStatus.Failed;
        run.FinishedAt = DateTime.UtcNow;
        run.ErrorMessage = ex.ToString();
        
        run.DurationMilliseconds = (int)(run.FinishedAt - run.StartedAt).TotalMilliseconds;

        await _context.SaveChangesAsync();
    }
}