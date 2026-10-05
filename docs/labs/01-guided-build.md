# Lab 1 - guided build: your first agent and skill

**16 minutes. Everybody builds.**

## Goal

Leave with one custom agent and one packaged skill, committed to your branch, and one
instruction you changed because of what you saw the agent actually do.

## Before you start

- You have done [Lab 0](00-prerequisites.md) and Copilot answers with real filenames.
- You are on your own branch (`git switch -c lab/<your-name>` if not).
- Have `.github/agents/reviewer.agent.md` open in a tab. It is the worked example.

## Time budget

| Step | Minutes |
|---|---|
| 1. Pick the job | 2 |
| 2. Write the agent | 4 |
| 3. Write the skill | 4 |
| 4. Run it on something real | 3 |
| 5. Fix one instruction | 2 |
| 6. Commit | 1 |

If you are behind at the ten minute mark, skip to step 4 with whatever you have. A rough
agent you have actually run beats a polished one you have not.

---

## Step 1 - pick one job your team genuinely does repeatedly

Two minutes. Pick something you do **weekly**, not something impressive.

The single most common failure in this lab is picking a job that is too big. "Refactor
our authentication layer" is not an agent. "Check a pull request description against our
template" is.

Good candidates:

- Reviewing a change for a specific class of problem your team keeps hitting
- Scaffolding a new file of a kind you add often - an endpoint, a migration, a test class
- Turning a ticket into a draft test plan
- Writing the release note from a diff
- Checking that documentation still matches the code it describes

Bad candidates:

- Anything with the word "all" in it
- Anything you have done once
- Anything where you could not say what a correct answer looks like

**The test:** can you describe what "it did this well" looks like, in one sentence,
before you write anything? If not, pick something else. You cannot evaluate what you
cannot describe, and lab 2 is going to ask you to.

Write the sentence down. You will need it in lab 2.

---

## Step 2 - write the agent

Four minutes. An agent is **who**: a role, with boundaries.

Create `.github/agents/<your-agent>.agent.md`. Use a short, lower case, hyphenated name.

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

### The frontmatter fields

| Field | Required | What it does |
|---|---|---|
| `name` | yes | What appears in the agent dropdown |
| `description` | yes | How the model decides this agent is the right one |
| `argument-hint` | no | Placeholder text when the agent is invoked |
| `tools` | no | The tools it may use. An allowlist. Omit to inherit everything |
| `agents` | no | Which subagents it may delegate to. `[]` stops it delegating |
| `handoffs` | no | Buttons that pass work to another agent |
| `disable-model-invocation` | no | Stops other agents calling this one as a subagent |

### What makes an agent worth having

Look at the three prohibitions in the example above, and at the ones in
`.github/agents/reviewer.agent.md` - *"You do not fix anything"*, repeated three ways.

**The prohibitions are the design.** A general-purpose prompt will happily do a job
adjacent to the one you asked for, and you will never see it happen, because the output
looks the same either way. What makes your agent an agent rather than a saved prompt is
the list of things it must not do.

So before you move on, make sure your file answers three questions:

1. **Who is it?** One role. If you need the word "and", you have two agents.
2. **What may it touch?** Name the files or the kinds of file.
3. **What must it never do?** At least two things, specifically.

A note on `tools`: it is an allowlist, so you say what the agent *may* use. There is no
denylist. If your agent should not be able to edit, give it `tools: ['read', 'search']`
rather than writing "do not edit" and hoping. An instruction is a request; a missing tool
is a boundary. Both have their place, and knowing which one you have is the point.

---

## Step 3 - write the skill

Four minutes. A skill is **how**: a procedure with steps.

A skill is a folder, not a file. Create:

```
.github/skills/<your-skill>/SKILL.md
```

The folder name and the `name` in the frontmatter must match exactly, and both must be
lower case letters, numbers and hyphens. If they do not match, the skill does not load
and nothing tells you.

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

### The frontmatter fields

| Field | Required | What it does |
|---|---|---|
| `name` | yes | Must match the folder name. Lower case, numbers, hyphens only |
| `description` | yes | The only thing the agent sees when choosing a skill |
| `argument-hint` | no | Placeholder text for the slash command |
| `user-invocable` | no | Whether it appears in the `/` menu. Defaults to true |
| `disable-model-invocation` | no | Stops the model loading it automatically |
| `context` | no | Set to `fork` to run it in its own subagent |

### The description is the selector

This is the part people get wrong, so it is worth being blunt about it.

The agent does not read your skill to decide whether to use it. It reads the `name` and
the `description`, and nothing else, and matches them against what you asked for. The body
loads only after one matches.

That means your description is not documentation. It is the entire interface.

