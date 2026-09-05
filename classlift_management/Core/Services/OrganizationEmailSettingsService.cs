using Core.Contexts;
using Core.Interfaces;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Core.Services;

public sealed class OrganizationEmailSettingsService : IOrganizationEmailSettingsService
{
    private const int SettingsId = 1;
    private readonly AppDbContext _dbContext;

    public OrganizationEmailSettingsService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<OrganizationEmailSettings?> GetAsync(
        CancellationToken cancellationToken = default) =>
        _dbContext.OrganizationEmailSettings
            .AsNoTracking()
            .SingleOrDefaultAsync(
                settings => settings.OrganizationEmailSettingsId == SettingsId,
                cancellationToken);

    public async Task SaveAsync(
        string senderEmail,
        string receiverEmail,
        CancellationToken cancellationToken = default)
    {
        var settings = await _dbContext.OrganizationEmailSettings
            .SingleOrDefaultAsync(
                item => item.OrganizationEmailSettingsId == SettingsId,
                cancellationToken);

        var now = DateTime.UtcNow;
        if (settings is null)
        {
            settings = new OrganizationEmailSettings
            {
                OrganizationEmailSettingsId = SettingsId,
                SenderEmail = senderEmail,
                ReceiverEmail = receiverEmail,
                CreatedAtUtc = now,
                UpdatedAtUtc = now
            };
            _dbContext.OrganizationEmailSettings.Add(settings);
        }
        else
        {
            settings.SenderEmail = senderEmail;
            settings.ReceiverEmail = receiverEmail;
            settings.UpdatedAtUtc = now;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
