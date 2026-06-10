using static TradingSystem.TradeResult;

namespace TradingSystem;

public class TradingAccount(Money customerMoney)
{
    private Money _customerMoney = customerMoney;
    private Dictionary<string, int> _positions = [];
    private List<Position> _positionsList = []; 
    
    public IReadOnlyDictionary<string, int> Positions => _positions;

    public TradeResult ProcessTrade(StockRequest stockRequest, string tradeType = "Buy")
    {
        if (tradeType == "Sell")
        {  
            _positions[stockRequest.StockName] -= stockRequest.RequestAmount;
            SellPosition(stockRequest);
            return Success;
        }

        if (_customerMoney.IsLessThan(stockRequest.TotalCost()))
        {
            return InsufficientBalance;
        }

        AddPosition(stockRequest);
        _customerMoney = _customerMoney.Subtract(stockRequest.TotalCost());
        return Success;
    }

    private void SellPosition(StockRequest stockRequest)
    {
       var positionToSell = _positionsList.FirstOrDefault(x => x.StockName == stockRequest.StockName);
        
        var position = positionToSell?.Sell(stockRequest.RequestAmount);
         _positionsList.
    }   


    private void AddPosition(StockRequest stockRequest)
    {
        var position = _positions.FirstOrDefault(x => x.Key == stockRequest.StockName);
        if (position.Key == null)
        {
            _positions.Add(stockRequest.StockName, stockRequest.RequestAmount);
        }
        else
        {
            _positions[position.Key] += stockRequest.RequestAmount;
        }
    }
}

public record Position(string StockName, int Amount)
{
    public Position Buy(int amount) => this with { Amount = Amount + amount };
    public Position Sell(int amount) => this with { Amount = Amount - amount };
}
    
