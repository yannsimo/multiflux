using FinancialApplication.Controllers;
using FinancialApplication.Services;
using MarketData;
using ParameterInfo;
using PricingLibrary.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FinancialApplication
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Usage: Program <cheminFichierJSON> <cheminFichierCSV> <cheminFichierSortieJSON>");
                Console.WriteLine("  - cheminFichierJSON: Chemin vers le fichier de paramètres JSON");
                Console.WriteLine("  - cheminFichierCSV: Chemin vers le fichier de données de marché CSV");
                Console.WriteLine("  - cheminFichierSortieJSON: Chemin vers le fichier de sortie JSON");
                return;
            }

            string parametersFilePath = args[0];
            string marketDataFilePath = args[1];
            string outputFilePath = args[2];

            try
            {
                // Validation des fichiers d'entrée
                ValidateInputFiles(parametersFilePath, marketDataFilePath);

                // Chargement des données
                Console.WriteLine("Chargement des paramètres...");
                TestParameters testParameters = LoadTestParameters(parametersFilePath);

                Console.WriteLine("Chargement des données de marché...");
                List<DataFeed> dataFeeds = LoadMarketData(marketDataFilePath);

                // Validation des données chargées
                ValidateLoadedData(testParameters, dataFeeds);

                // Traitement du portfolio
                Console.WriteLine("Traitement du portfolio...");
                var controller = new PricingController(testParameters);
                var outputDataList = await controller.CalculatePortfolioValuesAsync(dataFeeds);

                // Sauvegarde des résultats
                Console.WriteLine("Sauvegarde des résultats...");
                SaveResults(outputDataList, outputFilePath);

                Console.WriteLine($"Traitement terminé avec succès. {outputDataList.Count} entrées générées.");
                Console.WriteLine($"Résultats sauvegardés dans : {outputFilePath}");
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"Erreur : Fichier introuvable - {ex.Message}");
                Environment.Exit(1);
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Erreur : Accès refusé - {ex.Message}");
                Environment.Exit(1);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Erreur : Argument invalide - {ex.Message}");
                Environment.Exit(1);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Erreur : Opération invalide - {ex.Message}");
                Environment.Exit(1);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur inattendue : {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Détails : {ex.InnerException.Message}");
                }
                Environment.Exit(1);
            }
        }

        private static void ValidateInputFiles(string parametersFilePath, string marketDataFilePath)
        {
            if (!File.Exists(parametersFilePath))
                throw new FileNotFoundException($"Le fichier de paramètres n'existe pas : {parametersFilePath}");

            if (!File.Exists(marketDataFilePath))
                throw new FileNotFoundException($"Le fichier de données de marché n'existe pas : {marketDataFilePath}");
        }

        private static TestParameters LoadTestParameters(string filePath)
        {
            try
            {
                return JsonService.DeserializeTestParameters(filePath);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Impossible de charger les paramètres depuis {filePath}", ex);
            }
        }

        private static List<DataFeed> LoadMarketData(string filePath)
        {
            try
            {
                var csvService = new CsvService();
                var dataFeeds = csvService.ReadCsvFile(filePath);

                if (dataFeeds == null || dataFeeds.Count == 0)
                    throw new InvalidOperationException("Aucune donnée de marché n'a pu être chargée.");

                return dataFeeds;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Impossible de charger les données de marché depuis {filePath}", ex);
            }
        }

        private static void ValidateLoadedData(TestParameters testParameters, List<DataFeed> dataFeeds)
        {
            if (testParameters == null)
                throw new ArgumentException("Les paramètres de test sont null.");

            // Validation détaillée des TestParameters
            ValidateTestParameters(testParameters);

            if (dataFeeds == null || dataFeeds.Count == 0)
                throw new ArgumentException("Les données de marché sont vides ou null.");

            // Validation supplémentaire : vérifier que les dates sont dans l'ordre chronologique
            for (int i = 1; i < dataFeeds.Count; i++)
            {
                if (dataFeeds[i].Date <= dataFeeds[i - 1].Date)
                {
                    throw new ArgumentException($"Les données de marché ne sont pas dans l'ordre chronologique. " +
                        $"Problème détecté entre {dataFeeds[i - 1].Date:yyyy-MM-dd} et {dataFeeds[i].Date:yyyy-MM-dd}");
                }
            }

            Console.WriteLine($"Données validées : {dataFeeds.Count} jours de données de marché du {dataFeeds[0].Date:yyyy-MM-dd} au {dataFeeds[dataFeeds.Count - 1].Date:yyyy-MM-dd}");
        }

        private static void ValidateTestParameters(TestParameters testParameters)
        {
            var missingFields = new List<string>();

            if (testParameters.PricingParams == null)
                missingFields.Add("PricingParams");
            else
            {
                if (testParameters.PricingParams.InitialSpots == null)
                    missingFields.Add("PricingParams.InitialSpots");
                if (testParameters.PricingParams.UnderlyingPositions == null)
                    missingFields.Add("PricingParams.UnderlyingPositions");
            }

            if (testParameters.AssetDescription == null)
                missingFields.Add("AssetDescription");
            else
            {
                if (testParameters.AssetDescription.UnderlyingCurrencyCorrespondence == null)
                    missingFields.Add("AssetDescription.UnderlyingCurrencyCorrespondence");
                if (testParameters.AssetDescription.CurrencyRates == null)
                    missingFields.Add("AssetDescription.CurrencyRates");
            }

            if (testParameters.PayoffDescription == null)
                missingFields.Add("PayoffDescription");

            if (testParameters.RebalancingOracleDescription == null)
                missingFields.Add("RebalancingOracleDescription");

            if (missingFields.Any())
            {
                throw new InvalidOperationException($"Les champs suivants sont manquants ou null dans les TestParameters : {string.Join(", ", missingFields)}. " +
                    "Vérifiez que votre fichier JSON contient toutes les propriétés requises.");
            }

            Console.WriteLine("Validation des TestParameters réussie.");
        }

        private static void SaveResults(List<OutputData> outputDataList, string outputFilePath)
        {
            try
            {
                // Créer le répertoire de destination s'il n'existe pas
                string outputDirectory = Path.GetDirectoryName(outputFilePath);
                if (!string.IsNullOrEmpty(outputDirectory) && !Directory.Exists(outputDirectory))
                {
                    Directory.CreateDirectory(outputDirectory);
                }

                JsonService.SerializeOutputDataList(outputDataList, outputFilePath);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Impossible de sauvegarder les résultats dans {outputFilePath}", ex);
            }
        }
    }
}