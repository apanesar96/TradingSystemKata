using AwesomeAssertions;
using NUnit.Framework;
using static TradingSystem.TradeResult;

namespace TradingSystem;

public class TradingAccountTests
{
    [Test]
    public void GivenAnBuyOrderOf1Stock_AndCustomerMoneyOf()
    {
        var customerMoney = new Money(2);
        var result = new TradingAccount(customerMoney).ProcessTrade(new BuyTrade(new StockRequest("AAL", 1, 1)));
        result.Should().Be(Success);
    }

    [Test]
    public void GivenABuyOrderWith1Pound_AndCustomerMoneyOf0()
    {
        var customerMoney = new Money(0);
        var result = new TradingAccount(customerMoney).ProcessTrade(new BuyTrade(new StockRequest("AAL", 1, 1)));
        result.Should().Be(InsufficientBalance);
    }
    
    [Test]
    public void GivenABuyOrderOf2StocksAt1Pound_AndCustomerMoney1Pound_ReturnsOrderRejected()
    {
        var customerMoney = new Money(1);
        var result = new TradingAccount(customerMoney).ProcessTrade(new BuyTrade(new StockRequest("AAL" , 1, 2)));
        result.Should().Be(InsufficientBalance);
    }
    
    [Test]
    public void GivenACustomerBuysTwice_AndCustomerMoneyIsEnoughForOneBuy_ReturnsOrderAcceptedThenOrderRejected()
    {
        var customerMoney = new Money(2);
        var tradingSystem = new TradingAccount(customerMoney);
        var result1 = tradingSystem.ProcessTrade(new BuyTrade(new StockRequest("AAL" , 1, 1)));
        var result2 = tradingSystem.ProcessTrade(new BuyTrade(new StockRequest("AAL" , 1, 1)));
        
        result1.Should().Be(Success);
        result2.Should().Be(InsufficientBalance);
    }

    [Test]
    public void GivenACustomerHasSufficientAmountToBuyTrade_ThenTheStockIsAddedToTheAccount()
    {
        var tradingAccount = new TradingAccount(new Money(100));
        var stockName = "AAL";  
        
        tradingAccount.ProcessTrade(new BuyTrade(new StockRequest( stockName , 1, 1)));
        
        tradingAccount.Positions.Should().BeEquivalentTo(new[] { new Position(stockName, 1) });
    }

    [Test]
    public void GivenACustomerHasSufficientAmountToBuyTheSameStockTwice_ThenTheStockIsAddedToTheAccount()
    {
        var tradingAccount = new TradingAccount(new Money(100));
        var stockName = "AAL";  
        
        tradingAccount.ProcessTrade(new BuyTrade(new StockRequest( stockName , 1, 1)));
        tradingAccount.ProcessTrade(new BuyTrade(new StockRequest( stockName , 1, 1)));
        
        tradingAccount.Positions.Should().BeEquivalentTo(new[] { new Position(stockName, 2) });
    }

    [Test]
    public void GivenACustomerHasSufficientFunds_ThenTheStockRequestIsAddedToTheAccount()
    {
        var tradingAccount = new TradingAccount(new Money(100));
        var stockName = "AAL";  
        
        tradingAccount.ProcessTrade(new BuyTrade(new StockRequest( stockName , 1, 10)));
        
        tradingAccount.Positions.Should().BeEquivalentTo(new[] { new Position(stockName, 10) });
    }

    [Test]
    public void GivenACustomerHasSufficientFunds_WhenTheCustomerWantsToSellAShare_ThenTheShareIsReduced()
    {
        var tradingAccount = new TradingAccount(new Money(105));
        var stockName = "AAL";  
        
        tradingAccount.ProcessTrade(new BuyTrade(new StockRequest( stockName , 1, 100)));
        tradingAccount.ProcessTrade(new SellTrade(new StockRequest( stockName , 1, 10)));
        
        tradingAccount.Positions.Should().BeEquivalentTo(new[] { new Position(stockName, 90) });
    }

    [Test]
    public void GivenASellPosition_WhenStockIsSold_CustomerBalanceIsUpdated()
    {
        var tradingAccount = new TradingAccount(new Money(100));
    
        // Buy 100 shares at £1 — balance is now £0
        tradingAccount.ProcessTrade(new BuyTrade(new StockRequest("AAL", 1, 100)));
    
        // Sell 50 shares at £1 — balance SHOULD be £50
        tradingAccount.ProcessTrade(new SellTrade(new StockRequest("AAL", 1, 50)));
    
        // Buy 50 at £1 — this only works if sell credited £50 back
        var result = tradingAccount.ProcessTrade(new BuyTrade(new StockRequest("AAL", 1, 50)));
        result.Should().Be(Success);
    }

    [Test]
    public void GivenABuyPosition_WhenCustomerHasSufficientFundSForABuyButComissionDoesNotCoverBalance_OrderIsRejected()
    {
        var tradingAccount = new TradingAccount(new Money(100));
        
        var result = tradingAccount.ProcessTrade(new BuyTrade(new StockRequest("AAL", 1, 100)));
        
        result.Should().Be(InsufficientBalance);
    }
  

}