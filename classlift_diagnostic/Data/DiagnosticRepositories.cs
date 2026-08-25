using System.Collections.Concurrent;
using ClassLift.Diagnostic.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassLift.Diagnostic.Data;

public interface IDiagnosticRepository
{
    Task AddAsync(DiagnosticLead lead, CancellationToken cancellationToken);
    Task<DiagnosticLead?> FindAsync(Guid id, CancellationToken cancellationToken);
    Task<AdminLeadPage> ListAsync(string? search, string? intent, int page, int pageSize, CancellationToken cancellationToken);
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

    public async Task<AdminLeadPage> ListAsync(string? search, string? intent, int page, int pageSize, CancellationToken cancellationToken)
    {
        var query = db.DiagnosticLeads.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(x => x.Name.Contains(term) || x.Email.Contains(term) ||
                (x.Organization != null && x.Organization.Contains(term)) ||
                (x.WebsiteUrl != null && x.WebsiteUrl.Contains(term)) || x.PrimaryPain.Contains(term));
        }
        if (!string.IsNullOrWhiteSpace(intent)) query = query.Where(x => x.LeadIntent == intent);
        var total = await query.CountAsync(cancellationToken);
        var items = await query.OrderByDescending(x => x.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize)
            .Select(x => new AdminLeadSummary(x.Id, x.CreatedAt, x.Name, x.Email, x.Organization, x.WebsiteUrl,
                x.BusinessType, x.StudentCount, x.PrimaryPain, x.ImplementationTimeline,
                x.TotalScore, x.Classification, x.LeadIntent))
            .ToListAsync(cancellationToken);
        return new(items, total, page, pageSize);
    }
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

    public Task<AdminLeadPage> ListAsync(string? search, string? intent, int page, int pageSize, CancellationToken cancellationToken)
    {
        var query = _leads.Values.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(x => new[] { x.Name, x.Email, x.Organization, x.WebsiteUrl, x.PrimaryPain }
                .Any(value => value?.Contains(search, StringComparison.OrdinalIgnoreCase) == true));
        if (!string.IsNullOrWhiteSpace(intent)) query = query.Where(x => x.LeadIntent == intent);
        var ordered = query.OrderByDescending(x => x.CreatedAt).ToArray();
        var items = ordered.Skip((page - 1) * pageSize).Take(pageSize)
            .Select(x => new AdminLeadSummary(x.Id, x.CreatedAt, x.Name, x.Email, x.Organization, x.WebsiteUrl,
                x.BusinessType, x.StudentCount, x.PrimaryPain, x.ImplementationTimeline,
                x.TotalScore, x.Classification, x.LeadIntent)).ToArray();
        return Task.FromResult(new AdminLeadPage(items, ordered.Length, page, pageSize));
    }
}
