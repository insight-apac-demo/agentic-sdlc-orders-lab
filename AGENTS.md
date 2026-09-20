# Orders Service - agent instructions

Portable instructions for any coding agent working in this repository.
Claude Code reads this via the `@AGENTS.md` import in CLAUDE.md.
GitHub Copilot reads `.github/copilot-instructions.md`, which points here too.

## What this is

A small internal orders service for a workshop. A handful of endpoints, a Razor Pages
ops screen, an xUnit suite, and some deliberate technical debt. Nothing here is
production and no real customer data is present.

## Stack

- .NET 8, C#, nullable enabled
- ASP.NET Core minimal APIs plus Razor Pages for the internal ops screen
- EF Core with SQLite (`orders.db`, created and seeded on first run)
- xUnit for tests

## Build, run and test

```
dotnet build OrdersService.sln
dotnet test OrdersService.sln
dotnet run --project src/Orders.Api
```

The app listens on the URL printed at startup. Delete `orders.db` to reset; it is
recreated and reseeded on the next run. `scripts/reset.ps1` does this for you.

## Architecture

- Business logic lives in `Services/`. Endpoints and page models translate and delegate.
- Domain entities in `Domain/` stay pure: no EF attributes, no HTTP types, no I/O.
- Persistence is reached only through `OrdersDbContext`.
- Refunds and any other money movement go through `IPaymentService`. Never call a
  payment provider directly.

## Conventions

- Every state change on an order writes an `AuditEntry` with actor, timestamp and reason.
  An order that changes state without an audit row is a defect.
- All times are UTC. Persist and compare `DateTimeOffset`, obtained from the injected
  `TimeProvider`.
- All money is `decimal`, rounded through `Data/MoneyRounding.Round`. Two decimal
  places, banker's rounding.

## Never do these

- NEVER use `DateTime.Now`, `DateTime.Today` or `DateTimeOffset.Now`. Use the injected
  `TimeProvider` and work in UTC.
- NEVER use `Console.WriteLine` for logging. Use `ILogger`.
- NEVER log an entity, a request object or a customer. Log identifiers only. Customer
  name, email and phone must not reach a log sink.
- NEVER introduce a second rounding helper. There is one, and it is `MoneyRounding`.
- NEVER change a test so that it passes. Change the code, or say why the test is wrong.
- NEVER output unfinished placeholders such as `// ... rest of code`.

## Working agreement

- Read the ticket in `tickets/` and the specification in `docs/spec/` before changing code.
- Plan before you edit. Name the files you intend to touch and why.
- Complete only the requested task. Log adjacent improvements as recommendations
  rather than implementing them.
- Mark anything you had to assume as `[DECISION NEEDED]` rather than choosing silently.
- Run `dotnet test` before you claim a change is done.

## Known technical debt

Left here deliberately, so the effect of these instructions is visible:

- `Endpoints/OrdersEndpoints.cs` does the ship logic inline instead of calling the service.
- The same file uses `Console.WriteLine`.
- `Services/PricingHelper.cs` is a second rounding implementation that disagrees with
  `MoneyRounding`. It is unowned. See TICKET-103 and TICKET-104.
