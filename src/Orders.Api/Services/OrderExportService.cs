using System.Globalization;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Orders.Api.Data;
using Orders.Api.Domain;

namespace Orders.Api.Services;

/// <summary>
/// CSV export of order history for Finance. See docs/spec/order-export.md.
/// </summary>
public interface IOrderExportService
{
    Task<string> ExportCsvAsync(
        DateTimeOffset from, DateTimeOffset to, CancellationToken ct = default);
}

public class OrderExportService : IOrderExportService
{
    public const string Header = "reference,customerReference,placedUtc,status,total";

    private readonly OrdersDbContext _db;
    private readonly ILogger<OrderExportService> _log;

    public OrderExportService(OrdersDbContext db, ILogger<OrderExportService> log)
    {
        _db = db;
        _log = log;
    }

    public async Task<string> ExportCsvAsync(
        DateTimeOffset from, DateTimeOffset to, CancellationToken ct = default)
    {
        var orders = await _db.Orders
            .Include(o => o.Customer)
            .OrderBy(o => o.PlacedUtc)
            .ToListAsync(ct);

        var inRange = orders
            .Where(o => o.PlacedUtc >= from && o.PlacedUtc <= to)
            .ToList();

        _log.LogInformation(
            "Exporting {Count} orders for {From} to {To} on behalf of {Customers}",
            inRange.Count, from, to,
            string.Join(", ", inRange.Select(o => o.Customer?.Email ?? "")));

        var sb = new StringBuilder();
        sb.AppendLine(Header);
        foreach (var o in inRange)
        {
            sb.AppendLine(string.Join(",", new[]
            {
                Csv(o.Reference),
                Csv(o.Customer?.FullName ?? ""),
                o.PlacedUtc.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"),
                Csv(o.Status.ToString()),
                MoneyRounding.Round(o.TotalAmount).ToString(CultureInfo.InvariantCulture)
            }));
        }

        return sb.ToString();
    }

    private static string Csv(string value) =>
        value.Contains(',') || value.Contains('"')
            ? "\"" + value.Replace("\"", "\"\"") + "\""
            : value;
}
