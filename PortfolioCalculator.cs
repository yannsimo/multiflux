using FinancialApplication.Interfaces;
using FinancialApplication.Models;
using GrpcPricing.Protos;
using MarketData;
using ParameterInfo;

namespace FinancialApplication.Services
{
    /// <summary>
    /// Service responsable des calculs liés au portfolio
    /// Responsabilité : Logique métier de calcul et mise à jour
    /// </summary>
    public class PortfolioCalculator
    {
        private readonly IRiskFreeRateProvider _riskFreeRateProvider;

        public PortfolioCalculator(IRiskFreeRateProvider riskFreeRateProvider)
        {
            _riskFreeRateProvider = riskFreeRateProvider;
        }

        public void UpdatePortfolio(Portfolio portfolio, DataFeed currentDataFeed,
            PricingOutput pricingResult, TestParameters parameters)
        {
            // Vérifier si c'est un jour de rééquilibrage
            if (IsRebalancingDay(currentDataFeed.Date, portfolio.LastRebalancingDate,
                parameters.RebalancingOracleDescription.Period))
            {
                UpdatePositions(portfolio, pricingResult, parameters);
                portfolio.LastRebalancingDate = currentDataFeed.Date;
            }

            // Mettre à jour le cash
            UpdateCash(portfolio, currentDataFeed, pricingResult, parameters);
        }

        private void UpdatePositions(Portfolio portfolio, PricingOutput pricingResult, TestParameters parameters)
        {
            var positions = portfolio.Positions;
            foreach (string symbol in positions.Keys.ToList())
            {
                int index = parameters.PricingParams.UnderlyingPositions[symbol];
                positions[symbol] = pricingResult.Deltas[index];
            }
        }

        private void UpdateCash(Portfolio portfolio, DataFeed dataFeed,
            PricingOutput pricingResult, TestParameters parameters)
        {
            double riskFreeRate = _riskFreeRateProvider.GetRiskFreeRateAccruedValue(
                parameters, portfolio.LastRebalancingDate, dataFeed.Date);

            double portfolioStockValue = portfolio.CalculateStockValue(dataFeed);
            portfolio.Cash = (pricingResult.Price - portfolioStockValue) * riskFreeRate;
        }

        private bool IsRebalancingDay(DateTime currentDate, DateTime lastRebalancingDate, int period)
        {
            int numberOfDays = (currentDate - lastRebalancingDate).Days;
            return numberOfDays % period == 0;
        }
    }
}
    