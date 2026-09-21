# Briefs

One Markdown file per piece of work, written before the specification, from the raw
material in `docs/inputs/`.

A brief is not a specification. It records what the inputs say, where they disagree, and
what a human still has to decide. It contains no acceptance criteria.

The shape we use:

```
# Brief: <one line>

## Questions for you        <- first, and the only part a person edits by hand
## The problem
## What the inputs agree on
## Where the inputs disagree
## Explicitly out of scope
## Sources read
```

## Questions for you

This block goes **first**, before anything else, and it is the one part of the file a
person edits by hand. It exists because the agent that wrote this cannot ask anybody: it
runs to completion and hands back a file.

Each entry is one question in words somebody can answer without reading the rest of the
brief, the context in one sentence, the assumption the rest of the brief was written on,
and a blank for the answer:

```markdown
## Questions for you

Answer these in this file. This block is the only part of it edited by hand.

- **Is the cancellation window fourteen days from placement, or any time before
  dispatch?** Operations and Finance say fourteen days; Customer Service say before
  dispatch. Owner: Operations. *Written on: fourteen days.*
  - Answer:

- **Can an agent cancel on a customer's behalf, or only the customer?** Raised in the
  stand-up on the Wednesday and never answered. Owner: Customer Service.
  *Written on: customer only.*
  - Answer:
```

An answer goes on the `- Answer:` line, in the file. Once it is there the question is
settled: the `spec-writer` writes the criteria on it and records it under **Decisions
taken**, rather than carrying it forward as though nobody had decided. Nothing else in the brief is
hand-edited: if an answer contradicts what the brief says, the analyst is run again rather
than the prose patched, so the brief always matches its sources.

**If there is nothing to confirm, the block says so.** Do not delete it - an absent block
and an empty one look the same to a reader, and only one of them means "nothing to
decide".

## Why the answers gate the specification

The `spec-writer` reads this block before it writes anything, and **will not finish a
specification while an `- Answer:` is blank**. It writes what it can, marks every
criterion that depends on an unanswered question `[ASSUMED - see Q<n>]`, and says which
questions are holding it up.

That is deliberate. An acceptance criterion is what tests get written from, so an
assumption buried in one is not a footnote - it is the behaviour the system will have.
The alternative, writing the whole specification against one side of an open conflict and
noting the conflict at the bottom, produces a document that looks finished and is not.

`cancel-order-brief.md` is written live in Session 1. The worked version is on the
`reference/definition` branch.
