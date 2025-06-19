using GrpcPricing.Protos;
using MarketData;
using ParameterInfo;

namespace FinancialApplication.Models
{
    /// <summary>
    /// Modèle de données pur représentant un portfolio
    /// Responsabilité : Stocker les données uniquement
    /// </summary>
    public class Portfolio
    {
        public Dictionary<string, double> Positions { get; }
        public double Cash { get; set; }
        public DateTime LastRebalancingDate { get; set; }

        public Portfolio(Dictionary<string, double> initialPositions, double initialCash, DateTime initialDate)
        {
            Positions = new Dictionary<string, double>(initialPositions);
            Cash = initialCash;
            LastRebalancingDate = initialDate;
        }

        public double CalculateTotalValue(DataFeed dataFeed)
        {
            double stockValue = Positions.Sum(position =>
            {
                if (!dataFeed.SpotList.TryGetValue(position.Key, out double price))
                    throw new KeyNotFoundException($"Prix pour l'action {position.Key} introuvable.");
                return position.Value * price;
            });

            return stockValue + Cash;
        }

        public double CalculateStockValue(DataFeed dataFeed)
        {
            return Positions.Sum(position =>
            {
                if (!dataFeed.SpotList.TryGetValue(position.Key, out double price))
                    throw new KeyNotFoundException($"Prix pour l'action {position.Key} introuvable.");
                return position.Value * price;
            });
        }
    }
}
