# ClassLift Business Rules

This file is the concise, canonical record of business rules confirmed by the product owner. It supports implementation decisions and will be the source for a future user manual.

## Courses and pricing

1. A course with a Session Count uses fixed per-session pricing.
   - Hourly Cost must be cleared, stored as null, and disabled on the Add Course form.
   - Session Cost is required.
2. A private course without a Session Count uses hourly pricing and requires Credit Tracking.
3. A Group course requires a Session Count and Session Cost.
4. Max Capacity is optional for a Group course. When supplied, it limits active registrations.
5. The Staff course list is paginated and can be filtered by Specialty, Provider, Course Type, and active status. Sorting and active filters are preserved while paging.

## Course registration messaging

1. After a participant is successfully registered in a course, Staff sees a success message using the organization's configured participant term and the wording "registered successfully".

## Group-course registration

1. A newly created Group-course registration remains pending until it is confirmed.
2. A participant may remove their own pending course registration from the Confirmations page before confirmation. The removal removes the pending registration, its unconfirmed session registrations, and its associated course fee record; confirmed or completed registrations are not removed by this action.
3. A parent must confirm a Group-course registration before the first Group session date begins, using the first session's local time zone. If the root registration is still unconfirmed at midnight (00:00) at the start of that date:
   - cancel the root registration;
   - cancel all of its non-terminal child-session registrations;
   - preserve Completed and Deleted session history; and
   - recalculate course availability when Max Capacity is configured.
3. When Staff changes the location of a Group master session, ClassLift copies the new location to every child session linked to that master session.
4. Before Staff can register a participant in a Group course, the number of Group master sessions whose status is Open or Completed must equal the course's Session Count exactly, unless the course has already started.
   - If the course has not started and the total is lower, registration is blocked and Staff is instructed to finish setting up the course sessions.
   - If at least one Group master session is already Completed, a mid-course registration is allowed even when the remaining session setup is incomplete; the participant is registered for the available Open sessions.
   - If the total is higher, registration is blocked because the course session data is inconsistent.
   - Canceled, Deleted, and child-session copies are not included in this count.
   - No registration, fee, balance, or child-session data is created when this validation fails.
5. In the Staff course-registration selector, a full Group course remains selectable and is labeled as full; the server still rejects the registration if capacity is reached.

## Activity registration

1. A participant may remove their own pending activity registration from the Confirmations page before confirmation. The removal removes the pending registration and its associated activity fee record.

## Activity completion

1. An Activity is considered ended when its scheduled start time plus its Scheduled Hours has passed. The background status updater then changes the Activity and its Confirmed participant registrations to Completed.
2. All Activity creation fields are required, including description, address, capacity, cost, time zone, scheduled date, scheduled hours, and registration status.

## Canceled activities and refunds

1. When Staff cancels an Activity, all active registrations with status Registered, Confirmed, or Scheduled become Canceled.
2. When Credit Tracking is enabled, each canceled registration with a paid Fee receives one balance refund using the Fee's original Total Cost. The Fee record remains unchanged, and the refund is recorded in child balance history. Unpaid registrations are canceled without a refund.

## Course completion and reporting

1. Fixed-session Group and private sessions are completed automatically after their scheduled end time.
2. Private sessions without a fixed Session Count are completed manually by the coach using actual hours.
3. Standard course reports include completed child sessions that have Actual Hours recorded.
4. My Enrollment History returns only course sessions with status Completed. Canceled and OnLeave sessions are excluded.
5. The Enrollments History course table uses the organization's configured provider term instead of a fixed Coach heading.
6. Staff can view current Group-course schedules and participant attendance in a read-only page grouped by course and Session.

## Canceled course sessions

1. When Staff cancels a Group course session, the affected child sessions are canceled as part of the same operation.
2. The canceled session's Actions must let Staff explicitly choose either Create Replacement or Refund Session Cost; the system must not automatically perform either choice.
3. A replacement session is created with a new future schedule and follows the normal Group-session registration process.
4. If Staff chooses Refund Session Cost, the system credits each affected participant's course balance by that course's Session Cost and records the canceled session and participant session in the balance history. A refund cannot be applied twice to the same participant session.
5. Refund Session Cost is available only when the tenant's plan includes Credit Tracking (Balance). If the plan does not include it, Staff must create a replacement session; the refund operation is hidden in the page and rejected by the server.
6. When Staff chooses Refund Session Cost instead of creating a replacement, the course's final Session Count is reduced by one. Creating a replacement does not reduce Session Count. The reduction is applied only once with the refund operation.
7. When Staff creates a replacement for a canceled Group session, the cancellation Staff Note and replacement Staff Note are entered separately. Each note is saved only to the corresponding session workflow.
8. Only courses with IsActive set to true are shown to Coaches and in Staff's course-registration choices. A Group course reaching Max Capacity stops new registrations but remains active and visible until the course ends or Staff deactivates it.

## Private-course scheduling

1. On Coach Manage Schedules, a Private course session whose status is RequestToReschedule does not show the Edit action. The Coach may still use Remove, subject to the existing removal rules.
2. A Provider Note containing non-whitespace text is required before a Coach can update or remove a Private course session. The displayed Provider term comes from the organization's terminology settings, and the rule is enforced by both the Manage Schedules page and the server.
3. Coach Manage Schedules displays both Provider Note and Participant Note labels using the organization's configured terminology rather than fixed Coach or Child wording.
4. Coach View Enrollments uses the organization's configured Provider and Participant terminology in its note table headings.
5. In Coach Manage Enrollments, entering Actual Hours as 0 removes the scheduled session. This removal workflow must not deduct Token balance.
6. In Coach Manage Enrollments, a Coach can edit and save the Provider Note for a completed session.

