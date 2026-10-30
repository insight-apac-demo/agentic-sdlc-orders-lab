# Session 3B - The instruction layer, and reviewing an agent's pull request

Two exercises.

1. **Draft an instruction file, then make the edit that matters** - generating an
   instruction file takes seconds. The work is deleting the lines that are true today but
   are not rules, and adding the one rule nothing could have generated.
2. **Critical review of an agent's pull request** - `reference/agent-pr` implements
   TICKET-101 the way an agent plausibly does. It builds, every test passes, and it reads
   well. Review it against the specification and find what is wrong.

**Tool:** GitHub Copilot Chat in VS Code. **Time:** about 10 minutes, then 20.

---

## Exercise 1 - draft an instruction file

1. On a branch of your own, move `.github/copilot-instructions.md` and `AGENTS.md` out of
   the way, so the repository has no instructions.
2. In Agent or Plan mode, ask a fixed question and keep the answer:

   ```
   Plan an endpoint that returns an order's audit history.
   ```

3. Ask for an instruction file - `/init` if your build has it, or in words: *"Write a
   copilot-instructions.md for this repository."*
4. Read the draft. Delete two or three lines that describe the codebase today rather than
   a rule: a package version, a folder that happens to be empty, one person's habit.
5. Add one *do-not* by hand:

   ```
   Never query OrdersDbContext from an endpoint - put the rule in Services/.
   ```

6. A new chat, the same fixed question. Compare with step 2.
7. Optionally, create a reviewer agent (`/create-agent` in a local agent session, or by
   hand in `.github/agents/`) and look at the tool list it is given. A reviewer that can
   read and search but not edit is a different risk profile from one that can.
8. Put the real files back and compare: `AGENTS.md` holds the rules, and the tool-specific
   files are short because they point at it.

## Exercise 2 - review the agent's pull request

1. Check out the branch and confirm it is green:

   ```bash
   git switch reference/agent-pr
   dotnet test OrdersService.sln
   ```

2. Read the specification it was built against: `docs/spec/cancel-order.md` on that
   branch.
3. Review the diff against `main` - `git diff main...reference/agent-pr` - with three
   sources open beside it: the specification's acceptance criteria, the do-not list in
   `AGENTS.md`, and the seed data table in the root `README.md`.
4. Ask the **Reviewer** agent for a second opinion (it reports and never edits):

   ```
   Review the changes on this branch against docs/spec/cancel-order.md and AGENTS.md.
   Report findings only. Do not edit anything.
   ```

   Compare its findings with your own. Note what each of you missed.
5. **Check the behaviour, not just the code.** Run the app, cancel a few of the seeded
   orders - including the ones near the fourteen-day boundary - and watch the console
   output while you do. Then compare with `reference/realisation`, which implements the
   same ticket properly.
6. For each defect you find, write down which check would have caught it: the
   specification, `AGENTS.md`, or neither.

The answers are deliberately not in this repository. The Session 4B labs depend on the
review being done blind.

## Reset

```bash
git checkout .
git clean -fd docs src tests
git switch main
bash scripts/reset.sh
```
