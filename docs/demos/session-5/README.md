# Session 5 - Claude Code in VS Code and the desktop app

A set of short walkthroughs rather than one demo. Each takes five to twelve minutes and
stands on its own.

**Tool:** Claude Code - the VS Code extension (publisher Anthropic) and the Claude desktop
app's Code tab, both signed in with the same work account. Some steps use claude.ai.
**Branch:** a throwaway branch, never `main`.

---

## Setup

```bash
git clone https://github.com/insight-apac-demo/agentic-sdlc-orders-lab.git
cd agentic-sdlc-orders-lab
dotnet test OrdersService.sln          # read the failure count, not the word Passed
git switch -c walk/claude
```

Open the repository folder itself in VS Code. Check the mode indicator before every
prompt - a new session may start in Auto, and most of these walkthroughs should start in
**Plan**.

---

## Module 5A - the platform

### Model and effort

1. In VS Code, Plan mode, `/model`. Choose a model at **low** effort and ask:

   ```
   TICKET-103 changes how a line total is rounded. Which code does that touch, and what else in the backlog depends on the same rule? Do not edit anything.
   ```

2. New session, same model at **high** effort, same question.
3. Compare: does each answer find `Data/MoneyRounding.cs`, and notice that TICKET-104
   depends on the same rule? Effort tends to change what it noticed, not how it wrote.
4. `/compact`, and read the summary it carries forward.

### A claude.ai Project for the Orders Service

1. claude.ai - Projects - New project, named *Orders Service*. In the goal or instructions:

   ```
   This project is the Orders Service, an internal .NET 8 orders API. Cite the file and section for every rule you rely on. Where a document is silent, say so rather than guessing.
   ```

2. Add `AGENTS.md`, `docs/spec/order-export.md` and `tickets/TICKET-105.md` as files.
3. Ask *"Does TICKET-105 conflict with anything in AGENTS.md or the export spec? Cite the
   section."*, then *"Draw the request flow for the export endpoint as a sequence
   diagram."*
4. Note the limit: the files are a copy, and go stale when the repository changes.

### Optional - Microsoft Foundry as the provider

If you have a Foundry resource with Claude deployments and the Azure AI User role: `az
login`, add the `CLAUDE_CODE_USE_FOUNDRY`, `ANTHROPIC_FOUNDRY_RESOURCE` and
`ANTHROPIC_DEFAULT_*_MODEL` variables to the `env` block of `~/.claude/settings.json`,
reload, and check `/status` shows Microsoft Foundry as the provider. Remove them afterwards.

---

## Module 5B - working in the repository

### First session - change nothing

1. Plan mode: *"Which files would change for TICKET-102? Do not edit anything."*
2. New session, same question with `@tickets/TICKET-102.md` in the prompt. Naming the file
   is cheaper and more reliable than letting it search.
3. Select the body of an endpoint in `src/Orders.Api/Endpoints/OrdersEndpoints.cs` that
   queries the DbContext and ask *"Which rule in AGENTS.md does this break?"* It knows the
   house rules because `CLAUDE.md` imports `AGENTS.md`.

### Plan mode to a reviewed change, and back again

1. Plan mode: *"Plan TICKET-102. Cite the AGENTS.md sections you rely on."*
2. Comment on the plan - for example *"Put the filter in Services/, behind IOrdersService -
   AGENTS.md section 2."* - and send it back.
3. Accept the plan, switch to Manual. Accept one diff, reject one with a reason.
4. Ask it to run the tests: no prompt, because of the allow rule in
   `.claude/settings.json`. Ask it to commit: it asks, because of the ask rule.
5. Decline the commit, then **Rewind code to here** on the message where you accepted the
   plan. The files go back; the conversation stays.

### One session - btw, compact, rewind, resume

1. Plan mode: *"TICKET-104: plan loyalty points on completed orders."*
2. `/btw What does MoneyRounding.Round actually do?` - answered on the side, without
   growing the main conversation.
3. Accept, let it make a few edits, then rewind them.
4. `/compact keep the decision about rounding`.
5. Close the panel, find the session again in Session history by its TICKET-104 title.

### Draft an instruction file, then make the edit that matters

1. Rename `CLAUDE.md` to `CLAUDE.md.bak`. Plan mode: *"Plan an endpoint that returns an
   order's audit history."* Keep the answer.
2. *"Write a CLAUDE.md for this repository."* Delete the lines that are facts, not rules.
   Add: `Never query OrdersDbContext from an endpoint - put the rule in Services/.`
3. New session, same question, compare. Then restore the real `CLAUDE.md` - one
   `@AGENTS.md` line and a short Claude Code section.

### A skill, chosen without being asked

1. Create `.claude/skills/add-endpoint/SKILL.md`:

   ```markdown
   ---
   name: add-endpoint
   description: >
     Adds a minimal-API endpoint to Orders.Api the
     house way. Use when a ticket asks for a new
     route. Not for Razor Pages or migrations.
   allowed-tools: Read, Grep, Glob, Edit,
     Bash(dotnet test *)
   ---

   1. Find the ticket and its spec in docs/spec/.
      No spec: stop and say so.
   2. Put the rule in Services/, behind the
      interface. The endpoint only translates.
   3. Write an AuditEntry for any state change.
   4. Write the test first, then dotnet test.
   5. Report each acceptance criterion as met
      or not, by number.
   ```

2. Plan mode: `/add-endpoint TICKET-102`. The plan follows the five steps.
3. New session, without naming the skill: *"Customer service want to search orders by
   customer reference - TICKET-102. Plan it."* Does it load the skill?
4. Cut the description to *"Helps with endpoints."* and ask again. The description is the
   only thing a skill is chosen on.

### An MCP server, from file to first call

1. Create `.mcp.json`:

   ```json
   {
     "mcpServers": {
       "microsoft-learn": {
         "type": "http",
         "url": "https://learn.microsoft.com/api/mcp"
       }
     }
   }
   ```

2. New session - read the approval prompt before approving. `/mcp` to see its tools.
3. *"TICKET-105 wants a CSV download from a minimal API. Check Microsoft Learn for the
   current .NET 8 way to return a file result, then tell me which file in this repository
   would change."* What came back from the tool is somebody else's text, now in your
   context.

### Pin legacy behaviour before anyone changes it

1. Read the TODO in `src/Orders.Api/Services/PricingHelper.cs`.
2. Plan mode: *"Write characterisation tests for PricingHelper that pin what it does today,
   including midpoint cases. Do not change PricingHelper. Put them in tests/Orders.Tests.
   Plan first."* Make sure the plan includes values like 2.345 and 0.125.
3. Let it write and run them. They pass - they describe today's behaviour.
4. *"Now add the same cases for MoneyRounding.Round, and tell me where the two disagree."*
5. *"Should I fix PricingHelper?"* The right answer is no - see `AGENTS.md`, TICKET-103 and
   TICKET-104.

### Six habits, on one small ticket

On a scratch branch, change `MoneyRounding` to `MidpointRounding.AwayFromZero` so a test
fails, and screenshot the failure. Then: paste the screenshot and ask *"Why is this
failing? Do not fix it yet."*; press Esc and narrow it to `Data/` and the test file;
`/btw` a side question; plan the fix and the test that proves it; let it fix and test;
then review the diff from a fresh session that did not write it.

---

## Put things back

```bash
rm -f CLAUDE.md.bak
git checkout CLAUDE.md src tests
git clean -fd tests                  # the new characterisation tests are untracked
rm -rf .claude/skills/add-endpoint .mcp.json
bash scripts/reset.sh
```

None of this should reach `main`.
