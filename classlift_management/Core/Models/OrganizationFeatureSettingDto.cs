namespace Core.Models;

public sealed class OrganizationFeatureSettingDto
{
    public string FeatureKey { get; set; } = string.Empty;
    public string FeatureName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsEnabled { get; set; }
}

public sealed class UpdateOrganizationFeatureSettingRequest
{
    public bool IsEnabled { get; set; }
}
