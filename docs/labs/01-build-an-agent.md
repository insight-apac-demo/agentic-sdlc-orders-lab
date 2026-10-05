# Lab 1 - build an agent and a skill

**16 minutes, independent.**

## Goal

One custom agent and one skill, committed to your branch, for a job your team actually
does - and one instruction you changed because of what you watched the agent do.

## Before you start

- You have done [Lab 0](00-prerequisites.md) and Copilot answers with real filenames.
- You are on your own branch (`git switch -c lab/<your-name>` if not).
- You know where the two kinds of file live: `.github/agents/<name>.agent.md` and
  `.github/skills/<name>/SKILL.md`.

There is a complete, working agent and skill at
[`reference/agent-and-skill.md`](reference/agent-and-skill.md). Read it first if you want
the shape, or copy it and adapt it if you would rather spend the time on the running and
the fixing - which is where the lesson is.

Roughly how the sixteen minutes goes:

| | |
|---|---|
| Choose the job | 2 |
| Write the two files | 8 |
| Run it, twice | 4 |
| Change one instruction and re-run | 2 |

If you are still choosing at the four minute mark, take a job from the table below and
move on. The writing is not where the learning is.

---

## What you are producing

**An agent.** A role. Its file has to answer three questions, and if it does not answer
all three you have written a saved prompt rather than an agent:

1. **Who is it?** One role. If you need the word "and", you have two agents.
2. **What may it touch?** Name the files, or the kinds of file.
3. **What must it never do?** At least two things, specifically.

**A skill.** A procedure. Its file needs a goal, numbered steps, constraints, and quality
criteria - the standard you would hold the output to if somebody handed it to you.

Two rules that fail silently if you get them wrong:

- A skill's `name` must be lower case letters, numbers and hyphens, **and must match its
  folder name.** If it does not, the skill does not load and nothing tells you.
- A skill's `description` is the only thing the agent reads when deciding whether to use
  it. Write when to reach for it, not what it is about.

---

## Choosing the job

This is where the lab is won or lost, so spend two minutes on it and no more.

Pick something your team does **weekly**. Not something impressive. The single most common
failure here is picking a job too big to finish, and you will know you have done it because
you cannot write the next sentence.

**The test: write one sentence describing what a good result looks like, before you write
any file.** If you cannot, pick something else - you cannot evaluate what you cannot
describe, and lab 2 is going to ask you to.

Keep that sentence. You need it twice more today.

| Works | Does not work |
|---|---|
| Checking a change for the one problem your team keeps hitting | "Refactor our authentication layer" |
| Scaffolding a file you add often - an endpoint, a migration, a test class | Anything with "all" in it |
| Turning a ticket into a draft test plan | Anything you have done once |
| Writing the release note from a diff | Anything where you could not say what correct looks like |
| Checking whether documentation still matches the code | |

---

## Write the two files

Eight minutes for both. Skeletons below - fill them in rather than copying. The complete
worked versions are in [`reference/agent-and-skill.md`](reference/agent-and-skill.md) if
you want the finished shape beside you.

### The agent

Create `.github/agents/<your-agent>.agent.md`. The file name is lower case and hyphenated
and ends in `.agent.md`.

```markdown
---
name: Your Agent Name
description: What it does, and when somebody should reach for it.
---

One or two sentences: who this is, and what it is for.

## What to read, in order

1.

## What you produce

-

## Never

-
-

## Finish with

```

`name` and `description` are the only required fields. The `name` is the display name - it
appears in the agent picker, may contain spaces and capitals, and does **not** have to
match the file name.

Add `tools` if you want to narrow what it can reach. It is an allowlist, so a reviewer
that must not edit gets `tools: ['read', 'search']` rather than an instruction asking it
not to. An instruction is a request; a missing tool is a boundary.

### The skill

A skill is a folder, not a file. Create `.github/skills/<your-skill>/SKILL.md`.

```markdown
---
name: your-skill
description: What the procedure does, and when to run it.
---

# Title

## Goal

What a correct result looks like, in a sentence or two.

## Steps

1.

## Constraints

-

## Quality criteria

-
```

The folder name and the `name` in the frontmatter must match exactly, and both are lower
case letters, numbers and hyphens only. This is the first of the two silent failures
above - get it wrong and the skill does not load, with no error.

---

## Running it

Writing the files is the quick part. Running it is the lab.

**1. Reload the window.** `Ctrl-Shift-P` > Developer: Reload Window. Neither file is
picked up until you do, and this is the single most common reason people think their
agent is broken.

