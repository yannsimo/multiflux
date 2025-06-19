using GrpcPricing.Protos;
using MarketData;
using ParameterInfo;

namespace FinancialApplication.Interfaces
{
    public interface IRiskFreeRateProvider
    {
        double GetRiskFreeRateAccruedValue(TestParameters parameters, DateTime startDate, DateTime endDate);
    }

    public interface IPricingService
    {
        Task<PricingOutput> GetPricingAsync(DataFeed dataFeed, TestParameters parameters, List<List<double>> spots);
    }
}