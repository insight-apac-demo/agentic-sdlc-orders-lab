# Demos - run them yourself

Every session in the programme was built around a live demo or a set of walkthroughs on
this repository. This folder has a short README for each, so you can replay them on your
own machine.

These are the high-level steps, not the facilitator scripts. They tell you what to set up,
what to type and what to look for. They do not tell you what to say.

| Session | Demo | Tool | README |
|---|---|---|---|
| 1B | From a folder of emails to a specification, then to working code | GitHub Copilot, custom agents | [session-1b](session-1b/README.md) |
| 2A | Writing a specification, and breaking it before anything is built | GitHub Copilot Chat | [session-2a](session-2a/README.md) |
| 2B | When the inputs move; building what 2A specified | GitHub Copilot Chat | [session-2b](session-2b/README.md) |
| 3B | The instruction layer, and reviewing an agent's pull request | GitHub Copilot Chat | [session-3b](session-3b/README.md) |
| 4B | Hands-on labs - already self-directed | GitHub Copilot Chat | [docs/labs](../labs/README.md) |
| 5 | Claude Code in VS Code and the desktop app | Claude Code | [session-5](session-5/README.md) |
| 6 | Permissions, hooks, parallel agents, cloud sessions, prompt injection | Claude Code | [session-6](session-6/README.md) |

Sessions 2A and 2B are a pair: 2B's second demo builds the specification 2A writes. Run
them in that order, and do not reset in between.

---

## Before any of them

You need the toolchain from [Lab 0](../labs/00-prerequisites.md): .NET 8 SDK (or later),
Git and VS Code. Then, depending on the session, either GitHub Copilot Chat or the Claude
Code extension, signed in with a work account.

```bash
git clone https://github.com/insight-apac-demo/agentic-sdlc-orders-lab.git
cd agentic-sdlc-orders-lab
dotnet test OrdersService.sln        # expect: Failed: 0
dotnet run --project src/Orders.Api  # open the URL it prints, then Ctrl-C
```

Three habits that save most of the trouble:

- **Open the repository folder itself in VS Code**, not the folder above it. Instruction
  files and custom agents load per workspace, and nothing tells you when they have not.
- **Stop the app before running tests.** A running `Orders.Api` locks its build output and
  the next `dotnet test` fails with `MSB3027` - a build error that looks like the agent
  broke something.
- **Work on a branch of your own.** The demos write files into the repository.

## Reset between runs

```bash
git checkout .
git clean -fd docs src tests
bash scripts/reset.sh                # or scripts/reset.ps1 - deletes the SQLite file
git status                           # expect: nothing listed
```

`git clean` without `-x` leaves `bin`, `obj` and the database alone. The database is
recreated and reseeded on the next run.

## If you get stuck

The reference branches hold the output of the demos as they should come out, so you can
skip a step that will not cooperate and carry on:

| Branch | Holds |
|---|---|
| `reference/definition` | The brief and specification from the 1B Definition demo |
| `reference/realisation` | TICKET-101 built properly, with tests |
| `reference/agent-pr` | TICKET-101 built the way an agent plausibly does it. Green, and wrong |
| `lab/order-export` | TICKET-105 built the same way. The 4B lab 3 review target |

## A note on timezones

The seed data is relative to the current time, so the same orders straddle the fourteen-day
cancellation boundary whenever you run it. A correct implementation behaves the same in
any timezone. An incorrect one may not: the demos were run on AEST (UTC+10), and a defect
that shows up there can hide, or invert, on another UTC offset.
