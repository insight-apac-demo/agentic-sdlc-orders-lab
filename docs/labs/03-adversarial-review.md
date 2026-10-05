# Lab 3 - adversarially review an agent's work

**22 minutes, independent. Then a debrief you do not skip.**

## Goal

Build the ticket with an agent, watch it succeed, then review the version where it did
not - and write up what you find as a regression case and a quality gate.

## The premise

You have just been shown that green means nothing. CI checks the code against the tests,
and the tests were written by the same agent, from the same misunderstanding. Coverage
measures which lines ran, not whether the assertions mean anything. Linting confirms the
code is well-formed, not that it is correct.

This lab is where you find that out with your own hands.

---

## Time budget

| Step | Minutes |
|---|---|
| 1. Build it, and notice that it works | 6 |
| 2. Now look at the one that did not | 2 |
| 3. Review the diff against the spec | 8 |
| 4. Write the regression case | 3 |
| 5. Write the quality gate | 2 |
| 6. Commit | 1 |

Step 1 is not a warm-up. It is the first finding.

---

## Step 1 - build it, and notice that it works

Six minutes.

The ticket is [`tickets/TICKET-105.md`](../../tickets/TICKET-105.md) - export order
history as CSV. The specification is
[`docs/spec/order-export.md`](../spec/order-export.md), already written, with nine
numbered acceptance criteria.

Make a branch so your work is reviewable as a diff:

```bash
git switch -c lab/export-$(git config user.name | tr ' ' '-')
```

Then put an agent on it. Use the one you built in lab 1 if it is the right shape for the
job; otherwise use Agent mode.

```
Read tickets/TICKET-105.md and docs/spec/order-export.md.
Implement it, with tests, following AGENTS.md.
```

Let it finish, then run the suite and try it:

```bash
dotnet build OrdersService.sln
dotnet test  OrdersService.sln
dotnet run --project src/Orders.Api
```

```bash
# a wide range - keep this, you will want it in step 3
curl "http://localhost:5000/api/orders/export?from=2026-01-01&to=2030-01-01" | tee ~/my-export.csv

# an empty one
curl -i "http://localhost:5000/api/orders/export?from=2030-01-01&to=2030-02-01" | head -1
```

Save it outside the repository, as above. In a moment you will switch branches and your
own build will not be in the working tree any more - and a stray `.csv` inside the repo
would get in the way of that switch.

### What you should see, and why it matters

**It will probably be correct.** Dates as ISO-8601 with a trailing `Z`. The customer
column holding a reference like `C-2291`, not a name. An empty range returning `200` and
a header row rather than a `404`.

That is not a failed exercise. **That is the finding, and it is the most useful thing in
this module.**

This was run three times while these labs were written - once from the ticket and the
specification, once from the ticket alone, once by a different agent - and it came out
right every time. Same model, same repository, same prompt shape.

So the question worth asking is not *can the agent do it*. It plainly can. The question
is **why** it got it right, and the answer is on your screen: `order-export.md` criterion
4 says dates are UTC with a trailing `Z`. Criterion 6 says an empty range returns a header
row and not a `404`. Criterion 7 says no customer name, email or phone appears anywhere.

The specification told it what not to do, and so it did not do it. **The specification is
the control.** That is Session 2 and Session 3 paying off in front of you, and it is worth
sitting with for thirty seconds before you move on.

Now hold on to the uncomfortable half of that: your agent is exactly as good as the
criteria you wrote. If you had written that spec in a hurry, on a Friday, would it have
named those three things?

Stop the app with `Ctrl-C` before moving on.

### Commit it before you go any further

```bash
git add -A
git commit -m "TICKET-105 export, built from the specification"
```

**This is not optional and it is not tidiness.** The next step switches branches, and git
refuses to switch when uncommitted work would be overwritten. Your build touches
`OrdersEndpoints.cs` and `Program.cs`, and so does the branch you are about to move to, so
without this you get:

```
error: Your local changes to the following files would be overwritten by checkout
```

Commit, and the switch is clean.

---