## Participant schedule display

1. Upcoming Private-course schedules include only Scheduled, RequestToReschedule, and Deleted sessions.
2. Upcoming Group-course schedules include only Scheduled, RequestToLeave, OnLeave, and Canceled sessions.

## Accounts and user manual

1. Coaches can maintain their chosen name, contact details, city, address, status, and photo consent from Account Settings. The coach address is used only for tax documents and formal document delivery. Member ID and bank payment details remain Staff-managed. Blank optional values clear the corresponding details.
2. ClassLift keeps course operations and their directly related financial workflows with the Staff role. Staff who manage attendance, course registration, withdrawals, refunds, balances, and related financial records may complete that workflow without transferring the task to a separate Finance role.
3. Bank, Transit, and Account details remain restricted sensitive information. They are not part of the ordinary Coach self-service profile and should be visible only to Admin or specifically authorized Staff.

## Payments and balance history

1. When a course or activity registration has a fee of zero, its fee record is marked paid and its description is `Free registration — no payment is required.`.

2. Payment records are financial audit records. Staff must not delete them; the payment list does not offer a Remove action, and the server rejects deletion requests.
3. A participant's balance continues to use the balance snapshot created by each balance transaction. Since the system has not entered formal use, historical balance repair is not part of this rule.
4. Staff balance adjustments require only an amount and remarks. Screenshot upload is not required.
5. Staff may add a Direct Payment for a course or activity only after the participant's registration is Confirmed. The payment form warns Staff and disables Add Payment until confirmation.

1. Parents use one shared account for the participant portal rather than separate parent accounts.
2. The initial user manual will be written in English.
3. The initial user manual will be one combined manual covering all roles.
4. An administrator can configure one reply-to email address and one notification recipient email address for their organization. ClassLift sends from a platform-controlled, SMTP-verified address and directs recipient replies to the organization's reply-to address.
5. After authentication, a user who followed a protected ClassLift link returns to that same local page. Missing, invalid, or external return destinations fall back to the ClassLift home page.

## Email notifications

1. After Staff successfully registers a participant in a course, ClassLift emails the family's shared participant-portal address and asks the family to confirm the registration.
   - The email links directly to the participant Confirmations page.
   - Registration, fee, and Group child-session records must all be created before delivery is attempted.
   - Missing recipients or email-delivery failures do not roll back the registration; Staff sees a warning instead.
2. When a private course is confirmed, ClassLift sends separate notifications to:
   - the organization's configured notification recipient email address; and
   - the coach assigned to that private course.
   - Each recipient receives a link appropriate to their role.
   - A missing recipient or delivery failure does not roll back the confirmation; the family sees a warning.
3. When a Group course is confirmed, ClassLift notifies the organization's configured notification recipient email address.
   - The confirmation workflow must complete successfully before delivery is attempted.
   - A missing recipient or delivery failure does not roll back the confirmation; the family sees a warning.
4. When an activity is confirmed, ClassLift notifies the organization's configured notification recipient email address.
   - The activity and fee workflow must complete successfully before delivery is attempted.
   - A missing recipient or delivery failure does not roll back the confirmation; the family sees a warning.
5. After Staff successfully updates a Group course session, ClassLift sends one notification to each affected family's shared participant-portal email address, deduplicated by email address.
   - The database update completes before notification delivery is attempted.
   - A missing recipient or email-delivery failure does not roll back the session update; Staff sees a warning instead.
   - When Staff changes the session status to Canceled, the email subject and content must clearly identify the session as canceled.
   - When Staff Note contains text, the updated-session email includes it in both HTML and plain-text content.
6. After Staff successfully creates one or more Group course sessions, ClassLift sends one notification email per affected shared family email address.
   - All sessions created by the same recurring-session request are summarized in that single family email.
   - Notification delivery begins only after every requested session creation has reported success.
   - Missing recipients or email-delivery failures do not roll back created sessions; Staff sees a warning instead.
7. After a Coach successfully creates one or more Private course sessions, ClassLift sends one email to the family's shared participant-portal address.
   - Sessions created by the same recurring-session request are summarized in one email.
8. After a Coach successfully updates, deletes, or completes a Private course session, ClassLift sends a corresponding notification to the family's shared participant-portal address.
   - The business operation completes before notification delivery is attempted.
   - Missing recipients or email-delivery failures do not roll back the session action; the Coach sees a warning instead.
   - A completion email includes the actual hours, and all session-action emails link to the participant Schedules page.
9. After a family successfully submits or changes a course-session request, ClassLift sends one notification for that submitted course form.
   - Private-course requests go to the assigned Coach and link to that registration's Coach schedule page.
   - Group-course requests go to the organization's configured notification recipient and link to Staff's session-registration page.
   - The email identifies the participant, course, request type, affected session time, and family note.
   - Saving the request completes before email delivery is attempted; delivery failure does not roll back the saved request and the family sees a warning.

## Open questions

- None currently.
