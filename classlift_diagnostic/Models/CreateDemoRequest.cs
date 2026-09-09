namespace ClassLift.Diagnostic.Models;

public sealed class CreateDemoRequest
{
    public string? Name { get; init; }
    public string? Email { get; init; }
    public string? Organization { get; init; }
    public string? WebsiteUrl { get; init; }
    public string? Phone { get; init; }
    public string? PreferredTime { get; init; }
    public string? CompanySize { get; init; }
    public string? MainGoal { get; init; }
    public string? CurrentSystem { get; init; }
    public string? Message { get; init; }

    public Dictionary<string, string[]> Validate()
    {
        var errors = new Dictionary<string, string[]>();
        if (string.IsNullOrWhiteSpace(Name)) errors[nameof(Name)] = ["请输入姓名。"];
        if (string.IsNullOrWhiteSpace(Email) || !System.Net.Mail.MailAddress.TryCreate(Email, out _)) errors[nameof(Email)] = ["请输入有效的工作 Email。"];
        if (string.IsNullOrWhiteSpace(Organization)) errors[nameof(Organization)] = ["请输入机构名称。"];
        if (string.IsNullOrWhiteSpace(MainGoal)) errors[nameof(MainGoal)] = ["请选择主要目标。"];
        return errors;
    }
}
