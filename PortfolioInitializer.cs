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

        public async Task<Portfolio> InitializeAsync(DataFeed firstDataFeed, TestParameters testParameters, List<OutputData> outputDataList, List<List<double>> spots)
        {

            // Obtenir le résultat de la première journée
            Task<PricingOutput> resultTask = PriceFirstDay(testParameters, spots);
            PricingOutput result = await resultTask;

            // Créer le portefeuille
            Portfolio portfolio = await CreatePortfolioAsync(testParameters, firstDataFeed, resultTask, spots);

            // Générer les données de sortie
            OutputData output = await OutputDataService.CreateOutputDataAsync(portfolio.LastRebalancingDate, resultTask, portfolio);

            // Ajouter les données de sortie à la liste
            outputDataList.Add(output);

            return portfolio;
        }


        private async Task<PricingOutput> PriceFirstDay(TestParameters testParameters, List<List<double>> spots)
        {
            var initialValues = testParameters.PricingParams.InitialSpots;
            spots.Add(initialValues.ToList());

            Pricer pricer = new Pricer(spots, 0.0, false);
            PricingOutput result = await pricer.GetPricingOutputAsync();

            spots.RemoveAt(spots.Count - 1);

            return result;
        }


        private async Task<Portfolio> CreatePortfolioAsync(TestParameters testParameters, DataFeed firstDataFeed, Task<PricingOutput> pricingResultTask, List<List<double>> spots)
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
            return new Portfolio(initialPositions, firstDataFeed, results.Price, spots);
        }



    }
}
