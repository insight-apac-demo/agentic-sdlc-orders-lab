# Brief: Customer self-service order cancellation

## Questions for you

Answer these in this file. This block is the only part of it edited by hand.

- **Is the cancellation window fourteen days from order placement, or any time before dispatch?** Operations and Finance support fourteen days from placement; Customer Service supports cancellation until shipment, and the stand-up explicitly left the rule unresolved. Owner: Operations, with the published policy to be checked. *Written on: unresolved; this brief assumes neither rule.*
  - Answer: fouteen days

- **Does the refund happen immediately or enter a queue?** Finance permits either, but says a queued refund may take up to three working days and changes the confirmation message. Owner: Finance. *Written on: unresolved; this brief assumes neither timing.*
  - Answer: as per finance

- **Are customers notified by email only, or should SMS also be included?** Customer Service calls email the minimum and says SMS has unknown cost and no agreed owner. Owner: not named in the inputs; ownership needs assigning. *Written on: email is identified as the minimum, but SMS scope is unresolved.*
  - Answer: email only

- **Can an agent cancel on a customer's behalf, or only the customer?** The question was raised in the stand-up and Dani said she would return to it; no answer is recorded. Owner: not named in the inputs; ownership needs assigning. *Written on: unresolved; this brief assumes neither actor rule.*
  - Answer: only the customer

## The problem

Customer Service is handling about 200 cancellation requests per day by phone. Each request requires a lookup, a refund form, and a system note. The sponsor wants customers to cancel their own orders this quarter, and Operations describes the manual process as a burden on its team. The existing ticket says the operations screen already contains a disabled Cancel button.

The business needs a customer-facing cancellation process whose eligibility rule, refund timing, notification, and permitted actor are decided before implementation. Operations also needs an answer to disputes about who cancelled an order and when.

## What the inputs agree on

- The requested capability is self-service cancellation of whole orders by customers; the ticket and the input guide identify this work as TICKET-101.
- An order that has already shipped cannot be cancelled. Operations describes an order on a van as a return handled by a different process.
- The refund must use the existing payment service. Finance specifically warns against calling the payment provider directly after a previous double-refund incident.
- Operations needs cancellation history showing what was cancelled, who cancelled it, and when. The schema provides append-only `AuditEntries` with entity, action, actor, UTC occurrence time, and reason fields.
- Partial-order cancellation is not part of this work. The stand-up says to do the whole-order case first.
- Subscriptions are not in this system and are not part of this work.
- Customers need confirmation that cancellation worked. Customer Service identifies email as the minimum notification channel.

## Where the inputs disagree

- **Eligibility window:** Operations says, "There is a fourteen day thing," and Finance says, "That is from the order being placed, not from dispatch." TICKET-101 also states there is a fourteen-day rule. Customer Service says, "if it has not shipped, you can cancel it," and the stand-up records that the team parked the question until somebody checks what is published. The sources do not establish whether the rule is fourteen days from placement, any time before shipment, or how the two conditions interact.
- **Notification scope:** Customer Service says, "Email is the minimum," while also reporting that some staff think customers should receive a text and that the cost is unknown. SMS is raised but not decided.

## Explicitly out of scope

- Cancelling an order after it has shipped; Operations says this is a return process instead.
- Cancelling only part of an order; the stand-up parks this and directs the team to handle whole orders first.
- Subscription cancellation; Finance says subscriptions are not in this system.

The refund timing, SMS notification, and agent authority are not treated as out of scope because the inputs raise them without parking or deciding them. They remain questions for the owner.

## Sources read

- 7 files in `docs/inputs/`: the input guide plus the six raw-input files (`00-the-ask.md` through `05-schema.sql`).
- 1 ticket: `tickets/TICKET-101.md`, read after all input files as required.
- 8 distinct source files read in total for this brief.
- 2 conflicts recorded: cancellation eligibility and notification scope.
- 4 open decision questions recorded: eligibility, refund timing, notification scope, and agent authority.