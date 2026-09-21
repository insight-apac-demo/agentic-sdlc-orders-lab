# Agents

Five agents covering Definition and Realisation. The first three are the Session 1
Definition demo and the worked example Session 4 asks you to build your own version of;
the last two are Session 3.

**Definition** - raw project input to a reviewed specification.

| Agent | Produces | Forbidden from |
|---|---|---|
| **Analyst** | A brief, with every conflict and open question named, and the questions a human owes at the top | Resolving a conflict, or writing requirements |
| **Spec Writer** | Numbered, testable acceptance criteria and an out-of-scope list | Closing an open question, or finishing while one is unanswered |
| **Spec Reviewer** | A list of what an implementer would still have to guess | Editing anything, or deciding a `[DECISION NEEDED]` |

**Realisation** - an agreed specification to a change somebody can review.

| Agent | Produces | Forbidden from |
|---|---|---|
| **Planner** | `docs/plans/<slug>.md` - every file it will change, and why | Writing code, or planning against an unanswered question |
| **Reviewer** | A numbered list of findings against the criteria and the invariants | Fixing anything at all |

The Planner exists instead of the plan mode built into most tools for one reason: a plan
you can only see in a chat window cannot be reviewed, cannot be diffed, and does not exist
tomorrow. This one is a file committed beside the change it describes.

The Reviewer reads the diff first and the plan **last**. Read the plan first and you check
whether the code matches the plan, which is not the same question as whether the code is
right - a plan and an implementation can agree perfectly and both be wrong about the spec.

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
is what Session 5 teaches when it puts permissions and hooks around an agent.

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
