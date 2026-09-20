# Backlog

Ordinary tickets, written the way tickets actually arrive: some clear, some not.

| Ticket | Title | Used in |
|---|---|---|
| TICKET-101 | Let a customer cancel their own order | Sessions 1, 2, 3 |
| TICKET-102 | Search orders by customer reference | Session 4 |
| TICKET-103 | Partial refund for a damaged line | Session 6 (fan-out) |
| TICKET-104 | Loyalty points on completed orders | Session 6 (fan-out) |
| TICKET-105 | Export order history as CSV | Session 6 (fan-out), Session 4 lab |
| TICKET-106 | Bulk-cancel stale unshipped orders | Session 6 (headless) |
| TICKET-107 | Shipping estimate on the ops screen | Spare |

TICKET-103, TICKET-104 and TICKET-105 are the three used for the parallel-agent
exercise in Session 6A. They touch different files on purpose. Two of them also touch
the same unowned convention, which is the point of the exercise.
