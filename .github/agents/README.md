# Definition-phase agents

Three agents that take raw project input to a reviewed specification. They are the
Session 1 Definition demo, and the worked example that Session 4 asks you to build your
own version of.

| Agent | Does | Never does |
|---|---|---|
| **Analyst** | Reads the inputs, writes a brief, names every conflict and open question | Chooses between conflicting sources; writes requirements |
| **Spec Writer** | Turns the brief into numbered, testable acceptance criteria | Resolves an open question to make the spec tidier |
| **Spec Reviewer** | Reports what an implementer would still have to guess | Edits the specification; resolves `[DECISION NEEDED]` items |

## Why three, and not one prompt

Each one has a different job, a different definition of done, and - importantly - a
different thing it is forbidden from doing. The Analyst is forbidden from resolving a
conflict. The Spec Writer is forbidden from closing an open question. The Reviewer is
forbidden from editing.

Those prohibitions are the whole point. A single general-purpose prompt will happily do
all three jobs at once, and in doing so will quietly resolve the disagreement it was
supposed to surface. Separating the roles is what stops that, and it is the same
separation-of-duties argument that Module 4B makes about review.

## Running them

In VS Code, pick the agent from the chat agent dropdown. Each one ends with a handoff
button that passes the work to the next, so the sequence runs as
Analyst to Spec Writer to Spec Reviewer, and the Reviewer hands back to the Spec Writer
to close gaps.

The same three exist as Claude Code subagents in `.claude/agents/`, with identical
bodies and different frontmatter. Session 3 maps the two formats to each other.

## Deliberately not set here

`tools`, `model` and hooks are all left unset. Scoping an agent's tools and pinning its
model are Session 4 and Session 6 material, and they are exactly what the Session 6
`security-reviewer` example demonstrates. Keeping them off here means these three run
anywhere, for anyone, on day one.

## Related

- The toolkit GitHub publishes for this is **Spec Kit** (`github/spec-kit`), which ships
  the same shape as a set of skills: an assessment workflow
  (`/speckit-assess-intake`, `-research`, `-define`, `-shape`, `-decide`) and a
  spec-driven one (`/speckit-constitution`, `/speckit-specify`, `/speckit-plan`,
  `/speckit-tasks`, `/speckit-implement`, `/speckit-converge`).
  Session 2 looks at it properly. These three agents are the hand-rolled version, so
  that the moving parts are visible rather than behind a CLI.
