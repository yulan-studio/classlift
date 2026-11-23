using Core.Models;

namespace Core.Interfaces;

public interface IOrganizationPaymentSettingsService
{
    Task<OrganizationPaymentSettings?> GetAsync(CancellationToken cancellationToken = default);
    Task SaveAsync(string directPaymentInstructionTemplate, CancellationToken cancellationToken = default);
}
