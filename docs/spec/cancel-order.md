# Spec: Let a customer cancel their own order

> **Not ready to build from.** Q1 in `docs/brief/cancel-order-brief.md` is unanswered, and the criteria marked `[ASSUMED]`
> below depend on it. See *Still blocked* at the end.

**Traces to:** TICKET-101, and the brief in `docs/brief/cancel-order-brief.md`

## Why

The customer-service team handles around 200 cancellation requests a day by hand. Each
is a call, a lookup, a refund and a note. Self-service removes the call.

## Goal

A customer can cancel their own order when it has not yet shipped and was placed less
than fourteen days ago, and the refund is queued automatically.

## Acceptance

1. `POST /api/orders/{id}/cancel` returns 200 and the updated order when the order is in
   Placed and was placed less than fourteen days ago.  **[ASSUMED - see Q1]**
2. Returns 409 when the order is not in Placed. Shipped, Delivered and already Cancelled
   orders are all refused.
3. Returns 409 when the order was placed fourteen days ago or more, measured in UTC.
   **[ASSUMED - see Q1]**
4. Returns 404 when no order with that id exists.
5. A refund for the full order total is queued through `IPaymentService`. The payment
   provider is never called directly.
6. An `AuditEntry` is written with the actor, the UTC timestamp and the supplied reason.
7. The order's status becomes Cancelled.
8. The Cancel button on the order detail page is enabled only for orders that satisfy
   criteria 1 to 3, and cancelling from the screen produces the same result as the API.
9. No customer name, email or phone appears in any log line produced by this path.

## Out of scope

- Partial cancellation of individual lines
- Subscription cancellation
- Agent-initiated cancellation on a customer's behalf
- The notification itself. Criterion 6 gives us the record; telling the customer is a
  separate piece of work once question 3 is answered.

## Open questions

Carried from the brief. The answers live there, not here.

1. **[DECISION NEEDED]** Fourteen days from placement, or any time before dispatch?
   Owner: Operations. **Criteria 1 and 3 assume fourteen days and change if this does.**
2. **[DECISION NEEDED]** Refund synchronous or queued? Owner: Finance. Criterion 5
   assumes queued.
3. **[DECISION NEEDED]** Email only, or email and SMS? Out of scope either way for now.

## Assumptions

Neither of these was asked in the brief; both had to be settled to write a criterion.

- The boundary is exact and exclusive: an order placed exactly fourteen days ago is
  refused, not allowed. Nothing in the inputs says which way the boundary falls.
- "Placed" means the order's own status, not the absence of a dispatch record.

## Still blocked

**Q1 is unanswered, and criteria 1 and 3 depend on it.** This specification is not ready
to build from. Answer Q1 in `docs/brief/cancel-order-brief.md` and run the spec-writer
again; the `[ASSUMED]` markers come off and the question moves to Decisions taken.

Q2 is unanswered but not blocking - criterion 5 is written against queued, and a
synchronous refund would change the confirmation wording rather than the criterion.

## Notes for whoever builds this

- Times are UTC and come from the injected `TimeProvider`. The boundary is exact: an
  order placed exactly fourteen days ago is refused.
- The refund amount is the order total, already rounded through `MoneyRounding`.
- ORD-4418 in the seed data sits a few hours inside the boundary. It is the fastest way
  to tell whether your comparison is correct.
