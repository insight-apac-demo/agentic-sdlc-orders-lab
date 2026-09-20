namespace Orders.Api.Contracts;

public record OrderSummaryDto(
    Guid Id,
    string Reference,
    string CustomerReference,
    DateTimeOffset PlacedUtc,
    string Status,
    decimal TotalAmount);
