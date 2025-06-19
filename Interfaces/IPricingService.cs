using GrpcPricing.Protos;
using MarketData;
using ParameterInfo;

namespace multiflux.Interfaces
{
    public interface IPricingService
    {
        Task<PricingOutput> GetPricingAsync(DataFeed dataFeed, TestParameters parameters, List<List<double>> spots);
    }
}
