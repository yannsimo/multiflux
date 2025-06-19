/*using FinancialApplication;
using MarketData;
using ParameterInfo;

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

    // FIXED: Made this method async to properly handle sequential processing
    public async Task ProcessDayAsync(DataFeed currentDataFeed, List<OutputData> outputDataList, TestParameters testParameters, Portfolio Portfolio)
    {
        await Processor.ProcessAsync(Portfolio, currentDataFeed, outputDataList, testParameters, spots);
    }
}
*/