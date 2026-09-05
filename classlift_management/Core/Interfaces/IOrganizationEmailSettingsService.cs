using Core.Models;

namespace Core.Interfaces;

public interface IOrganizationEmailSettingsService
{
    Task<OrganizationEmailSettings?> GetAsync(CancellationToken cancellationToken = default);

    Task SaveAsync(
        string senderEmail,
        string receiverEmail,
        CancellationToken cancellationToken = default);
}
