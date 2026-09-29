# ClassLift Business Rules

## Organizations and tenants

- The platform database stores organizations, tenant registrations, plans,
  subscriptions, invoices, payments, and subscription events.
- Each organization has a separate tenant MySQL database.
- `Organization.IsActive` and `Tenantregistry.IsActive` represent whether the
  organization and tenant are active in the platform.
- Deactivating an organization does not automatically delete its tenant
  database unless an explicit cleanup workflow does so.

## Public signup and email verification

- A public signup creates the organization, tenant registry, tenant database,
  schema, and trial subscription.
- The tenant registry starts inactive until the administrator confirms the
  email verification link.
- The email verification link is valid for 24 hours.
- If the link is not confirmed within 24 hours, the daily cleanup job fully
  deletes the tenant database and all related platform records, including the
  organization, tenant registry, subscriptions, invoices, payments, and
  subscription events.

## Trial subscriptions

- A new organization starts with a 30-day free trial.
- The tenant database is initialized with two system accounts: the initial
  administrator and staff account.
- When the trial expires, the system counts rows in the tenant database's
  `users` table.
- If `users <= 2`, the organization is considered unused:
  - `Organization.IsActive` is set to `false`.
  - `Tenantregistry.IsActive` is set to `false`.
  - The trial subscription is set to `Cancelled`.
  - No invoice is generated.
  - The tenant database and platform records are retained for administrator
    review or manual cleanup.
- If `users > 2`, the organization is considered used:
  - The subscription becomes `Active`.
  - A prorated invoice is generated for the remainder of the current month.
  - Recurring monthly billing continues while the subscription is active.
- The threshold of two accounts must be updated if the number of initialized
  system accounts changes.

## Pricing and billing

- A subscription stores a snapshot of `PricePerCoach` when it is created or
  when an organization changes plans.
- Changing a plan's price does not change existing organization subscription
  prices.
- New subscriptions and plan changes use the current plan price.
- The number of billable teachers is the total number of rows in the tenant
  database's `coaches` table.
- Invoice usage is calculated as:

  ```text
  coach count * subscription monthly price per coach
  ```

- Partial-month invoices are prorated by the number of used days in the month.
- There is no minimum monthly price.
- Invoice records are created with a pending status and a due date 15 days
  after the billing period ends.

## Data retention and deletion

- Unverified signups older than 24 hours are permanently deleted automatically.
- Trial organizations with no additional users are deactivated but retained.
- Retained inactive organizations may be reviewed and deleted manually by an
  administrator.
- Permanent deletion must remove the tenant database and all related platform
  records together.
