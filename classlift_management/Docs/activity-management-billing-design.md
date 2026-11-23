# Activity Management Billing and Entitlement Design

## Purpose

This document defines how Activity Management is made available as a separately paid add-on while remaining restricted to the three eligible subscription plans.

## Core decision

The three existing Plans provide purchase eligibility only. They do not automatically enable Activity Management.

An organization may use Activity Management only when both conditions are true:

```text
Eligible Plan
AND
Active Activity Management Add-on subscription
```

In other words:

- Plan membership is the prerequisite for purchasing the add-on.
- The add-on purchase and successful payment are the prerequisite for actual use.

## Data model responsibilities

### Existing PlanFeatures table

Keep `PlanFeatures` for ordinary features that are included in a plan. Activity Management should not be treated as an automatically included feature in this table if the three plans only grant eligibility to purchase it.

### Plan add-on eligibility

Create or use a Platform/Billing mapping for the plans that may purchase the add-on:

```text
PlanAddOnEligibility
--------------------
PlanId
AddOnId
IsEligible
```

The three existing plans should be mapped to the Activity Management add-on with `IsEligible = true`.

### Add-on definition

```text
FeatureAddOns
-------------
AddOnId
FeatureKey       = activity_management
PricePerMonth
MinimumMonthlyPrice
IsActive
```

### Organization add-on subscription

```text
OrganizationAddOnSubscriptions
------------------------------
OrganizationId
AddOnId
Status           Active / Trial / Grace / PastDue / Cancelled / Expired
StartedAt
CurrentPeriodEnd
CancelledAt
Price
BillingCycle
```

Platform/Billing owns the add-on price, invoices, payment status, renewal, cancellation, and grace-period rules.

## Entitlement calculation

Platform/Billing should calculate the final entitlement as follows:

```csharp
var canPurchase = planAllowsActivityAddOn;
var isPaid = addOnStatus is "Active" or "Trial" or "Grace";
var activityEnabled = canPurchase && isPaid;
```

Only the final `activityEnabled` result should be sent to Management as an enabled Feature.

Management should not infer access from Plan membership alone.

## Management enforcement

Management should use the existing Feature authorization mechanism for the final entitlement:

```csharp
_currentTenant.HasFeature(FeatureCodes.ActivityManagement)
```

The check must cover:

- Activity navigation and list pages
- Creating, editing, and deleting Activity Series
- Activity Session management
- Activity registration and confirmation
- Activity fees and payments
- Cancellation and refunds
- Related notifications, queries, and server endpoints

The user interface may hide unavailable links, but every server-side operation must also reject unauthorized requests.

## User messages

If the organization is on an eligible Plan but has not purchased the add-on:

> Your current plan is eligible for Activity Management, but the Activity Management add-on has not been purchased.

If the current Plan is not eligible:

> Activity Management is not available with your current plan.

## Final relationship

```text
PlanFeatures
    Ordinary included features

PlanAddOnEligibility
    Whether a Plan may purchase Activity Management

OrganizationAddOnSubscriptions
    Whether the organization actually purchased and paid for Activity Management

Final entitlement
    PlanAddOnEligibility = true
    AND add-on status is Active, Trial, or Grace
```

## Business rule

Activity Management is a separately billable Feature Add-on. The three existing Plans define eligibility to purchase the add-on; being on an eligible Plan does not by itself enable Activity Management. Actual use requires an active paid add-on, or an explicitly supported trial or grace status.
