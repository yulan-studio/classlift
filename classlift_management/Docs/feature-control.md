# ClassLift Feature 控制实现与维护指南

## 1. 目标与范围

Feature 控制用于根据机构当前购买的 Plan，决定该机构是否能够看到和使用某项功能。

当前实现同时控制：

- 前台导航和按钮是否显示；
- Controller 页面能否访问；
- AJAX/API 接口能否调用；
- 没有 Feature 时显示什么信息。

Feature 控制是机构级权限，不是用户级权限。用户角色和机构 Feature 必须分别检查：

```text
用户角色允许访问
        AND
机构当前 Plan 包含 Feature
        ↓
最终允许访问
```

例如 Reports 功能要求：

```csharp
[Authorize(Roles = "Admin")]
[RequiresFeature(FeatureCodes.StandardReporting)]
```

这表示用户必须是 Admin，并且用户所在机构的 Plan 必须包含 `standard_reporting`。

---

## 2. 数据库设计

Feature 配置存放在平台数据库 `classlift_platform`，不是每个租户自己的业务数据库。

### 2.1 主要数据表

#### `features`

定义系统中有哪些可控制的功能。

| 字段 | 说明 |
| --- | --- |
| `FeatureID` | 主键 |
| `FeatureKey` | 程序使用的稳定标识，例如 `standard_reporting` |
| `FeatureName` | 给管理员或用户显示的名称 |
| `Description` | 功能说明 |
| `CreatedAt` | 创建时间 |

`FeatureKey` 有唯一索引。Feature 上线后不要随意修改它，因为程序代码使用该值进行判断。

#### `subscriptionplans`

定义 Starter、Growth、Pro 等订阅计划。

| 字段 | 说明 |
| --- | --- |
| `PlanID` | 主键 |
| `PlanName` | Plan 名称 |
| `Description` | Plan 描述 |
| `PricePerCoach` | 每名 Coach 的价格 |
| `MinimumMonthlyPrice` | 最低月费 |
| `IsActive` | Plan 是否有效 |
| `CreatedAt` | 创建时间 |

#### `planfeatures`

连接 Plan 和 Feature，是多对多关系的中间表。

| 字段 | 说明 |
| --- | --- |
| `PlanFeatureID` | 主键 |
| `PlanID` | 所属 Plan |
| `FeatureID` | 所属 Feature |
| `IsLocked` | 配置是否锁定；不是 Feature 的 on/off 字段 |
| `CreatedAt` | 创建时间 |

当前系统的开关规则是：

```text
planfeatures 中存在 PlanID + FeatureID 记录 = Feature 开启
planfeatures 中不存在该记录                 = Feature 关闭
```

`IsLocked` 不参与当前 Feature 是否启用的判断。不要把 `IsLocked = false` 解释为 Feature 关闭。

`PlanID + FeatureID` 有唯一索引，防止同一个 Plan 重复添加同一个 Feature。

#### `organizations`

`CurrentPlanID` 指向机构当前生效的 Plan。

当前程序将下面这个字段作为 Feature 查询的 Plan 来源：

```text
organizations.CurrentPlanID
```

只有机构和当前 Plan 都是 Active，程序才会加载 Feature。

#### `organization_subscriptions`

保存 Trial、Active、Cancelled、Expired 等订阅和计费历史。

该表已经映射到 EF Core，但当前 Feature 判断不直接根据它的 `Status` 计算，而是使用 `organizations.CurrentPlanID`。升级或降级 Plan 时，应在同一个数据库事务中同步更新订阅记录和 `CurrentPlanID`，避免两者不一致。

### 2.2 数据关系

```text
organizations
    └── CurrentPlanID
          └── subscriptionplans
                └── planfeatures
                      └── features
```

订阅历史关系：

```text
organizations
    └── organization_subscriptions
          └── subscriptionplans
```

### 2.3 查询某个机构的有效 Feature

下面的 SQL 可用于排查某个机构为什么有或没有某项功能：

