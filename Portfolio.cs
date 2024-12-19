using FinancialApplication;
using MarketData;

public class Portfolio
{
    private PositionManager positionManager;
    private CashManager cashManager;
    private double portfolioValue;
    private DateTime lastRebalancingDate;
    private DataFeed shareValues;

    public PositionManager PositionManager
    {
        get { return positionManager; }
        set { positionManager = value; }
    }

    public CashManager CashManager
    {
        get { return cashManager; }
        set { cashManager = value; }
    }

    public double PortfolioValue
    {
        get { return portfolioValue; }
        set { portfolioValue = value; }
    }

    public DateTime LastRebalancingDate
    {
        get { return lastRebalancingDate; }
        set { lastRebalancingDate = value; }
    }

    public DataFeed ShareValues
    {
        get { return shareValues; }
        set { shareValues = value; }
    }

    public Portfolio(Dictionary<string, double> positions, DataFeed shareValues, double portfolioValue)
    {
        PositionManager = new PositionManager(positions);
        ShareValues = shareValues;
        PortfolioValue = portfolioValue;
        LastRebalancingDate = shareValues.Date;
        double initialStockValue = PositionManager.CalculateStockValue(shareValues);
        double initialCash = portfolioValue - initialStockValue;
        CashManager = new CashManager(initialCash);
    }

    public double GetPortfolioValue()
    {
        double stockValue = PositionManager.CalculateStockValue(ShareValues);
        return stockValue + CashManager.GetCash();
    }

    public void SetPortfolioValue(double value)
    {
        PortfolioValue = value;
    }

    public void UpdateValue(DataFeed currentDataFeed, double riskFreeRate)
    {
        CashManager.UpdateCash(riskFreeRate, PortfolioValue, currentDataFeed, PositionManager);
        PositionManager.UpdateDeltas(currentDataFeed, CalculateDelta);
        LastRebalancingDate = currentDataFeed.Date;
        SetPortfolioValue(GetPortfolioValue());
    }

    private double CalculateDelta(DataFeed currentDataFeed)
    {
        // Placeholder pour le calcul du delta. Implémentez votre logique ici.
        return 1.0;
    }
}
