using Microsoft.Extensions.Logging.Abstractions;
using Orders.Api.Services;
using Xunit;

namespace Orders.Tests;

public class OrderExportTests
{
    private static OrderExportService ExportFor(TestHost h) =>
        new(h.Db, NullLogger<OrderExportService>.Instance);

    private static string[] Rows(string csv) =>
        csv.Split('\n', StringSplitOptions.RemoveEmptyEntries)
           .Select(l => l.TrimEnd('\r'))
           .ToArray();

    [Fact]
    public async Task Export_starts_with_the_agreed_header_row()
    {
        using var h = new TestHost();
        var now = h.Clock.GetUtcNow();

        var csv = await ExportFor(h).ExportCsvAsync(now.AddDays(-30), now);

        Assert.Equal("reference,customerReference,placedUtc,status,total", Rows(csv)[0]);
    }

    [Fact]
    public async Task Export_returns_every_order_in_a_wide_range()
    {
        using var h = new TestHost();
        var now = h.Clock.GetUtcNow();

        var csv = await ExportFor(h).ExportCsvAsync(now.AddDays(-30), now);

        // eight seeded orders, plus the header
        Assert.Equal(9, Rows(csv).Length);
    }

    [Fact]
    public async Task Export_filters_to_the_requested_range()
    {
        using var h = new TestHost();
        var now = h.Clock.GetUtcNow();

        var csv = await ExportFor(h).ExportCsvAsync(now.AddDays(-5), now);
        var rows = Rows(csv);

        // ORD-4419 at 0.5 days and ORD-4416 at 3 days are the only two inside
        Assert.Equal(3, rows.Length);
        Assert.Contains(rows, r => r.StartsWith("ORD-4419"));
        Assert.Contains(rows, r => r.StartsWith("ORD-4416"));
    }

    [Fact]
    public async Task Export_writes_totals_with_a_decimal_point()
    {
        using var h = new TestHost();
        var now = h.Clock.GetUtcNow();

        var csv = await ExportFor(h).ExportCsvAsync(now.AddDays(-30), now);

        foreach (var row in Rows(csv).Skip(1))
        {
            var total = row.Split(',').Last();
            Assert.Contains('.', total);
        }
    }
}
