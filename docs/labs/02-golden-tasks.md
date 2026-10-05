# Lab 2 - write three golden tasks

**18 minutes. Guided.**

## Goal

Three real tasks with known-good outcomes, committed beside your agent, that you can run
every time you change it.

## Why this comes before the review lab

You cannot say whether your agent works until you have said what working means. That is
all a golden task is: a job somebody has genuinely done, the answer they genuinely got,
and a way of telling whether the agent produced it.

The rule the module holds you to is **no eval set, no promotion**. An agent does not get
more autonomy because it has been impressive lately. It gets it by passing a fixed set of
tasks whose right answers were written down in advance. Without that, "the agent has
earned more trust" is an opinion, and an opinion is not something you can take to a
risk function.

---

## The four kinds, and which one you are writing today

A real eval set has all four. They answer different questions.

| Kind | What it is | Answers |
|---|---|---|
| **Golden tasks** | 10 to 20 real tasks with a known-good result | Can it do the job at all? |
| **Regression suite** | Every past failure, captured as a test | Did we just break what worked? |
| **Adversarial suite** | Hostile inputs, scope creep, unsafe actions | What happens when it is attacked? |
| **Judged rubrics** | A second model scoring against written criteria | How good is an output with no single right answer? |

Today you write **three golden tasks**, and one of them doubles as a regression case.
Lab 3 adds a second regression case from a defect you find yourself.

Judged rubrics are the one to be careful with. A model scoring another model's output is
useful for things with no single right answer, but it has to be calibrated against human
scores first or you are just measuring whether two models agree with each other.

---

## Time budget

| Step | Minutes |
|---|---|
| 1. Pick three jobs | 4 |
| 2. Write them up | 10 |
| 3. Run them and record the result | 3 |
| 4. Commit | 1 |

---

## Step 1 - pick three jobs you know the right answer to

Four minutes.

The constraint that matters: **you must already know the correct outcome.** Not be able
to recognise it when you see it - know it, in advance, written down. If you are deciding
whether the output is good by reading it and forming a view, that is a review, not an
eval, and it will drift the moment you are tired.

Pick:

1. **A straightforward one.** The job your agent exists to do, on an easy input. If this
   fails, nothing else matters.
2. **An awkward one.** Same job, but on the input that has edge-case shape - an empty
   result, a boundary, two things that conflict, a file that is not what it looks like.
3. **One drawn from something that previously went wrong.** A time the job was done badly
   by a person or a tool, and you know exactly how.

The third one is the quality bar for this lab. An eval set written entirely from things
that have always worked tests nothing. If you genuinely have no past failure to draw on,
use the worst thing your agent did in lab 1 - you watched it happen an hour ago and that
counts.

### Keep them small

Each task has to run in **under a minute**. This is the most common failure in this lab.

A task that takes five minutes to run will not be run. It will be skipped when you are
busy, which is exactly when the agent most needs checking. Small and actually run beats
thorough and aspirational.

If a task is too big, cut it down to the single most load-bearing part of it.

---

## Step 2 - write them up

Ten minutes. Create `.github/agents/<your-agent>.evals.md`, beside your agent definition.

The agent loader only reads `*.agent.md`, so an `.evals.md` file sits there inertly and
stays next to the thing it describes.

Use this shape. There is a complete worked example at
[`reference/golden-tasks.md`](reference/golden-tasks.md).

```markdown
# Golden tasks - <Agent name>

Agent:  `.github/agents/<name>.agent.md`
Skill:  `.github/skills/<name>/SKILL.md`
Run these whenever either file changes, or the model changes.

Last run: 2026-10-09, 3/3 pass

---

## GT-1 - <short name>

**Kind:** golden
**Source:** <where this came from - a real ticket, a real PR, a real incident>

**Input**

> Exactly what you give the agent. Paste the prompt, name the branch, name the file.
> Someone else has to be able to reproduce this without asking you what you meant.

**Known-good outcome**

What a correct answer contains. Be specific. Three or four bullets.

**Pass condition**

The single sentence that decides it. Must be answerable yes or no by somebody who was
not there.

**Why this one**

One line. What would be true about the agent if this failed?
```

### The pass condition is the whole thing

If you write nothing else carefully, write this carefully.

