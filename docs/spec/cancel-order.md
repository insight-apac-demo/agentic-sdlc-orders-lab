# Spec: Customer self-service order cancellation

## Why

Customer Service handles roughly 200 cancellation requests a day by phone - a lookup, a
refund form and a system note for each one. The sponsor wants customers able to cancel
their own orders this quarter, and Operations needs a reliable record of who cancelled
what and when so it can answer disputes. TICKET-101 and the brief at
`docs/brief/cancel-order-brief.md` are the source material; this spec turns the answered
questions in that brief into behaviour.

## Goal

A customer can cancel their own whole order themselves, through the API, while it is
still within the cancellation window and has not shipped. The cancellation is recorded in
`AuditEntries`, the refund is requested through the existing `IPaymentService`, and the
customer receives an email confirmation. No agent-initiated cancellation, no partial
cancellation, no SMS.

**Not yet ready to build from as final.** Two items are marked `[DECISION NEEDED]` under
Open questions - refund timing (Q2, reopened) and refund/notification failure semantics -
and criterion 11 carries `[ASSUMED - see Q2]` until the first of those is answered.
Everything else below is settled.

## Acceptance

1. A new endpoint, `POST /api/orders/{id}/cancel`, accepts a request body carrying the
   calling customer's `CustomerReference` and an optional `Reason`.
2. Returns 404 when no order exists with the given `{id}`.
3. Returns 400 when `CustomerReference` is missing, empty, or does not correspond to any
   `Customer` record at all - distinct from a well-formed reference that simply is not
   the order's owner (criterion 4). (Assumption 1 - the brief does not say how the
   caller's identity is established.)
4. Returns 403 when `CustomerReference` is well-formed and belongs to a real customer,
   but that customer is not the one who placed the order. (Assumption 1.)
5. Returns 409 when the order's `Status` is anything other than `Placed` - covering
   `Shipped`, `Delivered` and already-`Cancelled` orders - with a response body that
   names which of those three states caused the refusal, and a message that does not
   imply the request will be retried. (Decision: eligibility window, part 1 - shipped
   orders are never cancellable.)
6. Returns 409 when the order is `Placed` but `PlacedUtc` is fourteen days old or more,
   measured against the injected `TimeProvider` in UTC (an order becomes ineligible at
   exactly fourteen days, not after), with a response body that names the eligibility
   window as the reason, distinct from the reason given in criterion 5. (Decision:
   eligibility window, part 2.) Distinguishing the two refusal reasons in the response
   is what lets Operations answer "why was I refused," the same dispute-resolution need
   the brief raises for "who cancelled what and when."
7. Returns 200 and cancels the order when it is `Placed`, less than fourteen days old,
   and the `CustomerReference` matches: `Status` is set to `Cancelled`.
8. A successful cancellation writes exactly one `AuditEntry` with `EntityType` "Order",
   `EntityId` the order's id, `Action` "Cancelled", `Actor` the calling customer's
   reference, `OccurredUtc` the current time from the injected `TimeProvider`, and
   `Reason` the caller-supplied reason, or the literal string `"Customer requested
   cancellation"` when none is supplied.
9. The order's `Status` change to `Cancelled` and the `AuditEntry` from criterion 8 are
   written in the same database transaction: either both are persisted or neither is.
   [DECISION NEEDED - see Open questions] whether the refund request (criterion 10) and
   the email confirmation (criterion 12) must succeed for the cancellation to commit, or
   are best-effort steps that do not roll it back if they fail.
10. A successful cancellation requests a refund of the order's full `TotalAmount` through
    `IPaymentService.QueueRefundAsync` - never a payment provider called directly - and
    the returned provider reference is recorded against the order's `AuditEntry` (or a
    field of equivalent durability), identified by the order or customer reference only,
    never by customer name, email or phone.
11. The success response states that the refund may take up to three working days.
    [ASSUMED - see Q2] This is written on the assumption that the refund is always
    queued through the existing `IPaymentService.QueueRefundAsync`; it is not yet a
    decision the brief has actually made (see Open questions, Q2).
12. A successful cancellation results in exactly one confirmation being sent to the
    customer's email address, through an injectable abstraction analogous to
    `IPaymentService` (for example `INotificationService`), and no notification is sent
    by any other channel. (Decision: notification scope, Q3.)
13. Nothing written to a log for this feature includes the customer's name, email address
    or phone number; log entries use the order reference and/or customer reference only,
    per `AGENTS.md` §5.
14. Cancelling one order never changes the status, lines or audit history of any other
    order, including other orders belonging to the same customer.

## Out of scope

- Cancelling an order that has already shipped - Operations treats that as a return,
  handled by a different process.
- Cancelling part of an order - the stand-up parked this; whole orders only.
- Subscription cancellation - Finance confirms subscriptions are not in this system.
- An agent or member of staff cancelling on a customer's behalf - settled by Q4 below.
- SMS or any notification channel other than email - settled by Q3 below.
- Any change to the disabled Cancel button's visual state on the ops screen; this spec
  covers the API only. (The ops screen itself is out of scope unless a separate ticket
  asks for it.)

## Open questions

