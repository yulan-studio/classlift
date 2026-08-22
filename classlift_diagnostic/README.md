# ClassLift Business Scalability Diagnostic

一个基于 ASP.NET Core、MySQL 和原生前端的业务可规模化诊断 V1。

## 当前架构

- ASP.NET Core 8 Web API
- EF Core 8 + Pomelo MySQL
- Railway MySQL
- 原生 HTML/CSS/JavaScript 前端
- 服务端确定性评分
- 本地无数据库时自动使用内存 Repository

## 本地运行

需要安装 .NET 8 SDK 或更新版本。

```powershell
dotnet restore
dotnet run
```

打开终端中显示的本地 URL。健康检查地址：

```text
/api/health
```

没有配置 MySQL 时，健康检查会显示 `database: in-memory`。重启应用后，内存数据会消失。

## 使用本地 MySQL

在项目根目录创建不会提交到 Git 的 `appsettings.Development.json`：

```json
{
  "ConnectionStrings": {
    "MySql": "Server=localhost;Port=3306;Database=classlift_diagnostic;User=root;Password=YOUR_PASSWORD;SslMode=Preferred"
  }
}
```

启动时应用会自动执行尚未应用的 EF Core Migration。

## Railway 部署步骤

### 1. 把代码推送到 GitHub

Railway 将通过根目录的 `Dockerfile` 构建应用。

### 2. 在 Railway 项目中添加 MySQL

如果已有 MySQL，直接使用现有服务。确保 Web Service 能引用这些变量：

```text
MYSQLHOST
MYSQLPORT
MYSQLUSER
MYSQLPASSWORD
MYSQLDATABASE
```

在 Railway 的 Web Service → Variables 中，使用 Reference Variable 引用 MySQL 服务对应的变量，不要手工把密码写入代码。

### 3. 创建 Web Service

选择 `Deploy from GitHub repo`，指向本项目。Railway 会识别 `Dockerfile`。

应用会读取 Railway 的 `PORT`，绑定到 `0.0.0.0`，并在启动时应用数据库 Migration。

### 4. 生成公网域名

进入 Web Service → Settings → Networking → Generate Domain。

### 5. 验证

依次检查：

1. 打开 `/api/health`，确认 `status` 为 `healthy`、`database` 为 `mysql`。
2. 完成一份测试诊断。
3. 报告顶部应出现真实的 Report ID。
4. 在 MySQL 的 `diagnostic_leads` 表中确认产生了一条记录。

## API

### 创建诊断

```text
POST /api/diagnostics
Content-Type: application/json
```

服务端会验证答案、计算五维评分、保存 Lead，并返回：

```json
{
  "leadId": "...",
  "createdAt": "...",
  "scores": {
    "operationalEfficiency": 16,
    "systemization": 9,
    "keyPersonIndependence": 5,
    "financialControl": 13,
    "scalability": 3,
    "total": 46,
    "classification": "People Dependent"
  },
  "leadIntent": "HIGH"
}
```

### 读取诊断

```text
GET /api/diagnostics/{leadId}
```

## AI 报告

AI 没有配置或调用失败时，系统仍会生成确定性报告，不会阻断 Lead 保存。

在 Railway Web Service → Variables 中添加：

```text
OPENAI_API_KEY=你的 OpenAI API Key
OPENAI_MODEL=gpt-5-mini
```

不要把 API Key 写进 `appsettings.json` 或提交到 Git。AI 请求使用 Responses API 的 Structured Outputs，并设置 `store: false`。姓名、Email 和机构名称不会发送给模型。

可以通过 API 返回报告中的 `aiGenerated` 判断内容来源：

```json
{
  "report": {
    "aiGenerated": true
  }
}
```

## 安全提醒

- 不要提交 `appsettings.Development.json`、`.env`、数据库密码或 OpenAI API Key。
- Railway 密钥只放在 Service Variables。
- 当前读取接口适合开发验证；正式上线前应为报告和销售后台增加授权或不可猜测的访问令牌。