- **"Release notes"** - a topic. Will fire unpredictably.
- **"Write a release note from a merged diff in the house format. Use when a change has
  been merged and somebody needs to describe it to people who do not read code."** -
  a trigger. Says what it does and when to reach for it.

### Anything else the procedure needs goes in the folder

A skill is a folder precisely so it can carry its own material - a template, a checklist,
an example of a good output, a script. Reference them from `SKILL.md` and they are loaded
only when they are reached:

```
.github/skills/release-note-from-diff/
├── SKILL.md
├── template.md
└── examples/
    └── good-release-note.md
```

You do not need this today. Know it is there.

---

## Step 4 - run it on something real

Three minutes. This is not optional and it is not a formality.

1. Reload the VS Code window so the new files are picked up
   (`Ctrl-Shift-P` > Developer: Reload Window).
2. Open the Chat panel and select your agent from the dropdown.
3. Point it at something real in this repository. Suggestions:

   - A release-note agent: `git switch reference/realisation` first, then point it at
     `main...HEAD`.
   - A review agent: point it at `reference/agent-pr`, which is a real change with real
     problems in it.
   - A scaffolding agent: ask it to scaffold the thing it scaffolds, for `TICKET-102`.

4. **Read what comes back properly.** Not "did it produce something" - it will. Read it
   against the one sentence you wrote in step 1.

Write down the worst thing it did. Not the most obviously wrong - the worst. The two are
usually different, and the gap between them is most of what this module is about.

---

## Step 5 - fix the one instruction that would have prevented it

Two minutes, and this is the step that teaches.

Go back to your agent or your skill and change **one** instruction - the one that would
have prevented the worst thing you just watched it do.

Not a rewrite. One instruction.

Some of the shapes this usually takes:

| What it did | The instruction that stops it |
|---|---|
| Invented a change that is not in the diff | "Delete any bullet you cannot point at a hunk for" |
| Wandered into files nobody asked about | "Complete only the requested task. Adjacent improvements go in a separate list" |
| Hedged everything | "Never use 'various', 'several' or 'improved' without saying what" |
| Explained its reasoning at length | "Produce the note only. No preamble, no summary of what you did" |
| Quietly guessed at something ambiguous | "Anything you had to assume is marked `[DECISION NEEDED]`, named, not resolved" |

Then run it again on the same input and see whether it worked. You now have the loop that
the rest of this module is about: observe, change one thing, re-run, compare.

---

## Step 6 - commit

```bash
git add .github/agents .github/skills
git commit -m "Add <your-agent> agent and <your-skill> skill"
```

---

## Done when

- [ ] An agent definition exists at `.github/agents/<name>.agent.md` and is committed.
- [ ] A skill exists at `.github/skills/<name>/SKILL.md` and is committed.
- [ ] You have run it at least once against real code in this repository.
- [ ] You can name one instruction you changed after seeing the output, and say why.

The last one is the real bar. An agent nobody has watched run is a guess.

---

## Fallback

If your environment is fighting you, do not spend the lab debugging it.

Copy one of the five existing agents and adapt it rather than starting from a blank file:

```bash
cp .github/agents/reviewer.agent.md .github/agents/my-reviewer.agent.md
```

Change the `name`, change the `description`, and change the "What you are looking for"
section to the class of problem your team actually hits. That is a real agent and it took
four minutes.

For the skill, the complete `SKILL.md` in [step 3](#step-3---write-the-skill) above is a
working file. Copy it, change the `name` to match your folder, rewrite the steps, and you
have a skill. `.github/skills/` is empty until somebody writes the first one, and today
that is you.

You still have to do steps 4 and 5. Adapting somebody else's agent and never running it
teaches nothing at all.

---

## Common failures

**The job was too big.** You will know because you cannot write the one-sentence
description of a good result. Shrink it and start again - it costs two minutes now and
saves the whole lab.

**The agent and the skill say the same thing.** If your skill body is just the agent body
again, you have written one thing twice. The agent is a standing role; the skill is a
procedure it loads when a specific job comes up. If there is genuinely only one job, you
need an agent and no skill - and that is a legitimate answer, so say so.

**You wrote prohibitions that are really preferences.** "Prefer concise output" is not a
prohibition. "Never exceed one paragraph" is. Prohibitions are testable; preferences are
not, and lab 2 is going to ask you to test these.

**It worked first time.** Treat that with suspicion rather than satisfaction. Point it at
something harder before you believe it - `reference/agent-pr` is a change that builds,
passes every test and is wrong in three places, and almost nothing catches all three.

---

Next: [Lab 2 - write three golden tasks](02-golden-tasks.md)
