namespace Billing.Models;

public sealed class Staff
{
    public int StaffId { get; set; }

    public int UserId { get; set; }

    public required string Name { get; set; }

    public string? Phone { get; set; }

    public string? Wechat { get; set; }
}
