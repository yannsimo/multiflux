using MarketData;
using multiflux;
using ParameterInfo;

namespace FinancialApplication.Controllers
{
    public class PricingController
    {
        private readonly PortfolioService _portfolioService;
        private readonly TestParameters _testParameters;

        public PricingController(TestParameters testParameters)
        {
            _testParameters = testParameters;
            var riskFreeRateProvider = new RiskFreeRateProvider();
            var pricingService = new PricingService();
            _portfolioService = new PortfolioService(pricingService, riskFreeRateProvider);
        }

        public async Task<List<OutputData>> CalculatePortfolioValuesAsync(List<DataFeed> shareValueFeeds)
        {
            return await _portfolioService.ProcessPortfolioAsync(_testParameters, shareValueFeeds);
        }
    }
}