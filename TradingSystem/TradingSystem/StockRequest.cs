namespace TradingSystem;

public record StockRequest(string StockName, int StockPrice, int RequestAmount)
{
    public Money TotalCost() => new Money(StockPrice * RequestAmount) ;
};