# Orders Service

@AGENTS.md

---

## Claude Code specifics

Everything that decides behaviour is in `AGENTS.md`. This file holds only what is specific
to running Claude Code against this repository.

**Permissions live in `.claude/settings.json`** and are committed, because they are a team
baseline rather than a preference. Personal conveniences go in `.claude/settings.local.json`,
which is git-ignored - keep it that way. The deny rules on credential paths are the control
that survives a mode change; the allow rules are only there to stop `dotnet test` prompting.

**Use plan mode for anything touching `Services/` or `Data/`.** Those are where the
invariants in §2 live, and the plan is the cheapest place to catch a misreading of one - by
roughly two orders of magnitude over catching it in review.

**When asked to review, write findings to a file rather than only to the transcript.** The
return trip from a subagent is a summary, and a summary is lossy in exactly the way that
matters: it keeps the conclusion and drops the reasoning. This repository is used for
exercises that are meant to be auditable afterwards.

**Subagents do not inherit your conversation, but they are not blind.** They load this file
and the `AGENTS.md` it imports. Brief one as though it has just walked in having read the
standards - not as though it has been listening.

## Agents

`.claude/agents/` holds five subagents mirroring the Copilot definitions in
`.github/agents/`. Definition: `analyst`, `spec-writer`, `spec-reviewer`. Realisation:
`planner`, `reviewer`.

`spec-reviewer` and `reviewer` are given no write tool on purpose: "reports only, never
edits" is enforced there rather than merely instructed.

The Copilot mirrors leave tools unset so they run on any tier. That difference is itself
worth noticing - the same five agents, one set with a gate and one set on its honour.

They carry `user-invocable: false` so they do not appear twice in the VS Code agent picker
alongside the Copilot copies. Claude Code still loads them.
