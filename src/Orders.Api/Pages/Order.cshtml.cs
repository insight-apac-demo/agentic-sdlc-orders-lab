using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Orders.Api.Domain;
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

    public Order? Order { get; private set; }
    public string? Message { get; private set; }

    public int AgeInDays => Order is null ? 0 : (int)(_clock.GetUtcNow() - Order.PlacedUtc).TotalDays;

    public bool CanCancel => Order is not null && _svc.IsCancellable(Order);

    public string WhyNot
    {
        get
        {
            if (Order is null) return "";
            if (Order.Status != OrderStatus.Placed) return "Only an order still in Placed can be cancelled.";
            return "Outside the 14 day cancellation window.";
        }
    }

    public async Task OnGetAsync(Guid id, CancellationToken ct)
    {
        Order = await _svc.GetAsync(id, ct);
    }

    public async Task<IActionResult> OnPostAsync(Guid id, string? reason, CancellationToken ct)
    {
        var actor = User.Identity?.Name ?? "ops.console";
        var result = await _svc.CancelAsync(id, actor, reason ?? "", ct);

        Order = await _svc.GetAsync(id, ct);
        Message = result.Outcome switch
        {
            CancelOutcome.Cancelled => "Cancelled. Refund " + result.RefundReference + " queued.",
            CancelOutcome.NotCancellable => "Not cancelled: the order is not in a cancellable state.",
            CancelOutcome.OutsideWindow => "Not cancelled: outside the 14 day window.",
            _ => "Not cancelled: order not found."
        };
        return Page();
    }
}
