# TICKET-103 - Partial refund for a damaged line

**Reported by:** Customer service
**Priority:** Medium

When one line of an order arrives damaged we currently refund the whole order or
nothing. We want to refund a single line.

Acceptance:

- `POST /api/orders/{id}/refund-line` takes a line id and a reason
- Refund amount is the line total, rounded to the house convention
- The refund is queued through `IPaymentService`
- An audit row is written with the actor, the reason and the amount
- The order stays in its current status; a partial refund is not a cancellation
- Refunding the same line twice is refused with 409

**Note:** this changes how a line total is calculated. So does TICKET-104.
