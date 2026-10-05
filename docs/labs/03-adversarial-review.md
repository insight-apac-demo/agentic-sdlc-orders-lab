# Lab 3 - adversarially review your own agent

**22 minutes, independent. Then a debrief you do not skip.**

## Goal

Run your agent on a seeded ticket, review what it produced against the specification, and
write up what you find as a regression case and a quality gate.

## The premise

You have just been shown that green means nothing. CI checks the code against the tests,
and the tests were written by the same agent, from the same misunderstanding. Coverage
measures which lines ran, not whether the assertions mean anything. Linting confirms the
code is well-formed, not that it is correct.

This lab is where you find that out on your own work rather than somebody else's.

---

## Time budget

| Step | Minutes |
|---|---|
| 1. Run the agent on TICKET-105 | 6 |
| 2. Review the diff against the spec | 8 |
| 3. Write the regression case | 4 |
| 4. Write the quality gate | 2 |
| 5. Commit | 2 |

If step 1 overruns, abandon it and go to the [fallback](#fallback). The review is the
lab; the authoring is just how you get something to review.

---

## Step 1 - run the agent against the seeded ticket

Six minutes.

The ticket is [`tickets/TICKET-105.md`](../../tickets/TICKET-105.md) - export order
history as CSV. The specification is
[`docs/spec/order-export.md`](../spec/order-export.md), already written, with numbered
acceptance criteria.

Make a branch so your work is reviewable as a diff:

```bash
git switch -c lab/export-$(git config user.name | tr ' ' '-')
```

Then put an agent on it. Use the one you built in lab 1 if it is the right shape for the
job; otherwise use Agent mode, or the `planner` agent followed by a build.

A prompt that works:

```
Read tickets/TICKET-105.md and docs/spec/order-export.md.
Implement it, with tests, following AGENTS.md.
```

Let it finish. Run the suite:

```bash
dotnet build OrdersService.sln
dotnet test  OrdersService.sln
```

**Do not fix anything yet.** Green or red, whatever it produced is what you are about to
review. If it is red in a way that stops you reading the diff at all, take the
[fallback](#fallback) rather than spending the lab debugging.

---

## Step 2 - review the diff against the specification

Eight minutes, and this is the lab.

```bash
git diff main...HEAD
```

**Read the specification first, then the diff. Not the other way round.**

Reading the code first means you end up checking whether the code is reasonable, which is
a different question from whether it does what was agreed. A change can be perfectly
reasonable code and satisfy no criterion at all, and that is the most expensive kind of
change to let through, because nothing about it looks wrong.

Work the checklist from Module A, in this order:

1. **A criterion that is not met.** Walk the nine criteria in `order-export.md` one at a
   time. For each, point at the line in the diff where it is met - or say that it is not.
   A criterion nobody implemented is the finding that matters most and the one a
   style-focused review never finds.
2. **A criterion met in a way that will not hold.** Correct for the test that was
   written, wrong at a boundary the test does not reach. Check dates, comparisons, empty
   collections, and the exact edge of any range.
3. **An invariant broken.** `AGENTS.md` §2. Money through `MoneyRounding`, time from the
   injected `TimeProvider`, business logic in `Services/`.
4. **Anything on the do-not list.** `AGENTS.md` §5. `DateTime.Now`, `.DateTime` on a
   `DateTimeOffset`, `Console.WriteLine`, anything identifying a customer reaching a log
   or an output.
5. **A test changed so that it passes.** Compare the test diff against the code diff. A
   loosened assertion or a deleted guard is the guard telling you something.
6. **Scope nobody asked for.** Files changed that no criterion needs.

### Run it, do not just read it

The fastest way to find the defect in this ticket is to execute it.

```bash
dotnet run --project src/Orders.Api
```

Then exercise the export over a range that includes the boundary. `ORD-4418` sits 13.8
days old and `ORD-4422` sits 15.4 days old, so a range with its edge between them will
tell you quickly whether the date handling is right. An empty range is worth one request
of its own.

### Find at least one real defect

At least three classes of defect are reliably produced by this ticket. **Most people find
one.** That ratio is the lesson, not a failure - it is the honest measure of what review
catches when the code looks fine and the tests are green.

Two rules while you are looking:

- **The list is sealed.** Your facilitator has it and will reveal it in the debrief.
  Finding out now would cost you the only chance you get to measure your own review
  against an honest baseline.
- **Do not ask the agent to go looking.** It wrote this. An agent reviewing its own work
  is anchored on its own reasoning and cannot see what it assumed - that is the entire
  reason the Reviewer agent exists as a separate role with no sight of the author's
  conversation. If you want a second opinion, run the `reviewer` agent in a fresh chat,
  and treat its output as a claim to verify rather than an answer.

### If you use the Reviewer agent, run it twice

Run the same review twice, unchanged, in two fresh chats.

Findings that appear both times are worth acting on. Findings that appear once are worth
reading. In a single run those two look identical, which is exactly the trap, and it is
why "the agent reviewed it" is not a control.

---

## Step 3 - write the defect up as a regression case

Four minutes.

Add it to the eval file you created in lab 2,
`.github/agents/<your-agent>.evals.md`, using the same shape. Mark it as a regression:

```markdown
## GT-4 - CSV export over a range that crosses the boundary

**Kind:** regression
**Source:** Lab 3, this session. Found by hand after the suite was green.

**Input**

> GET /api/orders/export?from=<13 days ago>&to=<now>

**Known-good outcome**

- ORD-4418 appears in the output.
- Every date in the body is UTC, ISO-8601.

**Pass condition**

ORD-4418 appears in the export for a range whose edge falls between 13 and 14 days ago.

**Why this one**

The suite was green when this shipped. The tests were written in the same timezone as
the code, so they agreed with each other and both were wrong.
```

The point of writing it as a regression case rather than just fixing it: a defect you fix
can come back. A defect you turn into a test cannot come back unnoticed.

---

## Step 4 - write the quality gate

Two minutes. One sentence.

What would have **blocked this before a human ever saw it?** Not "we should review more
carefully" - a mechanism.

Examples of the right shape:

- "A test that asserts every date in the export body matches `^\d{4}-\d{2}-\d{2}T.*Z$`."
- "A pre-commit hook that fails on `DateTime.Now` anywhere under `src/`."
- "A CI step that greps the export output for any value appearing in the Customers table."
- "An eval that runs the export over a range ending inside the fourteen-day boundary and
  checks ORD-4418 is present."

Examples of the wrong shape:

- "Be more careful with dates." Not a gate, a wish.
- "Add it to the review checklist." A checklist is a reminder, not a gate. It will be
  skipped on the day it matters.
- "Ask the agent to double-check." The agent already thought it was right.

A gate has a property a checklist does not: it fails without anybody choosing to care.

Write your gate at the bottom of your eval file, under a `## Gates` heading.

---

## Step 5 - commit

```bash
git add .github/agents/<your-agent>.evals.md
git commit -m "Add regression case and quality gate from the TICKET-105 review"
```

Push the branch if you want it looked at afterwards.

---

## Done when

- [ ] A written regression case, added to your task file.
- [ ] A stated quality gate, in one sentence, that fails without anybody choosing to care.
- [ ] An honest answer to: **would my review have caught this on a busy Friday?**

That third one is not a box to tick. It is the question the whole module is built around,
and the only honest answers are "yes, because of X" or "no". "Probably" means no.

---

## Fallback

If your agent did not produce anything reviewable, or you ran out of time in step 1,
review a diff that is already prepared:

```bash
git switch reference/agent-pr
git diff main...HEAD
```

That branch is TICKET-101 - order cancellation - implemented the way an agent plausibly
does it. It builds. All seventeen tests pass. CI is green. It is wrong in three places,
and the specification it is wrong against is on the same branch at
`docs/spec/cancel-order.md`.

The defect classes are the same ones TICKET-105 produces, so the lesson is identical. You
lose the authoring and you keep the lesson.

Everything from step 2 onwards works unchanged.

---

## Debrief

Seven minutes, together. Have answers ready for these - they are the content, not the
wrap-up.

**Which defect did your evals catch that review missed?**
Usually the regression - the thing that used to work and quietly stopped.

**Which did review catch that evals missed?**
Usually the scope creep and the leaked data. Evals only test what you thought to test.

**How long did the review actually take?**
Be honest. This is the number that decides whether your process survives contact with a
real sprint. A review that takes forty minutes will not happen in week three.

**What would you automate first?**
The answer is nearly always the check you found most tedious - which is also the one you
will quietly stop doing. That is not a coincidence, and it is the best argument for a gate
over a checklist that exists.

The point of the lab is to feel why neither method is sufficient alone.

---

## Common failures

**You read the code first.** Almost everybody does. You end up assessing whether it looks
like your codebase, which it will, because that is what these models are good at.

**You found the cosmetic one and stopped.** The first thing you notice is rarely the worst
thing in the diff. Finish the criteria list even after you have found something.

**You asked the agent whether it was right.** It will tell you it is. It thought so when
it wrote the code.

**You fixed it instead of writing it up.** The fix is the least valuable thing you can do
here, because the next agent run will reintroduce it. The regression case and the gate are
what make it stay fixed.

**You wrote a gate that is really a reminder.** If it requires a human to remember, it is
not a gate.

---

Back to the [lab index](README.md).
