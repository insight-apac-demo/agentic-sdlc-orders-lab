namespace Orders.Api.Data;

/// <summary>
/// The house money convention. Every monetary value that is stored, displayed or
/// sent to the payment provider goes through here.
/// </summary>
public static class MoneyRounding
{
    public const int Places = 2;

    public static decimal Round(decimal value) =>
        Math.Round(value, Places, MidpointRounding.ToEven);
}
