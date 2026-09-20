using Orders.Api.Domain;

namespace Orders.Api.Services;

public interface IOrdersService
{
    Task<IReadOnlyList<Order>> ListAsync(OrderStatus? status, CancellationToken ct = default);
    Task<Order?> GetAsync(Guid id, CancellationToken ct = default);
    Task<Order?> GetByReferenceAsync(string reference, CancellationToken ct = default);
    Task<bool> MarkShippedAsync(Guid id, string actor, CancellationToken ct = default);

    /// <summary>Spec: docs/spec/cancel-order.md</summary>
    Task<CancelResult> CancelAsync(Guid id, string actor, string reason, CancellationToken ct = default);

    /// <summary>True when the order satisfies acceptance criteria 1 to 3 right now.</summary>
    bool IsCancellable(Order order);
}
