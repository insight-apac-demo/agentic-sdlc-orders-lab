using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Orders.Api.Domain;
using Orders.Api.Services;

namespace Orders.Api.Pages;

public class IndexModel : PageModel
{
    private readonly IOrdersService _svc;
    private readonly TimeProvider _clock;

    public IndexModel(IOrdersService svc, TimeProvider clock)
    {
        _svc = svc;
        _clock = clock;
    }

    [BindProperty(SupportsGet = true)]
    public string? Status { get; set; }

    public IReadOnlyList<Order> Orders { get; private set; } = Array.Empty<Order>();

    public string[] StatusOptions => Enum.GetNames<OrderStatus>();

    public async Task OnGetAsync(CancellationToken ct)
    {
        OrderStatus? parsed = Enum.TryParse<OrderStatus>(Status, true, out var s) ? s : null;
        Orders = await _svc.ListAsync(parsed, ct);
    }

    public int AgeInDays(Order o) => (int)(_clock.GetUtcNow() - o.PlacedUtc).TotalDays;
}
