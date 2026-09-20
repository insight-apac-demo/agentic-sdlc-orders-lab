using Orders.Api.Domain;
using Orders.Api.Services;
using Xunit;

namespace Orders.Tests;

/// <summary>
/// One test per acceptance criterion in docs/spec/cancel-order.md.
/// </summary>
public class CancelOrderTests
{
    [Fact] // criterion 1
    public async Task Cancels_an_order_inside_the_window()
    {
        using var h = new TestHost();
        var o = await h.Service.GetByReferenceAsync("ORD-4417");

        var result = await h.Service.CancelAsync(o!.Id, "priya.raman", "Changed my mind");

        Assert.Equal(CancelOutcome.Cancelled, result.Outcome);
        var after = await h.Service.GetAsync(o.Id);
        Assert.Equal(OrderStatus.Cancelled, after!.Status);
    }

    [Fact] // criterion 2
    public async Task Refuses_an_order_that_has_shipped()
    {
        using var h = new TestHost();
        var o = await h.Service.GetByReferenceAsync("ORD-4412");
        var result = await h.Service.CancelAsync(o!.Id, "ops", "");
        Assert.Equal(CancelOutcome.NotCancellable, result.Outcome);
    }

    [Fact] // criterion 2
    public async Task Refuses_an_order_that_has_been_delivered()
    {
        using var h = new TestHost();
        var o = await h.Service.GetByReferenceAsync("ORD-4420");
        var result = await h.Service.CancelAsync(o!.Id, "ops", "");
        Assert.Equal(CancelOutcome.NotCancellable, result.Outcome);
    }

    [Fact] // criterion 2
    public async Task Refuses_an_order_that_is_already_cancelled()
    {
        using var h = new TestHost();
        var o = await h.Service.GetByReferenceAsync("ORD-4417");
        await h.Service.CancelAsync(o!.Id, "ops", "");
        var second = await h.Service.CancelAsync(o.Id, "ops", "");
        Assert.Equal(CancelOutcome.NotCancellable, second.Outcome);
    }

    [Fact] // criterion 3
    public async Task Refuses_an_order_outside_the_window()
    {
        using var h = new TestHost();
        var o = await h.Service.GetByReferenceAsync("ORD-4422");
        var result = await h.Service.CancelAsync(o!.Id, "ops", "");
        Assert.Equal(CancelOutcome.OutsideWindow, result.Outcome);
    }

    [Fact] // criterion 3 - the boundary canary
    public async Task Cancels_an_order_that_is_hours_inside_the_boundary()
    {
        using var h = new TestHost();
        var o = await h.Service.GetByReferenceAsync("ORD-4418");
        var age = (h.Clock.GetUtcNow() - o!.PlacedUtc).TotalDays;
        Assert.InRange(age, 13.5, 14.0);

        var result = await h.Service.CancelAsync(o.Id, "ops", "boundary check");
        Assert.Equal(CancelOutcome.Cancelled, result.Outcome);
    }

    [Fact] // criterion 3 - exactly fourteen days is refused
    public async Task Refuses_an_order_exactly_on_the_boundary()
    {
        using var h = new TestHost();
        var o = await h.Service.GetByReferenceAsync("ORD-4418");
        h.Clock.Advance(TimeSpan.FromDays(14) - (h.Clock.GetUtcNow() - o!.PlacedUtc));

        var result = await h.Service.CancelAsync(o.Id, "ops", "");
        Assert.Equal(CancelOutcome.OutsideWindow, result.Outcome);
    }

    [Fact] // criterion 4
    public async Task Returns_not_found_for_an_unknown_order()
    {
        using var h = new TestHost();
        var result = await h.Service.CancelAsync(Guid.NewGuid(), "ops", "");
        Assert.Equal(CancelOutcome.NotFound, result.Outcome);
    }

    [Fact] // criterion 5
    public async Task Queues_a_refund_through_the_payment_service()
    {
        using var h = new TestHost();
        var o = await h.Service.GetByReferenceAsync("ORD-4417");
        var result = await h.Service.CancelAsync(o!.Id, "ops", "");

        Assert.NotNull(result.RefundReference);
        Assert.Single(h.Payments.Refunds);
        Assert.Equal(o.TotalAmount, h.Payments.Refunds[0].Amount);
    }

    [Fact] // criterion 6
    public async Task Writes_an_audit_row_with_actor_reason_and_time()
    {
        using var h = new TestHost();
        var o = await h.Service.GetByReferenceAsync("ORD-4417");
        await h.Service.CancelAsync(o!.Id, "priya.raman", "Ordered the wrong size");

        var audit = Assert.Single(h.Db.AuditEntries.Where(
            a => a.EntityId == o.Id && a.Action == "Cancelled"));
        Assert.Equal("priya.raman", audit.Actor);
        Assert.Equal("Ordered the wrong size", audit.Reason);
        Assert.Equal(h.Clock.GetUtcNow(), audit.OccurredUtc);
    }

    [Fact] // criterion 8
    public async Task IsCancellable_agrees_with_the_endpoint_rules()
    {
        using var h = new TestHost();
        Assert.True(h.Service.IsCancellable((await h.Service.GetByReferenceAsync("ORD-4418"))!));
        Assert.False(h.Service.IsCancellable((await h.Service.GetByReferenceAsync("ORD-4422"))!));
        Assert.False(h.Service.IsCancellable((await h.Service.GetByReferenceAsync("ORD-4412"))!));
    }
}
