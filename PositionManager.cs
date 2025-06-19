using GrpcPricing.Protos;
using MarketData;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using ParameterInfo;

namespace FinancialApplication
{
    public class PositionManager
    {
        private Dictionary<string, double> Positions;

        public PositionManager(Dictionary<string, double> positions)
        {
            Positions = positions;
        }

        public double CalculateStockValue(DataFeed dataFeed)
        {

            double stockValue = 0;
            foreach (var position in Positions)
            {
                string symbol = position.Key;
                double quantity = position.Value;

                if (dataFeed.SpotList.TryGetValue(symbol, out double price))
                {
                    stockValue += quantity * price;
                }
                else
                {
                    throw new Exception($"Prix pour l'action {symbol} introuvable dans les données de la feed.");
                }
            }
            return stockValue;
        }

        public async Task UpdateDeltas(PricingOutput pricingResultTask, TestParameters tes)
        {
            var results =  pricingResultTask;

            foreach (string symbol in Positions.Keys)
            {
                double delta = results.Deltas[tes.PricingParams.UnderlyingPositions[symbol]];
                Positions[symbol] = delta;
            }
        }

        public Dictionary<string, double> GetPositions()
        {
            return Positions;
        }
    }

}
