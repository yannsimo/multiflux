using FinancialApplication;
using GrpcPricing.Protos;
using MarketData;
using ParameterInfo;

public class Portfolio
{
    private PositionManager positionManager;
    private CashManager cashManager;
    private double portfolioValue;
    private DateTime lastRebalancingDate;
    private DataFeed shareValues;
    List<List<double>> _spots;
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

    public Portfolio(Dictionary<string, double> positions, DataFeed shareValues, double portfolioValue, List<List<double>> spots)
    {
        PositionManager = new PositionManager(positions);
        ShareValues = shareValues;
        PortfolioValue = portfolioValue;
        LastRebalancingDate = shareValues.Date;
        double initialStockValue = PositionManager.CalculateStockValue(shareValues);
        double initialCash = portfolioValue - initialStockValue;

        CashManager = new CashManager(portfolioValue, initialCash);
        _spots = spots;
    }

    public double GetPortfolioValue()
    {

        return PortfolioValue;
    }

    public void SetPortfolioValue(double value)
    {
        PortfolioValue = value;
    }

    public void UpdateValue(DataFeed currentDataFeed, double riskFreeRate, List<List<double>> spots, TestParameters tes, Task<PricingOutput> pricingResultTask)
    {
        if (IsRebalancingDay(currentDataFeed.Date, LastRebalancingDate, tes.RebalancingOracleDescription.Period))
        {
            PositionManager.UpdateDeltas(pricingResultTask, tes);
            LastRebalancingDate = currentDataFeed.Date;
        }
        CashManager.UpdateCash(riskFreeRate, currentDataFeed, pricingResultTask, PositionManager);


        double stockValue = PositionManager.CalculateStockValue(currentDataFeed);

        PortfolioValue = stockValue + CashManager.GetCash();
        bool isMonitoring = tes.PayoffDescription.PaymentDates.Contains(currentDataFeed.Date);
        if (!isMonitoring)
        {
            spots.RemoveAt(spots.Count - 1);
        }
    }
    private bool IsRebalancingDay(DateTime currentDate, DateTime lastRebalancingDate, int period)
    {
        int numberOfDays = (currentDate - lastRebalancingDate).Days;
        return numberOfDays % period == 0;
    }

}
