using FinancialApplication.Models;
using GrpcPricing.Protos;
using MarketData;
using multiflux.Services;
using ParameterInfo;

public class PortfolioFactory
{
    public async Task<Portfolio> CreateInitialPortfolioAsync(DataFeed firstDataFeed,
        TestParameters parameters, List<List<double>> spots)
    {
        // Calculer le pricing initial
        var initialPricing = await CalculateInitialPricing(parameters, spots);

        // Créer les positions initiales basées sur les deltas
        var initialPositions = CreateInitialPositions(parameters, initialPricing);

        // Calculer le cash initial
        double initialStockValue = CalculateInitialStockValue(initialPositions, firstDataFeed);
        double initialCash = initialPricing.Price - initialStockValue;

        return new Portfolio(initialPositions, initialCash, firstDataFeed.Date);
    }

    private async Task<PricingOutput> CalculateInitialPricing(TestParameters parameters, List<List<double>> spots)
    {
        var initialValues = parameters.PricingParams.InitialSpots;
        spots.Add(initialValues.ToList());

        var pricer = new Pricer(spots, 0.0, false);
        var result = await pricer.GetPricingOutputAsync();

        spots.RemoveAt(spots.Count - 1);
        return result;
    }

    private Dictionary<string, double> CreateInitialPositions(TestParameters parameters, PricingOutput pricing)
    {
        var positions = new Dictionary<string, double>();

        foreach (var kvp in parameters.PricingParams.UnderlyingPositions)
        {
            string symbol = kvp.Key;
            int index = kvp.Value;
            double delta = pricing.Deltas[index];
            positions.Add(symbol, delta);
        }

        return positions;
    }

    private double CalculateInitialStockValue(Dictionary<string, double> positions, DataFeed dataFeed)
    {
        return positions.Sum(position =>
        {
            if (!dataFeed.SpotList.TryGetValue(position.Key, out double price))
                throw new KeyNotFoundException($"Prix pour l'action {position.Key} introuvable.");
            return position.Value * price;
        });
    }
}