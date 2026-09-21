# Orders Service - agent working instructions

A small internal orders service used as the worked example throughout the Agentic SDLC
Immersion programme. .NET 8, ASP.NET Core minimal APIs, Razor Pages for an internal ops
screen, EF Core over SQLite, xUnit. Eight seeded orders, a short ticket backlog, and some
deliberate technical debt.

Tool-neutral. Claude Code reaches this through the `@AGENTS.md` import in `CLAUDE.md`;
Copilot reaches it through `.github/copilot-instructions.md`. Keep this file the source
and those two as pointers.

`README.md` describes the layout and how to run it. Read it before changing anything.

> **This repository is a teaching artefact.** Nothing here is production, there is no real
> customer data, and some of the debt below is deliberate. Where a rule exists because a
> session depends on it, that is said explicitly.

---

## 1. Where the specification lives

The code is not the specification and neither is this file. Both point elsewhere.

| What | Where | How to cite it |
|---|---|---|
| What to build | `tickets/TICKET-###.md` | Ticket number, e.g. `TICKET-101` |
| What "done" means | `docs/spec/<slug>.md` | Criterion number, e.g. `cancel-order §4` |
| Why it is wanted at all | `docs/brief/<slug>-brief.md` | Section name |
| The raw material the brief came from | `docs/inputs/` | Filename |
| How a change will be built | `docs/plans/<slug>.md` | Section name |
| The house shape for each of those | `docs/{brief,spec,plans}/README.md` | - |

Paths in this file below `src/Orders.Api` are written relative to it -
`Services/PricingHelper.cs` means `src/Orders.Api/Services/PricingHelper.cs`.

**A ticket is not a specification.** TICKET-101 is deliberately underspecified - it is the
input to the Definition stage, not an instruction to start coding. If `docs/spec/` has no
file for the work you have been given, the first job is to write one, not to infer the
acceptance criteria from the ticket title.

**Acceptance criteria decide behaviour.** Where a criterion and a brief disagree, the
criterion wins and the disagreement is a finding worth raising. Where the criterion is
silent, that is an open question, not licence to choose.

**Never restate a requirement here.** A requirement that lives in two places has two
versions, and the copy in this file is the one the code will follow. Point at it instead.

---

## 2. Invariants

Commitments, not conventions. Breaking one is a defect, not a style disagreement, and
several of them exist because the alternative is silently wrong rather than visibly broken.

- **Business logic lives in `Services/`.** Endpoints and page models translate and delegate.
  An endpoint that queries `OrdersDbContext` directly and applies a rule is a violation, not
  a shortcut. (`Endpoints/OrdersEndpoints.cs` currently violates this on purpose - see §6.)
- **Domain entities stay pure.** No EF attributes, no HTTP types, no I/O in `Domain/`.
  Persistence concerns belong in `Data/`.
- **Every state change on an order writes an `AuditEntry`** with actor, UTC timestamp and
  reason. An order that changes state without one is half-built. This is the criterion most
  often dropped, because nothing fails when it is missing.
- **Money movement goes through `IPaymentService`.** Never call a payment provider directly
  from an endpoint, a service or a job.
- **All money is `decimal`, rounded through `Data/MoneyRounding.Round`** - two places,
  banker's rounding. One rounding implementation, not two.
- **All time is UTC, from the injected `TimeProvider`.** Persist and compare
  `DateTimeOffset`. Never read the machine clock directly.

---

## 3. Build, test, run

```bash
dotnet build OrdersService.sln
dotnet test  OrdersService.sln          # expect: Passed! Failed: 0, Passed: 13
dotnet run --project src/Orders.Api     # ops screen on the URL it prints
```

**A change is not finished until `dotnet test` passes.** Not "looks right", not "compiles".
Run it, and read the failure count rather than the word Passed.

`orders.db` is created and seeded on first run. `scripts/reset.ps1` deletes it; it is
rebuilt in about two seconds. Reset between demo runs rather than hand-editing rows.

---

## 4. How to work a ticket

1. **Read the ticket, then the specification, then the plan.** If there is no
   specification, write one first - `docs/spec/README.md` has the shape. The five agents in
   `.github/agents/` cover this end to end: `analyst` and `spec-writer` produce the
   specification, `spec-reviewer` challenges it, `planner` turns it into a plan and
   `reviewer` checks the finished change against it.
