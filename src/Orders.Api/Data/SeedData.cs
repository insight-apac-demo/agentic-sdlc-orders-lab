using Microsoft.EntityFrameworkCore;
using Orders.Api.Domain;

namespace Orders.Api.Data;

public static class SeedData
{
    /// <summary>
    /// Deterministic seed. Same data every run, so a demo can be reset and repeated.
    /// Ages are relative to now, so the 14-day boundary always has orders either side of it.
    /// </summary>
    public static void Ensure(OrdersDbContext db, DateTimeOffset now)
    {
        if (db.Orders.Any())
        {
            return;
        }

        var customers = new[]
        {
            new Customer { Reference = "C-1180", FullName = "Priya Raman",   Email = "priya.raman@example.com",  Phone = "0400 111 222" },
            new Customer { Reference = "C-2291", FullName = "Tom Whitfield", Email = "t.whitfield@example.com",  Phone = "0400 333 444" },
            new Customer { Reference = "C-9902", FullName = "Aiko Tanaka",   Email = "aiko.tanaka@example.com",  Phone = "0400 555 666" },
            new Customer { Reference = "C-4417", FullName = "Dev Mehta",     Email = "dev.mehta@example.com",    Phone = "0400 777 888" }
        };
        db.Customers.AddRange(customers);

        var specs = new (string Reference, int CustomerIndex, double AgeDays, OrderStatus Status, (string Sku, string Desc, int Qty, decimal Price)[] Lines)[]
        {
            ("ORD-4412", 2, 16.0, OrderStatus.Shipped,   new[] { ("KB-01", "Mechanical keyboard", 1, 189.00m), ("CB-22", "USB-C cable", 2, 14.50m) }),
            ("ORD-4416", 1,  3.0, OrderStatus.Placed,    new[] { ("MS-07", "Wireless mouse", 1, 79.95m) }),
            ("ORD-4417", 0, 12.0, OrderStatus.Placed,    new[] { ("MN-30", "27 inch monitor", 1, 249.00m), ("ST-02", "Monitor stand", 1, 35.00m) }),
            ("ORD-4418", 3, 13.8, OrderStatus.Placed,    new[] { ("HP-11", "Headset", 1, 129.99m) }),
            ("ORD-4419", 2,  0.5, OrderStatus.Placed,    new[] { ("DK-05", "Docking station", 1, 219.00m), ("CB-22", "USB-C cable", 1, 14.50m) }),
            ("ORD-4420", 1, 21.0, OrderStatus.Delivered, new[] { ("CH-09", "Desk chair", 1, 449.00m) }),
            ("ORD-4421", 0,  7.2, OrderStatus.Placed,    new[] { ("LP-77", "Laptop sleeve", 3, 24.95m) }),
            ("ORD-4422", 3, 15.4, OrderStatus.Placed,    new[] { ("WB-18", "Webcam", 1, 96.00m), ("CB-22", "USB-C cable", 1, 14.50m) })
        };

        foreach (var s in specs)
        {
            var order = new Order
            {
                Reference = s.Reference,
                CustomerId = customers[s.CustomerIndex].Id,
                PlacedUtc = now.AddDays(-s.AgeDays),
                Status = s.Status,
                ShippedUtc = s.Status is OrderStatus.Shipped or OrderStatus.Delivered
                    ? now.AddDays(-s.AgeDays + 1)
                    : null
            };
            foreach (var l in s.Lines)
            {
                order.Lines.Add(new OrderLine
                {
                    OrderId = order.Id,
                    Sku = l.Sku,
                    Description = l.Desc,
                    Quantity = l.Qty,
                    UnitPrice = l.Price
                });
            }
            order.TotalAmount = MoneyRounding.Round(order.Lines.Sum(l => l.Quantity * l.UnitPrice));
            db.Orders.Add(order);
        }

        db.SaveChanges();
    }
}
