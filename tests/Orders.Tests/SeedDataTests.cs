using Orders.Api.Domain;
using Xunit;

namespace Orders.Tests;

public class SeedDataTests
{
    [Fact]
    public async Task Seed_straddles_the_fourteen_day_boundary()
    {
        using var h = new TestHost();
        var now = h.Clock.GetUtcNow();
        var placed = await h.Service.ListAsync(OrderStatus.Placed);

        Assert.Contains(placed, o => (now - o.PlacedUtc).TotalDays < 14);
        Assert.Contains(placed, o => (now - o.PlacedUtc).TotalDays > 14);
        Assert.Contains(await h.Service.ListAsync(OrderStatus.Shipped), o => true);
    }

    [Fact]
    public async Task Seed_includes_an_order_within_hours_of_the_boundary()
    {
        using var h = new TestHost();
        var now = h.Clock.GetUtcNow();
        var placed = await h.Service.ListAsync(OrderStatus.Placed);
        Assert.Contains(placed, o =>
        {
            var age = (now - o.PlacedUtc).TotalDays;
            return age > 13.5 && age < 14.0;
        });
    }
}
