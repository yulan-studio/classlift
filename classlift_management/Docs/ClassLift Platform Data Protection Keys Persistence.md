# ClassLift Platform Data Protection Keys Persistence

## Purpose

This document describes how ClassLift Platform persists ASP.NET Core Data Protection keys across Railway restarts and deployments.

Persisting these keys prevents previously issued authentication cookies and security tokens from becoming unreadable when the application restarts.

## What Data Protection keys protect

ASP.NET Core uses Data Protection keys to protect:

- Identity authentication cookies;
- antiforgery and CSRF tokens;
- password reset tokens;
- email confirmation tokens;
- remember-device and related Identity tokens.

If the keys are lost, users may be redirected to the login page and existing form tokens may fail validation.

## Required isolation by environment

Dev, Staging, and Production must each have a separate persistent Railway Volume. Do not share one Volume between environments.

Each Railway service should mount its own Volume at:

```text
/app/keys
```

The path can be the same in every environment because the Volumes themselves are separate.

## Platform code configuration

`Billing/Program.cs` uses a project-local folder during local development and the Railway Volume in deployed environments:

```csharp
var dataProtectionPath = builder.Environment.IsDevelopment()
    ? Path.Combine(builder.Environment.ContentRootPath, "data-protection-keys")
    : "/app/keys";

Directory.CreateDirectory(dataProtectionPath);

builder.Services
    .AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionPath))
    .SetApplicationName("ClassLift.Platform");
```

The application name must remain stable for the same environment. The local key folder must not be committed to source control.

## Railway setup

For each Railway environment:

1. Open the Platform service.
2. Create a new Volume.
3. Set the mount path to `/app/keys`.
4. Deploy the latest Platform code.
5. Sign in again after the first deployment because cookies created with the old non-persistent keys cannot be recovered.

Use separate Volumes for Dev, Staging, and Production.

## Local development

Local keys are stored in:

```text
data-protection-keys/
```

This folder is ignored by Git. If it is deleted, local users may need to sign in again, but the deployed environments are unaffected.

## Verification

After deployment:

1. Sign in to Platform.
2. Restart the Railway service.
3. Return to Platform and refresh the page.
4. Confirm that the session remains authenticated.
5. Submit a form after the restart and confirm that no antiforgery-token error occurs.

The Railway Volume should contain XML key files after the application starts.

## Security and operations

Data Protection keys are security-sensitive. Restrict access to the Volume and do not print key contents in logs. Back up Production keys according to the platform's operational backup policy.

If the application later runs multiple instances, all instances in the same environment must access the same key store. Separate environments must continue to use separate key stores.

## Current implementation status

- Platform code is configured to persist keys.
- Local key storage is excluded from Git.
- Railway Volumes still need to be created and mounted independently for Dev, Staging, and Production.
