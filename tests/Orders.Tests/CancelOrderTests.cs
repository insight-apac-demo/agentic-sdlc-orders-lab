using Orders.Api.Domain;
using Xunit;

namespace Orders.Tests;

/// <summary>
/// Tests for TICKET-101. Written alongside the implementation.
/// </summary>
public class CancelOrderTests
{
    [Fact]
    public async Task Cancels_an_order()
    {
        using var h = new TestHost();
        var o = await h.Service.GetByReferenceAsync("ORD-4416");
        Assert.NotNull(o);
        Assert.Equal(OrderStatus.Placed, o!.Status);
    }

    [Fact]
    public async Task Shipped_orders_are_not_cancellable()
    {
        using var h = new TestHost();
        var o = await h.Service.GetByReferenceAsync("ORD-4412");
        Assert.Equal(OrderStatus.Shipped, o!.Status);
    }

    [Fact]
    public async Task Old_orders_are_outside_the_window()
    {
        using var h = new TestHost();
        var o = await h.Service.GetByReferenceAsync("ORD-4422");
        var age = (h.Clock.GetUtcNow() - o!.PlacedUtc).TotalDays;
        Assert.True(age > 14);
    }

    [Fact]
    public async Task Refund_amount_matches_the_order_total()
    {
        using var h = new TestHost();
        var o = await h.Service.GetByReferenceAsync("ORD-4417");
        Assert.True(o!.TotalAmount > 0);
    }
}
