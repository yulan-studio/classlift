# ClassLift Database Design & Course Enrollment Workflows

> A code-verified guide to tenant data, private-course registration and scheduling, and group-course registration and scheduling.

**Version:** 1.0  
**Reviewed:** 30 August 2026  
**Source:** Current ClassLift repository implementation

> **Scope**
> This document describes the database model implemented in the ClassLift source code. It explains which records are inserted, linked, and updated during course registration, scheduling, confirmation, and token charging. It is not a generated database schema or a substitute for migration review.

| Document | Value |
| --- | --- |
| Audience | Owner, developers, support, and pilot operations |
| System boundary | Platform billing database plus one operational database per tenant |
| Primary workflow table | course_enrollments |
| Source basis | Entity models, EF Core mappings, repositories, services, controllers, and tenant documentation |
| Version | 1.0 • 30 August 2026 |

## 1. Executive summary

ClassLift uses a database-per-tenant architecture. A central platform database resolves domains and stores organizations, plans, subscriptions, and feature entitlements. Each organization’s operational data—users, children, coaches, courses, enrollments, fees, payments, balances, and coach income—lives in that tenant’s own database.

> **The central design idea**
> The course_enrollments table is polymorphic in practice: it stores course registrations, group master sessions, and child-level scheduled sessions. Row type is inferred from CourseType, ChildID, ScheduledAt, Status, and EnrollmentID_Ref.

### What gets inserted

| Workflow | Immediate inserts | Later inserts / updates |
| --- | --- | --- |
| Private registration | 1 root course_enrollments row; 1 fees row | Coach scheduling inserts child session rows. Confirmation may add child_balance and updates root/fee. |
| Group registration | 1 root course_enrollments row; 1 fees row; 1 child session row for each already-open group master session | Confirmation may add child_balance, confirms root, marks selected child session rows Scheduled. |
| Private scheduling | 1 child session row per occurrence | Completion can create balance and coach_income effects elsewhere in the workflow. |
| Group session creation | 1 master session row; then 1 child session copy per active root registration | New child registrations later receive copies of every currently open master session. |

### Most important implementation caveats

- The multi-step registration and session fan-out operations are not wrapped in one database transaction; a failure can leave partial data.
- EnrollmentID_Ref is not modeled as an explicit self-referencing foreign key/navigation in CourseEnrollment; its integrity is enforced mainly by application logic.
- Statuses and course/payment types are strings in much of the tenant model, so consistent spelling and allowed transitions depend on code.

## 2. Database architecture

### 2.1 Platform database (shared control plane)

BillingDbContext represents the shared platform database. It identifies the tenant from a subdomain or custom domain, points to the tenant database, and controls plan/feature access.

| Table | Purpose | Key relationships |
| --- | --- | --- |
| organizations | Customer organization and current plan | CurrentPlanID → subscriptionplans |
| tenantregistry | Tenant database name and domain routing | OrganizationId → organizations; cascade delete |
| subscriptionplans | Plan catalog and CAD pricing fields | Referenced by organizations and subscriptions |
| features | Feature catalog keyed by FeatureKey | Many-to-many with plans |
| planfeatures | Plan-to-feature entitlement and lock state | PlanID + FeatureID unique |
| organization_subscriptions | Trial/active/cancelled/expired history and billed prices | OrganizationID → organizations; PlanID → subscriptionplans |

### 2.2 Tenant operational database (data plane)

AppDbContext is selected after tenant resolution. Its data belongs to one organization. This separation is the main tenant-isolation boundary; each request must resolve the correct database before authentication and controller execution.

```text
Incoming host
   └─ tenantregistry (platform DB)
       └─ DatabaseName
           └─ AppDbContext (one tenant DB)
               ├─ Identity and people
               ├─ Courses, activities, sessions, enrollments
               └─ Fees, payments, balances, coach income
```

> **Isolation rule**
> Operational queries should never select a tenant by an OrganizationID column inside shared business tables. Isolation comes from choosing the correct tenant connection/database first.

## 3. Tenant database: table map

