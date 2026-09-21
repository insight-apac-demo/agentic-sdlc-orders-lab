# Plan: TICKET-101 - Customer self-service order cancellation

## Questions for the reviewer

- **Which refund timing is approved: immediate or queued?** The specification reopens Q2 because Finance's brief answer, "as per finance," does not choose between them; criterion 11 is marked `[ASSUMED - see Q2]`. *Planned on: the existing `QueueRefundAsync` path and the queued-refund message, until Finance answers explicitly.*
  - Answer: fourteen days
- **Must refund or email failure roll back the cancellation?** Criterion 9 leaves this open, while the specification's assumption says status and audit commit and failed external work is retried out of band. *Planned on: refund and email are best effort after the order/audit transaction commits; the failure is logged with identifiers only.*
  - Answer yes, confirmed:

## What I understand this to mean

The API endpoint accepts the caller-supplied customer reference because the specification adopts the existing self-reported-actor model; it must distinguish an unknown reference from a known customer who owns a different order. Eligibility is both `Placed` and strictly younger than fourteen days, compared as UTC `DateTimeOffset` values from the injected `TimeProvider`. The service owns all eligibility, mutation, audit, transaction, refund, and notification decisions; the endpoint only maps service outcomes to the specified HTTP responses.

A cancellation changes only the addressed order, creates one audit row, queues the full rounded total through `IPaymentService`, records the returned provider reference durably, and sends one email through a new injectable notification abstraction. Customer data is used to send the email but is never included in logs. The Q2 and failure-semantics assumptions above are the only unresolved implementation choices; changing either answer would alter the refund/notification flow and related tests.

## How I will build it

- Add a cancellation request contract and a service result/outcome shape sufficient for the endpoint to distinguish not found, invalid/unknown customer, forbidden owner, non-placed status, expired window, and success without querying the database in the endpoint.
- Extend `IOrdersService`/`OrdersService` with cancellation. Load the order and customer, validate in the specified order, compare `PlacedUtc` with `_clock.GetUtcNow()` in UTC, and return a structured refusal rather than throwing for expected business outcomes.
- In one EF transaction, set `Status = Cancelled` and add exactly one `AuditEntry`; use the caller reason or the literal default. Queue the full `TotalAmount` through `IPaymentService`, persist its provider reference on the audit record, and commit the state/audit unit. Under the stated assumption, external refund/email failures do not undo the committed state; log only order/customer references.
- Add `INotificationService` and its application implementation, register it in `Program`, and send exactly one email confirmation on success. Keep the customer email out of logs. Register/use the existing payment abstraction only; do not introduce a provider call or change the ops page.
- Add the durable refund-reference field to `AuditEntry` and map it through `OrdersDbContext`; use the existing fresh-database/test schema path rather than an unrelated migration mechanism.
- Map `POST /api/orders/{id}/cancel` in `OrdersEndpoints` with the exact 404/400/403/409/200 outcomes and distinct refusal messages, including the non-retry wording and the queued-refund three-working-day success message.

## Criterion to file to test

| Criterion | Files | Test |
|---|---|---|
| 1. Cancel endpoint accepts customer reference and optional reason | `Contracts/CancelOrderRequest.cs`, `Endpoints/OrdersEndpoints.cs` | `Cancel_endpoint_accepts_customer_reference_and_optional_reason` |
| 2. Missing order returns 404 | `Services/OrdersService.cs`, `Endpoints/OrdersEndpoints.cs` | `Cancel_missing_order_returns_not_found` |
| 3. Missing/empty/unknown customer returns 400 | `Services/OrdersService.cs`, `Endpoints/OrdersEndpoints.cs` | `Cancel_unknown_or_missing_customer_returns_bad_request` |
| 4. Known non-owner returns 403 | `Services/OrdersService.cs`, `Endpoints/OrdersEndpoints.cs` | `Cancel_known_non_owner_returns_forbidden` |
| 5. Non-placed order returns state-specific 409 | `Services/OrdersService.cs`, `Endpoints/OrdersEndpoints.cs` | `Cancel_non_placed_order_returns_state_specific_conflict` |
| 6. Placed order at or beyond fourteen days returns window-specific 409 | `Services/OrdersService.cs`, `Endpoints/OrdersEndpoints.cs` | `Cancel_at_fourteen_days_returns_window_conflict` |
| 7. Eligible matching order becomes cancelled with 200 | `Services/IOrdersService.cs`, `Services/OrdersService.cs`, `Endpoints/OrdersEndpoints.cs` | `Cancel_eligible_order_returns_ok_and_changes_only_target_status` |
| 8. Exactly one correctly populated audit row is written | `Domain/AuditEntry.cs`, `Services/OrdersService.cs`, `Data/OrdersDbContext.cs` | `Cancel_writes_one_audit_entry_with_default_or_supplied_reason` |
| 9. Status and audit are atomic; external failure follows stated assumption | `Services/OrdersService.cs`, `tests/Orders.Tests/OrdersServiceTests.cs` | `Cancel_commits_status_and_audit_as_one_unit_when_external_work_fails` |
| 10. Full refund is queued and provider reference is durable | `Services/OrdersService.cs`, `Domain/AuditEntry.cs`, `Services/IPaymentService.cs` | `Cancel_queues_full_total_and_persists_refund_reference` |
| 11. Success states queued refund timing | `Endpoints/OrdersEndpoints.cs` | `Cancel_success_mentions_three_working_day_refund_window` |
| 12. Exactly one email confirmation is sent | `Services/INotificationService.cs`, `Services/NotificationService.cs`, `Program.cs`, `Services/OrdersService.cs` | `Cancel_sends_exactly_one_email_confirmation` |
| 13. Logs contain identifiers only | `Services/OrdersService.cs`, `Services/NotificationService.cs` | `Cancel_logs_no_customer_personal_details` |
| 14. Other orders, lines, and audits remain unchanged | `Services/OrdersService.cs` | `Cancel_does_not_mutate_other_orders_or_audits` |

The endpoint tests will add the minimal ASP.NET Core test-host dependency and replace the clock, database, payment, and notification registrations with deterministic fakes; service tests will continue using `TestHost` and will be extended to inject those fakes. Existing seed, rounding, and shipping tests should remain unchanged; no existing test is expected to break except constructor setup if the service dependencies are expanded.

## Deliberately not doing

- No ops-screen Cancel button change, partial or subscription cancellation, agent cancellation, SMS, authentication redesign, or payment-provider implementation.
- No unrelated repair of the known inline shipping endpoint, `PricingHelper`, logging, or other deliberate debt.
- No silent choice for Q2 or for external failure semantics. If the reviewer answers differently, revise the refund message, transaction boundary, failure tests, and notification behavior before implementation.
