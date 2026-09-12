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

## Group-course registration

1. A newly created Group-course registration remains pending until it is confirmed.
2. A parent must confirm a Group-course registration before the first Group session date begins, using the first session's local time zone. If the root registration is still unconfirmed at midnight (00:00) at the start of that date:
   - cancel the root registration;
   - cancel all of its non-terminal child-session registrations;
   - preserve Completed and Deleted session history; and
   - recalculate course availability when Max Capacity is configured.
3. When Staff changes the location of a Group master session, ClassLift copies the new location to every child session linked to that master session.
4. Before Staff can register a participant in a Group course, the number of Group master sessions whose status is Open or Completed must equal the course's Session Count exactly.
   - If the total is lower, registration is blocked and Staff is instructed to finish setting up the course sessions.
   - If the total is higher, registration is blocked because the course session data is inconsistent.
   - Canceled, Deleted, and child-session copies are not included in this count.
   - No registration, fee, balance, or child-session data is created when this validation fails.

## Course completion and reporting

1. Fixed-session Group and private sessions are completed automatically after their scheduled end time.
2. Private sessions without a fixed Session Count are completed manually by the coach using actual hours.
3. Standard course reports include completed child sessions that have Actual Hours recorded.

## Private-course scheduling

1. On Coach Manage Schedules, a Private course session whose status is RequestToReschedule does not show the Edit action. The Coach may still use Remove, subject to the existing removal rules.
2. A Provider Note containing non-whitespace text is required before a Coach can update or remove a Private course session. The displayed Provider term comes from the organization's terminology settings, and the rule is enforced by both the Manage Schedules page and the server.
3. Coach Manage Schedules displays both Provider Note and Participant Note labels using the organization's configured terminology rather than fixed Coach or Child wording.
4. Coach View Enrollments uses the organization's configured Provider and Participant terminology in its note table headings.

## Accounts and user manual

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
