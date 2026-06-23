namespace TradingSystem;

public record Money(decimal Amount)
{
    public Money Add(Money amount) => new(Amount + amount.Amount);
    public Money Subtract(Money other) =>  new(Amount - other.Amount);
    public bool IsLessThan(Money other) => Amount < other.Amount;

    public bool IsGreaterThan(Money other) => Amount > other.Amount;
}