2. **Plan before you edit, and write the plan down.** If `docs/plans/` already has a file
   for this work, that plan governs - read it, build what it says, and raise a finding
   rather than quietly diverging from it. If there is none, the `planner` agent writes one;
   failing that, write the plan yourself before touching a file. Either way it names the
   files you intend to change and why. A plan that does not name files has not been thought
   through, and the plan is the cheapest place to catch a misunderstanding - by about two
   orders of magnitude.
3. **Write the tests from the acceptance criteria**, one test per criterion, before
   implementing. The criteria are the specification; the test is its executable form.
4. **Implement** until they pass.
5. **Self-review the diff against the specification**, not against the codebase. Those are
   different questions. "Does this look like our code?" is not "does this do what we said?"
6. **State what you assumed.** Anything you had to guess is `[DECISION NEEDED]`, in the
   plan, named - not quietly resolved. A criterion already marked `[ASSUMED - see Qn]` was
   written on an assumption somebody has recorded: build it that way, and do not
   substitute your own.

**Complete only the requested task.** Adjacent improvements are recommendations, logged
separately. The helpful rewrite of three files nobody asked about is the most common way an
agent-authored change becomes unreviewable.

---

## 5. Never do these

Each of these is here because it is silently wrong rather than obviously broken.

- **Never `DateTime.Now`, `DateTime.Today` or `DateTimeOffset.Now`.** Use the injected
  `TimeProvider` and work in UTC.
- **Never call `.DateTime` on a `DateTimeOffset` to get something comparable.** It drops the
  offset, and the comparison that results is wrong by your machine's UTC offset - which
  means it passes every test written in the same timezone. Compare `DateTimeOffset` values
  directly.
- **Never log an entity, a request object, or anything identifying a customer.** Log
  identifiers. Customer name, email and phone must not reach a log sink.
- **Never use `Console.WriteLine`.** Use `ILogger`.
- **Never add a second rounding helper.** There is one.
- **Never change a test so that it passes.** Change the code, or explain why the test is
  wrong and change it deliberately.
- **Never widen a catch, loosen a type or delete a guard to go green.** If the suite only
  passes after a guard is removed, the guard was the thing telling you something.
- **Never emit unfinished placeholders** such as `// ... rest of code`.

---

## 6. Known landmines

Read this before you trip over one. The first three are deliberate; the rest are facts
about the stack that have already cost time.

- **`Services/PricingHelper.cs` is a second rounding implementation and it disagrees with
  `MoneyRounding`** - away-from-zero against banker's rounding. It is unowned. TICKET-103
  and TICKET-104 both touch it, from different directions, which is the point of the
  parallel-agent exercise in Session 6. Deliberate: do not "fix" it without reading both
  tickets.
- **`Endpoints/OrdersEndpoints.cs` does the ship logic inline** instead of calling the
  service, recomputes the total with `PricingHelper`, and uses `Console.WriteLine`. Three
  violations in one endpoint, deliberately, so that adding an instruction layer has a
  visible effect in Sessions 2, 3 and 5.
- **The Cancel button on the order detail page is disabled.** That feature does not exist;
  it is TICKET-101, and building it is the Session 1 demo.
- **SQLite cannot `ORDER BY` a `DateTimeOffset`.** `OrdersDbContext` converts them to UTC
  ticks with a value converter. Add a new `DateTimeOffset` column and you must add the
  converter too, or the query fails at runtime rather than at build.
- **The seed is relative to `now`, not to fixed dates.** Ages stay correct whenever the repo
  is cloned. Tests that assume a literal date will break; use `TestHost`'s clock.
- **ORD-4418 sits 13.8 days old** - a few hours inside the fourteen-day boundary. It is the
  fastest way to tell whether a time comparison is correct, and it is used as exactly that
  in Session 3.
- **`decimal` on SQLite is stored as text.** Do not sort or compare money in a database
  query; materialise first.

---

## 7. Handling uncertainty

Say so, and say it before you build rather than after.

If two sources disagree, name both and quote them. Do not choose silently - a silent choice
looks identical in the output to a decision that was actually made, and the reviewer has no
way to tell them apart.

If you cannot reach something you need, stop and say what you could not read and what you
would have done with it. An unreachable source produces a question. A guessed one produces
a confident, wrong change that passes review because nothing in it looks uncertain.

The places these sources already disagree are listed in the brief under *Where the inputs
disagree*. Expect them, and expect the fourteen-day cancellation window to be one of them.
