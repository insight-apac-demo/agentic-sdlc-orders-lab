# Brief: Self-service cancellation of whole orders

## Questions for you

Answer these in this file. This block is the only part of it edited by hand.

- **Is the cancellation window fourteen days from order placement, or any time before dispatch?** Operations and Finance say the policy is fourteen days from placement; Customer Service says customers should be able to cancel until the order ships. The stand-up parked the disagreement pending a check of the published policy. Owner: Operations.
  *Written on: no eligibility rule selected; this brief does not resolve the conflict.*
  - Answer:

- **Does the refund happen immediately, or is it queued for later processing?** Finance accepts either approach, but says a queued refund may take up to three working days and the confirmation screen must reflect the decision. Owner: Finance.
  *Written on: refund timing is unresolved; this brief does not assume immediate or queued processing.*
  - Answer: Fourteen days

- **Can an agent cancel on a customer's behalf, or only the customer?** The question was raised in the stand-up and Operations said they would come back to it. Owner: Operations.
  *Written on: actor authority is unresolved; this brief does not assume customer-only or agent-assisted cancellation.*
  - Answer: customer only

- **Is SMS notification part of this work, or is email the only notification channel?** Customer Service calls email the minimum and says some staff want text messages, but the cost and decision owner are unknown. Owner: Sponsor and Customer Service.
  *Written on: email is the only supported notification identified by the inputs; SMS scope remains unresolved.*
  - Answer: email only

## The problem

Customers currently request cancellations by phone. Operations describes roughly 200
requests per day, each requiring a lookup, a refund form, and a system note. The sponsor
wants customers to cancel their own orders this quarter so Customer Service is no longer
handling this volume manually. The existing operations screen has a Cancel button, but it
is disabled.

The change must account for the order's shipping state, refund handling, customer
confirmation, and an auditable record of who cancelled what and when. The inputs do not
agree on the business rule that determines whether an order is eligible for cancellation.

## What the inputs agree on

- The requested capability is customer self-service cancellation of an order, driven by
  the current manual workload.
- An order that has already shipped cannot be treated as a cancellation. Operations says
  it becomes a return instead.
- Any refund must use the existing payment service or payment integration. Finance
  explicitly rejects calling the payment provider directly.
- A cancellation needs a record identifying the actor, the order, the time, and the
  reason. Operations needs this to answer disputes, and the schema provides append-only
  audit entries with those fields.
- Customers need confirmation that the cancellation worked. Email is the minimum channel
  identified by Customer Service.
- The first scope is whole-order cancellation. Partial-order cancellation was parked in
  the stand-up.
- Subscriptions are not present in this system and were excluded in the stand-up.
- The current database represents placed, shipped, delivered, and cancelled order states,
  stores placement and shipment times, and has an append-only audit table.

## Where the inputs disagree

- **Eligibility window:** Operations says, "There is a fourteen day thing" and that an
  order cannot be cancelled after it has left the warehouse. Finance states, "That is
  from the order being placed, not from dispatch." Customer Service says, "if it has not
  shipped, you can cancel it." The stand-up records that this must be checked against the
  published policy before building. Owner of the decision: Operations.
- **Refund timing:** Finance says the refund may happen immediately or go in a queue, but
  says this must be decided because a queued refund can take up to three working days and
  changes the customer confirmation. No input selects one. Owner of the decision: Finance.
- **Who may cancel:** The stand-up asks whether only the customer or also an agent acting
  for the customer may cancel. Operations says they will return with an answer. No input
  resolves it. Owner of the decision: Operations.
- **Notification channel:** Customer Service identifies email as the minimum and reports a
  request for text messages, but says SMS is not their call and its cost is unknown. No
  input establishes whether SMS is included. Owners of the decision: Sponsor and Customer
  Service.

## Explicitly out of scope

- Partial-order or individual-line cancellation. The stand-up says to do the whole-order
  case first.
- Subscription cancellation. The stand-up says subscriptions are not in this system and
  excludes them.
- Treating an order that has shipped as a cancellation. Operations identifies that as a
  separate return process.

## Sources read

Read 7 files in `docs/inputs/`: the inputs README, the sponsor ask, three stakeholder
emails, the stand-up transcript, and the schema export. Also read the related ticket
`TICKET-101` last and the brief template in `docs/brief/README.md`.

Distinct raw input sources: 6 (excluding the inputs README). Conflicts found: 4.
Open questions found: 4. The conflicts and open questions are listed above; none has
been silently resolved.