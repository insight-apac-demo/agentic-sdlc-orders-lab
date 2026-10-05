# Spec: export order history as CSV

Source ticket: `TICKET-105`
Status: agreed. No open questions. Ready to build from.

## Why

Finance reconcile orders by hand at month end. Today they ask an engineer to run a query
and paste the result into a spreadsheet, which happens four or five times a month and
takes somebody twenty minutes each time. They want to pull the file themselves.

They do not need a report. They need the rows.

## Goal

An authenticated caller can request the orders placed between two dates and get back a
CSV file they can open in a spreadsheet, with no customer-identifying data in it.

## Acceptance

1. `GET /api/orders/export?from=<date>&to=<date>` returns `200` with a
   `Content-Type` of `text/csv`.
2. The first line is a header row: `reference,customerReference,placedUtc,status,total`.
3. The body contains one row per order whose placed date falls within the requested
   range, inclusive of both ends, ordered oldest first.
4. Every `placedUtc` value is rendered in UTC, ISO-8601, with a trailing `Z` - and the
   range comparison is made in UTC. An order is in the range if its UTC placed time is in
   the range, regardless of the clock on the machine serving the request.
5. `total` is the order total, rounded through `Data/MoneyRounding.Round`, and formatted
   with the invariant culture so the decimal separator is a point in every locale.
6. A range that matches no orders returns `200`, the header row, and no data rows. It
   does not return `404`, and it does not return an empty body.
7. No customer name, email or telephone number appears anywhere in the response, in the
   filename, or in any log line produced by this path. `customerReference` is an
   identifier and is permitted; the fields behind it are not.
8. The response carries a `Content-Disposition` header naming a file whose dates are the
   requested UTC dates, in the form `orders-<from>-<to>.csv`.
9. A missing or unparseable `from` or `to` returns `400` with a message naming the
   parameter at fault. It does not return `500`, and it does not silently default to a
   range of its own choosing.

## Out of scope

- Any user interface. This is an API endpoint only; the ops screen is not changed.
- Authentication and authorisation. The service has none yet and this ticket does not
  add any.
- Pagination, streaming, or any accommodation for large exports. The seeded data is eight
  orders and the real table is in the low thousands.
- Any format other than CSV.
- Order lines. The export is one row per order, not one row per line.

## Open questions

None. The two that were open at refinement are recorded under *Decisions taken*.

## Assumptions

- Finance open the file in Excel. Hence the invariant decimal format in criterion 5 and
  the header row in criterion 2.
- "Placed date" means the date the order was placed, not the date it was last modified.

## Decisions taken

- **Are the range ends inclusive?** Yes, both. Finance asked for "the month" and expect
  the first and last day of it to appear.
- **What happens to an order with no customer?** It cannot happen. `Customer` is
  required on `Order`, and the seed and the schema both enforce it.

---

## Notes for an implementer

Three things in this repository will bite you here, and all three are documented:

- `AGENTS.md` §5 - never `DateTime.Now`, and never `.DateTime` on a `DateTimeOffset` to
  get something comparable. The second one is the dangerous one: it drops the offset and
  produces a comparison that is wrong by your machine's UTC offset, which means it passes
  every test written in the same timezone.
- `AGENTS.md` §6 - SQLite cannot `ORDER BY` a `DateTimeOffset` directly, and `decimal` is
  stored as text. Materialise before you sort or compare money.
- `AGENTS.md` §2 - business logic lives in `Services/`. The endpoint translates and
  delegates.

`ORD-4418` is seeded 13.8 days old and is the fastest way to tell whether criterion 4 is
actually satisfied. A range whose edge falls between 13 and 14 days ago should include it.
