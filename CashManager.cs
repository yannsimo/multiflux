using GrpcPricing.Protos;
using MarketData;

namespace FinancialApplication
{
    public class CashManager
    {
        private double Cash;
        private readonly double Price_Option;


        public CashManager(double Price_Option_0, double initialCash)
        {

            Price_Option = Price_Option_0;
            Cash = initialCash;

        }

        public double GetCash() => Cash;

        public async void UpdateCash(double riskFreeRate, DataFeed shareValues, PricingOutput pricingResultTask, PositionManager PositionManager)
        {
            PricingOutput Results =  pricingResultTask;
            double portfolioStockValue = PositionManager.CalculateStockValue(shareValues);

            Cash = (Results.Price - portfolioStockValue) * riskFreeRate;
        }

    }

}
