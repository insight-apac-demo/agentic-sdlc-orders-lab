namespace Orders.Api.Domain;

public class Order
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Reference { get; set; } = string.Empty;
    public Guid CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public DateTimeOffset PlacedUtc { get; set; }
    public DateTimeOffset? ShippedUtc { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Placed;
    public decimal TotalAmount { get; set; }
    public List<OrderLine> Lines { get; set; } = new();
}
