# Session 4, Module B - labs

Ninety minutes, and almost all of it is you doing something. You build an agent, you
write the tests that say whether it works, and then you review its output like an
adversary.

Three labs and a setup step. Work through them in order - each one depends on the one
before it, and each is self-directed: a goal, the constraints that matter, and a way to
tell when you are done.

| # | Lab | Time | You end up with |
|---|---|---|---|
| 0 | [Prerequisites and setup](00-prerequisites.md) | before the session | A repository that builds, tests green, and Copilot answering |
| 1 | [Build an agent and a skill](01-build-an-agent.md) | 16 min | One custom agent and one skill, committed |
| 2 | [Write three golden tasks](02-golden-tasks.md) | 18 min | Three tasks with known-good outcomes, committed beside the agent |
| 3 | [Adversarially review an agent's work](03-adversarial-review.md) | 22 min | A written regression case and the quality gate that would have caught it |

Do lab 0 **before** the session starts. It takes about ten minutes the first time and
it is the single biggest cause of people losing the first half of the module.

## What you leave with

Four things, all committed to your own branch:

1. An agent definition - a role, with boundaries, that you wrote.
2. A skill - a repeatable procedure the agent can load on demand.
3. A golden-task file - three real tasks with known-good outcomes.
4. A written regression case, and a one-sentence quality gate that would have blocked
   the defect before a human ever saw it.

That last one is the artefact that matters. It is the difference between "the agent
seems to work" and evidence somebody in a second-line function will accept.

## The rule the module holds you to

**No eval set, no promotion.**

An agent does not earn more autonomy because it has been impressive lately. It earns it
by passing a fixed set of tasks whose right answers were written down in advance, every
time its configuration changes. Labs 2 and 3 are how you build that set for the agent
you wrote in lab 1.

## If your environment fights you

Every lab has a **fallback** section at the bottom. Use it rather than spending the lab
debugging a tool. You lose the authoring and you keep the lesson, which is the right
trade in a room with a clock running.

The reference branches exist for the same reason. Nothing in this module requires you to
have finished the one before it.

## A note on what these labs are not

They are not a tutorial on Copilot. Session 3 covered what agents, skills and instruction
files are, and how the tool loads them. These labs assume that and go straight to the
work. If you did not attend Session 3, [00-prerequisites.md](00-prerequisites.md) has the
twenty-line version of what you need.
