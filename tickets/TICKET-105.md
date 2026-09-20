# TICKET-105 - Export order history as CSV

**Reported by:** Finance
**Priority:** Low

Finance want a CSV of orders for a date range so they can reconcile by hand.

Acceptance:

- `GET /api/orders/export?from=&to=` returns `text/csv`
- Columns: reference, customer reference, placed date (UTC, ISO-8601), status, total
- Dates in the filename and the rows are UTC
- An empty range returns a header row and no data rows, not a 404
- The export must not include customer name, email or phone