## Step 2 - now look at the one that did not

Two minutes.

```bash
git switch lab/order-export
dotnet test OrdersService.sln
```

The same ticket. The same nine criteria. **It builds, and all seventeen tests pass.**

This is the version where those three criteria were missed. Nothing about it looks wrong,
which is the entire problem - and it is what a change arriving in your review queue
actually looks like.

---

## Step 3 - review the diff against the specification

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
2. **A criterion met in a way that will not hold.** Correct for the test that was
   written, wrong at a boundary the test does not reach. Check dates, comparisons, empty
   collections, and the exact edge of any range.
3. **An invariant broken.** `AGENTS.md` §2. Money through `MoneyRounding`, time from the
   injected `TimeProvider`, business logic in `Services/`.
4. **Anything on the do-not list.** `AGENTS.md` §5.
5. **A test changed so that it passes.** Compare the test diff against the code diff.
6. **Scope nobody asked for.** Files changed that no criterion needs.

### Run it, do not just read it

Three requests, and between them they reach most of the criteria:

```bash
dotnet run --project src/Orders.Api
```

1. **A wide range.** Then put `GET /api/orders` beside it, which returns the same orders
   with their timestamps, and compare one row against the other. Two views of the same
   data disagreeing is the fastest defect-finding tool you have.
2. **An empty range** - entirely in the future will do. Read criterion 6 first, then look
   at what you got.
3. **Your own export from step 1** - `~/my-export.csv`. Same ticket, same criteria,
   different answer. Put the two side by side:

   ```bash
   curl -s "http://localhost:5000/api/orders/export?from=2026-01-01&to=2030-01-01" > ~/theirs.csv
   diff ~/my-export.csv ~/theirs.csv
   ```

Read the output as Finance would, not as an engineer would. They open it in a spreadsheet
and reconcile by date. Ask what a column has to contain for that to work.

### Also open the tests

`tests/Orders.Tests/OrderExportTests.cs`. Four tests. None of them unreasonable, all of
them green. They check the header row, the row count for a wide range, the row count for
a narrow range, and that totals contain a decimal point.

Ask which criterion each one actually checks, and which criteria nothing checks at all.
That gap is why green means so little, and it is far easier to see in somebody else's
tests than in your own.

### Find at least one real defect

**Three criteria are breached. Most people find one.** That ratio is the lesson, not a
failure - it is the honest measure of what review catches when the code looks fine and the
tests are green.

Two rules while you are looking:

- **The list is sealed.** Your facilitator has it and will reveal it in the debrief.
  Finding out now costs you the only chance you get to measure your own review against an
  honest baseline.
- **Do not ask an agent to go looking** until you have finished your own pass. If you want
  a second opinion afterwards, run the `reviewer` agent in a fresh chat and treat its
  output as a claim to verify, not an answer.

### If you use the Reviewer agent, run it twice

Run the same review twice, unchanged, in two fresh chats.

Findings that appear both times are worth acting on. Findings that appear once are worth
reading. In a single run those two look identical, which is exactly the trap, and it is
why "an agent reviewed it" is not a control.

Two things to expect, because both happened while these labs were being written:

- It will find **more** than the three. Some of what it finds is real and some is noise,
  and telling them apart is your job, not its job.
- Ask it to fix what it finds and **it will**, despite being told three separate times
  that it must not. Its boundary is an instruction, not a gate. If you try this, do it on
  a branch you are willing to throw away - it modified six tracked files when we ran it.

---

## Step 4 - write the defect up as a regression case

Three minutes.

Add it to the eval file you created in lab 2,
`.github/agents/<your-agent>.evals.md`, using the same shape. Mark it as a regression:

```markdown
## GT-4 - CSV export dates are UTC, and say so

**Kind:** regression
**Source:** Lab 3, this session. Found by eye after the suite was green.

**Input**

> GET /api/orders/export?from=2026-01-01&to=2030-01-01

**Known-good outcome**

- Every value in the `placedUtc` column is ISO-8601 and ends in `Z`.
- Each one matches the `placedUtc` that `GET /api/orders` reports for the same order.

**Pass condition**

Every `placedUtc` cell matches `^\d{4}-\d{2}-\d{2}T[\d:.]+Z$`.

**Why this one**

The suite was green when this shipped. Four tests, none of which looked at a date.
```