```sql
SELECT
    o.OrganizationID,
    o.OrganizationName,
    p.PlanID,
    p.PlanName,
    f.FeatureKey,
    f.FeatureName
FROM organizations AS o
JOIN subscriptionplans AS p
    ON p.PlanID = o.CurrentPlanID
JOIN planfeatures AS pf
    ON pf.PlanID = p.PlanID
JOIN features AS f
    ON f.FeatureID = pf.FeatureID
WHERE o.OrganizationID = @OrganizationID
  AND o.IsActive = 1
  AND p.IsActive = 1
ORDER BY f.FeatureKey;
```

### 2.4 给 Plan 开启或关闭 Feature

开启 Feature 是添加中间表记录：

```sql
INSERT INTO planfeatures (PlanID, FeatureID, IsLocked)
SELECT @PlanID, FeatureID, 0
FROM features
WHERE FeatureKey = 'standard_reporting';
```

关闭 Feature 是删除对应中间表记录：

```sql
DELETE pf
FROM planfeatures AS pf
JOIN features AS f ON f.FeatureID = pf.FeatureID
WHERE pf.PlanID = @PlanID
  AND f.FeatureKey = 'standard_reporting';
```

生产环境修改前应先查询确认目标 Plan 和 Feature，并通过正常的管理后台或受控数据库发布流程执行。

---

## 3. EF Core 实体和关系映射

平台数据库由 `Core/Contexts/BillingDbConext.cs` 中的 `BillingDbContext` 访问。

主要 `DbSet`：

```csharp
public DbSet<Organization> Organizations { get; set; }
public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
public DbSet<Feature> Features { get; set; }
public DbSet<PlanFeature> PlanFeatures { get; set; }
public DbSet<OrganizationSubscription> OrganizationSubscriptions { get; set; }
```

对应实体：

- `Core/Models/Organization.cs`
- `Core/Models/SubscriptionPlan.cs`
- `Core/Models/Feature.cs`
- `Core/Models/PlanFeature.cs`
- `Core/Models/OrganizationSubscription.cs`

Fluent API 明确映射了实际 MySQL 表名、列名、索引、decimal 精度、enum 和外键删除行为。

这些数据库表已经存在，因此当前工作不需要为它们创建租户业务库 Migration。以后修改平台表结构时，要区分 `BillingDbContext` 的平台库结构和 `AppDbContext` 的租户业务库结构。

---

## 4. Feature Key 常量

程序中的 Feature Key 集中定义在：

```text
Core/FeatureCodes.cs
```

示例：

```csharp
public const string StandardReporting = "standard_reporting";
```

不要在 Controller 和 Razor 页面中重复写字符串：

```csharp
// 不推荐
currentTenant.HasFeature("standard_reporting");

// 推荐
currentTenant.HasFeature(FeatureCodes.StandardReporting);
```

这样可以减少拼写错误，并方便搜索某项 Feature 被哪些地方使用。

常量值必须与 `features.FeatureKey` 完全对应。

---

## 5. 程序如何读取 Feature

### 5.1 `IFeatureService`

接口位于：

```text
Core/Interfaces/IFeatureService.cs
```

它提供两个方法：

```csharp
Task<TenantFeatures> GetFeaturesAsync(int organizationId, ...);
Task<bool> IsEnabledAsync(int organizationId, string featureKey, ...);
```

- `GetFeaturesAsync`：加载机构当前 Plan 和全部 Feature；适合请求开始时一次性加载。
- `IsEnabledAsync`：只查询一个 Feature；适合不经过正常租户请求流程的独立业务检查。

### 5.2 `FeatureService`

实现位于：

```text
Core/Services/FeatureService.cs
```

`GetFeaturesAsync` 的判断条件是：

1. Organization ID 存在；
2. Organization 的 `IsActive = true`；
3. `CurrentPlanID` 不为空；
4. 当前 Plan 的 `IsActive = true`；
5. 读取该 Plan 在 `planfeatures` 中的所有 `FeatureKey`。

