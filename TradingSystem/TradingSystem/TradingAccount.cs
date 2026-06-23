using static TradingSystem.TradeResult;

namespace TradingSystem;

public class TradingAccount(Money customerMoney)
{
    private Money _customerMoney = customerMoney;
    
    private readonly List<Position> _positions = [];
    
    public IReadOnlyList<Position> Positions => _positions;
    
    public TradeResult ProcessTrade(ITrade trade)
    {  
        var existingPosition = _positions.FirstOrDefault(x => x.StockName == trade.StockRequest.StockName);

        var proposal = trade.Propose(existingPosition, _customerMoney);

        return ApplyProposal(proposal, existingPosition);
    }

    private TradeResult ApplyProposal(TradeProposal proposal, Position? existingPosition)
    {
        switch (proposal)
        {
            case RejectedProposal:
                return InsufficientBalance;
            case AcceptedProposal accepted:
                UpdateAccount(existingPosition, accepted);
                return Success;
            default:
                return InsufficientBalance;
        }
    }

    private void UpdateAccount(Position? existingPosition, AcceptedProposal accepted)
    {
        if (existingPosition != null) _positions.Remove(existingPosition);
        if (accepted.UpdatedPosition.Amount > 0)
        {
            _positions.Add(accepted.UpdatedPosition);
        }
        _customerMoney = accepted.UpdatedBalance;
    }
}

public record Commission(double Amount)
{
    public static Commission BuyCommission => new Commission(1.5);

    public Money ToDeductFrom(Money orderValue)
    {
        var amount = (orderValue.Amount * 1.5M) / 100;    
        return new Money(amount);
    }
}

public interface ITrade
{
    StockRequest StockRequest { get; }
    TradeProposal Propose(Position? existingPosition, Money customerBalance);
}


public record Position(string StockName, int Amount)
{
    public Position Buy(int amount) => this with { Amount = Amount + amount };
    public Position Sell(int amount) => this with { Amount = Amount - amount };
}

public record SellTrade(StockRequest StockRequest) : ITrade
{
    public TradeProposal Propose(Position? existingPosition, Money customerBalance)
    {
        if (existingPosition == null || existingPosition.Amount < StockRequest.RequestAmount)
            return new RejectedProposal("Insufficient shares to sell");

        return new AcceptedProposal(
            existingPosition.Sell(StockRequest.RequestAmount),
            customerBalance.Add(StockRequest.TotalCost()));
    }
}

public record BuyTrade(StockRequest StockRequest) : ITrade
{
    private readonly Commission _commission = Commission.BuyCommission;
    public TradeProposal Propose(Position? existingPosition, Money customerBalance)
    {
        var commissionToAdd = _commission.ToDeductFrom(StockRequest.TotalCost());
        var stockWithCommissionPrice = StockRequest.TotalCost().Add(commissionToAdd);
        if (customerBalance.IsLessThan(stockWithCommissionPrice))
            return new RejectedProposal("Insufficient funds");

        var updatedPosition = existingPosition?.Buy(StockRequest.RequestAmount)
                              ?? new Position(StockRequest.StockName, StockRequest.RequestAmount);

        return new AcceptedProposal(updatedPosition, customerBalance.Subtract(stockWithCommissionPrice));
    }
}


public abstract record TradeProposal;
public record AcceptedProposal(Position UpdatedPosition, Money UpdatedBalance) : TradeProposal;
public record RejectedProposal(string Reason) : TradeProposal;
