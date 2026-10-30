# Session 2A - Writing a specification, and breaking it

A twenty-one word request goes to a chat with no context. Then you write a short brief
yourself, have the agent turn it into a specification, and break that specification three
ways before anything is built.

The point: a context-free run returns a thorough, well-formatted specification for a
company you are not. The plain one written from your own brief is the one you can check -
and checking it still finds two problems, one of which no amount of better writing would
have caught.

**Ticket:** TICKET-107, estimated dispatch date. **Do not open the ticket until the end.**
**Tool:** GitHub Copilot Chat in VS Code. No custom agents, no build, nothing run.
**Branch:** `main`, clean. **Time:** about 20 minutes.

> Session 2B builds the specification this demo writes. If you plan to run 2B, do not
> reset afterwards.

---

## Setup

1. Reset the working tree (see [the demos README](../README.md#reset-between-runs)).
   `docs/spec/` should contain only `README.md` and `order-export.md`.
2. Open the repository in VS Code.
3. Open a **second VS Code window on an empty folder** - for example a new folder under
   your temp directory. It must have no `.github`, no `AGENTS.md`, no `docs`. Copilot loads
   `.github/copilot-instructions.md` automatically wherever it finds one, and that file
   would answer the ambiguity for it.

## Steps

1. **Run A - the ask, as it arrives.** In the empty window, Copilot Chat in **Ask** mode
   (not Plan, not Agent), send:

   ```
   Can we show people when their order will be dispatched? It's two days
   after they order. Write me the specification.
   ```

   If it asks a clarifying question, do not answer it - tell it to use its own judgement.
   Copy the answer into `docs/spec/run-a.md` in the repository window.
2. **Count what it invented.** Read Run A. Look for things nobody told it: an audience
   (customers - but this is an internal ops screen), a customer email, a shop with
   carriers and warehouses, a default on the question it flagged (calendar days), API
   fields and feature flags. Then read its open questions, and list the ones it did not
   ask.
3. **Write the brief yourself.** In the repository window, create `docs/brief/dispatch.md`
   and type it - it should take about ninety seconds:

   ```
   # Brief: estimated dispatch date

   Why:   Customer service are fielding "where is my order" calls all day.
          Operations want the answer on the screen they already have open.

   Goal:  Somebody looking at an order can see when it is expected to be
          dispatched, without working it out themselves.

   Acceptance:
     - The order detail page shows an estimated dispatch date
     - The estimate is two business days after the order was placed
     - Saturday and Sunday are not business days
     - Nothing is shown once an order has left us

   Out of scope:
     - Public holidays
     - Showing this to customers
     - Changing the estimate after it is first shown

   Open questions:  [DECISION NEEDED]
     - Which clock? The orders carry an offset and the team is in Sydney
     - An order placed late on a Friday - does the count start on Monday?
   ```

   Keep the fourth acceptance line loose, exactly as written. Step 5 depends on it.
4. **Run B - from what you wrote.** A **new** chat in the repository window:

   ```
   Read docs/brief/dispatch.md and docs/spec/README.md.
   Write a specification to docs/spec/run-b.md.

   Use markdown headings, like the worked specs in this repository:
   ## Why, ## Goal, ## Acceptance, ## Out of scope, ## Open questions,
   ## Assumptions.

   Rules:
   - Every acceptance criterion must be one sentence I could write a single
     test for today, without knowing the answer to anything still open.
   - Do NOT resolve anything marked [DECISION NEEDED]. Carry it forward.
   - Do not write a requirement that does not trace back to the brief.
   - Do not state the same rule twice as a positive and a negative.
   - Where the brief is ambiguous, say so rather than choosing.
   ```

   Compare `run-a.md` and `run-b.md` side by side (select both, right-click, **Compare
   Selected**). Run A looks better. Run B is the one that is about this system.
5. **Break it, three ways.**
   - *Trace it to the source:*

     ```
     For each numbered acceptance criterion in docs/spec/run-b.md, quote the
     exact line of docs/brief/dispatch.md it comes from. If a criterion
     traces back to nothing, say so.
     ```

     Run the same check on Run A against the original two sentences. Almost nothing
     traces.
   - *Make it explain:*

     ```
     What did you assume, and what did you decide not to do?
     List separately anything you resolved that I had marked
     [DECISION NEEDED].
     ```

   - *Break it the way a person would:*

     ```
     I am going to give you four cases. For each one, tell me exactly what
     docs/spec/run-b.md says we should show, quoting the criterion. If the spec
     does not answer, say "the specification does not cover this".

     1. ORD-4422, placed fifteen days ago and still in Placed
     2. ORD-4420, which is Delivered
     3. An order that has been Cancelled
     4. An order placed at 5pm on a Friday
     ```

     Case 1 is the one to stop on: the spec answers clearly, and the answer is a dispatch
     date a fortnight in the past. Case 2 is a gap - "left us" does not say whether
     Delivered counts.
6. **A reviewer in one sentence.** A new chat:

   ```
   You are reviewing a specification. You may not edit it and you may not
   write any code. Read docs/spec/run-b.md and report only what an
   implementer would still have to guess. End with the single question you
   would most want answered.
   ```

   That paragraph, saved to a file, is all a custom reviewer agent is.
7. **Mark your own homework.** Now open `tickets/TICKET-107.md`. Its first criterion names
   a state (*orders in Placed*), which closes the ORD-4420 gap. Its second criterion has
   the same ORD-4422 problem as yours. One miss is a writing problem; the other is only
   found by asking.

## Reset

Only when you are finished with Session 2B as well: `git clean -fd docs` removes
`dispatch.md`, `run-a.md` and `run-b.md`.
