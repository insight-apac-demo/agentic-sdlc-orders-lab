# TICKET-104 - Loyalty points on completed orders

**Reported by:** Marketing
**Priority:** Low

One point per whole dollar of order value, awarded when an order reaches Delivered.

Acceptance:

- A `LoyaltyAward` row is written when an order moves to Delivered
- Points are one per whole dollar of the order total, rounded to the house convention
  before the points are calculated
- Awarding twice for the same order is refused
- An audit row is written

**Note:** this depends on how an order total is rounded. So does TICKET-103.
