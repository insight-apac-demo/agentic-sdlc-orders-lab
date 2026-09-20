using Microsoft.EntityFrameworkCore;
using Orders.Api.Data;
using Orders.Api.Domain;

namespace Orders.Api.Services;

public class OrdersService : IOrdersService
{
    private readonly OrdersDbContext _db;
    private readonly TimeProvider _clock;
    private readonly IPaymentService _payments;
    private readonly ILogger<OrdersService> _log;

    public OrdersService(
        OrdersDbContext db,
        TimeProvider clock,
        IPaymentService payments,
        ILogger<OrdersService> log)
    {
        _db = db;
        _clock = clock;
        _payments = payments;
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

    // ---- TICKET-101, spec docs/spec/cancel-order.md ----

    public const int CancellationWindowDays = 14;

    public bool IsCancellable(Order order)
    {
        if (order.Status != OrderStatus.Placed)
        {
            return false;
        }
        var age = _clock.GetUtcNow() - order.PlacedUtc;
        return age.TotalDays < CancellationWindowDays;
    }

    public async Task<CancelResult> CancelAsync(
        Guid id, string actor, string reason, CancellationToken ct = default)
    {
        var order = await _db.Orders.Include(o => o.Lines)
            .FirstOrDefaultAsync(o => o.Id == id, ct);

        if (order is null)
        {
            return new CancelResult(CancelOutcome.NotFound);
        }

        // Criterion 2: only an order still in Placed can be cancelled.
        if (order.Status != OrderStatus.Placed)
        {
            return new CancelResult(CancelOutcome.NotCancellable);
        }

        // Criterion 3: strictly less than fourteen days, measured in UTC.
        var now = _clock.GetUtcNow();
        if ((now - order.PlacedUtc).TotalDays >= CancellationWindowDays)
        {
            return new CancelResult(CancelOutcome.OutsideWindow);
        }

        // Criterion 5: the refund goes through the payment service, never the provider.
        var refundReference = await _payments.QueueRefundAsync(order.Id, order.TotalAmount, ct);

        order.Status = OrderStatus.Cancelled;

        // Criterion 6: attributable, in UTC, with the supplied reason.
        _db.AuditEntries.Add(new AuditEntry
        {
            EntityId = order.Id,
            Action = "Cancelled",
            Actor = actor,
            OccurredUtc = now,
            Reason = reason
        });

        await _db.SaveChangesAsync(ct);

        // Criterion 9: identifiers only. No customer, no entity.
        _log.LogInformation(
            "Order {OrderId} cancelled by {Actor}, refund {RefundReference}",
            order.Id, actor, refundReference);

        return new CancelResult(CancelOutcome.Cancelled, refundReference);
    }
}
