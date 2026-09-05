namespace Core.Models;

public sealed class OrganizationEmailSettings
{
    public int OrganizationEmailSettingsId { get; set; } = 1;
    public string SenderEmail { get; set; } = string.Empty;
    public string ReceiverEmail { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}
