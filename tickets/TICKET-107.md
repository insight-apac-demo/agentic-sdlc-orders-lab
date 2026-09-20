# TICKET-107 - Shipping estimate on the ops screen

**Reported by:** Operations
**Priority:** Low

Show an estimated dispatch date on the order detail page. Two business days from the
order being placed, skipping weekends.

Acceptance:

- The detail page shows "Estimated dispatch" for orders in Placed
- Two business days after the placed date, weekends excluded
- Nothing is shown for orders that have already shipped
- Public holidays are out of scope for now
