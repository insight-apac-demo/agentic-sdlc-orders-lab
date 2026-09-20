using Microsoft.AspNetCore.Mvc.RazorPages;
using Orders.Api.Services;

namespace Orders.Api.Pages;

public class OrderModel : PageModel
{
    private readonly IOrdersService _svc;
    private readonly TimeProvider _clock;

    public OrderModel(IOrdersService svc, TimeProvider clock)
    {
        _svc = svc;
        _clock = clock;
    }

    public Domain.Order? Order { get; private set; }

    public int AgeInDays => Order is null ? 0 : (int)(_clock.GetUtcNow() - Order.PlacedUtc).TotalDays;

    public async Task OnGetAsync(Guid id, CancellationToken ct)
    {
        Order = await _svc.GetAsync(id, ct);
    }
}
