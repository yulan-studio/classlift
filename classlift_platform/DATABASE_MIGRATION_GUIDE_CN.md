# ClassLift 数据库迁移说明

本文说明 `classlift_platform` 如何迁移平台数据库和租户数据库，以及开发人员新增数据库结构时应遵循的流程。

## 一、数据库架构

ClassLift 使用一个平台数据库和多个租户数据库：

- `classlift_platform`：保存组织、租户注册信息、订阅、账单和平台用户等管理数据。
- 租户数据库：每个组织拥有一个独立的 MySQL 数据库，保存该组织的业务数据。
- `tenantregistry`：平台数据库中的租户注册表，是当前批量租户迁移的唯一来源。

`tenantregistry.DatabaseName` 必须与实际 MySQL 数据库名称一致。迁移程序不会扫描 MySQL 中所有数据库，也不会自动推测错误的数据库名称。

## 二、迁移代码位置

主要代码位于：

- `Billing/Program.cs`
- `Billing/Services/Provisioning/TenantSchemaService.cs`
- `Billing/Interfaces/ITenantSchemaService.cs`
- `Billing/TenantScripts/`

`TenantSchemaService` 负责执行租户 SQL 脚本；平台数据库使用 EF Core migration。

## 三、执行已有数据库迁移

发布程序后执行：

```powershell
dotnet publish Billing/Billing.csproj -c Release -o .\publish
dotnet .\publish\Billing.dll --migrate
```

`--migrate` 的执行顺序如下：

1. 连接平台数据库 `classlift_platform`。
2. 执行平台数据库的 `BillingDbContext.Database.MigrateAsync()`。
3. 查询 `tenantregistry` 中所有数据库记录。
4. 不过滤 `IsActive`，所以 Active 和 InActive 租户都会处理。
5. 逐个连接 `DatabaseName` 指定的租户数据库。
6. 创建 `__TenantSchemaMigrations` 表（如果不存在）。
7. 按文件名升序读取 `Billing/TenantScripts/*.sql`。
8. 跳过已经记录的脚本，执行尚未记录的脚本。
9. 每个脚本成功执行后，将文件名写入 `__TenantSchemaMigrations`。

如果某个租户失败，程序会继续处理其他租户；全部处理结束后会抛出汇总异常，使部署失败，避免迁移失败被静默忽略。

## 四、如何新增租户迁移脚本

新增数据库结构时，不要修改已经执行过的旧 SQL 文件。应新增一个 SQL 文件，例如：

```text
Billing/TenantScripts/z20261012_Add_ProgressReportDrafts.sql
```

脚本示例：

```sql
CREATE TABLE `ProgressReportDrafts` (
    `ProgressReportDraftId` int NOT NULL AUTO_INCREMENT,
    `SessionEnrollmentId` int NOT NULL,
    `GeneratedDraft` text NULL,
    `CreatedAtUtc` datetime(6) NOT NULL,
    PRIMARY KEY (`ProgressReportDraftId`)
) CHARACTER SET = utf8mb4;
```

文件命名建议：

```text
zYYYYMMDD_简短说明.sql
```

例如：

```text
z20261012_Add_ProgressReportDrafts.sql
z20261020_Add_StudentIndexes.sql
```

当前脚本按文件名字典序执行。项目中已有旧基础脚本名称不是数字开头，因此新迁移使用 `z` 前缀，确保排在旧基础脚本之后。

## 五、迁移记录机制

每个租户数据库都有独立的迁移记录表：

```sql
__TenantSchemaMigrations
```

表中按文件名记录已成功执行的脚本：

```sql
SELECT MigrationID, ScriptName, AppliedAt
FROM __TenantSchemaMigrations
ORDER BY MigrationID;
```

迁移判断依据是 `ScriptName`，也就是文件名。因此：

- 已执行的脚本不会再次执行。
- 新增脚本会被执行。
- 修改旧脚本不会重新执行。
- 重命名已执行脚本会被当作新脚本，可能导致重复建表或重复建索引。

