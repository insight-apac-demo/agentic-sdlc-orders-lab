# Orders Service

@AGENTS.md

## Claude Code specifics

- Permissions, hooks and model defaults live in `.claude/settings.json`. Do not put
  personal preferences there; use `.claude/settings.local.json`, which is git-ignored.
- Prefer plan mode for anything touching `Services/` or `Data/`.
- When asked to review, report findings to a file rather than only to the transcript.
  A summary is lossy and this repository is used for auditable exercises.
