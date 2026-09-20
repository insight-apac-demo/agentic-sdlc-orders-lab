namespace Orders.Api.Services;

public interface IPaymentService
{
    Task<string> QueueRefundAsync(Guid orderId, decimal amount, CancellationToken ct = default);
}
