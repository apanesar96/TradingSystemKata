using AwesomeAssertions;
using NUnit.Framework;
using static TradingSystem.TradeResult;

namespace TradingSystem;

public class TradingAccountTests
{
    [Test]
    public void GivenAnBuyOrderOf1Stock_AndCustomerMoneyOf()
    {
        var customerMoney = new Money(1);
        var result = new TradingAccount(customerMoney).ProcessTrade(new StockRequest("AAL", 1, 1));
        result.Should().Be(Success);
    }

    [Test]
    public void GivenABuyOrderWith1Pound_AndCustomerMoneyOf0()
    {
        var customerMoney = new Money(0);
        var result = new TradingAccount(customerMoney).ProcessTrade(new StockRequest("AAL", 1, 1));
        result.Should().Be(InsufficientBalance);
    }
    
    [Test]
    public void GivenABuyOrderOf2StocksAt1Pound_AndCustomerMoney1Pound_ReturnsOrderRejected()
    {
        var customerMoney = new Money(1);
        var result = new TradingAccount(customerMoney).ProcessTrade(new StockRequest("AAL" , 1, 2));
        result.Should().Be(InsufficientBalance);
    }
    
    [Test]
    public void GivenACustomerBuysTwice_AndCustomerMoneyIsEnoughForOneBuy_ReturnsOrderAcceptedThenOrderRejected()
    {
        var customerMoney = new Money(1);
        var tradingSystem = new TradingAccount(customerMoney);
        var result1 = tradingSystem.ProcessTrade(new StockRequest("AAL" , 1, 1));
        var result2 = tradingSystem.ProcessTrade(new StockRequest("AAL" , 1, 1));
        
        result1.Should().Be(Success);
        result2.Should().Be(InsufficientBalance);
    }

    [Test]
    public void GivenACustomerHasSufficientAmountToBuyTrade_ThenTheStockIsAddedToTheAccount()
    {
        var tradingAccount = new TradingAccount(new Money(100));
        var stockName = "AAL";  
        
        tradingAccount.ProcessTrade(new StockRequest( stockName , 1, 1));
        
        var expectedPositions = new Dictionary<string, int> { {stockName, 1}};
        tradingAccount.Positions.Should().BeEquivalentTo(expectedPositions);
    }

    [Test]
    public void GivenACustomerHasSufficientAmountToBuyTheSameStockTwice_ThenTheStockIsAddedToTheAccount()
    {
        var tradingAccount = new TradingAccount(new Money(100));
        var stockName = "AAL";  
        
        tradingAccount.ProcessTrade(new StockRequest( stockName , 1, 1));
        tradingAccount.ProcessTrade(new StockRequest( stockName , 1, 1));
        
        var expectedPositions = new Dictionary<string, int> { {stockName, 2}};
        tradingAccount.Positions.Should().BeEquivalentTo(expectedPositions);
    }

    [Test]
    public void GivenACustomerHasSufficientFunds_ThenTheStockRequestIsAddedToTheAccount()
    {
        var tradingAccount = new TradingAccount(new Money(100));
        var stockName = "AAL";  
        
        tradingAccount.ProcessTrade(new StockRequest( stockName , 1, 10));
        
        var expectedPositions = new Dictionary<string, int> { {stockName, 10}};
        tradingAccount.Positions.Should().BeEquivalentTo(expectedPositions);
    }

}