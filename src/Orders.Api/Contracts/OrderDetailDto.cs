namespace Orders.Api.Contracts;

public record OrderLineDto(string Sku, string Description, int Quantity, decimal UnitPrice, decimal LineTotal);

public record OrderDetailDto(
    Guid Id,
    string Reference,
    string CustomerReference,
    string CustomerName,
    DateTimeOffset PlacedUtc,
    DateTimeOffset? ShippedUtc,
    string Status,
    decimal TotalAmount,
    IReadOnlyList<OrderLineDto> Lines);
