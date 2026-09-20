using Orders.Api.Data;
using Xunit;

namespace Orders.Tests;

public class MoneyRoundingTests
{
    [Theory]
    [InlineData(10.004, 10.00)]
    [InlineData(10.006, 10.01)]
    [InlineData(2.675, 2.68)]
    [InlineData(0.0, 0.0)]
    public void Rounds_to_two_places(decimal input, decimal expected)
    {
        Assert.Equal(expected, MoneyRounding.Round(input));
    }

    [Fact]
    public void Uses_bankers_rounding_at_the_midpoint()
    {
        Assert.Equal(2.02m, MoneyRounding.Round(2.025m));
        Assert.Equal(2.04m, MoneyRounding.Round(2.035m));
    }
}
