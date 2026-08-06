# AGENTS.md

## Scope

These instructions apply to every file in `classlift_portal`.

ClassLift Portal is a small, dependency-free static marketing site with a public
organization-signup flow. Keep it usable as plain static files unless a task
explicitly introduces a build system.

## Project structure

- `index.html`: product overview and feature marketing
- `pricing.html`: plans, pricing, and sales calls to action
- `contact.html`: sales and support contact information
- `signup.html`: organization signup form and API integration
- `icons/` and `favicon.ico`: branding and browser icons

There are currently no generated files, package dependencies, automated tests,
or deployment configuration in this directory.

## Editing conventions

- Use semantic, accessible HTML and plain browser JavaScript.
- Follow the existing Tailwind utility-class approach.
- Preserve responsive behavior at mobile and desktop widths.
- Keep shared navigation, calls to action, contact details, and footer content
  consistent across all four pages.
- Prefer links for navigation and buttons for actions.
- Give form controls explicit labels and retain native browser validation where
  possible.
- Do not add secrets, credentials, private API keys, or environment-specific
  tokens to the static files.
- Do not add a framework, package manager, or build step unless the task clearly
  requires it and the tradeoff is documented in `README.md`.

## Signup integration

`signup.html` sends `POST /api/public/signup` with JSON containing:

- `organizationName`
- `subdomain`
- `adminName`
- `adminEmail`
- `adminPassword`

The backend also accepts `planId`, which currently defaults to Starter (`1`) when
omitted. A successful response contains `tenantUrl`; an error response normally
contains `message`.

API hosts are selected from the portal hostname:

- `dev.classlift.ca` -> `https://dev.platform.classlift.ca`
- `staging.classlift.ca` -> `https://staging.platform.classlift.ca`
- `classlift.ca` or `www.classlift.ca` -> `https://platform.classlift.ca`

Treat changes to this mapping, request fields, response handling, redirect
behavior, and password validation as integration changes. Keep the displayed
subdomain preview and the value sent to the API identical. Do not silently send
local or unknown-host traffic to a shared environment.

## Verification

After editing, verify as applicable:

1. All local links and referenced assets exist.
2. Navigation and the mobile menu work on every page.
3. Pages remain readable at narrow and wide viewport sizes.
4. HTML has no malformed attributes or duplicate IDs.
5. Keyboard focus and labels work for interactive controls.
6. Signup rejects invalid and mismatched passwords.
7. Subdomain normalization matches the submitted value.
8. Signup success, API error, malformed response, and network failure states are
   understandable and do not expose sensitive information.

Do not submit the signup form against dev, staging, or production merely as a
smoke test; it provisions organization data. Use a specifically authorized test
environment or mock endpoint.
