/*using GrpcPricing.Protos;
using MarketData;
using multiflux;
using multiflux.Services;
using ParameterInfo;
using TimeHandler;

namespace FinancialApplication
{
    public class PortfolioProcessor
    {
        // FIXED: Made method properly async and renamed to ProcessAsync
        public async Task ProcessAsync(Portfolio portfolio, DataFeed currentDataFeed, List<OutputData> outputDataList, TestParameters testParameters, List<List<double>> spots)
        {
            if (portfolio == null)
            {
                throw new NullReferenceException("L'objet portfolio n'a pas pu être instancié.");
            }

            // FIXED: Await the pricing result before proceeding
            PricingOutput pricingResult = await PriceCurrentDayAsync(currentDataFeed, testParameters, spots);

            // Obtenir le taux sans risque
            double riskFreeRate = RiskFreeRateProvider.GetRiskFreeRateAccruedValue(testParameters, portfolio.LastRebalancingDate, currentDataFeed.Date);

            // Mettre à jour la valeur du portefeuille pour la journée actuelle
            // FIXED: Pass the actual result instead of the task
            portfolio.UpdateValue(currentDataFeed, riskFreeRate, spots, testParameters, pricingResult);

            // FIXED: Create output data with the actual pricing result
            OutputData output = await OutputDataService.CreateOutputDataAsync(currentDataFeed.Date, pricingResult, portfolio);

            // FIXED: Add to list after all async operations are complete
            outputDataList.Add(output);
        }

        private async Task<PricingOutput> PriceCurrentDayAsync(DataFeed currentDataFeed, TestParameters testParameters, List<List<double>> spots)
        {
            double[] currentValues = ShareValueFetcher.GetShareValues(
                currentDataFeed,
                testParameters.AssetDescription.UnderlyingCurrencyCorrespondence.Keys
            );

            bool isMonitoring = testParameters.PayoffDescription.PaymentDates.Contains(currentDataFeed.Date);
            spots.Add(currentValues.ToList());

            var mathDateConverter = new MathDateConverter(testParameters.NumberOfDaysInOneYear);
            double dateNow = mathDateConverter.ConvertToMathDistance(
                testParameters.PayoffDescription.CreationDate,
                currentDataFeed.Date
            );

            var pricer = new Pricer(spots, dateNow, isMonitoring);
            PricingOutput result = await pricer.GetPricingOutputAsync();
            return result;
        }
    }
}*/