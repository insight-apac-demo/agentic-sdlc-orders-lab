using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Orders.Api.Data;
using Orders.Api.Services;

namespace Orders.Tests;

/// <summary>
/// One in-memory SQLite database per test, seeded deterministically.
/// </summary>
public sealed class TestHost : IDisposable
{
    public SqliteConnection Connection { get; }
    public OrdersDbContext Db { get; }
    public FakeTimeProvider Clock { get; }
    public OrdersService Service { get; }

    public TestHost(DateTimeOffset? now = null)
    {
        Clock = new FakeTimeProvider(now ?? new DateTimeOffset(2026, 6, 15, 9, 0, 0, TimeSpan.Zero));
        Connection = new SqliteConnection("Filename=:memory:");
        Connection.Open();

        var options = new DbContextOptionsBuilder<OrdersDbContext>()
            .UseSqlite(Connection)
            .Options;

        Db = new OrdersDbContext(options);
        Db.Database.EnsureCreated();
        SeedData.Ensure(Db, Clock.GetUtcNow());

        Service = new OrdersService(Db, Clock, NullLogger<OrdersService>.Instance);
    }

    public void Dispose()
    {
        Db.Dispose();
        Connection.Dispose();
    }
}

public sealed class FakeTimeProvider : TimeProvider
{
    private DateTimeOffset _now;
    public FakeTimeProvider(DateTimeOffset now) => _now = now;
    public override DateTimeOffset GetUtcNow() => _now;
    public void Advance(TimeSpan by) => _now = _now.Add(by);
}