| Area | Tables | Role |
| --- | --- | --- |
| Identity | users, roles, userroles, userclaims, userlogins, roleclaims, usertokens | Authentication, authorization, and user identity |
| People | admins, staff, coaches, children, parents, parent_child, emergency_contacts | Role profiles and child/guardian relationships |
| Organization settings | organization_email_settings | One tenant-local sender address and notification recipient address configured by an administrator |
| Catalog | courses, specialties, coach_specialty, provinces, cities | Course offering, coach expertise, and location reference data |
| Course operations | course_enrollments, course_notifications | Registration, master sessions, child sessions, notes, and notifications |
| Activities | activities, activity_enrollments, activity_feedback, activity_notifications | Separate activity/event workflow |
| Money | fees, payments, payment_package, child_balance, coach_income | Charges, receipts, balance ledger, and coach earnings |

### 3.1 Core course relationship map

```text
coaches ───────────────┐
specialties ───────────┼──> courses
                       │      │
children ──────────────┼──────┴──> course_enrollments
                       │               │
course_enrollments ────┘               ├──> fees ──> payments
 (semantic self-link via                ├──> child_balance
  EnrollmentID_Ref)                    └──> coach_income
```

### 3.2 The course_enrollments row types

| Logical row type | ChildID | ScheduledAt | EnrollmentID_Ref | Typical status |
| --- | --- | --- | --- | --- |
| Root child registration | Set | NULL | NULL | Registered → Confirmed |
| Private child session | Set | Set | Private root registration ID | Scheduled → Completed / Canceled / OnLeave |
| Group master session | NULL | Set | NULL | Open / Closed → Completed / Canceled |
| Group child session copy | Set | Set | Group master session ID | Registered → Scheduled → Completed / other |

> **Critical interpretation**
> For a private course, EnrollmentID_Ref points to the child’s root registration. For a group course, it points to the group master session. The same column therefore has two business meanings.

## 4. Private-course registration

### 4.1 Preconditions and calculation

1. Staff submits RegisterCourse for a child and a Private course.
1. The service rejects a duplicate active root registration for the same child/course.
1. If SessionCount is set, total cost is SessionCost × SessionCount. If SessionCount is not set, Credit Tracking must be enabled and the payment model is forced to Token with no upfront total.

### 4.2 Insert sequence

| Order | Table | Inserted values / meaning |
| --- | --- | --- |
| 1 | course_enrollments | Root registration: ChildID and CourseID set; EnrollmentID_Ref NULL; ScheduledAt NULL; ScheduledHours from form; Status Registered; audit fields set. |
| 2 | fees | CourseEnrollmentID = new root EnrollmentID; PaymentModel; calculated TotalCost or NULL; description; IsPaid = true when total is NULL/0, otherwise false; audit fields. |

No scheduled session row is inserted during private registration. The root row is the durable registration/contract record, and the fee points to it.

### 4.3 Example records

| Table | ID | Important columns |
| --- | --- | --- |
| course_enrollments | EnrollmentID 410 | ChildID 25; CourseID 8; EnrollmentID_Ref NULL; ScheduledAt NULL; Status Registered |
| fees | FeeID 93 | CourseEnrollmentID 410; PaymentModel Token; TotalCost 360.00; IsPaid false |

### 4.4 Confirmation

- The child confirms the root enrollment. For an unpaid Token fee with a fixed TotalCost, ClassLift inserts a negative child_balance ledger entry for the course.
- The root course_enrollments.Status changes from Registered to Confirmed.
- The fee is marked paid when token deduction applies.
- For an unlimited/token private course where TotalCost is NULL, confirmation does not deduct the package price; the fee was created as paid and per-session charging is handled later.
> **Atomicity concern**
> Balance deduction, enrollment confirmation, and fee update are separate SaveChanges calls. If a later step fails, the records can disagree until repaired.

## 5. Private course scheduling

### 5.1 Coach-created session rows

A coach schedules against a selected private root registration. The controller verifies course ownership, recurrence limits, and the configured session-count ceiling, then converts local time to UTC.

