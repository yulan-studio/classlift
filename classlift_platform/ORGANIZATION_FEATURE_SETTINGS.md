# Organization Feature Settings

## Overview

Plans determine which Features an organization is entitled to use. Some
Features may additionally be disabled by an organization administrator. This
is controlled by `Feature.IsOrganizationConfigurable`.

普通 Feature 的默认行为保持不变：只要套餐包含该 Feature，就视为启用，组织不能关闭它。只有明确标记为可配置的 Feature 才会读取组织级设置。

目前可配置的 Feature 是：

```text
student_schedule_request
```

## Data model

The `features` table contains:

| Column | Meaning | Default |
|---|---|---|
| `IsOrganizationConfigurable` | Whether an organization may change this Feature's enabled state | `false` |

The `organization_feature_settings` table stores overrides:

| Column | Meaning |
|---|---|
| `OrganizationFeatureSettingID` | Primary key |
| `OrganizationID` | Organization being configured |
| `FeatureID` | Configurable Feature |
| `IsEnabled` | Organization-level enabled state |

`OrganizationID` and `FeatureID` have a unique constraint. A missing settings
row means enabled, preserving backward-compatible behavior for existing
organizations.

## Effective access rules

For an active subscription, a Feature is available when:

1. The organization's plan includes the Feature; and
2. Either the Feature is not organization-configurable, or its organization
   setting is missing or enabled.

An organization setting cannot grant access to a Feature that is not included
in the organization's plan.

The effective Feature context is cached for ten minutes. Callers that change a
setting should clear or allow expiry of the cache before expecting a subsequent
permission check to reflect the change.

## Management API

The management application uses these Platform endpoints:

```http
GET /api/organizations/{organizationId}/feature-settings
```

Returns all organization-configurable Features. Each item includes
`featureKey`, `featureName`, and `isEnabled`.

```http
PUT /api/organizations/{organizationId}/feature-settings/{featureKey}
Content-Type: application/json
X-Platform-Api-Key: <shared-secret>

{ "isEnabled": false }
```

The PUT endpoint returns `204 No Content` when successful. A Feature that is
not organization-configurable returns `404`; an invalid or inactive
organization also returns `404`.

## Service-to-service authentication

Platform reads the shared secret from:

```text
ManagementApi:ApiKey
```

For Railway, configure:

```text
ManagementApi__ApiKey=<shared-secret>
```

Management should store the same value under its client-side setting, for
example `PlatformApi__ApiKey`, and send it as `X-Platform-Api-Key`. The key is
compared using a constant-time comparison and is never written to logs or
responses.

Requests without the service key may still be made by authenticated Platform
users using the normal organization authorization rules. Service requests are
not redirected to a login page; invalid service credentials return `401`.

## Applying the database migration

From the repository root:

```powershell
$env:ASPNETCORE_ENVIRONMENT = "Development"
dotnet ef database update `
  --context BillingDbContext `
  --project .\Billing\Billing.csproj `
  --startup-project .\Billing\Billing.csproj
```

The migration adds `IsOrganizationConfigurable`, creates
`organization_feature_settings`, and marks `student_schedule_request` as
configurable.

## Troubleshooting

- `401`: the request is missing or has an incorrect `X-Platform-Api-Key`.
- `404`: the organization is invalid/inactive, or the Feature is not marked
  configurable.
- `500` on GET: check that the deployed Platform image includes the latest
  Feature Settings API query fix.
- `502`: check the Management `PlatformApi:BaseUrl`, Platform availability, and
  reverse-proxy/Railway routing. A `502` is normally generated before the
  Platform controller executes.
