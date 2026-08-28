# ClassLift Management

ClassLift Management is a .NET 8 MVC application for organizations that manage children, parents, coaches, courses, activities, schedules, attendance, payments, fees, and staff. It uses ASP.NET Core Identity, Razor views, Entity Framework Core, and MySQL.

Production uses a database-per-tenant model. Local development uses a single MySQL database named `classlift`.

## Technology

- .NET 8 and ASP.NET Core MVC
- Razor views, Bootstrap, jQuery, and feature-specific JavaScript
- Entity Framework Core 8 with Pomelo MySQL
- ASP.NET Core Identity with integer user and role keys
- Cloudflare R2 through the AWS S3 SDK
- SMTP email
- NUnit test project
- Docker deployment

## Solution structure

| Path | Purpose |
| --- | --- |
| `Web/` | Web entry point, controllers, Razor views, configuration, and static assets |
| `Core/` | Domain models, EF contexts, repositories, services, DTOs, form models, middleware, and background processing |
| `Test/` | NUnit tests; currently contains only a placeholder test |
| `Data/` | Legacy/unused project that is not included in the solution |
| `DB/` | Historical database scripts and backups; do not treat these as a migration system |
| `Docs/` | Focused technical documentation |

The usual request path is:

```text
Razor view -> MVC controller -> service -> repository -> AppDbContext -> MySQL
```

## Prerequisites

- .NET 8 SDK
- MySQL 8-compatible server
- A local database named `classlift`
- Optional SMTP account for email testing
- Optional Cloudflare R2 account for upload testing

Check the installed SDK:

```powershell
dotnet --version
```

## Configuration

Never commit real database, SMTP, or R2 credentials. Environment variables should use double underscores for nested ASP.NET configuration keys.

### Required database configuration

The application reads the base MySQL connection string from either:

```text
ServerConnection
ConnectionStrings:ServerConnection
```

For a local PowerShell session:

```powershell
$env:ConnectionStrings__ServerConnection = "Server=localhost;Port=3306;User ID=YOUR_USER;Password=YOUR_PASSWORD;"
```

Do not include a database name in this base value. The application adds `classlift` locally, `classlift_platform` for the production registry, or the selected tenant database name.

### SMTP configuration

Configure these keys when testing email:

```text
SmtpSettings__Server
SmtpSettings__Port
SmtpSettings__Username
SmtpSettings__Password
SmtpSettings__SenderEmail
SmtpSettings__SenderName
SmtpSettings__EnableSsl
```

For Gmail on port 587, enable TLS and use a Google app password rather than the normal account password.

### Cloudflare R2 configuration

Uploads require:

```text
CloudflareR2__AccountId
CloudflareR2__AccessKey
CloudflareR2__SecretKey
CloudflareR2__BucketName
CloudflareR2__Region
CloudflareR2__PublicUrl
```

## Local development

Restore and build from the repository root:

```powershell
dotnet restore classlift_management.sln
dotnet build classlift_management.sln
```

Run the web application:

```powershell
dotnet run --project Web/Web.csproj
```

The launch profiles normally expose:

- `http://localhost:5026`
- `https://localhost:7225`

The current tenant middleware recognizes `localhost` and `127.0.0.1` and connects `AppDbContext` to the local `classlift` database. It does not query the platform database for plan features on those request paths, so local feature entitlements default to an empty set.

The health endpoint is:

```text
GET /health
```

It reports process health only; it does not currently verify MySQL, SMTP, or R2 connectivity.

## Tests

Run all tests:

```powershell
dotnet test classlift_management.sln
```

The NUnit test project references `Core`, but the current test suite is still only a placeholder. It does not reference `Web`, so middleware and controller integration tests will require additional test-project setup. High-value additions are tenant isolation, authentication/authorization, enrollment rules, payment calculations, and course/session status transitions.

## Current product capabilities

