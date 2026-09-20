using Microsoft.EntityFrameworkCore;
using Orders.Api.Data;
using Orders.Api.Domain;

namespace Orders.Api.Services;

public class OrdersService : IOrdersService
{
    private readonly OrdersDbContext _db;
    private readonly TimeProvider _clock;
    private readonly ILogger<OrdersService> _log;

    public OrdersService(OrdersDbContext db, TimeProvider clock, ILogger<OrdersService> log)
    {
        _db = db;
        _clock = clock;
        _log = log;
    }

    public async Task<IReadOnlyList<Order>> ListAsync(OrderStatus? status, CancellationToken ct = default)
    {
        var q = _db.Orders.Include(o => o.Customer).Include(o => o.Lines).AsQueryable();
        if (status is not null)
        {
            q = q.Where(o => o.Status == status);
        }
        return await q.OrderByDescending(o => o.PlacedUtc).ToListAsync(ct);
    }

    public Task<Order?> GetAsync(Guid id, CancellationToken ct = default) =>
        _db.Orders.Include(o => o.Customer).Include(o => o.Lines)
            .FirstOrDefaultAsync(o => o.Id == id, ct);

    public Task<Order?> GetByReferenceAsync(string reference, CancellationToken ct = default) =>
        _db.Orders.Include(o => o.Customer).Include(o => o.Lines)
            .FirstOrDefaultAsync(o => o.Reference == reference, ct);

    public async Task<bool> MarkShippedAsync(Guid id, string actor, CancellationToken ct = default)
    {
        var order = await _db.Orders.FirstOrDefaultAsync(o => o.Id == id, ct);
        if (order is null || order.Status != OrderStatus.Placed)
        {
            return false;
        }

        var now = _clock.GetUtcNow();
        order.Status = OrderStatus.Shipped;
        order.ShippedUtc = now;

        _db.AuditEntries.Add(new AuditEntry
        {
            EntityId = order.Id,
            Action = "Shipped",
            Actor = actor,
            OccurredUtc = now,
            Reason = "Dispatched from warehouse"
        });

        await _db.SaveChangesAsync(ct);
        _log.LogInformation("Order {OrderId} marked shipped by {Actor}", order.Id, actor);
        return true;
    }
}
