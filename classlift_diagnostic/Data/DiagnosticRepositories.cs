using System.Collections.Concurrent;
using ClassLift.Diagnostic.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassLift.Diagnostic.Data;

public interface IDiagnosticRepository
{
    Task AddAsync(DiagnosticLead lead, CancellationToken cancellationToken);
    Task<DiagnosticLead?> FindAsync(Guid id, CancellationToken cancellationToken);
}

public sealed class MySqlDiagnosticRepository(DiagnosticDbContext db) : IDiagnosticRepository
{
    public async Task AddAsync(DiagnosticLead lead, CancellationToken cancellationToken)
    {
        db.DiagnosticLeads.Add(lead);
        await db.SaveChangesAsync(cancellationToken);
    }

    public Task<DiagnosticLead?> FindAsync(Guid id, CancellationToken cancellationToken) =>
        db.DiagnosticLeads.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
}

public sealed class InMemoryDiagnosticRepository : IDiagnosticRepository
{
    private readonly ConcurrentDictionary<Guid, DiagnosticLead> _leads = new();
    public Task AddAsync(DiagnosticLead lead, CancellationToken cancellationToken)
    {
        _leads[lead.Id] = lead;
        return Task.CompletedTask;
    }
    public Task<DiagnosticLead?> FindAsync(Guid id, CancellationToken cancellationToken) =>
        Task.FromResult(_leads.GetValueOrDefault(id));
}
