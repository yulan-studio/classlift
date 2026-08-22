namespace ClassLift.Diagnostic.Models;

public sealed record CreateDiagnosticRequest(
    string? BusinessType,
    string? StudentCount,
    int? AdminCount,
    IReadOnlyList<string>? CurrentTools,
    string? PrimaryPain,
    string? DesiredOutcome,
    string? Motivation,
    IReadOnlyList<string>? CostOfInaction,
    IReadOnlyList<string>? PreviousSolutions,
    string? RootCause,
    int? KeyPersonDependency,
    IReadOnlyList<string>? BuyingCriteria,
    string? ImplementationTimeline,
    string? SelfIdentifiedPriority,
    string? Name,
    string? Email,
    string? Organization)
{
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
        Required(nameof(DesiredOutcome), DesiredOutcome);
        Required(nameof(Motivation), Motivation);
        Required(nameof(ImplementationTimeline), ImplementationTimeline);
        Required(nameof(SelfIdentifiedPriority), SelfIdentifiedPriority);
        Required(nameof(Name), Name);
        Required(nameof(Email), Email);

        if (AdminCount is null or < 0 or > 10_000)
            errors[nameof(AdminCount)] = ["行政人员数量无效。"];
        if (CurrentTools is null || CurrentTools.Count == 0)
            errors[nameof(CurrentTools)] = ["请至少选择一项当前工具。"];
        if (CostOfInaction is null || CostOfInaction.Count is < 1 or > 3)
            errors[nameof(CostOfInaction)] = ["请选择 1–3 项。"];
        if (BuyingCriteria is null || BuyingCriteria.Count is < 1 or > 3)
            errors[nameof(BuyingCriteria)] = ["请选择 1–3 项。"];
        if (KeyPersonDependency is not (0 or 5 or 10 or 15 or 20))
            errors[nameof(KeyPersonDependency)] = ["关键人员依赖评分无效。"];
        if (!string.IsNullOrWhiteSpace(Email) && !System.Net.Mail.MailAddress.TryCreate(Email, out _))
            errors[nameof(Email)] = ["Email 格式无效。"];

        return errors;
    }
}