In addition to the main children, coaches, courses, activities, enrollment, and payment workflows, the application currently supports:

- Per-user time-zone preferences, with scheduled course and activity times stored in UTC and converted for display.
- Tenant-specific participant/provider terminology and branding assets stored through Cloudflare R2.
- A tenant-specific home-page URL, also stored in R2, with a configured application fallback.
- Province-based city selection for child and coach addresses.
- WhatsApp and postal-code contact fields.
- Per-session pricing for applicable courses.

When changing any of these features, check the corresponding EF migrations under `Core/Migrations` and preserve compatibility with existing tenant databases.

## Multi-tenancy

Production tenant resolution works as follows:

1. The base server connection is adapted to database `classlift_platform`.
2. `BillingDbContext` reads the central `tenantregistry` table.
3. The request hostname is matched against an active custom domain or managed subdomain.
4. `TenantConnectionStringFactory` replaces the database name in the base connection string.
5. The request-scoped `CurrentTenant` receives the selected values.
6. The request-scoped `AppDbContext` connects to that tenant database.

`BillingDbContext` is not one context per tenant. It is a short-lived EF context for the single platform registry database. `AppDbContext` is the tenant-scoped application context. Neither context should be registered as a singleton.

See the authoritative [tenant connection-string guide](Docs/tenant-connection-strings.md) for deeper background. Tenant behavior is security-sensitive, so still verify the guide against current code when making changes.

See the [Feature control implementation and maintenance guide](Docs/feature-control.md) for the platform schema, runtime entitlement flow, Razor visibility checks, backend enforcement, 403 behavior, testing, and instructions for adding new features.

## Background processing

`TenantStatusUpdater` updates expired activities, enrollments, sessions, and completed courses. It is currently registered only outside the Development environment and runs immediately at startup, then every ten minutes.

Production processing enumerates active tenants from `classlift_platform` and creates a separate dependency-injection scope for each tenant.

## Database migrations

EF migrations live under `Core/Migrations`. For commands run with `Web` as the startup project, `Web/DesignTimeAppDbContextFactory.cs` reads `ServerConnection` (or `ConnectionStrings:ServerConnection`), loads the environment-specific configuration file, and targets the local `classlift` database.

An older duplicate factory remains at `Core/Contexts/AppDbContextFactory.cs`; it still expects `DefaultConnection`. Until that duplicate is removed, specify the project and startup project explicitly and verify which factory EF selects before generating or applying a migration. For example:

```powershell
dotnet ef migrations list --project Core/Core.csproj --startup-project Web/Web.csproj
```

Do not run migration or cleanup commands against production without explicitly confirming the target server and database.

## Docker

Build from the repository root:

```powershell
docker build -t classlift-management .
```

The image publishes `Web/Web.csproj` and starts `Web.dll`. Runtime configuration must be supplied through environment variables.

## Known development risks

- Configuration files and historical database artifacts have contained credentials or sensitive data; inspect changes before committing.
- The named `Classlift` CORS policy is used in `Program.cs`, but no matching policy registration is currently visible.
- Authorization and anti-forgery validation are inconsistent across controllers.
- The project builds with many nullable-reference warnings on a clean rebuild.
- Several controllers are large, especially child and coach workflows.
- Route and cookie configuration contain duplication and obsolete commented code.
- Uploaded images and generated build artifacts have historically appeared in source control.
- Unknown tenant hosts currently continue through the pipeline unresolved and may fail later when `AppDbContext` is requested.

## Before submitting a change

```powershell
dotnet build classlift_management.sln --no-restore
dotnet test classlift_management.sln --no-build
git diff --check
git status --short
```

For tenancy, enrollment, payment, background-job, upload, or authorization changes, perform targeted manual or integration testing in addition to compilation.

## Developer guidance

Read [PROJECT.md](PROJECT.md) before making structural changes or using Codex on this repository. It records architectural invariants, safe change boundaries, and a practical verification checklist.
