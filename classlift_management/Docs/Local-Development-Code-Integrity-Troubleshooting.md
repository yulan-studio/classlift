# 本地开发时 Web.exe 被 Windows 阻止的排错指南

## 适用问题

Visual Studio 启动 ClassLift 时出现：

```text
Unable to start program
An Application Control policy has blocked this file.
```

或者 Code Integrity 日志显示：

```text
did not meet the Enterprise signing level requirements
```

被阻止的文件可能包括：

```text
Web\\bin\\Debug\\net8.0\\Web.exe
Web\\bin\\Debug\\net8.0\\Web.dll
Core\\bin\\Debug\\net8.0\\Core.dll
```

## 1. 先确认不是项目代码错误

在项目根目录执行：

```powershell
dotnet build Web/Web.csproj
```

如果显示 `Build succeeded`，但 Visual Studio 启动时仍然被阻止，说明编译本身成功，问题发生在 Windows 加载程序阶段。

## 2. 检查 Code Integrity 日志

以管理员身份打开 PowerShell，执行：

```powershell
Get-WinEvent `
  -LogName "Microsoft-Windows-CodeIntegrity/Operational" `
  -MaxEvents 30 |
  Format-List TimeCreated, Id, Message
```

重点查看 Event ID `3077` 和 `3033`。

如果看到类似内容：

```text
Code Integrity determined that a process attempted to load Web.dll
that did not meet the Enterprise signing level requirements.
```

说明 Windows 正在阻止未满足签名要求的本地构建文件，而不是 migration、数据库或应用业务代码错误。

## 3. 检查是否安装了第三方安全软件

```powershell
Get-CimInstance -Namespace root\\SecurityCenter2 `
  -ClassName AntiVirusProduct |
  Select-Object displayName, pathToSignedProductExe
```

如果只显示：

```text
Windows Defender
```

则没有发现第三方杀毒软件。Windows Defender 本身仍可能配合 Smart App Control 阻止未签名程序。

## 4. 检查 AppLocker 命令是否可用

```powershell
Get-AppLockerPolicy -Effective -Xml
```

如果提示 `Get-AppLockerPolicy is not recognized`，通常表示当前 Windows 版本没有提供该 PowerShell 管理模块。这不能排除 Code Integrity 或 Smart App Control。

## 5. 检查 Device Guard 查询接口

```powershell
Get-CimInstance -Namespace root\\Microsoft\\Windows\\CI `
  -ClassName Win32_DeviceGuard
```

如果提示 `Invalid namespace`，表示当前系统没有提供这个 WMI 查询接口。此结果本身不能证明没有代码完整性保护，应继续检查 Code Integrity 日志和 Smart App Control。

## 6. 检查本机 Code Integrity 注册表设置

```powershell
reg query "HKLM\\SYSTEM\\CurrentControlSet\\Control\\CI\\Policy"
```

如果看到：

```text
VerifiedAndReputablePolicyState    REG_DWORD    0x1
SAC_EnforcementReason              REG_DWORD    0x1
```

通常表示 Smart App Control 正在强制执行。

也可以查看策略文件：

```powershell
Get-ChildItem C:\Windows\System32\CodeIntegrity -Force
```

不要直接删除该目录中的文件，因为错误操作可能影响系统启动或其他程序运行。

## 7. 检查电脑是否由组织管理

打开：

```text
设置 → 账户 → 访问工作或学校
```

如果显示连接到公司、学校、MDM 或其他组织，不要自行移除安全策略，应联系管理员。

如果这是个人电脑，并且没有组织管理，才考虑调整本机 Smart App Control 设置。

## 8. 处理方案 A：保留 Smart App Control

可以保留系统防护，但需要使用满足签名和信誉要求的构建方式：

1. 使用受信任的 Windows Code Signing 证书签名 `Web.exe`、`Web.dll` 和相关程序集；
2. 通过 CI/CD 生成签名后的开发或测试构建；
3. 使用另一台不受该策略限制的开发环境；
4. 对 AppLocker/WDAC 环境，可以由管理员配置允许规则。

注意：Smart App Control 通常没有简单的“只允许这个本地 Debug 文件”按钮。个人电脑上的本地未签名 Debug 构建可能仍然被阻止。

## 9. 处理方案 B：关闭 Smart App Control

如果这是个人电脑，并且只运行自己编译或可信来源的程序，可以关闭 Smart App Control：

```text
Windows 安全中心
→ 应用和浏览器控制
→ 智能应用控制
→ 关闭
```

系统可能显示确认提示：

```text
Yes, I am sure
```

确认前应了解：某些 Windows 版本关闭后不能直接重新开启，可能需要重置或重新安装 Windows 才能恢复。

关闭后：

1. 重启电脑；
2. 重新打开 Visual Studio；
3. 重新编译项目：

```powershell
dotnet build Web/Web.csproj
```

4. 再启动 ClassLift。

建议继续保持：

- Windows Defender 实时保护；
- Windows 防火墙；
- Windows 更新；
- 仅运行可信的 `.exe` 和 `.dll` 文件。

## 10. 这次问题的最终判断

本次日志同时显示 `Core.dll`、`Web.dll` 和 `Web.exe` 被阻止，且错误包含：

```text
Policy ID: {0283ac0f-fff1-49ae-ada1-8a933130cad6}
```

这说明：

- .NET 项目已经成功编译；
- `dotnet.exe` 和 Visual Studio 调试器本身可以运行；
- 新生成的本地程序集未满足当前 Windows Enterprise signing level；
- 问题不是 Stripe、Entity Framework migration 或 ClosedXML 本身造成的；
- 个人开发电脑上最直接的解决方案通常是关闭 Smart App Control，或者建立可信的代码签名构建流程。

## 11. 之后运行 EF migration

Smart App Control 允许开发程序运行后，再执行：

```powershell
dotnet ef database update `
  --project Core `
  --startup-project Web `
  --context AppDbContext
```

如果仍然失败，先检查：

1. `Web.dll` 是否能被正常加载；
2. 数据库连接是否指向正确的本地数据库；
3. `__EFMigrationsHistory` 是否已经记录目标 migration。

## 12. 修改安全策略后 Visual Studio 出现重复键错误

如果没有重启电脑，Visual Studio 可能出现以下错误：

```text
An element with the same key but a different value already exists.
Key: Microsoft.WebTools.ProjectSystem.WebServer.SelfHostWebServer
```

这通常是 Visual Studio 在系统安全策略变化后，重复加载 Web Tools 项目组件导致的，与业务代码、Excel 导入和数据库 migration 无关。

处理步骤：

1. 关闭所有 Visual Studio 窗口。
2. 在 PowerShell 中停止残留的 Visual Studio 和 .NET 进程：

```powershell
Get-Process devenv,dotnet -ErrorAction SilentlyContinue |
    Stop-Process -Force
```

3. 重启电脑。
4. 重新打开解决方案并运行项目。

如果问题仍然存在，关闭 Visual Studio 后删除项目根目录下的 `.vs` 文件夹：

```text
C:\Users\yulan\source2\repos\classlift\classlift_management\.vs
```

`.vs` 是 Visual Studio 本地缓存，不包含源代码。

如果仍然失败，可以清理 Visual Studio 组件缓存。关闭 Visual Studio 后，打开：

```text
%LOCALAPPDATA%\Microsoft\VisualStudio
```

在对应 Visual Studio 版本目录中删除：

```text
ComponentModelCache
```

然后重新启动 Visual Studio。
