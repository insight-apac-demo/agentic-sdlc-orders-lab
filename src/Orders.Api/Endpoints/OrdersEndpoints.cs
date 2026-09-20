using Microsoft.EntityFrameworkCore;
using Orders.Api.Contracts;
using Orders.Api.Data;
using Orders.Api.Domain;
using Orders.Api.Services;

namespace Orders.Api.Endpoints;

public static class OrdersEndpoints
{
    public static void MapOrdersEndpoints(this IEndpointRouteBuilder app)
    {
        var g = app.MapGroup("/api/orders").WithTags("Orders");

        g.MapGet("/", async (string? status, IOrdersService svc, CancellationToken ct) =>
        {
            OrderStatus? parsed = null;
            if (!string.IsNullOrWhiteSpace(status))
            {
                if (!Enum.TryParse<OrderStatus>(status, true, out var s))
                {
                    return Results.BadRequest(new { error = "Unknown status: " + status });
                }
                parsed = s;
            }

            var orders = await svc.ListAsync(parsed, ct);
            var dto = orders.Select(o => new OrderSummaryDto(
                o.Id, o.Reference, o.Customer?.Reference ?? "", o.PlacedUtc,
                o.Status.ToString(), o.TotalAmount)).ToList();
            return Results.Ok(dto);
        });

        g.MapGet("/{id:guid}", async (Guid id, IOrdersService svc, CancellationToken ct) =>
        {
            var o = await svc.GetAsync(id, ct);
            if (o is null)
            {
                return Results.NotFound();
            }
            return Results.Ok(ToDetail(o));
        });

        // NOTE: this endpoint does the work inline instead of calling the service.
        // It also recomputes the total with PricingHelper rather than MoneyRounding.
        // See AGENTS.md - both of these are violations. Left here on purpose.
        g.MapPost("/{id:guid}/ship", async (
            Guid id,
            ShipRequest req,
            OrdersDbContext db,
            TimeProvider clock,
            ILoggerFactory lf,
            CancellationToken ct) =>
        {
            var log = lf.CreateLogger("Ship");
            var order = await db.Orders.Include(o => o.Lines)
                .FirstOrDefaultAsync(o => o.Id == id, ct);

            if (order is null)
            {
                return Results.NotFound();
            }

            if (order.Status != OrderStatus.Placed)
            {
                return Results.Conflict(new { error = "Order is not in a shippable state." });
            }

            var now = clock.GetUtcNow();
            order.Status = OrderStatus.Shipped;
            order.ShippedUtc = now;
            order.TotalAmount = PricingHelper.RoundMoney(
                order.Lines.Sum(l => PricingHelper.LineTotal(l.Quantity, l.UnitPrice)));

            db.AuditEntries.Add(new AuditEntry
            {
                EntityId = order.Id,
                Action = "Shipped",
                Actor = req.Actor,
                OccurredUtc = now,
                Reason = "Dispatched from warehouse"
            });

            await db.SaveChangesAsync(ct);
            Console.WriteLine("shipped " + order.Reference);
            log.LogInformation("Order {Reference} shipped", order.Reference);

            return Results.Ok(ToDetail(order));
        });

        // TICKET-101
        g.MapPost("/{id:guid}/cancel", async (
            Guid id,
            CancelRequest req,
            OrdersDbContext db,
            IPaymentService payments,
            ILoggerFactory lf,
            CancellationToken ct) =>
        {
            var log = lf.CreateLogger("Cancel");

            var order = await db.Orders
                .Include(o => o.Customer)
                .Include(o => o.Lines)
                .FirstOrDefaultAsync(o => o.Id == id, ct);

            if (order is null)
            {
                return Results.NotFound();
            }

            if (order.Status == OrderStatus.Shipped)
            {
                return Results.Conflict(new { error = "already shipped" });
            }

            if (order.PlacedUtc.DateTime < DateTime.Now.AddDays(-14))
            {
                return Results.Conflict(new { error = "outside window" });
            }

            await payments.QueueRefundAsync(order.Id, order.TotalAmount, ct);

            order.Status = OrderStatus.Cancelled;
            await db.SaveChangesAsync(ct);

            log.LogInformation("Cancelled {Order} for {Customer} <{Email}>",
                order.Reference, order.Customer!.FullName, order.Customer.Email);

            return Results.Ok(ToDetail(order));
        });
    }

    internal static OrderDetailDto ToDetail(Order o) => new(
        o.Id,
        o.Reference,
        o.Customer?.Reference ?? "",
        o.Customer?.FullName ?? "",
        o.PlacedUtc,
        o.ShippedUtc,
        o.Status.ToString(),
        o.TotalAmount,
        o.Lines.Select(l => new OrderLineDto(
            l.Sku, l.Description, l.Quantity, l.UnitPrice,
            MoneyRounding.Round(l.Quantity * l.UnitPrice))).ToList());
}
