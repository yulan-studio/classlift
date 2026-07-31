# ClassLift Development Guide

This file is the working guide for developers and Codex sessions. Read it before modifying the application. `README.md` covers setup; this document explains where changes belong and which invariants must be preserved.

## Product scope

ClassLift supports four principal operational roles:

- Admin: organization administration and staff management
- Staff: children, coaches, courses, activities, enrollments, schedules, fees, and payments
- Coach: assigned courses, schedules, attendance, hours, and income
- Child: personal registrations, confirmations, schedules, history, and payments

Parent and emergency-contact records support child workflows. Identity users are linked to role-specific domain records.

## Architectural map

### Web layer

- `Web/Program.cs`: dependency injection, authentication, middleware, routing, configuration, and hosting
- `Web/Controllers/`: MVC endpoints organized mostly by feature
- `Web/Views/`: server-rendered Razor UI
- `Web/wwwroot/js/`: client behavior for cities, coaches, courses, payments, registrations, schedules, sessions, and specialties
- `Web/wwwroot/css/`: Bootstrap plus application styles

Controllers currently return both HTML views and JSON responses. Match the existing action style within a feature unless intentionally refactoring the whole workflow.

### Core layer

- `Models/`: EF entities and tenant state
- `Contexts/AppDbContext.cs`: Identity plus tenant business data
- `Contexts/BillingDbConext.cs`: central platform tenant registry
- `Repositories/`: EF data access
- `Services/`: application and domain operations
- `Interfaces/`: repository and service contracts
- `ViewModels/`: data shaped for Razor views
- `FormModels/`: incoming form/update models
- `DTOs/`: reporting and cross-layer projections
- `Middleware/TenantResolutionMiddleware.cs`: hostname-to-tenant resolution
- `BackendService/TenantStatusUpdater.cs`: scheduled cross-tenant updates
- `R2/`: Cloudflare R2 upload integration

The repository/service split is established but not perfectly uniform. Before adding a new abstraction, inspect the nearest existing feature and prefer consistency.

## Dependency-injection lifetimes

Preserve these lifetime rules:

| Component | Lifetime | Reason |
| --- | --- | --- |
| `CurrentTenant` | Scoped | Contains state for one HTTP request or worker scope |
| `AppDbContext` | Scoped | EF context for the resolved tenant; not thread-safe |
| `BillingDbContext` | Scoped | EF context for the central platform database; not thread-safe |
| Repositories/services using EF | Scoped | Must share the correct scoped tenant context |
| `TenantConnectionStringFactory` | Singleton | Holds only immutable base configuration and builds strings locally |
| `R2StorageService` | Singleton | Current registration; review disposal/client ownership before changing |
| `EmailService` | Transient | Stateless wrapper around SMTP configuration |

Never make an EF `DbContext` singleton. Never store a request's `CurrentTenant` inside a singleton.

## Tenant-resolution invariants

Tenant isolation is the most security-sensitive behavior in the project.

For HTTP requests:

1. `TenantResolutionMiddleware` must execute before authentication, authorization, or anything that resolves `AppDbContext`.
2. Localhost selects database `classlift` directly.
3. Localhost must return before resolving `BillingDbContext`.
4. Non-local tenant hosts query only the central `classlift_platform.tenantregistry` table.
5. Only active registry entries may resolve.
6. Database names must come from trusted configuration or the registry, never request parameters.
7. Populate `CurrentTenant` before resolving any service or repository that injects `AppDbContext`.
8. `AppDbContext` must throw rather than silently select a default database when the tenant is unresolved.

Current local-host matching covers only `localhost` and `127.0.0.1`. Do not assume that `::1` or `*.localhost` works unless the middleware is changed and tested.

The local fallback is currently based on the Host value, not explicitly gated by `IHostEnvironment.IsDevelopment()`. Treat changes here as a security review item.

For background processing:

- There is no HTTP hostname.
- Production loads active registry rows from `BillingDbContext`.
- Each tenant must get a new asynchronous DI scope.
- Set that scope's `CurrentTenant` before resolving `AppDbContext` or dependent services.
- Operations must remain idempotent because the worker repeats every ten minutes and multiple application replicas may eventually run it concurrently.

## Main domain areas

| Area | Primary controller | Core components |
| --- | --- | --- |
| Accounts/login | `AccountController` | Identity, `UserRegistrationService` |
| Admin/staff | `AdminController`, `StaffController` | Admin/staff services and repositories |
| Children/parents | `ChildController`, `ParentController` | Child, parent, parent-child, balance, calendar, emergency contact |
| Coaches | `CoachController` | Coach, specialty, schedule, income |
| Courses | `CourseController` | Course and course-enrollment services |
| Activities | `ActivityController` | Activity and activity-enrollment services |
| Payments/fees | `PaymentPackageController`, `FeeController` | Payment, package, fee, and balance services |
| Reports | `ReportController` | Report service/repository and report DTOs |
| Uploads | `UploadController` | `R2StorageService` |
| Notifications | Notification controllers and workflow calls | `EmailService` |

Some controllers, especially `ChildController` and `CoachController`, contain multiple concerns. For new substantial features, prefer a focused service and small controller action instead of expanding an already large method.

## Data and transaction guidance

- Use `AsNoTracking()` for read-only queries.
- Project to DTOs/view models when full entities and navigation graphs are unnecessary.
- Preserve async database calls and pass cancellation tokens where the surrounding API supports them.
- Use a transaction for operations that update enrollment, balance, payment, or related records together.
- Do not trust posted IDs alone; verify tenant scope, role, and record ownership.
- Keep financial calculations in services, not Razor views or JavaScript.
- Make repeated background updates safe to run more than once.
- Avoid string-built SQL and string-built connection strings.