- **[DECISION NEEDED] Q2 - refund timing, reopened.** The brief's answer line reads "as
  per finance," but Finance's own email is explicit that nobody has actually chosen yet:
  *"I do not mind whether the refund happens immediately or goes in the queue, but
  somebody needs to decide, because it changes what we tell the customer."* "As per
  finance" restates that deferral; it does not make the choice Finance asked someone to
  make. Treating it as settled would mean this specification, not the brief, decided
  between an immediate refund and a queued one - exactly the silent resolution
  `docs/spec/README.md` says not to do. Criterion 11 is marked `[ASSUMED - see Q2]` and
  carries the queued reading only so the spec is buildable while this is open; it is not
  a substitute for Finance actually deciding. Owner: Finance. **Until this is answered
  with an explicit choice (immediate vs. queued), the refund-timing behaviour in this
  specification is not ready to build from as final - only as a placeholder.**
- **[DECISION NEEDED] Do the refund request and the email confirmation have to succeed
  for the cancellation to commit, or are they best-effort?** Neither the brief nor the
  ticket considers what happens when `IPaymentService.QueueRefundAsync` or the
  notification call fails after the order's `Status` and `AuditEntry` have already been
  written. This is not a question the brief raised at all, so it cannot be answered here
  by inference from anything Finance, Operations or Customer Service said - see criterion
  9. Owner: not named in the inputs; needs assigning, most likely Operations (who own the
  audit trail) jointly with Finance (who own the refund).

## Assumptions

Things this spec had to assume that the brief did not raise at all - written as
assumptions, not requirements, and flagged back to the brief's authors:

1. **How the caller's identity is established.** The brief settles *who* may cancel
   ("only the customer," Q4) but not *how the system knows* the caller is that customer -
   it never raised the question. This spec assumes the request carries a
   `CustomerReference` supplied by the caller, exactly as the existing `ship` endpoint's
   `ShipRequest.Actor` is a caller-supplied string with no independent verification. If the
   service later gains real customer authentication, this endpoint should use it instead
   of a self-reported reference; until then, a customer able to guess another customer's
   reference is not stopped by this spec. This gap should be raised with the brief's
   owners rather than resolved quietly again.
2. **Refund amount.** This spec assumes the refund is for the order's full `TotalAmount`,
   consistent with whole-order cancellation being the only case in scope; the brief never
   separately states the refund amount because it never considers a partial case.
3. **Notification is new infrastructure.** No email- or notification-sending code exists
   anywhere in this codebase today. This spec assumes the implementer introduces an
   abstraction for it - named here as `INotificationService` so criterion 12 has
   something concrete to test against, mirroring how `IPaymentService` isolates the
   payment provider - rather than that one already exists to be reused. The exact
   interface shape is a planning decision, not this spec's to fix.
4. **Refund and notification failure do not roll back the cancellation.** Criterion 9
   fixes the order's `Status` change and its `AuditEntry` as one atomic unit, but nothing
   in the brief says whether a failed refund call or a failed email should undo that unit.
   This spec assumes they do not - the cancellation stands, and the refund or
   notification is retried out of band - because rolling back a cancellation the customer
   was already told succeeded would contradict criterion 7's 200 response. This is a
   genuine gap the brief never raised (see Open questions); it is written as an
   assumption here only so the spec is buildable, not as a decision on the reviewer's
   behalf.

## Decisions taken

Three of the four questions the brief left open have since been answered with an actual
decision. The fourth, Q2, is reopened above rather than treated as decided - see Open
questions.

- **Q1 - eligibility window.** *Answer: "fourteen days."* This resolves the disagreement
  between Finance's fourteen-days-from-placement rule and Customer Service's
  until-shipment rule in favour of the fourteen-day rule, counted from `PlacedUtc` (Marcus
  Webb, Finance: "That is from the order being placed, not from dispatch"). It sits
  alongside the separately-agreed fact that a shipped order can never be cancelled, so both
  conditions apply: not shipped, **and** less than fourteen days old. Criteria 5 and 6.
- **Q3 - notification scope.** *Answer: "email only."* Settles Customer Service's
  minimum-channel position and closes off the raised-but-undecided SMS option. Criterion
  12; SMS moved to Out of scope.
- **Q4 - who can cancel.** *Answer: "only the customer."* Settles the stand-up's parked
  question in favour of the customer alone; no agent-on-behalf-of path is built. Criteria
  3 and 4; agent-initiated cancellation moved to Out of scope.

## Traceability

- Criteria 1-4, 7, 14: brief's "What the inputs agree on" (self-service, whole-order,
  customer-reference identity per Assumption 1) and Q4.
- Criteria 5, 6: "Where the inputs disagree" (eligibility window) plus Q1's answer, and
  the agreed fact that shipped orders cannot be cancelled.
- Criteria 8, 9, 13: the agreed need for cancellation history ("who cancelled what and
  when") and `AGENTS.md` §2 and §5 invariants (audit entries, no customer detail in logs);
  criterion 9's atomicity requirement and its open failure-semantics question are new,
  raised by review rather than stated in the brief.
- Criteria 10, 11: the agreed payment-service invariant; criterion 11's refund-timing
  wording is `[ASSUMED - see Q2]`, not a settled decision - see Open questions.
- Criterion 12: "Customers need confirmation" plus Q3's answer.
- Out-of-scope list: "Explicitly out of scope" in the brief, plus Q3 and Q4's answers.
- Assumptions 1-4: not traceable to any brief passage; each is flagged above rather than
  answered silently, per `docs/spec/README.md`.
