using FinancialApplication;
using MarketData;
using multiflux.Services;
using ParameterInfo;

public class PortfolioManager
{
    
    private readonly int RebalancingPeriod;
     
    private readonly PortfolioInitializer Initializer;
    private readonly PortfolioProcessor Processor;
    
    public PortfolioManager(int rebalancingPeriod)
    {
        RebalancingPeriod = rebalancingPeriod;
        Initializer = new PortfolioInitializer();
        Processor = new PortfolioProcessor(RebalancingPeriod);
    }

    public async Task<Portfolio> InitializeFirstDay(List<DataFeed> shareValueFeeds, List<OutputData> outputDataList, TestParameters testParameters)
    {
        if (shareValueFeeds == null || !shareValueFeeds.Any())
        {
            throw new ArgumentException("shareValueFeeds cannot be null or empty", nameof(shareValueFeeds));
        }

        var initializer = new PortfolioInitializer();
        Portfolio Portfolio = await initializer.InitializeAsync(shareValueFeeds.First(), testParameters, outputDataList);

        return Portfolio;
    }
    public void ProcessDay(DataFeed currentDataFeed, List<OutputData> outputDataList, TestParameters testParameters, Portfolio Portfolio)
    {
        Processor.Process(Portfolio, currentDataFeed, outputDataList, testParameters);
    }
}