## Authentication and authorization

The application uses ASP.NET Core Identity with roles named `Admin`, `Staff`, `Coach`, and `Child`.

When changing an endpoint:

- Inspect its `[Authorize]` role requirements.
- Verify resource ownership in addition to role membership where appropriate.
- Use `[ValidateAntiForgeryToken]` on state-changing Razor form posts.
- Do not allow a registration request to grant itself a privileged role.
- Keep logout and other state-changing actions as POST operations.
- Avoid returning personal, payment, or tenant data from an unprotected JSON endpoint.

Do not assume authorization is inherited consistently; many attributes are action-level and some are commented out.

## Email

`Core/Services/EmailService.cs` uses `System.Net.Mail.SmtpClient` and binds `SmtpSettings` in `Program.cs` with startup validation.

When changing email behavior:

- Never log credentials or the complete SMTP configuration.
- Validate or encode user-derived HTML content.
- Avoid making a successful database operation appear failed solely because notification delivery failed; decide explicitly whether email is transactional or best-effort.
- For Gmail, use port 587, TLS, and an app password.
- A live send is an external side effect: use an explicitly approved recipient.

## Uploads and R2

`R2StorageService` uploads using the S3-compatible API and returns a public URL. Before expanding uploads, add validation for maximum size, allowed content types, actual file signatures, safe object names, and access policy. Do not place sensitive child or payment material at permanently public URLs.

## Configuration rules

Runtime configuration keys include:

- `ServerConnection` or `ConnectionStrings:ServerConnection`
- `SmtpSettings:*`
- `CloudflareR2:*`
- `PORT` for container hosting
- `ASPNETCORE_ENVIRONMENT`

Use environment variables or a secret manager for secrets. In environment-variable form, use `__` for `:`, for example `SmtpSettings__Password`.

Do not print configuration values while debugging. Showing whether a value is present is normally sufficient.

## Database schema and migrations

Business and Identity mappings are combined in `AppDbContext`. Identity tables use custom lowercase names such as `users`, `roles`, and `userroles`. Business entities also map to existing singular/plural table names. Preserve these mappings unless a deliberate migration accompanies the change.

Migrations are under `Core/Migrations`, but `AppDbContextFactory` currently uses the obsolete `DefaultConnection` key and does not load development configuration. Repair and test the design-time factory before generating or applying migrations.

Treat files in `DB/` as historical operational artifacts, not authoritative schema migrations.

## Testing strategy

The existing NUnit project has only a placeholder test and does not yet reference the application projects. For new tests, prioritize:

1. Tenant hostname resolution and cross-tenant isolation
2. Authentication, role authorization, and ownership
3. Enrollment capacity and duplicate prevention
4. Payment, fee, balance, and coach-income calculations
5. Session/activity/course completion transitions
6. Background-worker idempotency
7. SMTP configuration validation without sending real mail

Use unit tests for pure calculations and integration tests for EF, Identity, middleware, and controller behavior.

## Known issues and cleanup boundaries

Do not silently fix unrelated issues during a focused change, but consider these when scoping future work:

- `UseCors("Classlift")` has no visible matching `AddCors` registration.
- Unknown tenant hosts proceed unresolved instead of returning a clear response.
- Local fallback is not explicitly environment-gated.
- Design-time migration configuration is stale.
- Test coverage is effectively zero.
- Nullable warnings are numerous on clean builds.
- Route and cookie registration are duplicated.
- Authorization and anti-forgery coverage need auditing.
- `ActivityFeedbackController.cs.cs` has an accidental double extension.
- `Data` is not part of the solution.
- Old/commented implementations and old views/styles remain.
- The existing tenancy guide may be ahead of current implementation; code is the source of truth.

## Codex workflow

For a future Codex task:

1. Read `README.md`, this file, and any feature-specific document under `Docs/`.
2. Check `git status --short` before editing; preserve unrelated user changes.
3. Locate the controller, service, interface, repository, model, view, and JavaScript involved.
4. Trace tenant and authorization behavior before modifying data access.
5. Make the smallest cohesive change.
6. Add or update tests when practical.
7. Build the affected project, then run the relevant tests.
8. Run `git diff --check` and review the final diff for secrets and generated files.
9. Report what changed, how it was verified, and any remaining limitation.

Do not perform these actions unless explicitly requested:

- Send a real email
- Upload or delete an R2 object
- Apply a database migration
- Execute SQL cleanup scripts
- Modify production tenant records
- Commit, push, or deploy

## Verification matrix

| Change type | Minimum verification |
| --- | --- |
| Razor/CSS/JS | Build plus manual rendering of affected page and browser console check |
| Controller/service | Build plus focused unit or integration test |
| EF query/model | Build, relevant tests, generated SQL/schema impact review |
| Tenancy | Local host, active subdomain, custom domain, inactive/unknown tenant, cross-tenant isolation |
| Identity/authorization | Anonymous, allowed role, denied role, ownership checks, anti-forgery behavior |
| Payment/balance | Calculation tests, transaction behavior, duplicate/retry behavior |
| Email | Configuration validation and approved test recipient only |
| Background worker | One tenant failure isolation, cancellation, idempotency, repeated run |
| Deployment/config | Release build, Docker build where relevant, missing/invalid configuration behavior |

Standard local checks:

```powershell
dotnet build classlift_management.sln --no-restore
dotnet test classlift_management.sln --no-build
git diff --check
git status --short
```
