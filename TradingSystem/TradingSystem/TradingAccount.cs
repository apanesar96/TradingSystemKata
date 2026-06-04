using System.Collections.ObjectModel;
using static TradingSystem.TradeResult;

namespace TradingSystem;

public class TradingAccount(int customerMoney)
{
    private int _customerMoney = customerMoney;
    private Dictionary<string, int> _positions = [];
    
    public IReadOnlyDictionary<string, int> Positions => _positions;

    public TradeResult ProcessTrade(StockRequest stockRequest)
    {
        if (_customerMoney < stockRequest.TotalCost())
        {
            return InsufficientBalance;
        }

        AddPosition(stockRequest);
        _customerMoney -= stockRequest.TotalCost();
        return Success;
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

public record StockRequest(string StockName, int StockPrice, int RequestAmount)
{
    public int TotalCost() => StockPrice * RequestAmount;
};

public enum TradeResult
{
    InsufficientBalance,
    Success,
}