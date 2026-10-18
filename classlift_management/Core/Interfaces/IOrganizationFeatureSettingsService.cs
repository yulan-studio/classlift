using Core.Models;

namespace Core.Interfaces;

public interface IOrganizationFeatureSettingsService
{
    Task<IReadOnlyList<OrganizationFeatureSettingDto>> GetAsync(CancellationToken cancellationToken = default);
    Task UpdateAsync(string featureKey, bool isEnabled, CancellationToken cancellationToken = default);
}
