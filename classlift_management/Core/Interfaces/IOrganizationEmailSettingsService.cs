using Core.Models;

namespace Core.Interfaces;

public interface IOrganizationEmailSettingsService
{
    Task<OrganizationEmailSettings?> GetAsync(CancellationToken cancellationToken = default);

    Task SaveAsync(
        string replyToEmail,
        string receiverEmail,
        CancellationToken cancellationToken = default);
}
