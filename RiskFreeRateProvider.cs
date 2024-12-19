using ParameterInfo;
using System;

namespace FinancialApplication
{
    public class RiskFreeRateProvider
    {
        public static double GetRiskFreeRateAccruedValue(TestParameters testParameters, DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("La date de début ne peut pas être postérieure à la date de fin.");
            string domesticCurrencyId = testParameters.AssetDescription.DomesticCurrencyId;
            if (!testParameters.AssetDescription.CurrencyRates.TryGetValue(domesticCurrencyId, out double annualRiskFreeRate))
                throw new KeyNotFoundException($"Le taux sans risque pour la devise '{domesticCurrencyId}' est introuvable.");
            int numberOfDays = (endDate - startDate).Days;
            double accruedValue = Math.Pow(1 + annualRiskFreeRate, numberOfDays / testParameters.NumberOfDaysInOneYear);
            return accruedValue;
        }
    }
}
