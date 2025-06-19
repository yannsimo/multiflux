using FinancialApplication.Interfaces;
using FinancialApplication;
using GrpcPricing.Protos;
using MarketData;
using multiflux.Services;
using ParameterInfo;
using TimeHandler;

namespace multiflux
{
    public class PricingService : IPricingService
    {
        public async Task<PricingOutput> GetPricingAsync(DataFeed dataFeed, TestParameters parameters, List<List<double>> spots)
        {
            double[] currentValues = ShareValueFetcher.GetShareValues(
                dataFeed,
                parameters.AssetDescription.UnderlyingCurrencyCorrespondence.Keys
            );

            bool isMonitoring = parameters.PayoffDescription.PaymentDates.Contains(dataFeed.Date);
            spots.Add(currentValues.ToList());

            var mathDateConverter = new MathDateConverter(parameters.NumberOfDaysInOneYear);
            double dateNow = mathDateConverter.ConvertToMathDistance(
                parameters.PayoffDescription.CreationDate,
                dataFeed.Date
            );

            var pricer = new Pricer(spots, dateNow, isMonitoring);
            var result = await pricer.GetPricingOutputAsync();

            // Nettoyer les spots si ce n'est pas un jour de monitoring
            if (!isMonitoring)
            {
                spots.RemoveAt(spots.Count - 1);
            }

            return result;
        }
    }
}
