namespace ClassLift.Diagnostic.Models;

public sealed class CreateDiagnosticRequest
{
    public string? BusinessType { get; init; }
    public string? StudentCount { get; init; }
    public int? AdminCount { get; init; }
    public IReadOnlyList<string>? CurrentTools { get; init; }
    public IReadOnlyList<string>? ImprovementAreas { get; init; }
    public string? PrimaryPain { get; init; }
    public string? ImplementationTimeline { get; init; }
    public string? AdditionalNeeds { get; init; }
    public string? Name { get; init; }
    public string? Email { get; init; }
    public string? Organization { get; init; }

    // 兼容已经打开的旧版问卷页面。
    public string? DesiredOutcome { get; init; }
    public string? Motivation { get; init; }
    public IReadOnlyList<string>? CostOfInaction { get; init; }
    public IReadOnlyList<string>? PreviousSolutions { get; init; }
    public string? RootCause { get; init; }
    public int? KeyPersonDependency { get; init; }
    public IReadOnlyList<string>? BuyingCriteria { get; init; }
    public string? SelfIdentifiedPriority { get; init; }

    public Dictionary<string, string[]> Validate()
    {
        var errors = new Dictionary<string, string[]>();
        void Required(string key, string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) errors[key] = ["此字段不能为空。"];
        }

        Required(nameof(BusinessType), BusinessType);
        Required(nameof(StudentCount), StudentCount);
        Required(nameof(PrimaryPain), PrimaryPain);
        Required(nameof(ImplementationTimeline), ImplementationTimeline);
        Required(nameof(Name), Name);
        Required(nameof(Email), Email);

        if (CurrentTools is null || CurrentTools.Count == 0)
            errors[nameof(CurrentTools)] = ["请至少选择一项当前管理方式。"];
        if (ImprovementAreas is null || ImprovementAreas.Count == 0)
            errors[nameof(ImprovementAreas)] = ["请至少选择一项需要改善的问题。"];
        else if (!string.IsNullOrWhiteSpace(PrimaryPain) && !ImprovementAreas.Contains(PrimaryPain))
            errors[nameof(PrimaryPain)] = ["最急需解决的问题必须来自已选择的改善项目。"];
        if (!string.IsNullOrWhiteSpace(Email) && !System.Net.Mail.MailAddress.TryCreate(Email, out _))
            errors[nameof(Email)] = ["Email 格式无效。"];

        return errors;
    }
}
