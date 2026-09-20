using Orders.Api.Data;

namespace Orders.Api.Services;

/// <summary>
/// Stand-in for the real payment provider integration. Queues a refund and returns
/// a provider reference. Nothing here talks to a real provider.
/// </summary>
public class PaymentService : IPaymentService
{
    private readonly ILogger<PaymentService> _log;

    public PaymentService(ILogger<PaymentService> log) => _log = log;

    public Task<string> QueueRefundAsync(Guid orderId, decimal amount, CancellationToken ct = default)
    {
        var rounded = MoneyRounding.Round(amount);
        var reference = "RF-" + Guid.NewGuid().ToString("N")[..10].ToUpperInvariant();
        _log.LogInformation("Queued refund {Reference} for order {OrderId}, amount {Amount}",
            reference, orderId, rounded);
        return Task.FromResult(reference);
    }
}