The point of writing it as a regression case rather than just fixing it: a defect you fix
can come back. A defect you turn into a test cannot come back unnoticed.

---

## Step 5 - write the quality gate

Two minutes. One sentence.

What would have **blocked this before a human ever saw it?** Not "we should review more
carefully" - a mechanism.

Examples of the right shape:

- "A test that asserts every date in the export body matches `^\d{4}-\d{2}-\d{2}T.*Z$`."
- "A pre-commit hook that fails on `DateTime.Now` or `.ToLocalTime()` anywhere under
  `src/`."
- "A CI step that greps the export output for any value appearing in the Customers table."

Examples of the wrong shape:

- "Be more careful with dates." Not a gate, a wish.
- "Add it to the review checklist." A checklist is a reminder. It will be skipped on the
  day it matters.
- "Ask the agent to double-check." The agent already thought it was right.

A gate has a property a checklist does not: it fails without anybody choosing to care.

Write your gate at the bottom of your eval file, under a `## Gates` heading.

---

## Step 6 - commit

Switch back to your own branch first - `lab/order-export` is a shared review target and
your notes do not belong on it.

```bash
git switch -
git add .github/agents/<your-agent>.evals.md
git commit -m "Add regression case and quality gate from the TICKET-105 review"
```

---

## Done when

- [ ] You have seen your own agent implement the ticket correctly, and can say why it did.
- [ ] A written regression case, added to your task file.
- [ ] A stated quality gate, in one sentence, that fails without anybody choosing to care.
- [ ] An honest answer to: **would my review have caught this on a busy Friday?**

That last one is not a box to tick. It is the question the whole module is built around,
and the only honest answers are "yes, because of X" or "no". "Probably" means no.

---

## Fallback

**If step 1 overruns or your agent produces nothing usable**, skip it. Go straight to
step 2 - the review is the lab, and `lab/order-export` is already built and waiting. You
lose the contrast with your own run and you keep everything else.

**If you want a second, harder review target** after you have finished, there is one:

```bash
git switch reference/agent-pr
git diff main...HEAD
```

TICKET-101, order cancellation, built the way an agent plausibly does it. Also green -
seventeen tests - and wrong in more places than this one, with its specification on the
same branch at `docs/spec/cancel-order.md`. It is the Session 4 Module A review target,
so if you were in that session this morning you have seen it reviewed already.

---

## Debrief

Seven minutes, together. Have answers ready for these - they are the content, not the
wrap-up.

**Your agent got it right. Why?**
Because the specification named the three traps. Nothing else changed.

**Which defect did your evals catch that review missed?**
Usually the regression - the thing that used to work and quietly stopped.

**Which did review catch that evals missed?**
Usually the scope creep and the leaked data. Evals only test what you thought to test.

**How long did the review actually take?**
Be honest. This is the number that decides whether your process survives contact with a
real sprint. A review that takes forty minutes will not happen in week three.

**What would you automate first?**
The answer is nearly always the check you found most tedious - which is also the one you
will quietly stop doing. That is not a coincidence, and it is the best argument there is
for a gate over a checklist.

---

## Common failures

**You treated step 1 as setup.** It is the finding. If you skipped straight to step 2 you
missed the only direct evidence in this programme that a specification changes what an
agent produces.

**You read the code first.** Almost everybody does. You end up assessing whether it looks
like your codebase, which it will.

**You found the cosmetic one and stopped.** The first thing you notice is rarely the worst
thing in the diff. Finish the criteria list even after you have found something.

**You fixed it instead of writing it up.** The fix is the least valuable thing you can do
here, because the next agent run reintroduces it. The regression case and the gate are
what make it stay fixed.

**You wrote a gate that is really a reminder.** If it needs a human to remember, it is not
a gate.

---

Back to the [lab index](README.md).
