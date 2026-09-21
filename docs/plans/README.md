# Plans

One Markdown file per ticket, written after the specification is agreed and before any
code exists.

A plan is not a specification. The specification says what "done" means; the plan says
which files will change to get there, and what the author might have misunderstood.

The shape we use:

```
# Plan: <ticket> - <one line>

## Questions for the reviewer   <- first, the only part edited by hand
## What I understand this to mean
## How I will build it
## Criterion to file to test
## Deliberately not doing
```

## Why the plan is a file

A plan you can only see in a chat window cannot be reviewed, cannot be diffed, and does
not exist tomorrow. This one is committed alongside the change it describes, so the
question "was this built the way we agreed?" has an answer three months later.

That is the only real difference between an agent plan and the plan mode built into most
tools. Both make the agent think before it edits. Only one of them leaves something behind.

## Questions for the reviewer

Same shape as the brief's block, for the same reason: the planner runs to completion and
cannot ask anybody anything.

```markdown
## Questions for the reviewer

- **Should a cancelled order still appear in the default ops list?** The spec is silent
  and the current filter shows every status. Owner: Operations.
  *Planned on: it stays visible, with its new status.*
  - Answer:
```

The question in words somebody can answer without reading the rest, the context in one
sentence, the assumption the plan was written on, and a blank `- Answer:` line. Planning
continues underneath on the stated assumption; if an answer breaks it, the planner runs
again.

**The plan is never committed with a question open.** If there is nothing to confirm, the
block says so rather than being deleted.

## Criterion to file to test

One row per acceptance criterion. A criterion with no row is one nobody has planned for,
and it is reliably the one missing at the end.

| Criterion | Files | Test |
|---|---|---|
| 1. Returns 200 within the window | `Services/OrdersService.cs` | `Cancels_an_order_inside_the_window` |
| 6. Writes an `AuditEntry` | `Services/OrdersService.cs` | `Writes_an_audit_entry_on_cancel` |

## Length

A plan that takes longer to read than the change takes to make has failed. One page is
generous for a ticket this size. The uncertainty section earns whatever length it needs;
the file listing does not.

`cancel-order.md` is written in Session 3 and reviewed in the same session. Sessions 5 and
6 add the hooks and the gates that check a plan exists before an agent starts editing.
