# TICKET-102 - Search orders by customer reference

**Reported by:** Customer service
**Priority:** Medium

The ops screen only filters by status. When a customer calls, the agent has their
customer reference (`C-1180` and so on) and no way to find their orders without
scrolling.

Add a search box that accepts a customer reference and filters the list to that
customer's orders. Partial matches are fine. An unknown reference should show an empty
list and say so, not an error.

Acceptance:

- `GET /api/orders?customerReference=C-1180` returns only that customer's orders
- The ops screen has a search box that does the same thing
- Searching for something that does not exist shows "No orders match", not a stack trace
- Existing status filtering still works, and the two can be combined
