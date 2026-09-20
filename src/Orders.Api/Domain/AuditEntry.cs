namespace Orders.Api.Domain;

/// <summary>
/// Append-only record of who did what to an order, and why.
/// Every state change on an order must write one of these.
/// </summary>
public class AuditEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string EntityType { get; set; } = "Order";
    public Guid EntityId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string Actor { get; set; } = string.Empty;
    public DateTimeOffset OccurredUtc { get; set; }
    public string Reason { get; set; } = string.Empty;
}
