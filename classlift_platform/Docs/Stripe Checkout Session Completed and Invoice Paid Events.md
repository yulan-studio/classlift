# Stripe Checkout Session Completed and Invoice Paid Events

## Purpose

This document explains the difference between Stripe `checkout.session.completed` and `invoice.paid`, why their arrival order cannot be assumed, and how ClassLift should process both events safely for Plan and Add-on subscriptions.

## The difference between the two events

`checkout.session.completed` means that the customer completed a Stripe Checkout flow. For a subscription Checkout, it is used to obtain the Stripe Subscription ID and establish the local subscription relationship.

`invoice.paid` means that a specific Stripe Invoice was successfully paid. It is used to record the bill and payment, update the billing period, and keep the local subscription active.

| Event | Business meaning | Main local work |
|---|---|---|
| `checkout.session.completed` | Checkout flow completed | Create or activate the local subscription and save `StripeSubscriptionId` |
| `invoice.paid` | One specific invoice was paid | Create or update the local invoice and payment record, then update billing status |

## Why both events matter for the first subscription payment

For the first subscription purchase, both events may be related to the same customer action:

```text
Customer completes Checkout
        -> Stripe creates the subscription
        -> Stripe creates the first invoice
        -> The first invoice is paid
```

However, the events are delivered asynchronously. Stripe does not guarantee that the application will receive them in the same order in which the underlying operations occurred. The application may receive `checkout.session.completed` first, `invoice.paid` first, or the same event more than once after a retry.

The order must therefore be treated as nondeterministic.

## What happens on later billing cycles

The customer does not open a new Checkout Session for each renewal. Later cycles normally produce invoice events, such as:

```text
invoice.paid
invoice.payment_failed
```

There is normally no new `checkout.session.completed` event for each monthly renewal.

## ClassLift processing design

Plan and Add-on subscriptions use the same event model but separate local records.

```text
Plan:
checkout.session.completed -> organization_subscriptions
invoice.paid               -> invoices and payments

Add-on:
checkout.session.completed -> organization_addon_subscriptions
invoice.paid               -> addon_invoices and addon_payments
```

The `billing_scope` metadata separates the workflows:

```text
billing_scope=plan
billing_scope=addon
```

The Add-on workflow also uses `addon_key`, and the Plan workflow uses `plan_id` and `pending_plan_change_id`.

## How to handle either arrival order

### 1. Save a pending local request before Checkout

When a Plan change begins, ClassLift creates a `PendingPlanChange` with status `PendingPayment` before redirecting the customer to Stripe. This gives `invoice.paid` a local record to use even if it arrives before `checkout.session.completed`.

### 2. Make both handlers idempotent

Each handler must be safe to run more than once.

- Use `StripeSubscriptionId` to identify the local subscription.
- Use `StripeInvoiceId` to identify the local invoice.
- Use `ProviderTransactionId` to prevent duplicate payment records.
- If the record already exists, update it instead of inserting another record.

### 3. Do not activate from the browser redirect

The success URL only returns the customer to ClassLift. It is not proof of a verified payment. Subscription activation and payment recording must be performed by the signed Stripe Webhook.

### 4. Allow late events to complete missing work

If `checkout.session.completed` arrives first, it creates the local subscription and later `invoice.paid` adds the invoice and payment.

If `invoice.paid` arrives first, it uses the Stripe Subscription ID and `PendingPlanChange` or Add-on subscription metadata to create the missing local records. When `checkout.session.completed` arrives later, it sees the existing Stripe IDs and does not create duplicates.

### 5. Retry safely

Webhook delivery failures must be retried from Stripe. A retry should update the existing local records, not create duplicates. This is why the local Stripe IDs and unique indexes are required.

## Expected final state after the first successful payment

For a Plan:

```text
PendingPlanChange.Status = Paid
OrganizationSubscription.Status = Active
OrganizationSubscription.StripeSubscriptionId is populated
Invoice.InvoiceStatus = Paid
Payment.PaymentStatus = Succeeded
```

For an Add-on:

```text
OrganizationAddOnSubscription.Status = Active
OrganizationAddOnSubscription.StripeSubscriptionId is populated
AddOnInvoice.Status = Paid
AddOnPayment.PaymentStatus = Succeeded
```

## Operational verification

When testing in Stripe Sandbox, verify both event deliveries and the local database:

1. Confirm `checkout.session.completed` contains the expected `billing_scope` and identifiers.
2. Confirm `invoice.paid` contains the same Stripe Subscription ID.
3. Confirm the Webhook endpoint returns HTTP 2xx.
4. Confirm the relevant local subscription, invoice, and payment records exist.
5. Resend either event to verify idempotency; the record counts should not increase unexpectedly.

## Summary

`checkout.session.completed` establishes the Checkout-to-Subscription relationship. `invoice.paid` records each successful billing transaction. They are related but not interchangeable. Because Webhook events are asynchronous and can arrive out of order, ClassLift must use pending records, stable Stripe IDs, unique indexes, and idempotent upsert processing so either event order produces the same final local state.
