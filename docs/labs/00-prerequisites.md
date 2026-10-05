# Lab 0 - prerequisites and setup

Do this **before** the session. It takes about ten minutes the first time, and two
minutes on any machine that has done it once.

If you turn up without it, you will spend the guided build installing a toolchain
instead of writing an agent, and the guided build is only sixteen minutes long.

---

## 1. What you need installed

| Thing | Version | How to check |
|---|---|---|
| .NET SDK | 8.0 or later | `dotnet --version` |
| Git | any recent | `git --version` |
| VS Code | latest stable | Help > About |
| GitHub Copilot extension | latest | Extensions panel, check for updates |
| GitHub Copilot Chat extension | latest | Extensions panel, check for updates |

You also need a **GitHub Copilot licence** on the account you are signed into VS Code
with. Everyone on this programme has one. If the chat panel tells you that you have no
access, you are signed in as the wrong account - see [troubleshooting](#troubleshooting)
below.

### Installing .NET 8

- Windows: `winget install Microsoft.DotNet.SDK.8`
- macOS: `brew install --cask dotnet-sdk`
- Or download from <https://dotnet.microsoft.com/download/dotnet/8.0>

A newer SDK is fine. The projects target `net8.0` and a .NET 9 or 10 SDK will build them
without complaint.

---

## 2. Get the repository

```bash
git clone https://github.com/insight-apac-demo/agentic-sdlc-orders-lab.git
cd agentic-sdlc-orders-lab
```

If you would rather work in your own fork - and you should, because you will be
committing to it - fork it on GitHub first and clone your fork instead.

### Make a branch for your work

```bash
git switch -c lab/<your-name>
```

Everything you write today goes on that branch. It keeps `main` clean for the next
person who clones, and it makes your work easy to find afterwards.

---

## 3. Prove it builds before the session starts

```bash
dotnet build OrdersService.sln
dotnet test  OrdersService.sln
dotnet run --project src/Orders.Api
```

Expected results:

- The build succeeds with no errors.
- The tests report **Passed: 13, Failed: 0**. Read the failure count, not the word
  "Passed".
- The run prints a URL. Open it. You get an ops screen listing eight orders.

Press `Ctrl-C` to stop the app.

`orders.db` is created and seeded the first time you run. You never need to edit it. If
the data looks wrong, reset it:

```powershell
./scripts/reset.ps1       # Windows
```

```bash
./scripts/reset.sh        # macOS and Linux
```

Both delete the database, which rebuilds in about two seconds on the next `dotnet run`.
The PowerShell one also clears agent output left behind by a previous demo run and warns
you if the working tree is dirty; the shell one resets the database only.

---

## 4. Prove Copilot is answering

1. Open the repository folder in VS Code - the folder itself, not a parent directory.
   Agent customisations are discovered from the workspace root, so opening the wrong
   folder means none of them load and nothing tells you.
2. Open the Chat panel.
3. Pick **Agent** mode, not Ask.
4. Type: `Which file would you change to add a cancel endpoint?`

You should get an answer that names real files from this repository -
`Endpoints/OrdersEndpoints.cs`, `Services/OrdersService.cs` or similar.

If it answers in generalities and names no files, it is not reading the workspace. Stop
and fix that now; every lab today depends on it.

### Check the instruction file loaded

Ask: `What does AGENTS.md say you must never do?`

A correct answer mentions `DateTime.Now`, logging anything that identifies a customer,
and changing a test so that it passes. If you get a generic answer about best practice,
the instruction layer is not loading and the labs will behave oddly.

---

## 5. Know where things live

You will be writing into these. Have a look before the session so you are not reading
the layout for the first time with a clock running.

| Path | What it is |
|---|---|
| `.github/agents/*.agent.md` | Custom agents. Five worked examples. Yours goes here |
| `.github/skills/<name>/SKILL.md` | Skills. Yours goes here |
| `.github/copilot-instructions.md` | Points at `AGENTS.md` |
| `AGENTS.md` | The invariants, the do-not list, the known landmines. Read §2 and §5 |
| `tickets/` | The backlog. Lab 3 uses `TICKET-105` |
| `docs/spec/` | Acceptance criteria. Lab 3 reviews against `order-export.md` |
| `docs/labs/` | These labs |
| `docs/labs/reference/` | Fallback material, if you run out of time |

The five existing agents in `.github/agents/` are the worked example your own agent is
modelled on. `.github/agents/README.md` explains why there are five of them and not one
prompt, and it is the best twenty minutes of background reading in the repository.

---

## 6. The twenty-line version, if you missed Session 3

Three kinds of file change what an agent does, and they are different things.

**An instruction file** (`AGENTS.md`, `.github/copilot-instructions.md`) applies to
everything, all the time. It is loaded on every request without being asked for. Use it
for facts about this codebase that are always true.

**An agent** (`.github/agents/name.agent.md`) is a role. It has a name, a description,
a body of instructions, and optionally a list of tools it may use. You select it from the
chat agent dropdown. Use it when you want a persistent stance - a reviewer that never
fixes anything, a planner that never writes code.

**A skill** (`.github/skills/name/SKILL.md`) is a procedure. It is a folder containing a
`SKILL.md` plus any scripts, templates or examples that procedure needs. The agent reads
every skill's `name` and `description`, and loads the body only when one matches what you
asked for. Use it for a task with steps that should go the same way every time.

The short version: **an agent is who, a skill is how.** You will write one of each.

Two things that catch people out:

- A skill's `name` must be lower case letters, numbers and hyphens, and it must match its
  folder name. Get it wrong and the skill does not load, silently.
- A skill's `description` is not documentation. It is the only thing the agent sees when
  deciding whether to use your skill. Write when to reach for it, not what it is about.

---

## Troubleshooting

**`dotnet` is not recognised.** The SDK installed but your terminal was open before it
did. Close every terminal and open a new one.

**Tests fail on a clean clone.** Almost always a stale database. Run the reset script and
try again. If they still fail, say so in the chat at the start of the session rather than
working around it - it means something real has changed.

**The app starts but the port is in use.** Another copy is still running. Find it and
stop it, or set a different port:
`ASPNETCORE_URLS=http://localhost:5050 dotnet run --project src/Orders.Api`

**Copilot says you have no access.** VS Code is signed in as a different GitHub account
from the one holding your licence. Click the account icon at the bottom left, sign out,
and sign back in with the right one.

**Copilot answers but names no real files.** You opened a parent folder rather than the
repository root. Close the folder and open `agentic-sdlc-orders-lab` directly.

**Your custom agent does not appear in the dropdown.** Three usual causes: the file is
not under `.github/agents/`, it does not end in `.agent.md`, or the YAML frontmatter is
malformed - most often a colon inside an unquoted `description`. Wrap the value in single
quotes and reload the window.

**The agent ignores your skill.** The `name` does not match the folder name, or the
`description` says what the skill is about rather than when to use it. Both fail quietly.

**Everything is broken and the session has started.** Check out a reference branch and
carry on:

```bash
git switch reference/realisation     # TICKET-101 built properly
git switch reference/agent-pr        # TICKET-101 built the way an agent plausibly does
```

You lose the authoring. You keep the lesson. That is the right trade with a clock
running, and nobody is going to think less of you for it.

---

Next: [Lab 1 - guided build](01-guided-build.md)
