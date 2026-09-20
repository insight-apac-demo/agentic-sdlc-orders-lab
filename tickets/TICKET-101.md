# TICKET-101 - Let a customer cancel their own order

**Reported by:** Operations
**Priority:** High

Customer service is handling around 200 cancellation requests a day by hand. We want
customers to be able to do it themselves.

There is a fourteen day rule, and obviously we cannot cancel something that has already
shipped. Finance want the refund to go through the existing payment integration rather
than anything new.

The ops screen already has a Cancel button. It is disabled.

Raw material from the people who asked for this is in `docs/inputs/`.

> This ticket is deliberately underspecified. Turning it into a specification an agent
> can act on is the Session 1 Definition demo, and the Session 2 exercise.
