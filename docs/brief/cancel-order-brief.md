# Project brief: self-service order cancellation

**Status:** draft, produced from the material in `docs/inputs/`
**Owner:** Delivery
**Date:** written live in Session 1

## The problem

Customer service handles roughly 200 cancellation requests a day by hand. Each one is a
phone call, a lookup, a refund and a note. Operations describe it as their largest
avoidable workload.

## What we think is wanted

Customers can cancel their own order, without contacting anyone, when the order has not
yet left the warehouse and is within the published cancellation window.

## What the inputs actually agree on

- Cancellation must not be possible once an order has shipped. Operations are clear that
  a shipped order is a return, which is a different process entirely.
- The refund goes through the existing payment service. Finance are emphatic: a batch
  job called the provider directly two years ago and double-refunded about forty
  customers.
- Every cancellation must be attributable. Operations need to answer "who cancelled
  this, and when" when a customer disputes it.
- Customers must be told the cancellation worked. Today they hear nothing until an
  agent emails them.

## Where the inputs disagree

**The cancellation window.** Finance and Operations say fourteen days from the order
being placed. Customer Service say the rule customers actually want is "any time before
it ships", and that nobody has ever complained about a fourteen day limit.

These are different rules and they produce different behaviour for a real order. The
stand-up parked it with an action to go and read what is actually published. That action
has not been closed.

**This is a decision, not a detail.** It is recorded as an open question below rather
than resolved here.

## Explicitly not in this piece of work

- Partial cancellation of individual lines. Raised in stand-up and parked. Handled
  manually today.
- Subscriptions. Confirmed as not present in this system.
- Agent-initiated cancellation on a customer's behalf. Raised in stand-up; Operations
  were to come back and did not.

## Open questions

1. **[DECISION NEEDED] Which rule governs the window** - fourteen days from placement, or
   any time before dispatch? Owner: Operations, with the published policy in hand.
2. **[DECISION NEEDED] Refund timing** - synchronous in the request, or queued? Finance
   have no preference but note it changes what the confirmation screen must say. Queued
   may take up to three working days.
3. **[DECISION NEEDED] Notification channel** - email only, or email and SMS? Customer
   Service consider email the minimum. SMS cost is unknown and nobody owns it.

## Assumption made in order to proceed

The specification below is written against the fourteen day rule, because it is the
published policy as understood by two of the three sources. If question 1 resolves the
other way, the acceptance criteria change and this brief must be revised.