任何关键条件不满足时，返回空 Feature 集合。这是 default deny，也就是默认拒绝策略。

查询使用 `AsNoTracking()`，因为这里只读取配置，不修改实体。

### 5.3 DI 注册

`Web/Program.cs` 中注册：

```csharp
builder.Services.AddScoped<IFeatureService, FeatureService>();
```

`FeatureService` 和 `BillingDbContext` 都是 request scoped，同一个 HTTP 请求内使用同一作用域。

---

## 6. 每个请求如何加载 Feature

`Core/Middleware/TenantResolutionMiddleware.cs` 首先根据域名或 subdomain 在 `tenantregistry` 中找到机构。

解析租户后的主要流程：

```text
请求域名
  ↓
TenantRegistry
  ↓
OrganizationID
  ↓
FeatureService.GetFeaturesAsync
  ↓
CurrentTenant.PlanId / PlanName / EnabledFeatures
```

代码逻辑：

```csharp
var tenantFeatures = await featureService.GetFeaturesAsync(
    tenant.OrganizationId,
    context.RequestAborted);

currentTenant.PlanId = tenantFeatures.PlanId;
currentTenant.PlanName = tenantFeatures.PlanName;
currentTenant.EnabledFeatures = tenantFeatures.EnabledFeatures;
```

这样一次请求只在租户解析阶段加载一次完整 Feature 集合。后面的 Controller、Filter 和 Razor View 都读取 `CurrentTenant`，不重复查询平台数据库。

### localhost 行为

当前 localhost 分支没有设置平台 `OrganizationId`，因此不会加载机构 Plan，Feature 集合默认为空。

结果是：

- 本地 Reports 菜单隐藏；
- 直接访问受保护的 Reports 页面会显示 Feature unavailable；
- AJAX/API 请求返回 403。

如果需要本地测试真实 Plan，应后续增加 `LocalTenant:OrganizationId` 配置，而不是在开发环境中无条件开启全部 Feature。这样本地行为更接近生产环境。

---

## 7. `CurrentTenant` 中的 Feature 状态

`Core/Models/CurrentTenant.cs` 保存当前请求的租户信息：

```csharp
public int? PlanId { get; set; }
public string? PlanName { get; set; }

public IReadOnlySet<string> EnabledFeatures { get; set; }
    = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
```

统一判断方法：

```csharp
public bool HasFeature(string featureKey) =>
    !string.IsNullOrWhiteSpace(featureKey) &&
    EnabledFeatures.Contains(featureKey);
```

Feature Key 使用不区分大小写的 `HashSet`，单次判断接近 O(1)。

应用代码应该调用 `HasFeature`，不要自己遍历集合，也不要在 Razor 页面直接查询 `BillingDbContext`。

---

## 8. 后端访问控制

只隐藏菜单是不安全的。用户仍然可以手动输入 URL 或直接请求接口，所以 Controller 后端必须再次检查 Feature。

### 8.1 `RequiresFeatureAttribute`

文件：

```text
Web/Filters/RequiresFeatureAttribute.cs
```

Attribute 负责声明某个 Controller 或 Action 需要哪个 Feature：

```csharp
[RequiresFeature(FeatureCodes.StandardReporting)]
```

它本身不执行判断，而是把 Feature Key 传给 `RequiresFeatureFilter`。

### 8.2 `RequiresFeatureFilter`

文件：

```text
Web/Filters/RequiresFeatureFilter.cs
```

Filter 在 Action 执行前调用：

```csharp
if (_currentTenant.HasFeature(_featureKey))
{
    await next();
    return;
}
```

- 有 Feature：调用 `next()`，继续执行 Controller Action；
- 没有 Feature：不调用 `next()`，Controller Action 不会运行。

Filter 不查询数据库，只读取 Middleware 已填充的 `CurrentTenant`。

### 8.3 页面请求与 AJAX/API 请求的区别

