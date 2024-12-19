using MarketData;
using ParameterInfo;

namespace FinancialApplication
{
    public class PricingController
    {
        private readonly TestParameters testParameters;
        private readonly PortfolioManager PortfolioManager;

        public PricingController(TestParameters testparameters)
        {
            testParameters = testparameters;
            PortfolioManager = new PortfolioManager(testparameters.RebalancingOracleDescription.Period);
        }

        public async Task<List<OutputData>> CalculatePortfolioValuesAsync(List<DataFeed> shareValueFeeds)
        {
            if (shareValueFeeds == null || shareValueFeeds.Count == 0)
            {
                throw new ArgumentException("shareValueFeeds cannot be null or empty", nameof(shareValueFeeds));
            }

            var outputDataList = new List<OutputData>();
            var portfolioTask = PortfolioManager.InitializeFirstDay(shareValueFeeds, outputDataList, testParameters);
            var portfolio = await portfolioTask;  // Utiliser await pour traiter le résultat de la tâche

            for (int m = 0; m < testParameters.PayoffDescription.PaymentDates.Length; m++)
            {
                for (int i = 1; i < shareValueFeeds.Count && shareValueFeeds[i].Date < testParameters.PayoffDescription.PaymentDates[m]; i++)
                {
                    PortfolioManager.ProcessDay(shareValueFeeds[i], outputDataList, testParameters, portfolio);
                }
            }

            return outputDataList;
        }

    }
}
