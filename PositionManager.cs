using MarketData;

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
       
        public void UpdateDeltas(DataFeed dataFeed, Func<DataFeed, double> calculateDelta)
        {
            foreach (var symbol in Positions.Keys)
            {
                Positions[symbol] = calculateDelta(dataFeed);
            }
        }

        public Dictionary<string, double> GetPositions()
        {
            return Positions;
        }
    }

}
