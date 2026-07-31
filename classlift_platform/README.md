# ClassLift Platform

ClassLift Platform is the central administration and billing application for the
ClassLift multi-tenant SaaS product. It manages organizations, plans,
subscriptions, feature access, invoices, payments, and billing runs. It also
provisions a separate MySQL database and initial administrator account for each
new tenant.

> **Project status:** active prototype. The primary workflows are implemented
> and the application builds, but the security, database consistency,
> integrations, and automated testing work listed in [PROJECT.md](PROJECT.md)
> should be completed before a production launch.

## Technology

- .NET 8 and ASP.NET Core MVC
- Entity Framework Core 8 with Pomelo/MySQL
- ASP.NET Core Identity
- Hangfire with MySQL storage
- Bootstrap 5 and Razor views
- MailKit for SMTP email
- Docker for container deployment

## Repository layout

```text
.
|-- Billing/
|   |-- Controllers/       MVC and public API endpoints
|   |-- Data/              EF Core contexts and mappings
|   |-- Models/            Billing and tenant domain models
|   |-- Services/
|   |   |-- Billing/       Invoicing, subscriptions, payments, dunning
|   |   |-- Jobs/          Hangfire recurring jobs
|   |   |-- Notifications/ Email and notification services
|   |   `-- Provisioning/  Tenant database and administrator setup
|   |-- TenantScripts/     SQL used to initialize tenant databases
|   |-- Views/             Administrative Razor UI
|   `-- Program.cs         Application composition and middleware
|-- Dockerfile
|-- PROJECT.md             Architecture, workflows, risks, and roadmap
`-- classlift_platform.slnx
```

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- MySQL 8 with an existing `classlift_platform` management database
- A MySQL account allowed to use the management database and create tenant
  databases
- Optional SMTP credentials for email delivery

The application does not automatically create or migrate the management
database at startup. Ensure its billing schema exists, then apply the EF Core
Identity migration as appropriate for the target environment.

## Configuration

Use .NET configuration providers such as environment variables, user secrets,
or a deployment secret store. Do not commit real credentials to an
`appsettings*.json` file.

Required management and tenant database settings:

| Setting | Environment variable | Description |
|---|---|---|
| `TenantDatabase:Host` | `TenantDatabase__Host` | MySQL server host |
| `TenantDatabase:Port` | `TenantDatabase__Port` | MySQL server port, normally `3306` |
| `TenantDatabase:User` | `TenantDatabase__User` | MySQL user |
| `TenantDatabase:Password` | `TenantDatabase__Password` | MySQL password |
| `PORT` | `PORT` | HTTP port; defaults to `8080` |

Optional email settings use the `SmtpSettings` section:

```json
{
  "SmtpSettings": {
    "Host": "smtp.example.com",
    "Port": 465,
    "Username": "smtp-user",
    "Password": "use-a-secret-provider",
    "FromEmail": "billing@example.com",
    "FromName": "ClassLift",
    "EnableSsl": true
  }
}
```

For local development, user secrets are convenient:

```powershell
dotnet user-secrets init --project Billing/Billing.csproj
dotnet user-secrets set "TenantDatabase:Host" "localhost" --project Billing/Billing.csproj
dotnet user-secrets set "TenantDatabase:Port" "3306" --project Billing/Billing.csproj
dotnet user-secrets set "TenantDatabase:User" "classlift" --project Billing/Billing.csproj
dotnet user-secrets set "TenantDatabase:Password" "your-local-password" --project Billing/Billing.csproj
```

## Build and run

From the repository root:

```powershell
dotnet restore classlift_platform.slnx
dotnet build classlift_platform.slnx
dotnet run --project Billing/Billing.csproj
```

Unless `PORT` is set, the application listens on `http://localhost:8080`.
Authentication is required for the administrative UI. The public signup API is
the exception.

### Database migration

The checked-in EF migration creates the management application's ASP.NET Core
Identity tables:

```powershell
dotnet ef database update --project Billing/Billing.csproj
```

Review migrations against a backup before applying them outside development.
Tenant databases are initialized during signup from the ordered `.sql` files in
`Billing/TenantScripts`.

### Docker

Build from the repository root:

```powershell
docker build -t classlift-platform .
docker run --rm -p 8080:8080 `
  -e TenantDatabase__Host=host.docker.internal `
  -e TenantDatabase__Port=3306 `
  -e TenantDatabase__User=classlift `
  -e TenantDatabase__Password=your-password `
  classlift-platform
```

## Main application areas

- `/Dashboard` — billing overview
- `/Plans` — plan pricing and feature mappings
- `/Organizations` — organizations, subscriptions, and tenant details
- `/Invoices` and `/Payments` — receivables administration
- `/SubscriptionEvents` — subscription audit trail
- `/Billing` and `/BillingRuns` — manual job controls and execution history
- `POST /api/public/signup` — public tenant signup and provisioning

## Scheduled jobs

Hangfire registers these schedules in the Eastern time zone:

| Job | Schedule | Purpose |
|---|---:|---|
| Daily dunning | Daily at 02:00 | Mark overdue invoices |
| Daily billing | Daily at 02:30 | Activate expired trials and run dunning |
| Monthly billing | First day at 02:00 | Generate recurring invoices |

## Verification

The current baseline command is:

```powershell
dotnet build classlift_platform.slnx --no-restore
```

There is currently no automated test project. Adding unit and integration tests
for billing calculations, subscription transitions, signup rollback, and access
control is a high-priority roadmap item.

## Further documentation

See [PROJECT.md](PROJECT.md) for the system context, architecture, domain model,
known limitations, and recommended implementation roadmap.
