using GrpcPricing.Protos;
using ParameterInfo;
using System;
using System.Threading.Tasks;

namespace FinancialApplication
{
    public static class OutputDataService
    {
        public static async Task<OutputData> CreateOutputDataAsync(DateTime date, Task<PricingOutput> resultsTask, Portfolio portfolio)
        {
            // Attendre le résultat de la tâche
            var results = await resultsTask;

            // Vérifier si le résultat est valide
            if (results == null)
            {
                throw new InvalidOperationException("La tâche resultsTask n'a pas retourné de résultat valide.");
            }

            // Créer un objet OutputData à partir des résultats
            return new OutputData
            {
                Date = date,
                Value = portfolio.GetPortfolioValue(),
                Deltas = results.Deltas.ToArray(), // Convertir repeated double en tableau
                Price = results.Price,
                PriceStdDev = results.PriceStdDev,
                DeltasStdDev = results.DeltasStdDev.ToArray() // Convertir repeated double en tableau
            };
        }
    }
}
