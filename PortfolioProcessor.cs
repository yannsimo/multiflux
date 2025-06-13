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




        public async Task Process(Portfolio portfolio, DataFeed currentDataFeed, List<OutputData> outputDataList, TestParameters testParameters, List<List<double>> spots)
        {
            // Obtenir le résultat de pricing asynchrone pour la journée actuelle
            Task<PricingOutput> pricingResultTask = PriceCurrentDayAsync(currentDataFeed, testParameters, spots);
            if (portfolio == null)
            {
                throw new NullReferenceException("L'objet portfolio n'a pas pu être instancié.");
            }
            // Obtenir le taux sans risque
            double riskFreeRate = RiskFreeRateProvider.GetRiskFreeRateAccruedValue(testParameters, portfolio.LastRebalancingDate, currentDataFeed.Date);

            // Vérifier si c'est un jour de rééquilibrage

            // Mettre à jour la valeur du portefeuille pour la journée actuelle
            portfolio.UpdateValue(currentDataFeed, riskFreeRate, spots, testParameters, pricingResultTask);

            // Créer les données de sortie asynchrone pour la journée actuelle


            OutputData output = await OutputDataService.CreateOutputDataAsync(currentDataFeed.Date, pricingResultTask, portfolio);

            // Ajouter les données de sortie à la liste des données de sortie
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
}
