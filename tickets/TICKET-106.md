# TICKET-106 - Bulk-cancel stale unshipped orders

**Reported by:** Operations
**Priority:** Low

Orders that have sat in Placed for more than ninety days are never going to ship.
Operations want them closed off in a nightly job rather than by hand.

Acceptance:

- A command that can be run non-interactively and reports what it did
- Only orders in Placed, older than ninety days, are affected
- Each one writes an audit row with the actor recorded as the job, not a person
- The job reports a count and exits non-zero if any individual cancellation failed
- A dry-run mode that changes nothing and prints what it would have done

> Used in Session 6 as the headless, unattended run.
