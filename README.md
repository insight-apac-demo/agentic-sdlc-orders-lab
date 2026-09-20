# Orders Service - Agentic SDLC Immersion programme repository

The one repository used across all six sessions of the Agentic SDLC Immersion
programme.

Deliberately ordinary: a small internal orders service with a handful of endpoints,
an internal ops screen, a test suite, some genuine technical debt and a short ticket
backlog. Nothing here is production, and there is no real customer data.

## Quick start

```
dotnet build OrdersService.sln
dotnet test OrdersService.sln
dotnet run --project src/Orders.Api
```

Then open the URL printed at startup. The ops screen lists orders; clicking one shows
the detail, including a Cancel button that is disabled because the feature does not
exist yet. That feature is TICKET-101, and building it is the Session 1 demo.

`orders.db` is created and seeded on first run. Delete it, or run `scripts/reset.ps1`,
to get back to a known state in about two seconds.

## What is in here

| Path | What it is |
|---|---|
| `src/Orders.Api` | The service: endpoints, services, domain, EF Core, Razor Pages ops screen |
| `tests/Orders.Tests` | xUnit suite. Green on `main` |
| `tickets/` | The backlog we work from |
| `docs/inputs/` | The raw material for the Definition-stage demo: emails, a transcript, a schema and a vague ask |
| `docs/spec/` | Where specifications go. Written in Session 2 |
| `AGENTS.md` | Tool-neutral agent instructions. The file that changes agent behaviour |
| `CLAUDE.md` | Imports AGENTS.md, plus Claude Code specifics |
| `.github/copilot-instructions.md` | The Copilot mirror |

## The ops screen

Plain on purpose. Module 1B's pilot-candidate slides call internal admin tools a good
first agent candidate precisely because internal users forgive ugly UI, which lets the
work stay on logic and data flow. This screen is built to that brief: three views, no
design system, no animation.

## Seed data

Eight orders, seeded relative to the current time so the fourteen-day boundary always
has orders either side of it:

| Reference | Age | Status | Why it is there |
|---|---|---|---|
| ORD-4419 | 0.5 days | Placed | Comfortably cancellable |
| ORD-4416 | 3 days | Placed | Comfortably cancellable |
| ORD-4421 | 7.2 days | Placed | Comfortably cancellable |
| ORD-4417 | 12 days | Placed | Inside the window |
| ORD-4418 | 13.8 days | Placed | **The boundary canary.** Inside the window, but only by hours |
| ORD-4422 | 15.4 days | Placed | Outside the window. Should be refused |
| ORD-4412 | 16 days | Shipped | Already shipped. Should be refused |
| ORD-4420 | 21 days | Delivered | Terminal state |

ORD-4418 is the one that matters. An implementation that compares UTC against local
time will refuse it; a correct one will cancel it.

## Branches

| Branch | What it holds |
|---|---|
| `main` | The service as above. No cancellation. Tests green |
| `reference/definition` | The brief and specification as they should come out of the Definition demo |
| `reference/realisation` | TICKET-101 implemented properly, with tests |
| `reference/agent-pr` | TICKET-101 implemented the way an agent plausibly does it. CI green, and wrong. This is the Session 3 review target |

Reference branches exist so nobody is ever stuck watching. If your environment fights
you, check one out and carry on.

## Technical debt, on purpose

`AGENTS.md` lists it. It is there so that adding the instruction layer has a visible
effect in Sessions 2, 3 and 5.