普通浏览器页面请求通常接受 `text/html`。没有 Feature 时，Filter 将其引导到说明页面：

```text
/Account/FeatureUnavailable
```

最终页面状态码设置为：

```text
403 Forbidden
```

AJAX/API 请求没有 Feature 时直接返回：

```text
HTTP 403
```

这样 JavaScript 不会把 HTML 错误页面误当作 JSON 数据解析。

### 8.4 在整个 Controller 上保护

Reports 当前采用 Controller 级保护：

```csharp
[Route("Report")]
[Authorize(Roles = "Admin")]
[RequiresFeature(FeatureCodes.StandardReporting)]
public class ReportController : Controller
```

因此下面所有 Action 都被保护，包括：

- `/Report/Index`
- `/Report/GetChildDetails`
- `/Report/GetCoachDetails`
- `/Report/GetCourseDetails`

### 8.5 只保护单个 Action

如果同一个 Controller 中只有部分功能需要限制，可以把 Attribute 放在 Action 上：

```csharp
[RequiresFeature(FeatureCodes.AdvancedReporting)]
public IActionResult AdvancedReport()
{
    return View();
}
```

---

## 9. 前台 Razor 控制

`Web/Views/_ViewImports.cshtml` 已经注入 `CurrentTenant`，所以 Razor 页面可以直接判断。

Reports 菜单位于 `Web/Views/Shared/_Layout.cshtml`：

```razor
@if (CurrentTenant.HasFeature(FeatureCodes.StandardReporting))
{
    <li class="nav-item">
        <a class="nav-link" href="/Report/Index">Reports</a>
    </li>
}
```

可以用同样方式控制：

- 导航菜单；
- 页面按钮；
- Tab；
- 表单字段；
- Dashboard 卡片。

前台隐藏只改善用户体验，不能代替后端 `[RequiresFeature]`。

---

## 10. 没有 Feature 时的信息显示

说明页面位于：

```text
Web/Views/Account/FeatureUnavailable.cshtml
```

入口 Action 位于 `AccountController.FeatureUnavailable`。

页面会显示：

- 当前功能不可用；
- 如果能取得 Plan 名称，则说明当前 Plan 不包含该功能；
- Feature Key，方便支持人员和开发人员排查；
- 返回 Dashboard 的按钮。

`featureKey` 和 `planName` 只是显示信息，真正的安全判断已经在 Filter 中完成。即使用户修改 query string，也不能借此获得 Feature 权限。

---

## 11. 新增一个 Feature 的标准流程

以后增加 Feature 时，按下面顺序操作。

### 第一步：定义数据库 Feature

为 `features` 添加唯一且稳定的 `FeatureKey`。

命名建议：

- 全部小写；
- 使用下划线；
- 描述业务能力，不使用页面 URL；
- 例如 `advanced_reporting`，不要使用 `report_page_v2`。

### 第二步：分配给 Plan

在 `planfeatures` 中给需要该功能的 Plan 添加记录。

### 第三步：添加代码常量

在 `FeatureCodes.cs` 中添加：

```csharp
public const string NewFeature = "new_feature";
```

### 第四步：保护后端

在 Controller 或 Action 添加：

```csharp
[RequiresFeature(FeatureCodes.NewFeature)]
```

保留原有的 `[Authorize]` 或角色检查。

### 第五步：控制前台显示

```razor
@if (CurrentTenant.HasFeature(FeatureCodes.NewFeature))
{
    // 菜单、按钮或模块
}
```

### 第六步：验证允许与拒绝两条路径

至少选择两个 Plan：

- 一个包含新 Feature；
- 一个不包含新 Feature。

分别验证菜单、页面 URL 和 AJAX/API。

---

## 12. 测试清单

### 有 Feature 的机构

- Admin 登录后能看到 Reports 菜单；
- `/Report/Index` 正常打开；
- 三个 Report 数据接口返回正常 JSON；
- `CurrentTenant.PlanName` 和 Feature 集合正确。

