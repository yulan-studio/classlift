using Core.Contexts;
using Core.Interfaces;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Core.Services;

public sealed class OrganizationPaymentSettingsService : IOrganizationPaymentSettingsService
{
    public const string DefaultDirectPaymentInstructionTemplate = "Please pay {amount} for {payment_scope} using the payment method selected by the organization.";
    private readonly AppDbContext _dbContext;

    public OrganizationPaymentSettingsService(AppDbContext dbContext) => _dbContext = dbContext;

    public Task<OrganizationPaymentSettings?> GetAsync(CancellationToken cancellationToken = default) =>
        _dbContext.OrganizationPaymentSettings.AsNoTracking()
            .SingleOrDefaultAsync(settings => settings.OrganizationPaymentSettingsId == 1, cancellationToken);

    public async Task SaveAsync(string template, CancellationToken cancellationToken = default)
    {
        var settings = await _dbContext.OrganizationPaymentSettings
            .SingleOrDefaultAsync(item => item.OrganizationPaymentSettingsId == 1, cancellationToken);
        var now = DateTime.UtcNow;
        if (settings == null)
        {
            settings = new OrganizationPaymentSettings
            {
                OrganizationPaymentSettingsId = 1,
                DirectPaymentInstructionTemplate = template,
                CreatedAtUtc = now,
                UpdatedAtUtc = now
            };
            _dbContext.OrganizationPaymentSettings.Add(settings);
        }
        else
        {
            settings.DirectPaymentInstructionTemplate = template;
            settings.UpdatedAtUtc = now;
        }
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
