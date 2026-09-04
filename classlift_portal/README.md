# ClassLift Portal

ClassLift Portal is the public marketing and organization-signup website for
ClassLift, a course-management platform. It is implemented as four standalone
HTML pages using Tailwind CSS from its CDN and a small amount of browser
JavaScript.

## Pages

| File | Purpose |
| --- | --- |
| `index.html` | Product overview, audiences, and features |
| `pricing.html` | Starter, Growth, and Pro plan information |
| `contact.html` | Sales and support contact options |
| `signup.html` | Starter organization provisioning |

Brand and favicon images are stored in `icons/` and at `favicon.ico`.

## Run locally

No installation or build is required. Serve the directory with any static HTTP
server. For example, if Python is available:

```powershell
python -m http.server 8080
```

Then open `http://localhost:8080/`.

Tailwind CSS and the Inter font are fetched from external CDNs, so an internet
connection is required for the intended appearance.

> **Caution:** The current signup code treats an unknown hostname, including
> `localhost`, as the development environment. Do not submit the local form
> unless creating data in the shared development platform is intentional.

## Signup API

The signup page sends a JSON request to:

```text
POST {platformBaseUrl}/api/public/signup
Content-Type: application/json
```

Request fields:

```json
{
  "organizationName": "Maple Leaf Academy",
  "subdomain": "mapleleaf",
  "adminName": "Sarah Lee",
  "adminEmail": "admin@mapleleaf.edu",
  "adminPassword": "example-only"
}
```

The backend model also supports `planId`; when omitted it currently defaults to
Starter (`1`). On success, the endpoint returns a `message`; the page asks the
user to check their email and does not redirect. The tenant remains inactive
until the verification link activates it and redirects the browser to the tenant
website. Failure responses normally provide a user-facing `message`.

### Environment mapping

| Portal hostname | Platform API |
| --- | --- |
| `dev.classlift.ca` | `https://dev.platform.classlift.ca` |
| `staging.classlift.ca` | `https://staging.platform.classlift.ca` |
| `classlift.ca`, `www.classlift.ca` | `https://platform.classlift.ca` |
| Any other hostname | Currently defaults to the development API |

## Development notes

- Shared navigation, mobile-menu logic, calls to action, and footers are
  duplicated between pages; update all affected pages together.
- There is currently no automated test, lint, or HTML-validation command.
- The repository does not currently document a deployment provider or release
  command. This directory can be published as static files, but the environment
  domain must match the API-host mapping above.
- See `AGENTS.md` for repository-specific editing and verification guidance.

## Manual verification checklist

- Open every page and check desktop and mobile layouts.
- Follow all internal navigation and email links.
- Confirm every referenced icon and manifest exists.
- Exercise client-side signup validation without submitting the form.
- Test API success and failure behavior only with an authorized mock or test
  environment; a successful request provisions organization data.
