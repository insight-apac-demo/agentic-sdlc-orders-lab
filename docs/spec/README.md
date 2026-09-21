# Specifications

One Markdown file per piece of work, written before the code.

The shape we use:

```
# Spec: <one line>

Why:        the reason this exists, in business terms
Goal:       what is true when this is done
Acceptance: numbered, each one a sentence you could write a test for
Out of scope: the things this deliberately does not do
Open questions: [DECISION NEEDED] items, left open rather than guessed
Assumptions: anything you had to assume to write a criterion at all - as an
             assumption, not as a requirement
Decisions taken: questions from the brief that have been answered, with the answer
```

## `[ASSUMED]` markers

A criterion that depends on a question still open in the brief carries the marker on the
criterion itself, naming the question:

```
1. `POST /api/orders/{id}/cancel` returns 200 when the order is in Placed and was
   placed less than fourteen days ago.  [ASSUMED - see Q1]
```

Not only in the Open questions list at the bottom. A criterion is what a test gets written
from, so an assumption inside one is the behaviour the system will have - and an
implementer reads criterion 1 long before they reach line 40.

A specification with an `[ASSUMED]` marker in it is **not ready to build from**. The marker
comes off when the brief's question is answered and the spec-writer is run again.

`cancel-order.md` is written live in Session 1 and refined in Session 2. The worked
version is on the `reference/definition` branch.
