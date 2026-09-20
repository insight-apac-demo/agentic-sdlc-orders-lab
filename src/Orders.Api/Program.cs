using Microsoft.EntityFrameworkCore;
using Orders.Api.Data;
using Orders.Api.Endpoints;
using Orders.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddDbContext<OrdersDbContext>(o =>
    o.UseSqlite(builder.Configuration.GetConnectionString("Orders") ?? "Data Source=orders.db"));
builder.Services.AddScoped<IOrdersService, OrdersService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
    db.Database.EnsureCreated();
    SeedData.Ensure(db, TimeProvider.System.GetUtcNow());
}

app.UseStaticFiles();
app.MapRazorPages();
app.MapOrdersEndpoints();

app.Run();

public partial class Program { }
