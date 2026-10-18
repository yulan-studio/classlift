namespace Billing.Models;

public partial class OrganizationFeatureSetting
{
    public int OrganizationFeatureSettingId { get; set; }
    public int OrganizationId { get; set; }
    public int FeatureId { get; set; }
    public bool IsEnabled { get; set; }
    public virtual Organization Organization { get; set; } = null!;
    public virtual Feature Feature { get; set; } = null!;
}
