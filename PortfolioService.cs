using FinancialApplication.Interfaces;
using FinancialApplication.Models;
using FinancialApplication.Services;
using GrpcPricing.Protos;
using MarketData;
using ParameterInfo;

public class PortfolioService
{
    private readonly PortfolioFactory _portfolioFactory;
    private readonly PortfolioCalculator _portfolioCalculator;
    private readonly IPricingService _pricingService;
    private readonly List<List<double>> _spots;

    public PortfolioService(IPricingService pricingService, IRiskFreeRateProvider riskFreeRateProvider)
    {
        _portfolioFactory = new PortfolioFactory();
        _portfolioCalculator = new PortfolioCalculator(riskFreeRateProvider);
        _pricingService = pricingService;
        _spots = new List<List<double>>();
    }

    public async Task<List<OutputData>> ProcessPortfolioAsync(TestParameters parameters, List<DataFeed> dataFeeds)
    {
        if (dataFeeds == null || !dataFeeds.Any())
            throw new ArgumentException("Les données de marché ne peuvent pas être vides.");

        var outputDataList = new List<OutputData>();

        // Initialiser le portfolio avec le premier jour
        var portfolio = await _portfolioFactory.CreateInitialPortfolioAsync(dataFeeds[0], parameters, _spots);

        // Ajouter les données de sortie du premier jour
        var initialPricing = await _pricingService.GetPricingAsync(dataFeeds[0], parameters, _spots);
        var initialOutput = CreateOutputData(dataFeeds[0].Date, initialPricing, portfolio, dataFeeds[0]);
        outputDataList.Add(initialOutput);

        // Traiter les jours suivants
        for (int i = 1; i < dataFeeds.Count; i++)
        {
            var currentDataFeed = dataFeeds[i];

            // Obtenir le pricing pour le jour actuel
            var currentPricing = await _pricingService.GetPricingAsync(currentDataFeed, parameters, _spots);

            // Mettre à jour le portfolio
            _portfolioCalculator.UpdatePortfolio(portfolio, currentDataFeed, currentPricing, parameters);

            // Créer les données de sortie
            var output = CreateOutputData(currentDataFeed.Date, currentPricing, portfolio, currentDataFeed);
            outputDataList.Add(output);
        }

        return outputDataList;
    }

    private OutputData CreateOutputData(DateTime date, PricingOutput pricing, Portfolio portfolio, DataFeed currentDataFeed)
    {
        return new OutputData
        {
            Date = date,
            Value = portfolio.CalculateTotalValue(currentDataFeed),
            Deltas = pricing.Deltas.ToArray(),
            Price = pricing.Price,
            PriceStdDev = pricing.PriceStdDev,
            DeltasStdDev = pricing.DeltasStdDev.ToArray()
        };
    }
}
