using MarketData;
using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

public class CsvService
{
    public List<DataFeed> ReadCsvFile(string filePath)
    {
        try
        {

            filePath = filePath.Trim();

            if (string.IsNullOrWhiteSpace(filePath))
            {
                Console.WriteLine("Le chemin du fichier est vide.");
                return new List<DataFeed>();
            }

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Le fichier n'existe pas : {filePath}");
                return new List<DataFeed>();
            }

            List<ShareValue> shareValues = new List<ShareValue>();

            using (StreamReader reader = new StreamReader(filePath))
            {
                CsvConfiguration configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ",",
                    PrepareHeaderForMatch = args => args.Header.ToLower()
                };

                using (CsvReader csvReader = new CsvReader(reader, configuration))
                {
                    shareValues = csvReader.GetRecords<ShareValue>().ToList();
                }
            }


            var dataFeeds = MarketDataReader.ReadDataFeeds(shareValues);

            return dataFeeds;
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine($"Acc�s non autoris� au fichier : {ex.Message}");
            return new List<DataFeed>();
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Erreur d'E/S lors de la lecture du fichier : {ex.Message}");
            return new List<DataFeed>();
        }
        catch (CsvHelper.CsvHelperException ex)
        {
            Console.WriteLine($"Erreur lors du parsing CSV : {ex.Message}");
            return new List<DataFeed>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur inattendue lors de la lecture du fichier : {ex.Message}");
            return new List<DataFeed>();
        }
    }
}