using GrpcPricing.Protos;
using MarketData;
using multiflux;
using multiflux.Services;
using ParameterInfo;
using TimeHandler;

namespace FinancialApplication
{
    public class PortfolioProcessor
    {
         
        private readonly int RebalancingPeriod;

        public PortfolioProcessor(int rebalancingPeriod)
        {
             
            RebalancingPeriod = rebalancingPeriod;
        }

        public async Task Process(Portfolio portfolio, DataFeed currentDataFeed, List<OutputData> outputDataList, TestParameters testParameters)
        {
            // Obtenir le résultat de pricing asynchrone pour la journée actuelle
            Task<PricingOutput> pricingResultTask = PriceCurrentDayAsync(currentDataFeed, testParameters);
            if (portfolio == null)
            {
                throw new NullReferenceException("L'objet portfolio n'a pas pu être instancié.");
            }
            // Obtenir le taux sans risque
            double riskFreeRate = RiskFreeRateProvider.GetRiskFreeRateAccruedValue(testParameters, portfolio.LastRebalancingDate, currentDataFeed.Date);

            // Vérifier si c'est un jour de rééquilibrage
            if (IsRebalancingDay(currentDataFeed.Date, portfolio.LastRebalancingDate))
            {
                // Mettre à jour la valeur du portefeuille pour la journée actuelle
                portfolio.UpdateValue(currentDataFeed, riskFreeRate);

                // Créer les données de sortie asynchrone pour la journée actuelle
                OutputData output = await OutputDataService.CreateOutputDataAsync(currentDataFeed.Date, pricingResultTask, portfolio);

                // Ajouter les données de sortie à la liste des données de sortie
                outputDataList.Add(output);
            }
        }


        private async Task<PricingOutput> PriceCurrentDayAsync(DataFeed currentDataFeed, TestParameters testParameters)
        {
            double [] currentValues = ShareValueFetcher.GetShareValues(
                currentDataFeed,
                testParameters.AssetDescription.UnderlyingCurrencyCorrespondence.Keys
            );

            bool isMonitoring = testParameters.PayoffDescription.PaymentDates.Contains(currentDataFeed.Date);

            var mathDateConverter = new MathDateConverter(testParameters.NumberOfDaysInOneYear);
            double dateNow = mathDateConverter.ConvertToMathDistance(
                testParameters.PayoffDescription.CreationDate,
                currentDataFeed.Date
            );

            var pricer = new Pricer(currentValues, dateNow, isMonitoring);
            PricingOutput result = await pricer.GetPricingOutputAsync();

            return result;
        }


        private bool IsRebalancingDay(DateTime currentDate, DateTime lastRebalancingDate)
        {
            int numberOfDays = (currentDate - lastRebalancingDate).Days;
            return numberOfDays % RebalancingPeriod == 0;
        }
    }
}