### 没有 Feature 的机构

- Reports 菜单不显示；
- 手动输入 `/Report/Index` 显示 Feature unavailable；
- 页面最终状态码为 403；
- 直接请求 Report 数据接口返回 403；
- Report Action 内的业务代码不执行。

### 角色验证

- 非 Admin 即使机构有 Feature，也不能访问 Reports；
- Admin 只有在机构有 Feature 时才能访问。

### 异常数据验证

- Organization inactive：Feature 默认全部关闭；
- `CurrentPlanID` 为空：Feature 默认全部关闭；
- Plan inactive：Feature 默认全部关闭；
- Feature Key 拼写错误或数据库不存在：默认关闭；
- `planfeatures` 没有对应记录：默认关闭。

---

## 13. 常见问题排查

### 菜单没有显示

按顺序检查：

1. 当前请求是否成功解析 `TenantRegistry`；
2. `CurrentTenant.OrganizationId` 是否正确；
3. `organizations.IsActive` 是否为 1；
4. `CurrentPlanID` 是否有值；
5. `subscriptionplans.IsActive` 是否为 1；
6. `planfeatures` 是否存在 Plan + Feature 记录；
7. 数据库 `FeatureKey` 是否与 `FeatureCodes` 一致；
8. 用户角色是否满足菜单外层的角色条件。

### 菜单显示，但 URL 返回 403

通常表示前台和后端使用了不同的 Feature Code，或者请求没有经过正确的 tenant host。检查 Razor 与 `[RequiresFeature]` 是否使用同一个常量。

### localhost 一直返回 Feature unavailable

这是当前预期行为。localhost 没有关联平台 Organization，因此 Feature 集合为空。

### 数据库刚修改，但请求行为没有变化

当前实现没有 Feature 缓存，新请求会重新读取数据库。确认请求确实已重新发出，并确认修改的是 `classlift_platform` 而不是租户业务数据库。

---

## 14. 后续可选扩展

当前设计刻意保持简单。出现真实需求后可以增加：

- `IMemoryCache` 或 Redis 缓存；
- Organization Feature Override；
- Feature 数量限制，例如最大 Coach 数；
- 管理后台 Plan Feature Matrix；
- Upgrade Plan 按钮和计费页面；
- 订阅状态与 Feature 权限的更严格联动；
- 审计日志。

如果以后增加 Organization Override，应只修改 `FeatureService` 的有效 Feature 合并逻辑：

```text
Organization Override
    ?? Plan 默认配置
    ?? 默认关闭
```

Controller、`RequiresFeatureFilter` 和 Razor 的调用方式不需要改变。这也是业务代码统一通过 `CurrentTenant.HasFeature` 判断的主要原因。

---

## 15. 核心文件索引

| 责任 | 文件 |
| --- | --- |
| Feature Key 常量 | `Core/FeatureCodes.cs` |
| Feature 查询接口 | `Core/Interfaces/IFeatureService.cs` |
| Feature 查询实现 | `Core/Services/FeatureService.cs` |
| Feature 查询结果 | `Core/Models/TenantFeatures.cs` |
| 当前租户 Feature 集合 | `Core/Models/CurrentTenant.cs` |
| 平台 EF Context | `Core/Contexts/BillingDbConext.cs` |
| 请求时加载 Feature | `Core/Middleware/TenantResolutionMiddleware.cs` |
| Feature 声明 Attribute | `Web/Filters/RequiresFeatureAttribute.cs` |
| Feature 执行 Filter | `Web/Filters/RequiresFeatureFilter.cs` |
| 无 Feature 提示 Action | `Web/Controllers/Account/AccountController.cs` |
| 无 Feature 提示 View | `Web/Views/Account/FeatureUnavailable.cshtml` |
| Reports 后端示例 | `Web/Controllers/Report/ReportController.cs` |
| Reports 前台示例 | `Web/Views/Shared/_Layout.cshtml` |
| DI 注册 | `Web/Program.cs` |

