using FinancialApplication;
using MarketData;
using ModelConverter;
using ParameterInfo;
using PricingLibrary.Services;
using System;
using System.Collections.Generic;
using System.Text.Json;

public class PrincipalesProgrammes
{
    public static async Task Main(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Usage: PrincipalesProgrammes <cheminFichierJSON> <cheminFichierCSV>");
            return;
        }
        try
        {

            TestParameters testParameters = JsonService.DeserializeTestParameters(args[0]);
            CsvService csvService = new CsvService();
            List<DataFeed> dataFeeds = csvService.ReadCsvFile(args[1]);
            AfficherDetailsTestParameters(testParameters);

            var controller = new PricingController(testParameters);

            // Attendre le r�sultat de CalculatePortfolioValuesAsync
            var outputDataListTask = controller.CalculatePortfolioValuesAsync(dataFeeds);
            var outputDataList = await outputDataListTask;

            JsonService.SerializeOutputDataList(outputDataList, args[2]);
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine("Erreur de fichier : " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erreur inattendue : " + ex.Message);
        }
    }

    /// <summary>
    /// Affiche les d�tails du TestParameters de mani�re d�taill�e et lisible
    /// </summary>
    private static void AfficherDetailsTestParameters(TestParameters testParameters)
    {
        Console.WriteLine("\n--- D�tails des TestParameters ---");

        // Nombre de jours dans une ann�e
        Console.WriteLine($"Nombre de jours dans une ann�e : {testParameters.NumberOfDaysInOneYear}");

        // Description des actifs
        if (testParameters.AssetDescription != null)
        {
            Console.WriteLine("\nDescription des Actifs:");
            Console.WriteLine("Correspondance des devises sous-jacentes:");
            foreach (var correspondence in testParameters.AssetDescription.UnderlyingCurrencyCorrespondence)
            {
                Console.WriteLine($"  {correspondence.Key}: {correspondence.Value}");
            }

            Console.WriteLine("\nTaux de change:");
            foreach (var rate in testParameters.AssetDescription.CurrencyRates)
            {
                Console.WriteLine($"  {rate.Key}: {rate.Value}");
            }

            Console.WriteLine($"\nDevise domestique : {testParameters.AssetDescription.DomesticCurrencyId}");
        }

        // Description du r�balancement
        if (testParameters.RebalancingOracleDescription != null)
        {
            Console.WriteLine("\nDescription du R�balancement:");
            Console.WriteLine($"  P�riode : {testParameters.RebalancingOracleDescription.Period}");
            Console.WriteLine($"  Type : {testParameters.RebalancingOracleDescription.Type}");
        }

        // Description du payoff
        if (testParameters.PayoffDescription != null)
        {
            Console.WriteLine("\nDescription du Payoff:");
            Console.WriteLine($"  Type : {testParameters.PayoffDescription.Type}");
            Console.WriteLine($"  Date de cr�ation : {testParameters.PayoffDescription.CreationDate}");

            Console.WriteLine("  Dates de paiement :");
            foreach (var date in testParameters.PayoffDescription.PaymentDates)
            {
                Console.WriteLine($"    - {date}");
            }

            Console.WriteLine("  Strikes :");
            foreach (var strike in testParameters.PayoffDescription.Strikes)
            {
                Console.WriteLine($"    - {strike}");
            }
        }

        // Param�tres de pricing
        if (testParameters.PricingParams != null)
        {
            Console.WriteLine("\nParam�tres de Pricing:");

            Console.WriteLine("  Positions des sous-jacents :");
            foreach (var position in testParameters.PricingParams.UnderlyingPositions)
            {
                Console.WriteLine($"    {position.Key}: {position.Value}");
            }

            Console.WriteLine("  Volatilit�s :");
            for (int i = 0; i < testParameters.PricingParams.Volatilities.Length; i++)
            {
                Console.WriteLine($"    ID {i}: {testParameters.PricingParams.Volatilities[i]}");
            }

            Console.WriteLine("  Corr�lations :");
            for (int i = 0; i < testParameters.PricingParams.Correlations.Length; i++)
            {
                Console.Write("    ");
                for (int j = 0; j < testParameters.PricingParams.Correlations[i].Length; j++)
                {
                    Console.Write($"{testParameters.PricingParams.Correlations[i][j]} ");
                }
                Console.WriteLine();
            }

            Console.WriteLine("  Prix initiaux :");
            for (int i = 0; i < testParameters.PricingParams.InitialSpots.Length; i++)
            {
                Console.WriteLine($"    ID {i}: {testParameters.PricingParams.InitialSpots[i]}");
            }

            Console.WriteLine("  Tendances :");
            for (int i = 0; i < testParameters.PricingParams.Trends.Length; i++)
            {
                Console.WriteLine($"    ID {i}: {testParameters.PricingParams.Trends[i]}");
            }

            Console.WriteLine($"  Nombre d'�chantillons : {testParameters.PricingParams.SampleNb}");
            Console.WriteLine($"  �tape de diff�rence finie relative : {testParameters.PricingParams.RelativeFiniteDifferenceStep}");
        }
    }
}