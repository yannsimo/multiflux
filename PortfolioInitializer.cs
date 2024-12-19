using GrpcPricing.Protos;
using MarketData;
using multiflux;
using multiflux.Services;
using ParameterInfo;
using TimeHandler;

namespace FinancialApplication
{
    public class PortfolioInitializer
    {
        


        public PortfolioInitializer()
        {
            
        }

        public async Task<Portfolio> InitializeAsync(DataFeed firstDataFeed, TestParameters testParameters, List<OutputData> outputDataList)
        {
            var client = new PricingClient();

            // Obtenir le résultat de la première journée
            Task<PricingOutput> resultTask = PriceFirstDay(testParameters);
            PricingOutput result = await resultTask;

            // Créer le portefeuille
            Portfolio portfolio = await CreatePortfolioAsync(testParameters, firstDataFeed, resultTask);

            // Générer les données de sortie
            OutputData output = await OutputDataService.CreateOutputDataAsync(portfolio.LastRebalancingDate, resultTask, portfolio);

            // Ajouter les données de sortie à la liste
            outputDataList.Add(output);

            return portfolio;
        }


        private async Task<PricingOutput> PriceFirstDay(TestParameters testParameters)
        {
            var initialValues = testParameters.PricingParams.InitialSpots;
            Pricer pricer = new Pricer(initialValues, 0.0, false);
            PricingOutput result = await pricer.GetPricingOutputAsync();

            return result;
        }

       
        private async Task<Portfolio> CreatePortfolioAsync(TestParameters testParameters, DataFeed firstDataFeed, Task<PricingOutput> pricingResultTask)
        {
            // Attendre que la tâche pricingResultTask se complète
            var results = await pricingResultTask;

            // Vérifier si le résultat est valide
            if (results == null)
            {
                throw new InvalidOperationException("La tâche pricingResultTask n'a pas retourné de résultat valide.");
            }

            // Créer un dictionnaire des positions initiales en utilisant les deltas
            Dictionary<string, double> initialPositions = new Dictionary<string, double>();

            // Parcourir le dictionnaire des positions initiales
            foreach (var key in testParameters.PricingParams.UnderlyingPositions.Keys)
            {
                double delta = results.Deltas[testParameters.PricingParams.UnderlyingPositions[key]];
                initialPositions.Add(key, delta);
            }

            // Créer et retourner le portfolio avec les positions initiales et le premier DataFeed
            return new Portfolio(initialPositions, firstDataFeed, results.Price);
        }



    }
}
