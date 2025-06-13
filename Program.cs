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


            var controller = new PricingController(testParameters);

            // Attendre le r?sultat de CalculatePortfolioValuesAsync
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


}