# Session 1B - Definition and Realisation, with custom agents

Two demos, back to back, on TICKET-101 (*let a customer cancel their own order*).

1. **Definition** - three custom agents turn the raw material in `docs/inputs/` into a
   brief and then a specification. The moment to watch for: the inputs contradict each
   other, and the Analyst names the conflict instead of quietly picking a side.
2. **Realisation** - a Planner writes a plan to a file, you correct it in the file, a
   coding agent builds it and runs the tests, and you verify against the specification.

**Tool:** GitHub Copilot Chat in VS Code, using the custom agents in `.github/agents/`.
The same five agents exist for Claude Code in `.claude/agents/`.
**Branch:** `main`, clean. **Time:** about 30 minutes.

---

## Setup

1. Check the build and tests: `dotnet test OrdersService.sln` - expect `Failed: 0`.
2. Run the app, open ORD-4418, and confirm the Cancel button is disabled
   ("Cancellation is not built yet"). Stop the app.
3. Open the repository folder in VS Code and open the Copilot Chat agent dropdown. You
   need five agents: **Analyst**, **Spec Writer**, **Spec Reviewer**, **Planner** and
   **Reviewer**. If they are missing, reload the window. If they are still missing, see
   *If the agents do not appear* below.

## Demo 1 - Definition

1. **Read the agents.** Open `.github/agents/analyst.agent.md`. It is plain English, and
   each agent is forbidden from doing the next one's job - the Analyst may not resolve a
   conflict, the Spec Writer may not close an open question, the Reviewer may not edit.
2. **Look at the inputs.** `docs/inputs/` - three emails, a stand-up transcript, a schema
   and the one-line ask. Do not go looking for the contradiction; let the agent find it.
3. **Run the Analyst.** Select **Analyst** and send:

   ```
   Read everything in docs/inputs/ and produce the project brief for TICKET-101.
   ```

   It writes `docs/brief/cancel-order-brief.md`. Check that it names the
   *fourteen days* versus *any time before dispatch* conflict, quotes both sides, and does
   not choose one.
4. **Answer the first question yourself.** Open the brief at the top. The
   `## Questions for you` block is the only part a person edits by hand. Type a decision
   on the `- Answer:` line for question one.
5. **Hand off to the Spec Writer.** Click the **Write the specification** button. It fills
   the prompt box but does not send - press enter yourself. It writes
   `docs/spec/cancel-order.md`. Look for numbered acceptance criteria, your answer under
   *Decisions taken*, and `[DECISION NEEDED]` against anything you left blank.
   - On another run, leave question one blank: it refuses to finish and marks the
     affected criteria `[ASSUMED - see Q1]`.
6. **Hand off to the Spec Reviewer.** Click **Challenge this specification**. You get a
   list of gaps, and `docs/spec/` is unchanged. Optionally click **Close the gaps** and
   watch which criteria change and which open decisions do not.
7. **Check one thing by hand.** Confirm the spec has a criterion about writing an audit
   row. Demo 2 ends on it. If it is missing, add it in one line.

No handoff buttons? Select the agent from the dropdown and use these instead:

```
Read the brief at docs/brief/cancel-order-brief.md and turn it into a specification
at docs/spec/cancel-order.md, following docs/spec/README.md.
```

```
Review docs/spec/cancel-order.md. What would an implementer still have to guess?
```

## Demo 2 - Realisation

1. **Plan.** Select **Planner** and send:

   ```
   Plan TICKET-101 against docs/spec/cancel-order.md.
   ```

   It writes `docs/plans/cancel-order.md` and touches nothing under `src/`.
2. **Correct one thing, in the file.** Read the plan. Pick one thing to correct - logic in
   the endpoint rather than `Services/`, no audit row, no Razor page change - and type the
   correction into `docs/plans/cancel-order.md`. Answer anything in its *Questions for the
   reviewer* block. The agent that builds next reads the file, not your conversation.
3. **Build.** Switch to the ordinary Copilot agent and send:

   ```
   Implement docs/plans/cancel-order.md. Follow AGENTS.md.
   ```

   Watch for it running `dotnet test` without being asked, and for anything that breaks
   the do-not list in `AGENTS.md`.
4. **Verify against the spec.**

   ```
   Walk each acceptance criterion in docs/spec/cancel-order.md one at a time and
   show me where it is met in the diff.
   ```

   Then check it yourself: run the app, open ORD-4418, click Cancel. The status becomes
   Cancelled and an audit row is written.

## In Claude Code instead

The five agents are mirrored as subagents in `.claude/agents/`. Ask for them by name - for
example *"Use the analyst subagent to read docs/inputs/ and produce the brief for
TICKET-101"* - then carry on the same way. There are no handoff buttons; the files are the
handoff.

## If the agents do not appear

Your Copilot version or licence tier may not load custom agents from `.github/agents/`. Run
the same steps in an ordinary Agent-mode chat, pasting the body of each `.agent.md` file
ahead of its prompt. You lose the named agents on screen and keep everything else.

## Fallbacks and reset

- Demo 1 produces nothing usable: `git checkout origin/reference/definition -- docs/brief/cancel-order-brief.md docs/spec/cancel-order.md` gives you the
  brief and spec as they should come out.
- Demo 2 will not go green: `git switch reference/realisation` has TICKET-101 built
  properly.
- Reset: see [the demos README](../README.md#reset-between-runs).