A pass condition has to be answerable by somebody who was not in the room, without asking
you what you meant. Test it by reading it aloud and asking whether two people would agree
on the answer.

| Not a pass condition | A pass condition |
|---|---|
| The output is good quality | Every bullet names a file that appears in the diff |
| It handles the edge case correctly | Given an empty date range, the output has a header row and no data rows |
| It does not leak data | No customer name, email or phone appears anywhere in the output |
| It follows our standards | No bullet uses the words "various", "several" or "improved" |

The right-hand column has a property the left does not: you can be **wrong** about it.
That is what makes it evidence.

### Worked example

```markdown
## GT-3 - release note for a change that is mostly tests

**Kind:** golden, doubles as regression
**Source:** PR #214, March. The note we shipped described a test refactor as a
feature, and support spent a morning looking for a feature that did not exist.

**Input**

> Agent: Release Note Writer
> Branch: reference/realisation
> Prompt: "Write the release note for main...HEAD"

**Known-good outcome**

- One paragraph describing order cancellation becoming available.
- A bullet for the new cancel endpoint.
- A bullet for the Cancel button becoming enabled on the order detail page.
- Nothing at all about the test project, which also changed in this diff.

**Pass condition**

The note contains no bullet describing a change under `tests/`.

**Why this one**

This is the mistake that cost us a morning. If it comes back, it comes back silently,
because a release note that mentions too much still reads as thorough.
```

---

## Step 3 - run them, and write down what happened

Three minutes.

Run all three against the agent you built in lab 1. Record the result at the top of the
file:

```
Last run: 2026-10-09, 2/3 pass. GT-2 failed - returned a 404 shape for the empty range.
```

**A failure here is a good outcome, not a bad one.** You have just found something, before
it mattered, for the price of three minutes. That is the entire argument for having an
eval set.

Do not fix the agent and then quietly change the task so it passes. If you change a task,
say in the file why you changed it. A task edited to go green is the eval-set equivalent
of changing a test so that it passes, and `AGENTS.md` §5 has an opinion about that.

---

## Step 4 - commit

```bash
git add .github/agents/<your-agent>.evals.md
git commit -m "Add three golden tasks for <your-agent>"
```

---

## Done when

- [ ] Three golden tasks are committed next to your agent definition.
- [ ] Each has a pass condition you could hand to somebody else without explaining it.
- [ ] At least one is drawn from something that previously went wrong.
- [ ] The file records when you last ran them and what the result was.

---

## Where these would actually run

You are writing these by hand today. In a real setup they run in three places, and it is
worth knowing the shape even if you do not build it this week.

**Pre-merge, in CI**, on any change to an instruction file, a skill, an agent definition
or a model version. These four things are code. They regress like code, and a change to
any of them can break an agent that was working yesterday.

**Nightly, across the fleet.** Models get updated without you asking. A nightly run is
how you find out that something you did not change stopped working.

**As the promotion gate.** An agent moves up a stage of autonomy only by passing that
stage's eval set, and a regression sends it back down. That is what turns the trust
gradient from a diagram into a control.

---

## Fallback

If you have run out of time, or your agent is not in a state to be evaluated, take the
three reference tasks in [`reference/golden-tasks.md`](reference/golden-tasks.md) and
adapt them.

They are written against the **Reviewer** agent in `.github/agents/reviewer.agent.md`,
which exists and works, so you can run them immediately. Copy the file, change the
agent it points at, and change the inputs to your own.

You still have to run them. Three tasks you adapted and ran beats three you wrote and
did not.

---

## Common failures

**The tasks are too big.** They will not be run. Cut each one down to its single most
load-bearing part.

**The pass conditions are opinions.** "The output is helpful" cannot fail honestly. If
you cannot imagine the agent being judged wrong against it, it is not a pass condition.

**All three are happy paths.** An eval set made only of easy inputs tells you the agent
can do the easy thing. Nobody was worried about that.

**They test the model, not your agent.** "Does it write good English" is a question about
the model. "Does it refuse to describe a change it cannot point at" is a question about
the instructions you wrote. Only the second kind tells you whether your work was any good.

**You wrote ten.** Three, properly, with real pass conditions. A long eval set written in
eighteen minutes is a short eval set with padding.

---

Next: [Lab 3 - adversarially review your own agent](03-adversarial-review.md)