已执行脚本不要重命名或修改。如需变更，应新增一个更晚的迁移脚本。

## 六、新租户创建时的行为

新租户创建流程由 `TenantProvisioningService` 调用 `InitializeSchemaAsync()`：

1. 创建租户数据库。
2. 创建 `__TenantSchemaMigrations` 表。
3. 读取当前 `Billing/TenantScripts` 下的全部 SQL 文件。
4. 按文件名顺序执行全部脚本。
5. 记录每个成功执行的脚本。

因此，新租户会获得当前版本的完整数据库结构；已有租户则通过部署时的 `--migrate` 补齐新增脚本。

## 七、`tenantregistry` 数据要求

批量迁移只处理 `tenantregistry` 中存在且 `DatabaseName` 非空的记录。因此，执行迁移前必须确认：

- 每个仍在使用的租户数据库都有一条 registry 记录。
- `DatabaseName` 与实际数据库名完全一致。
- 不要把仍然存在、但尚未迁移的租户记录删除。
- 只有确认租户数据库已永久删除且不再管理时，才删除 registry 记录。

检查记录：

```sql
SELECT
    TenantRegistryID,
    OrganizationID,
    DatabaseName,
    IsActive
FROM tenantregistry
ORDER BY TenantRegistryID;
```

如果名称错误，应修正记录，而不是删除后忽略该数据库：

```sql
UPDATE tenantregistry
SET DatabaseName = 'classlift_correct_name'
WHERE TenantRegistryID = 123;
```

`IsActive` 不影响迁移。Inactive 租户仍然保留数据，因此也必须保持数据库结构最新。

## 八、部署配置

部署平台应在正式启动 Web 服务前执行：

```text
dotnet Billing.dll --migrate
```

在 Railway 等平台中，应将该命令配置为 Pre-deploy Command。正式服务仍然使用：

```text
dotnet Billing.dll
```

迁移命令和正式服务必须使用同一组数据库配置：

```text
TenantDatabase__Host
TenantDatabase__Port
TenantDatabase__User
TenantDatabase__Password
TenantDatabase__ServerVersion
```

如果没有执行 Pre-deploy Command，已有租户不会自动升级；新租户创建时仍会执行全部当前 SQL 脚本。

## 九、排查迁移未生效

### 1. 确认发布目录包含脚本

```powershell
Get-ChildItem .\publish\TenantScripts
```

应能看到新增的 `.sql` 文件。

### 2. 确认平台数据库中的 registry 记录

```sql
SELECT TenantRegistryID, DatabaseName, IsActive
FROM classlift_platform.tenantregistry;
```

### 3. 连接目标租户数据库检查迁移记录

```sql
SELECT *
FROM __TenantSchemaMigrations
WHERE ScriptName = 'z20261012_Add_ProgressReportDrafts.sql';
```

### 4. 检查目标表

```sql
SHOW TABLES LIKE 'ProgressReportDrafts';
```

如果脚本没有记录，通常表示：

- `tenantregistry.DatabaseName` 指向了错误数据库。
- 使用的环境变量连接到了另一套 MySQL 环境。
- 该数据库没有出现在 `tenantregistry` 中。
- 发布目录没有包含最新脚本。
- SQL 执行失败，应查看迁移命令的完整日志。

## 十、SQL 脚本编写限制

当前执行器使用分号拆分 SQL：

```csharp
sql.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
```

因此迁移脚本应使用简单的 DDL 语句。包含存储过程、触发器、复杂函数或字符串中大量分号的脚本，可能被错误拆分，需要先扩展脚本执行器后再使用。

## 十一、推荐发布流程

1. 新增一个带日期和 `z` 前缀的 SQL 文件。
2. 确认文件已复制到发布目录。
3. 备份平台数据库和相关租户数据库。
4. 确认 `tenantregistry` 中的数据库名称正确。
5. 执行 `dotnet Billing.dll --migrate`。
6. 检查迁移日志和 `__TenantSchemaMigrations`。
7. 抽查 Active 和 InActive 租户的新增表。
8. 迁移成功后启动正式 Web 服务。

