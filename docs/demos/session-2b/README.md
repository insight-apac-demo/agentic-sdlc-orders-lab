# Session 2B - When the inputs move, and building what 2A specified

Two demos.

1. **The question nobody thought mattered** - the specification TICKET-101 was built
   from parked one question as harmless. An email that arrived afterwards answers it. Every
   acceptance criterion survives; the document still answers three people wrongly, and
   the one thing the email asks for is not actually in the product, even though the test
   for it is green. Nothing is written in this demo - every step is a read.
2. **Building what 2A specified** - plan, correct, build and verify the TICKET-107
   specification written in Session 2A, then see both of its known problems on a real
   screen.

**Tool:** GitHub Copilot Chat in VS Code. Demo 1 in **Ask** mode, Demo 2 in **Agent**
mode. **Branch:** `main`. **Time:** about 20 minutes each.

---

## Setup

1. `git fetch --all` and `dotnet test OrdersService.sln` - green, so any red later is the
   agent's.
2. For Demo 2 you need `docs/spec/run-b.md` from [Session 2A](../session-2a/README.md). If
   you do not have it, run 2A first, or recreate it: type the brief from 2A step 3 into
   `docs/brief/dispatch.md` and run 2A's step 4 prompt.

## Demo 1 - the question nobody thought mattered

1. **Bring the built specification into the working tree**, so the agent can read it as a
   file:

   ```bash
   git show origin/reference/realisation:docs/spec/cancel-order.md > docs/spec/cancel-order.md
   ```

   Read *Decisions taken*, then open question 3 - *can an agent cancel on a customer's
   behalf?* - parked with a reason: the endpoint records an actor, so answering it later
   does not change the criteria. That reasoning is sound.
2. **Read the email nobody was waiting for.** The last lines of
   `docs/inputs/04-standup-transcript.md` park the same question. Then read
   `docs/inputs/06-email-operations-followup.md`, which answers it.
3. **Ask the one question.** Ask mode, so nothing can be edited:

   ```
   Read docs/spec/cancel-order.md and then
   docs/inputs/06-email-operations-followup.md.

   The email was sent after the specification was written. Tell me, in
   this order:
   1. Which open question in the specification this email answers.
   2. What in the specification is now wrong, and what has to change.
   3. What is already covered and needs no change at all.

   Quote both documents. Do not rewrite anything.
   ```

4. **First reveal - the prose, not the criteria.** All the criteria still stand. Now read
   the *Goal* and the *Out of scope* list: what the email makes the priority is listed as
   out of scope, and another out-of-scope item was deferred "once question 3 is answered".
   Out of scope and harmless are not the same thing.
5. **Second reveal - check the product, not the document.** The agent will say the actor
   is already recorded. Look at the two ways into a cancellation:

   ```bash
   git show origin/reference/realisation:src/Orders.Api/Pages/Order.cshtml.cs | grep -n actor
   git show origin/reference/realisation:src/Orders.Api/Contracts/CancelRequest.cs | grep -n record
   git show origin/reference/realisation:tests/Orders.Tests/CancelOrderTests.cs | grep -n -A9 Writes_an_audit_row
   ```

   Ask yourself: with no authentication configured anywhere in `Program.cs`, what actor
   does the screen record? And which path does the passing test exercise?

## Demo 2 - building what 2A specified

1. **Open `docs/spec/run-b.md`** and leave it exactly as Session 2A produced it.
2. **Plan first.** A new chat, Agent mode (or Plan mode if you have it):

   ```
   Read docs/spec/run-b.md and AGENTS.md.
   Plan the implementation. Do not write any code yet.
   Name the files you will change, what each change does, and which
   acceptance criterion each test will cover.
   List anything the specification does not tell you.
   ```

3. **Correct exactly one thing.** If the plan did not raise the Delivered order, raise it:

   ```
   The specification says nothing is shown once an order has left us.
   ORD-4420 is Delivered, not Shipped. Say explicitly what you will show
   for it, and add it to the plan as an assumption, not as a requirement.
   ```

   If the plan offers to fix estimates that are already in the past, stop it - build what
   the criteria say and record that as an open question:

   ```
   You raised what to do when the estimated date is already in the past.
   The specification does not cover that, so do not solve it. Build
   exactly what the criteria say, and record the question as an open
   question in the plan for somebody to decide.
   ```

4. **Build.**

   ```
   Build to the plan. Write the tests first, then make them pass.
   Run dotnet test when you are done.
   ```

5. **Verify against the criteria, not the code.** Run the app
   (`dotnet run --project src/Orders.Api`, usually `http://localhost:5000`). Order ids
   change on every seed, so use the **Status** filter on the front page to find orders.

   | Order | What to check |
   |---|---|
   | ORD-4419 | Placed hours ago. A believable estimate - work out the two business days yourself |
   | ORD-4412 | Shipped. Nothing shown |
   | ORD-4420 | Delivered. Whatever it shows, the specification never said |
   | ORD-4422 | Placed fifteen days ago. An estimate a fortnight in the past - the spec followed exactly, and wrong |

   Stop the app when you are done.

## Reset

```bash
git checkout .
git clean -fd docs src tests
bash scripts/reset.sh
```

Demo 2 usually adds a new test file; `git clean` on `src` and `tests` removes it so it does
not turn up in a later test run.
