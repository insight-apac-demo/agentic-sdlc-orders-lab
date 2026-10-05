# A worked agent and skill (reference set)

A complete, working pair for [Lab 1](../01-build-an-agent.md). Use it when you want to
see the shape before you write your own, or copy it and adapt it if you are short of time.

Both files below were run on 5 October 2026. The agent was discovered as
**Release Note Writer**, the skill as a project skill named `release-note-from-diff`, and
the agent produced a usable release note from `main...reference/realisation` on the first
attempt. They are not illustrative - they work.

---

## The agent

`.github/agents/release-note-writer.agent.md`

```markdown
---
name: Release Note Writer
description: Turns a merged diff into a release note in our house format. Use after a change is merged and before it ships. Never edits code.
argument-hint: point me at a branch or a diff range
---

You write release notes from diffs. You do not write code, and you do not edit any file
other than the release note itself.

## What to read, in order

1. The diff in full - `git diff main...HEAD`.
2. The ticket the change claims to implement, in `tickets/`.
3. `docs/spec/<slug>.md` if one exists.

## What you produce

- One paragraph, plain English, for somebody who does not read code.
- A bullet per user-visible change. Nothing about refactoring that changed no behaviour.
- A separate "Operational notes" section for anything that changes how the service is
  run or configured.

## Never

- Never describe a change you cannot point at in the diff.
- Never use the words "various", "several" or "improved" without saying what.
- Never include a change that is only a test change.

## Finish with

The one thing in this change most likely to surprise somebody on support.
```

### Agent frontmatter

| Field | Required | What it does |
|---|---|---|
| `name` | yes | The display name. Appears in the agent picker, and it is what you type after `--agent`. May contain spaces and capitals |
| `description` | yes | How the model decides this agent is the right one |
| `argument-hint` | no | Placeholder text when the agent is invoked |
| `tools` | no | The tools it may use. An allowlist. Omit to inherit everything |
| `agents` | no | Which subagents it may delegate to. `[]` stops it delegating |
| `handoffs` | no | Buttons that pass the work to another agent |
| `disable-model-invocation` | no | Stops other agents calling this one as a subagent |

The file name is lower case and hyphenated and ends in `.agent.md`. Unlike a skill, it does
**not** have to match the `name` field.

---

## The skill

`.github/skills/release-note-from-diff/SKILL.md`

```markdown
---
name: release-note-from-diff
description: Write a release note from a merged diff in the house format. Use when a change has been merged and somebody needs to describe it to people who do not read code.
argument-hint: a branch name or diff range
---

# Release note from a diff

## Goal

A release note a support engineer can read, that describes every user-visible change in
this diff and nothing else.

## Steps

1. Run `git diff main...HEAD --stat` and read the file list before any file content.
2. Read the full diff. Mark each changed file as user-visible, internal, or test-only.
3. Read the ticket in `tickets/` that the change claims to implement.
4. Write one plain-English paragraph covering the user-visible changes as a group.
5. Write one bullet per user-visible change, each naming the behaviour that changed.
6. List anything that changes how the service is configured or operated separately.
7. Re-read the diff and check every bullet against it. Delete any you cannot point at.

## Constraints

- Internal refactoring that changed no behaviour does not appear at all.
- Test-only changes do not appear.
- No change is described in words the ticket does not support.

## Quality criteria

- Every bullet can be traced to a specific hunk in the diff.
- A support engineer could answer "what changed?" from it without opening the code.
- Nothing in it is hedged. If you are unsure whether a change is user-visible, say so
  explicitly rather than describing it vaguely.
```

### Skill frontmatter

| Field | Required | What it does |
|---|---|---|
| `name` | yes | **Must match the folder name.** Lower case letters, numbers and hyphens only |
| `description` | yes | The only thing the agent sees when choosing a skill |
| `argument-hint` | no | Placeholder text for the slash command |
| `user-invocable` | no | Whether it appears in the `/` menu. Defaults to true |
| `disable-model-invocation` | no | Stops the model loading it automatically |
| `context` | no | Set to `fork` to run it in its own subagent |

A skill is a folder, so it can carry whatever the procedure needs. Reference these from
`SKILL.md` and they load only when they are reached:

```
.github/skills/release-note-from-diff/
├── SKILL.md
├── template.md
└── examples/
    └── good-release-note.md
```

---

## What to notice before you copy it

**The prohibitions are the design.** Three of them, stated plainly, under a heading that
says Never. A general-purpose prompt will happily do a job adjacent to the one you asked
for and you will never see it happen, because the output looks the same either way. What
makes this an agent rather than a saved prompt is the list of things it must not do.

**The description is a trigger, not a topic.** Compare "Release notes" with "Write a
release note from a merged diff in the house format. Use when a change has been merged and
somebody needs to describe it to people who do not read code." The agent sees only the
name and the description when it decides whether to load a skill. A topic fires
unpredictably; a trigger does not.

**The agent and the skill are not the same thing said twice.** The agent is a standing
role - who it is, what it may touch, what it must never do. The skill is a procedure with
numbered steps and quality criteria. If you find yourself writing the same content into
both, you need one of them and not the other, and that is a legitimate answer.

**It is not perfect, and that is useful.** Run it twice on the same input and compare the
"Operational notes" section. Across six runs it produced four different answers - audit
entries, configuration, a new constructor dependency, and "none" - while the closing line
about what would surprise support came out the same every time. Every version was written
with the same confidence.

The split is not random. The instructions are specific about the paragraph, the bullets
and the closing line, and those hold. The only thing they say about operational notes is
"anything that changes how the service is run or configured", and that is the section that
moves. Vague instruction, unstable output - which is the thing lab 1 asks you to fix.

Two runs will not always disagree. Twice in those six they matched, so if yours agree,
take a third sample rather than concluding the agent is deterministic.
