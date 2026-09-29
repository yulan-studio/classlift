# Changelog

## 2026-09-29

- Added `BUSINESS_RULES.md` to document current organization, trial,
  subscription, billing, and data-retention rules.
- Removed `MinimumMonthlyPrice` from subscription plans and organization
  subscriptions.
- Updated invoice calculation to use the tenant `coaches` table count.
- Preserved subscription price snapshots so plan price changes do not affect
  existing organizations.
- Added trial-expiry handling based on the tenant `users` count:
  - `users <= 2`: deactivate the organization and tenant registry, cancel the
    subscription, and do not generate an invoice.
  - `users > 2`: activate the subscription and bill using the actual coach
    count.
- Added automatic cleanup for public signups whose email verification remains
  unconfirmed for more than 24 hours.
- Expired unverified signups now remove the tenant database and all related
  platform records.
- Updated project documentation to reflect the current billing and tenant
  lifecycle behavior.
