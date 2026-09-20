using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Orders.Api.Domain;

namespace Orders.Api.Data;

public class OrdersDbContext : DbContext
{
    public OrdersDbContext(DbContextOptions<OrdersDbContext> options) : base(options) { }

    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderLine> OrderLines => Set<OrderLine>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<AuditEntry> AuditEntries => Set<AuditEntry>();

    // SQLite has no native DateTimeOffset, and cannot ORDER BY one. Everything is
    // stored as UTC ticks, which sorts correctly and round-trips exactly.
    private static readonly ValueConverter<DateTimeOffset, long> UtcTicks =
        new(v => v.UtcTicks, v => new DateTimeOffset(v, TimeSpan.Zero));

    private static readonly ValueConverter<DateTimeOffset?, long?> NullableUtcTicks =
        new(v => v.HasValue ? v.Value.UtcTicks : null,
            v => v.HasValue ? new DateTimeOffset(v.Value, TimeSpan.Zero) : null);

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Order>().HasIndex(o => o.Reference).IsUnique();
        b.Entity<Order>().Property(o => o.PlacedUtc).HasConversion(UtcTicks);
        b.Entity<Order>().Property(o => o.ShippedUtc).HasConversion(NullableUtcTicks);
        b.Entity<Order>().HasMany(o => o.Lines).WithOne().HasForeignKey(l => l.OrderId);

        b.Entity<AuditEntry>().Property(a => a.OccurredUtc).HasConversion(UtcTicks);
        b.Entity<AuditEntry>().HasIndex(a => new { a.EntityType, a.EntityId });

        b.Entity<Customer>().HasIndex(c => c.Reference).IsUnique();
    }
}
