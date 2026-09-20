namespace Orders.Api.Services;

public enum CancelOutcome
{
    Cancelled,
    NotFound,
    NotCancellable,
    OutsideWindow
}

public record CancelResult(CancelOutcome Outcome, string? RefundReference = null)
{
    public bool Succeeded => Outcome == CancelOutcome.Cancelled;
}
