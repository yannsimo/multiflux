using MarketData;

namespace FinancialApplication
{
    public static class ShareValueFetcher
    {
        public static double[] GetShareValues(DataFeed currentDataFeed, IEnumerable<string> shareIds)
        {
            var shareValues = new List<double>();
            foreach (var shareId in shareIds)
            {
                if (currentDataFeed.SpotList.TryGetValue(shareId, out double value))
                {
                    shareValues.Add(value);
                }
                else
                {
                    throw new KeyNotFoundException($"La valeur pour l'action '{shareId}' est introuvable dans la SpotList.");
                }
            }
            return shareValues.ToArray();
        }
    }
}
