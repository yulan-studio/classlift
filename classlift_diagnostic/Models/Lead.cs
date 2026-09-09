namespace ClassLift.Diagnostic.Models;

public sealed class Lead
{
    public Guid Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string? Organization { get; set; }
    public string? WebsiteUrl { get; set; }
    public string? Phone { get; set; }
    public string Status { get; set; } = "NEW";
}

public sealed class DemoRequest
{
    public Guid Id { get; set; }
    public Guid LeadId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public string? Phone { get; set; }
    public string? PreferredTime { get; set; }
    public string? TimeZone { get; set; }
    public string? CompanySize { get; set; }
    public string MainGoal { get; set; } = "";
    public string? CurrentSystem { get; set; }
    public string? Message { get; set; }
    public string Source { get; set; } = "request_demo";
    public string Status { get; set; } = "NEW";
}