| Order | Validation / insert | Database effect |
| --- | --- | --- |
| 1 | Validate root | Root must match ChildID and CourseID, have EnrollmentID_Ref NULL, and be Registered or Confirmed. |
| 2 | Validate recurrence | Maximum 365 occurrences; fixed SessionCount cannot be exceeded by existing plus requested sessions. |
| 3 | Insert each occurrence | course_enrollments child session with ChildID, CourseID, UTC ScheduledAt, local/time-zone fields, hours, location, Status Scheduled, and EnrollmentID_Ref = root EnrollmentID. |

### 5.2 Example after scheduling

| EnrollmentID | Row type | ChildID | ScheduledAt | EnrollmentID_Ref | Status |
| --- | --- | --- | --- | --- | --- |
| 410 | Private root | 25 | NULL | NULL | Confirmed |
| 511 | Private session | 25 | 2026-09-07 22:00 UTC | 410 | Scheduled |
| 512 | Private session | 25 | 2026-09-14 22:00 UTC | 410 | Scheduled |

> **Why the reference matters**
> All private sessions created for registration 410 can be counted, listed, and limited with EnrollmentID_Ref = 410. A later registration period can have a different root, keeping its sessions separate.

### 5.3 Downstream financial records

The model supports session-level money effects through child_balance.EnrollmentID and coach_income.EnrollmentID. Those records are not inserted by the scheduling method itself; they belong to later completion/payment processing.

## 6. Group-course session creation

### 6.1 Master-first fan-out

Staff creates an occurrence for a Group course. ClassLift first inserts one master session, then copies it to each active child registration.

| Order | Table / row | Database effect |
| --- | --- | --- |
| 1 | course_enrollments: master | ChildID NULL; EnrollmentID_Ref NULL; scheduled UTC/local/time-zone values; hours; location; StaffNote; Status Open. |
| 2 | Find roots | Load Registered and Confirmed root rows for the course; choose one active registration per child, preferring Confirmed. |
| 3 | course_enrollments: child copies | One per child; schedule/location/note copied; EnrollmentID_Ref = master ID; Status Scheduled if root Confirmed, otherwise Registered. |

### 6.2 Example fan-out

| EnrollmentID | Logical row | ChildID | EnrollmentID_Ref | Status |
| --- | --- | --- | --- | --- |
| 700 | Group master session | NULL | NULL | Open |
| 701 | Child A session copy | 25 | 700 | Scheduled |
| 702 | Child B session copy | 31 | 700 | Registered |

> **Status difference**
> Child A already confirmed the course, so the new copy is immediately Scheduled. Child B has only registered, so the new copy waits in Registered for confirmation.

### 6.3 Recurring sessions

For recurrence, the controller repeats the master-first fan-out for every occurrence. The course SessionCount is checked against Open + Completed master sessions before creation. There is no transaction covering the whole series or each fan-out.

## 7. Group course registration

### 7.1 Insert sequence for a new child

| Order | Table / query | Database effect |
| --- | --- | --- |
| 1 | Query open masters | Load group master sessions whose status is Open. |
| 2 | Capacity check | Count distinct children with active root registrations; enforce MaxCapacity. |
| 3 | course_enrollments: root | Insert ChildID/CourseID, EnrollmentID_Ref NULL, ScheduledAt NULL, Status Registered. |
| 4 | fees | Insert fee linked to the new root. TotalCost = SessionCost × number of currently open master sessions. |
| 5 | course_enrollments: copies | For every open master, insert one child session copy with EnrollmentID_Ref = master ID and Status Registered. |
| 6 | Course availability | If active root count reaches MaxCapacity, Course.IsActive may be set false. |

### 7.2 Confirmation

- The parent must confirm before the first Group session date begins, based on that session's local time zone. At midnight (00:00) at the start of that date, an unconfirmed root registration and all non-terminal child-session copies become eligible for automatic cancellation; Completed and Deleted history is preserved.
- Find the fee through the child/course root registration.
- For unpaid Token pricing, insert a negative child_balance entry equal to the fee total.
- Change the root registration status to Confirmed and mark the fee paid when applicable.
- For submitted child session rows still in Registered, change status to Scheduled.

