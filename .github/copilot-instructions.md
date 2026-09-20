# Orders Service - Copilot instructions

The full, tool-neutral instructions are in `AGENTS.md` at the repository root. Read that
file first and follow it. This file exists so Copilot picks the rules up as well.

Key points, repeated so they are hard to miss:

- .NET 8, C#, nullable enabled. EF Core over SQLite. xUnit.
- Business logic goes in `Services/`, not in endpoints or page models.
- All times UTC, via the injected `TimeProvider`. Never `DateTime.Now`.
- All money through `Data/MoneyRounding.Round`. Never add a second rounding helper.
- Every order state change writes an `AuditEntry`.
- Refunds go through `IPaymentService`. Never call a payment provider directly.
- Never log an entity, a request object or anything identifying a customer.
- Never use `Console.WriteLine`. Use `ILogger`.
- Run `dotnet test OrdersService.sln` before claiming a change is done.
