# Project brief: self-service order cancellation

**Status:** draft, produced from the material in `docs/inputs/`
**Owner:** Delivery
**Date:** written live in Session 1

## Questions for you

Answer these in this file. **This block is the only part of it edited by hand.** Each
question is answerable without reading the rest of the brief.

- **Is the cancellation window fourteen days from placement, or any time before the
  order is dispatched?** Operations and Finance both say fourteen days from placement.
  Customer Service say the rule customers actually want is any time before it ships, and
  that nobody has ever complained about the fourteen day limit. The stand-up parked this
  with an action to read the published policy; that action was never closed.
  Owner: Operations, with the published policy in hand. *Written on: fourteen days.*
  - Answer:

- **Is the refund raised synchronously in the request, or queued?** Finance have no
  preference, but note it changes what the confirmation screen must say - a queued refund
  may take up to three working days. Owner: Finance. *Written on: queued.*
  - Answer:

- **Can an agent cancel on a customer's behalf, or only the customer?** Asked in the
  Wednesday stand-up and never answered; the recording ends on "let me come back to you
  on that one". Owner: Customer Service. *Written on: the customer only.*
  - Answer:

Nothing else in this file is hand-edited. If an answer contradicts what is written below,
the analyst is run again rather than the prose patched, so the brief always matches its
sources.

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

## Sources read

Six inputs in `docs/inputs/`: the sponsor's one-line ask, three emails (Operations,
Finance, Customer Service), a partial stand-up transcript where the recording started
late, and the orders schema.

**Three sources, three conflicts, three open questions.** The conflicts and the questions
are in the block at the top of this file, because that is where a person can answer them.

Notification channel - email only, or email and SMS - was raised by Customer Service and
is parked above under what is not in this piece of work, rather than asked as a question:
nothing downstream depends on it, and SMS cost is unowned.
