namespace Orders.Api.Services;

// TODO: this duplicates Data/MoneyRounding and does NOT agree with it.
// MoneyRounding uses banker's rounding; this uses away-from-zero.
// Nobody owns this. See tickets/TICKET-103 and tickets/TICKET-104.
public static class PricingHelper
{
    public static decimal RoundMoney(decimal value)
    {
        return Math.Round(value, 2, MidpointRounding.AwayFromZero);
    }

    public static decimal LineTotal(int quantity, decimal unitPrice)
    {
        return RoundMoney(quantity * unitPrice);
    }
}
