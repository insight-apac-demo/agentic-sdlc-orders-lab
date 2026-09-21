# Orders Service - Copilot instructions

**`AGENTS.md` at the repository root is the source. Read it first and follow it.** This file
exists so Copilot picks the rules up as well, and repeats only what is most expensive to get
wrong.

## What this is

A small internal orders service, used as the worked example across a six-session Agentic
SDLC programme. .NET 8, ASP.NET Core minimal APIs, Razor Pages ops screen, EF Core over
SQLite, xUnit. Nothing here is production and there is no real customer data.

```bash
dotnet build OrdersService.sln
dotnet test  OrdersService.sln          # expect: Passed! Failed: 0, Passed: 13
dotnet run --project src/Orders.Api
```

A change is not finished until `dotnet test` passes. Read the failure count, not the word
Passed.

## Where the specification lives

| What | Where |
|---|---|
| What to build | `tickets/TICKET-###.md` |
| What "done" means | `docs/spec/<slug>.md` - the acceptance criteria decide behaviour |
| Why it is wanted | `docs/brief/<slug>-brief.md` |
| The raw material | `docs/inputs/` |

A ticket is not a specification. If `docs/spec/` has no file for the work, write one before
writing code - do not infer acceptance criteria from a ticket title.

## Invariants

- Business logic in `Services/`. Endpoints and page models translate and delegate.
- `Domain/` entities stay pure - no EF attributes, no HTTP types, no I/O.
- Every order state change writes an `AuditEntry` with actor, UTC time and reason.
- Money movement goes through `IPaymentService`, never a provider directly.
- All money is `decimal` through `Data/MoneyRounding.Round`. One rounding implementation.
- All time is UTC from the injected `TimeProvider`.

## Never

- Never `DateTime.Now` / `DateTime.Today` / `DateTimeOffset.Now`.
- Never call `.DateTime` on a `DateTimeOffset` to get something comparable - it drops the
  offset, and the result is wrong by your machine's UTC offset, which means it passes every
  test written in the same timezone.
- Never log an entity, a request object, or anything identifying a customer. Identifiers only.
- Never `Console.WriteLine`. Use `ILogger`.
- Never add a second rounding helper.
- Never change a test so that it passes.
- Never widen a catch, loosen a type or delete a guard to go green.
- Never emit `// ... rest of code`.

## Gotchas that have already cost time

- **SQLite cannot `ORDER BY` a `DateTimeOffset`.** `OrdersDbContext` converts to UTC ticks
  with a value converter. A new `DateTimeOffset` column needs one too, or the query fails at
  runtime rather than at build.
- **The seed is relative to `now`,** not fixed dates. Tests that assume a literal date break;
  use `TestHost`'s clock.
- **ORD-4418 is 13.8 days old** - just inside the fourteen-day boundary, and the fastest way
  to tell whether a time comparison is right.
- **`decimal` on SQLite is stored as text.** Do not sort or compare money in a query.
- **Some of the debt is deliberate.** `Services/PricingHelper.cs` disagrees with
  `MoneyRounding` on purpose, and `Endpoints/OrdersEndpoints.cs` violates three rules on
  purpose. `AGENTS.md` §6 says which and why. Do not "fix" them without reading it.

## Working style

Plan before you edit and name the files you intend to change. Write the tests from the
acceptance criteria before implementing. Review the diff against the specification rather
than against the codebase - those are different questions. Complete only the requested task;
log adjacent improvements separately. Mark anything you had to assume as `[DECISION NEEDED]`
rather than choosing silently.

## Agents

`.github/agents/` holds three Definition-phase agents - Analyst, Spec Writer, Spec Reviewer.
Each is forbidden from doing the next one's job. See `.github/agents/README.md`.