**2. Open Chat and select your agent.** Agent mode, then pick it by name from the agent
dropdown at the top of the chat input - the `name` from your frontmatter, not the file
name. If it is not in the list, see
[troubleshooting](00-prerequisites.md#troubleshooting) in lab 0.

**3. Give it something real from this repository.** Type a prompt, do not just name a
branch. Something of this shape:

```
Write the release note for main...origin/reference/realisation.
Output it in your reply - do not write a file.
```

Three targets worth using, depending on what your agent does:

- `main...origin/reference/realisation` - a correct, complete implementation of
  TICKET-101. Good for a release-note, summary or documentation agent.
- `main...origin/reference/agent-pr` - a change that builds, passes every test and
  breaches five of its nine acceptance criteria. Good for a review agent.
- `TICKET-102` - an unbuilt ticket. Good for a scaffolding or test-plan agent.

**Use the three-dot range rather than checking the branch out.** `git diff main...X` reads
another branch without switching to it, so you stay on your own branch. An agent pointed
at a reference branch you have checked out will write files onto it.

**And note the `origin/` prefix.** A fresh clone has only `main` as a local branch; the
reference branches exist as remote-tracking refs until you check one out. Without the
prefix you get `unknown revision`.

Then find the output - it may be in the chat, or your agent may have written a file - and
read it against the sentence you wrote earlier. Not "did it produce something". It will.

### Run it a second time, unchanged

Same agent, same input, new chat. Thirty seconds, and it is the most instructive thing
here.

Whatever appears in both runs is the agent. Whatever appears in one is luck, and a single
run cannot tell you which is which - they look identical.

You will usually find that some parts of the output are stable and some are not. Six runs
of the worked example produced four different "Operational notes" sections - audit
entries, configuration, a new constructor dependency, and none - while the closing line
about what would surprise support said the same thing every time. Every version was
written with the same confidence.

Two runs will not always disagree; twice out of those six they matched. If yours agree,
take a third sample rather than concluding the agent is deterministic - the point is that
you cannot tell a stable answer from a lucky one without more than one, and most people
never take a second.

Whatever turns out to be unstable is the part your instructions have not pinned down.

---

## Changing one instruction

Now change **one** instruction: the one that would have prevented the worst thing you
watched it do across the two runs. Not a rewrite. One.

| What it did | The instruction that stops it |
|---|---|
| Invented a change that is not in the diff | "Delete any bullet you cannot point at a hunk for" |
| Wandered into files nobody asked about | "Complete only the requested task. Adjacent improvements go in a separate list" |
| Hedged everything | "Never use 'various', 'several' or 'improved' without saying what" |
| Explained its reasoning at length | "Produce the output only. No preamble" |
| Quietly guessed at something ambiguous | "Anything you had to assume is marked `[DECISION NEEDED]`, named, not resolved" |
| The two runs disagreed | Whatever makes them agree. This is usually the best one available |

Re-run it on the same input, twice again, and see whether it worked. That loop - observe,
change one thing, re-run, compare - is the rest of the module in miniature.

### Expect it to half-work

This is what happened when we did it on the worked example. The vague instruction was
*"a separate Operational notes section for anything that changes how the service is run or
configured"*, and it was replaced with a specific one naming three things that count:
configuration keys, new constructor or DI dependencies, and database migrations.

Before the change, four runs gave four unrelated answers. After it, both runs found the
same fact - a new `IPaymentService` constructor dependency. That is the instruction
working: it pointed the agent at the right thing.

But one of those two runs opened with *"Operational notes: none"* and then described the
constructor dependency in the same sentence - having just been told, in that instruction,
that a constructor dependency is one of the three things that counts.

**So: substantially better, still not right.** That is the normal outcome and it is worth
seeing, because the temptation after one fix is to declare victory and stop. An
instruction can reliably make an agent *look* at the right thing without making it
*classify* what it finds. The second iteration is where you learn that, and almost nobody
does a second iteration.

**If it did nothing wrong**, you have not tested it yet rather than finished. Point it at
`main...origin/reference/agent-pr` and try again, or add an instruction about the shape of the
output so that next time you can see at a glance whether it did the job. "It worked" is
not a deliverable for this step; a changed instruction and a reason is.

---

## Committing

```bash
git add .github/agents .github/skills
git commit -m "Add <your-agent> agent and <your-skill> skill"
```

---

## Done when

- [ ] An agent definition exists at `.github/agents/<name>.agent.md` and is committed.
- [ ] A skill exists at `.github/skills/<name>/SKILL.md` and is committed.
- [ ] You have run it at least twice against real code in this repository.
- [ ] You can name one instruction you changed after seeing the output, and say why.

The last one is the real bar. An agent nobody has watched run is a guess.

---

## Fallback

Do not spend this lab debugging a tool.

Copy the worked pair from [`reference/agent-and-skill.md`](reference/agent-and-skill.md),
or an existing agent:

```bash
cp .github/agents/reviewer.agent.md .github/agents/my-reviewer.agent.md
```

Change the `name` **first**, then the `description`, then the section describing what it
looks for. That is a real agent and it took four minutes.

> **Change the `name` before you change anything else.** Copy the file and leave the
> frontmatter alone and you now have two agents both called `Reviewer`. The picker lists
> the name twice, `--agent "Reviewer"` silently resolves to one of them, and nothing warns
> you which. You can spend ten minutes editing a file that is not the one running.

You still have to run it twice and change one instruction. Adapting somebody else's agent
and never running it teaches nothing at all.

---

## Common failures

**The job was too big.** You will know because you could not write the one-sentence
description. Shrink it and start again - two minutes now, the whole lab otherwise.

**The agent and the skill say the same thing.** If the skill body is the agent body again,
you have written one thing twice. The agent is a standing role; the skill is a procedure it
loads when a specific job comes up.

**You wrote preferences, not prohibitions.** "Prefer concise output" is not a prohibition.
"Never exceed one paragraph" is. Prohibitions are testable, and lab 2 will ask you to test
these.

**The skill never fires.** Two causes, both silent: the `name` does not match the folder,
or the `description` describes the subject rather than the trigger.

**It worked first time.** Treat that with suspicion rather than satisfaction, especially if
you adapted the worked example. Point it at something harder before you believe it.

---

Next: [Lab 2 - write three golden tasks](02-golden-tasks.md)
