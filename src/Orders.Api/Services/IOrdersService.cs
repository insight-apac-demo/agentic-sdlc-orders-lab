using Orders.Api.Domain;

namespace Orders.Api.Services;

public interface IOrdersService
{
    Task<IReadOnlyList<Order>> ListAsync(OrderStatus? status, CancellationToken ct = default);
    Task<Order?> GetAsync(Guid id, CancellationToken ct = default);
    Task<Order?> GetByReferenceAsync(string reference, CancellationToken ct = default);
    Task<bool> MarkShippedAsync(Guid id, string actor, CancellationToken ct = default);
}
