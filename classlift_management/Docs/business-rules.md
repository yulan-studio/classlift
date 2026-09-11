# ClassLift Business Rules

This file is the concise, canonical record of business rules confirmed by the product owner. It supports implementation decisions and will be the source for a future user manual.

## Courses and pricing

1. A course with a Session Count uses fixed per-session pricing.
   - Hourly Cost must be cleared, stored as null, and disabled on the Add Course form.
   - Session Cost is required.
2. A private course without a Session Count uses hourly pricing and requires Credit Tracking.
3. A Group course requires a Session Count and Session Cost.
4. Max Capacity is optional for a Group course. When supplied, it limits active registrations.

## Group-course registration

1. A newly created Group-course registration remains pending until it is confirmed.
2. A parent must confirm a Group-course registration before the first Group session date begins, using the first session's local time zone. If the root registration is still unconfirmed at midnight (00:00) at the start of that date:
   - cancel the root registration;
   - cancel all of its non-terminal child-session registrations;
   - preserve Completed and Deleted session history; and
   - recalculate course availability when Max Capacity is configured.

## Course completion and reporting

1. Fixed-session Group and private sessions are completed automatically after their scheduled end time.
2. Private sessions without a fixed Session Count are completed manually by the coach using actual hours.
3. Standard course reports include completed child sessions that have Actual Hours recorded.

## Accounts and user manual

1. Parents use one shared account for the participant portal rather than separate parent accounts.
2. The initial user manual will be written in English.
3. The initial user manual will be one combined manual covering all roles.
4. An administrator can configure one reply-to email address and one notification recipient email address for their organization. ClassLift sends from a platform-controlled, SMTP-verified address and directs recipient replies to the organization's reply-to address.

## Email notifications

1. When a private course is confirmed, ClassLift sends separate notifications to:
   - the organization's configured notification recipient email address; and
   - the coach assigned to that private course.
2. After Staff successfully updates a Group course session, ClassLift sends one notification to each affected family's shared participant-portal email address, deduplicated by email address.
   - The database update completes before notification delivery is attempted.
   - A missing recipient or email-delivery failure does not roll back the session update; Staff sees a warning instead.
   - When Staff changes the session status to Canceled, the email subject and content must clearly identify the session as canceled.

## Open questions

- None currently.
