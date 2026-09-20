using Orders.Api.Domain;
using Xunit;

namespace Orders.Tests;

public class OrdersServiceTests
{
    [Fact]
    public async Task Lists_every_order_when_no_status_given()
    {
        using var h = new TestHost();
        var all = await h.Service.ListAsync(null);
        Assert.Equal(8, all.Count);
    }

    [Fact]
    public async Task Filters_by_status()
    {
        using var h = new TestHost();
        var placed = await h.Service.ListAsync(OrderStatus.Placed);
        Assert.All(placed, o => Assert.Equal(OrderStatus.Placed, o.Status));
        Assert.NotEmpty(placed);
    }

    [Fact]
    public async Task Finds_an_order_by_reference()
    {
        using var h = new TestHost();
        var o = await h.Service.GetByReferenceAsync("ORD-4417");
        Assert.NotNull(o);
        Assert.Equal(OrderStatus.Placed, o!.Status);
        Assert.NotNull(o.Customer);
    }

    [Fact]
    public async Task Marking_shipped_moves_state_and_writes_an_audit_row()
    {
        using var h = new TestHost();
        var o = await h.Service.GetByReferenceAsync("ORD-4416");
        var ok = await h.Service.MarkShippedAsync(o!.Id, "ops.test");

        Assert.True(ok);
        var after = await h.Service.GetAsync(o.Id);
        Assert.Equal(OrderStatus.Shipped, after!.Status);
        Assert.Contains(h.Db.AuditEntries, a => a.EntityId == o.Id && a.Action == "Shipped");
    }

    [Fact]
    public async Task Marking_shipped_twice_is_refused()
    {
        using var h = new TestHost();
        var o = await h.Service.GetByReferenceAsync("ORD-4416");
        Assert.True(await h.Service.MarkShippedAsync(o!.Id, "ops.test"));
        Assert.False(await h.Service.MarkShippedAsync(o.Id, "ops.test"));
    }

    [Fact]
    public async Task Order_totals_match_the_sum_of_their_lines()
    {
        using var h = new TestHost();
        foreach (var o in await h.Service.ListAsync(null))
        {
            var expected = Orders.Api.Data.MoneyRounding.Round(o.Lines.Sum(l => l.Quantity * l.UnitPrice));
            Assert.Equal(expected, o.TotalAmount);
        }
    }
}
