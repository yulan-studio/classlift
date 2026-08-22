using ClassLift.Diagnostic.Models;

namespace ClassLift.Diagnostic.Services;

public sealed class ScoringService
{
    public ScoreResult Calculate(CreateDiagnosticRequest request)
    {
        var operational = 22;
        var systemization = 18;
        var financial = 13;
        var scalability = 17;

        if (Has(request.CurrentTools, "纸张", "员工记住")) { operational -= 6; systemization -= 7; }
        if (Has(request.CurrentTools, "Excel")) { operational -= 3; systemization -= 4; }
        if (Has(request.CurrentTools, "多个")) { operational -= 3; systemization -= 3; }
        if (Has(request.CurrentTools, "一个课程管理软件", "自己开发")) systemization += 2;

        if (Has(request.PrimaryPain, "排课", "重复行政")) operational -= 5;
        if (Has(request.PrimaryPain, "收费", "工资")) { operational -= 3; financial -= 6; }
        if (Has(request.PrimaryPain, "了解公司的运营")) financial -= 3;
        if (Has(request.PreviousSolutions, "增加行政", "依赖一个")) scalability -= 5;
        if (Has(request.DesiredOutcome, "服务更多", "第二家店")) scalability -= 2;
        if (Has(request.CostOfInaction, "增加行政", "无法服务", "利润")) scalability -= 5;
        if (request.KeyPersonDependency <= 5) { systemization -= 2; scalability -= 2; }

        operational = Math.Clamp(operational, 0, 25);
        systemization = Math.Clamp(systemization, 0, 20);
        financial = Math.Clamp(financial, 0, 15);
        scalability = Math.Clamp(scalability, 0, 20);
        var keyPerson = request.KeyPersonDependency ?? 10;
        var total = operational + systemization + financial + scalability + keyPerson;
        var (classification, description) = Classify(total);

        return new(operational, systemization, keyPerson, financial, scalability, total, classification, description);
    }

    private static bool Has(string? value, params string[] terms) =>
        value is not null && terms.Any(value.Contains);

    private static bool Has(IEnumerable<string>? values, params string[] terms) =>
        values is not null && values.Any(value => terms.Any(value.Contains));

    private static (string, string) Classify(int score) => score switch
    {
        >= 80 => ("Highly Scalable", "你的运营已经具备较好的系统化基础，下一步是寻找更高价值的自动化机会。"),
        >= 60 => ("Growth Ready", "你已经具备一定系统化能力，但部分流程仍在限制增长。"),
        >= 40 => ("People Dependent", "业务目前仍比较依赖人工和关键员工，增长时运营成本可能同步上升。"),
        _ => ("High Operational Dependency", "大量核心流程依赖人工、个人经验或分散系统，扩大前值得先建立标准化基础。")
    };
}