### 7.3 Group example

| Table / ID | Meaning | Link |
| --- | --- | --- |
| course_enrollments 820 | Child root registration | ChildID 44; CourseID 12; Ref NULL |
| fees 188 | Course registration fee | CourseEnrollmentID 820 |
| course_enrollments 821 | Child copy of first open session | EnrollmentID_Ref 700 |
| course_enrollments 822 | Child copy of second open session | EnrollmentID_Ref 710 |
| child_balance 302 | Token debit at confirmation | ChildID 44; CourseID 12; negative BalanceChange |

> **Subtle but important**
> The group child copies do not reference root registration 820. They reference their master sessions (700 and 710). Group counts therefore use ChildID + CourseID + non-null EnrollmentID_Ref, while private counts use EnrollmentID_Ref = root ID.

## 8. Private vs. group: exact comparison

| Question | Private course | Group course |
| --- | --- | --- |
| What is registered? | A child-to-course root row | A child-to-course root row |
| Who creates sessions? | Coach schedules for one child/root | Staff creates master sessions |
| Is there a master session? | No | Yes: ChildID NULL, Ref NULL |
| Meaning of child session Ref | Root registration ID | Master session ID |
| Sessions inserted at registration | None | Copies of all currently Open masters |
| New sessions after confirmation | Coach adds sessions linked to root | New master fans out Scheduled copies |
| Fee basis | SessionCost × configured SessionCount, or unlimited Token | SessionCost × open master count at registration |
| Confirmation effect | Confirms root; optional token debit | Confirms root; optional token debit; Registered copies become Scheduled |

### 8.1 Read/query rules

- List active registrations from root rows: EnrollmentID_Ref NULL and status Registered or Confirmed.
- Count private sessions with EnrollmentID_Ref = private root EnrollmentID.
- Count group child sessions with matching ChildID and CourseID and EnrollmentID_Ref not NULL.
- Find group masters with ChildID NULL, EnrollmentID_Ref NULL, schedule present, and master statuses such as Open/Closed.
- Retrieve a registration fee from the root enrollment, not a child session copy.

## 9. Financial data flow

| Record | Created when | Primary link |
| --- | --- | --- |
| fees | Course or activity registration establishes an amount/payment model | CourseEnrollmentID points to root registration |
| payments | A payment/credit purchase or fee payment is recorded | ChildID plus optional FeeID and PaymentPackageID |
| child_balance | Credits or debits change the running child balance | ChildID; optional PaymentID, CourseID, ActivityID, EnrollmentID |
| coach_income | Coach earnings are posted | CoachID, CourseID, and session EnrollmentID |

```text
Registration                         Confirmation / completion
root enrollment ──> fee             fee ──> optional token debit ──> child_balance
scheduled child session ───────────────────────────────────────────> child_balance
scheduled child session ───────────────────────────────────────────> coach_income
```

> **Ledger interpretation**
> child_balance is an append-style running ledger: each row stores BalanceChange and the resulting Balance. Ordering and concurrency control matter because the next row reads the latest balance before calculating the new balance.

### 9.1 Payment-model behavior

- A fee with TotalCost NULL or 0 is created as paid. This is how unlimited/token private registration avoids an upfront course debit.
- A fixed unpaid Token fee is debited at confirmation and then marked paid.
- Payments and child-balance entries are related but distinct: a payment can add credit; a course confirmation or completed session can consume it.

## 10. Integrity, security, and scaling observations

| Priority | Observation | Recommended control |
| --- | --- | --- |
| High | Registration and fan-out use several SaveChanges calls without a shared transaction. | Wrap root + fee + copies, confirmation financial updates, and each master fan-out in explicit transactions; make retry behavior idempotent. |
| High | EnrollmentID_Ref has business-critical semantics but no explicit self-navigation/FK in the model. | Add a validated self-FK if compatible, or separate registration/master references into explicit nullable columns with constraints. |
| High | Balance is calculated from the latest ledger row before insert. Concurrent debits can race. | Use a transaction with row locking/optimistic concurrency, or a dedicated balance aggregate plus immutable ledger. |
| Medium | Statuses and types are strings in core workflows. | Centralize constants/enums and enforce allowed values/transitions, ideally with database constraints. |
| Medium | Recurring creation can partially succeed. | Treat a recurrence request as a batch with a transaction or return a durable batch result and safe retry key. |
| Medium | One table carries four logical row types. | Add indexes and constraints keyed to common predicates; document row-type rules in code and tests. |
| Low | AppDbContext exposes a likely erroneous CoacheIncomes DbSet<Coach> alongside CoachIncomes. | Remove or rename after checking migrations and runtime usage. |

