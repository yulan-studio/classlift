# Management and Platform Authentication Design

## Purpose

This document explains how authentication works between Management and Platform, and how Feature Settings are protected when a user works directly in Platform or through a tenant Management site.

## Authentication methods

The system supports two authentication paths:

1. Internal service authentication with `X-Platform-Api-Key`.
2. Browser user authentication with the signed-in user identity and organization authorization.

These are alternative paths. A request does not need to pass both paths.

## Management calling Platform

When a user opens Feature Settings in Management, Management calls Platform in the background:

```text
User in Management
    -> Management Feature Settings page
    -> OrganizationFeatureSettingsService
    -> Platform Feature Settings API
```

Management reads `PlatformApi:ApiKey` from its server configuration and sends `X-Platform-Api-Key`. Management may also forward the browser `Authorization` header, but the internal API key is the service-to-service credential used for this server call.

Platform validates the key against its `ManagementApi:ApiKey` configuration. When the key matches, Platform allows the request for the organization ID supplied by Management.

This path is used for loading and updating organization Feature Settings from Management, and for future Management-to-Platform entitlement queries.

The API key must remain server-side. It must never be placed in browser JavaScript, HTML, or a public response.

## User operating directly in Platform

A user may also use Platform's own administrative pages. In that case, the browser request normally does not contain the internal API key. Platform uses the authenticated user identity:

```text
Signed-in Platform user
    -> Platform page or Platform API
    -> User.Identity.IsAuthenticated
    -> OrganizationAuthorization.HasAccessAsync
    -> Feature Settings operation
```

Platform checks that the user is authenticated and has access to the requested organization. This prevents a user from changing the URL organization ID and reading or modifying another organization's settings.

## User operating through Management

When a user uses the tenant Management site, the user is authenticated in Management. Management resolves the tenant from the request host and obtains the organization ID from the resolved tenant context.

Management then calls Platform's Feature Settings API using the internal API key:

```text
Signed-in Management user
    -> Management resolves tenant and organization
    -> Management server adds X-Platform-Api-Key
    -> Platform validates the internal service request
    -> Platform reads or updates that organization’s settings
```

The browser user does not receive or directly use the Platform API key.

## Platform authorization decision order

The current Platform API uses this decision order:

```text
Does the request contain an API key?
    Yes -> Is the key correct?
              Yes: allow
              No: reject with Unauthorized

    No -> Is the current user authenticated?
              No: reject
              Yes -> Does the user belong to the requested organization?
                         Yes: allow
                         No: reject
```

An incorrect API key does not fall back to user authentication. This prevents a malformed or forged internal request from being silently treated as a browser request.

## Current implementation locations

Management sends the internal credential in `Core/Services/OrganizationFeatureSettingsService.cs` using `X-Platform-Api-Key`.

Platform validates the request in `Billing/Controllers/Api/OrganizationFeatureSettingsController.cs`. It uses `ManagementApi:ApiKey` for internal service validation and `OrganizationAuthorization.HasAccessAsync` for direct signed-in user requests.

## Security rules

- Keep the API key only in server configuration.
- Do not expose the API key to browser code or page models.
- Do not authorize a request only because it contains an `organizationId` route value.
- Direct browser requests must validate user membership or administrative access to the organization.
- Internal service requests must validate the shared API key using a constant-time comparison.
- Entitlement APIs for Activity Management should reuse this authentication pattern.

## Summary matrix

| User action | Caller | Credential | Platform check |
|---|---|---|---|
| View or change settings in Management | Management server | `X-Platform-Api-Key` | API key matches Platform configuration |
| View or change settings in Platform | Browser user | Signed-in user identity | User is authenticated and belongs to organization |
| Management entitlement query | Management server | `X-Platform-Api-Key` | API key matches; organization is resolved by trusted Management call |
| Direct entitlement query in Platform | Browser user | Signed-in user identity | User is authenticated and belongs to organization |

## Final principle

```text
Management-to-Platform server calls use service authentication.
Platform browser operations use user authentication plus organization authorization.
```
