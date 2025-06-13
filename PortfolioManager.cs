using FinancialApplication;
using MarketData;
using multiflux.Services;
using ParameterInfo;
using System.Collections.Generic;

public class PortfolioManager
{



    private readonly PortfolioInitializer Initializer;
    private readonly PortfolioProcessor Processor;
    private List<List<double>> spots;

    public PortfolioManager()
    {

        Initializer = new PortfolioInitializer();
        Processor = new PortfolioProcessor();
        spots = new List<List<double>>();
    }

    public async Task<Portfolio> InitializeFirstDay(List<DataFeed> shareValueFeeds, List<OutputData> outputDataList, TestParameters testParameters)
    {
        if (shareValueFeeds == null || !shareValueFeeds.Any())
        {
            throw new ArgumentException("shareValueFeeds cannot be null or empty", nameof(shareValueFeeds));
        }

        var initializer = new PortfolioInitializer();
        Portfolio Portfolio = await initializer.InitializeAsync(shareValueFeeds.First(), testParameters, outputDataList, spots);

        return Portfolio;
    }
    public void ProcessDay(DataFeed currentDataFeed, List<OutputData> outputDataList, TestParameters testParameters, Portfolio Portfolio)
    {
        Processor.Process(Portfolio, currentDataFeed, outputDataList, testParameters, spots);
    }
}