### 10.1 Suggested indexes to verify

- course_enrollments: (CourseID, ChildID, Status, EnrollmentID_Ref)
- course_enrollments: (EnrollmentID_Ref, Status)
- course_enrollments: (CourseID, Status, ScheduledAt)
- fees: CourseEnrollmentID; child_balance: (ChildID, CreatedDate); coach_income: (CoachID, CreatedDate)
These are review targets, not a claim that every index is currently absent. Confirm against the live database and migration snapshot before changing production.

## 11. Operational test checklist

### Private registration and scheduling

- Register a fixed-session private course: verify exactly one root row and one fee; verify no session rows.
- Confirm with sufficient Token balance: verify one debit, root Confirmed, fee paid.
- Attempt confirmation with insufficient balance: verify no status or fee changes.
- Schedule one and recurring sessions: verify UTC/local/time-zone values and every Ref equals the root ID.
- Try to exceed SessionCount and create a duplicate active registration.

### Group session creation and registration

- Create a master with two active children: verify one master plus two child copies and status selection by root confirmation.
- Register a child after two masters are Open: verify one root, one fee, and exactly two child copies referencing the master IDs.
- Confirm the registration: verify root Confirmed, relevant copies Scheduled, token debit once, fee paid.
- Fill MaxCapacity: verify duplicate prevention, distinct-child counting, and Course.IsActive behavior.
- Force a failure during fan-out in a test environment and check whether partial rows remain; use this to validate the transaction fix.

### Tenant isolation

- Resolve two hostnames to different DatabaseName values and prove that the same query returns only each tenant’s data.
- Verify tenant resolution runs before authentication/authorization and no connection-string secret is logged.
- Back up and restore one tenant independently; confirm platform registry and tenant database remain consistent.

## 12. Data dictionary: key fields

| Table | Field | Meaning |
| --- | --- | --- |
| tenantregistry | DatabaseName | Operational database selected for the resolved tenant |
| courses | CourseType | Group or Private; drives enrollment reference semantics |
| courses | SessionCount / SessionCost | Fixed quantity and per-session pricing inputs |
| course_enrollments | EnrollmentID | Identity key for any logical row type |
| course_enrollments | EnrollmentID_Ref | Private: root registration; Group child copy: master session |
| course_enrollments | ChildID | NULL identifies a group master; set for registrations/copies/private sessions |
| course_enrollments | ScheduledAt | UTC scheduled instant; NULL on a root registration |
| course_enrollments | ScheduledLocalTime / ScheduledTimeZoneId | Original local scheduling context |
| course_enrollments | Status | Lifecycle state interpreted with row type/course type |
| fees | CourseEnrollmentID | Root course registration being charged |
| fees | PaymentModel / TotalCost / IsPaid | How, how much, and whether the registration fee is settled |
| child_balance | BalanceChange / Balance | Ledger delta and resulting running balance |
| coach_income | EnrollmentID | Scheduled/completed session that generated earnings |

### Source boundary

This guide was derived from the current repository implementation, especially AppDbContext, BillingDbContext, CourseEnrollmentService/Repository, FeeService/Repository, ChildBalanceRepository, ChildController, CoachController, CourseController, entity models, migrations, and tenant connection-string documentation. Live production schema and data should be checked before migrations or cleanup.

> **Recommended next artifact**
> After the Growth Plan test is stable, create a migration-backed physical schema appendix: actual column types, nullability, indexes, foreign keys, row counts, and an ERD generated from a sanitized production-equivalent database.
