using MarketData;

namespace FinancialApplication
{
    public class CashManager
    {
        private double Cash;
        


        public CashManager(double initialCash)
        {
            Cash = initialCash;
        }
        
        public double GetCash() => Cash;

        public void UpdateCash(double riskFreeRate, double portfolioValue, DataFeed shareValues, PositionManager PositionManager)
        {
            double portfolioStockValue = PositionManager.CalculateStockValue(shareValues);
            Cash = (portfolioValue - portfolioStockValue)*riskFreeRate;
        }
       
    }

}
