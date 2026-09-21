# Definition-phase agents

Three agents that take raw project input to a reviewed specification. They are the Session 1
Definition demo, and the worked example that Session 4 asks you to build your own version of.

| Agent | Produces | Forbidden from |
|---|---|---|
| **Analyst** | A brief, with every conflict and open question named | Resolving a conflict, or writing requirements |
| **Spec Writer** | Numbered, testable acceptance criteria and an out-of-scope list | Closing an open question, or writing code |
| **Spec Reviewer** | A list of what an implementer would still have to guess | Editing anything, or deciding a `[DECISION NEEDED]` |

## Why three, and not one prompt

Each has a different job, a different definition of done, and - the part that matters - a
different thing it is **forbidden** from doing.

Those prohibitions are the whole design. A single general-purpose prompt will happily do all
three jobs in one turn, and in doing so it will quietly resolve the disagreement it was
supposed to surface. You will never see that happen. The output looks the same either way.

It is the same separation-of-duties argument Session 4 makes about code review: the author
cannot be the approver, not because authors are careless but because they cannot see what
they assumed.

## Running them

Pick the agent from the chat agent dropdown in VS Code. Each ends with a handoff button that
passes the work to the next, so the sequence runs Analyst → Spec Writer → Spec Reviewer, and
the Reviewer hands back to the Spec Writer to close gaps.

The same three exist as Claude Code subagents in `.claude/agents/`, with identical bodies.
Session 3 maps the two formats to each other.

## What is scoped, and what is only stated

The Claude Code definitions carry `tools`, and the Spec Reviewer has no write tool at all -
so "reports only, never edits" is enforced rather than requested. That is deliberate, and it
is what Session 6 teaches with its `security-reviewer` example.

The Copilot definitions deliberately leave `tools` unset, so they run on any tier without a
tool-name mismatch. **Every boundary in those three is an instruction, not a gate** - and
each file says so in its own words, because an agent that believes a rule is enforced when
it is not will behave differently from one that knows it is on its honour.

That gap between "instructed" and "enforced" is worth pointing at when you demo these. It is
the whole reason Session 5 spends a module on permissions and hooks before Session 6 goes
near autonomy.

## Related

GitHub publishes **Spec Kit** (`github/spec-kit`), which ships this shape as a proper
toolkit: an assessment workflow (`/speckit-assess-intake`, `-research`, `-define`, `-shape`,
`-decide`) and a spec-driven one (`/speckit-constitution`, `/speckit-specify`,
`/speckit-plan`, `/speckit-tasks`, `/speckit-implement`, `/speckit-converge`).

Session 2 looks at it properly. These three are the hand-rolled version, so the moving parts
are visible rather than behind a CLI - and so that "you could write these this afternoon"
is demonstrably true.
