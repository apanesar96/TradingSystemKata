namespace TradingSystem;

public record Money(int Amount)
{
    public Money Subtract(Money other) =>  new(Amount - other.Amount);
    public bool IsLessThan(Money other) => Amount < other.Amount;
}