namespace Billing.Configuration;

public sealed class StartupAdminOptions
{
    public bool Enabled { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
