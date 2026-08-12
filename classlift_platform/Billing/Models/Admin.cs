namespace Billing.Models;

public sealed class Admin
{
    public int AdminId { get; set; }

    public int UserId { get; set; }
    
    public required string Name { get; set; }

    public string? Phone { get; set; }

    public string? Wechat { get; set; }
}
