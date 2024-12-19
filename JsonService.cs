using System;
using System.IO;
using ModelConverter;
using ParameterInfo;

namespace PricingLibrary.Services
{
    public static class JsonService
    {
        /// <summary>
        /// Désérialise le contenu d'un fichier JSON dans un objet TestParameters.
        /// </summary>
        /// <param name="filePath">Le chemin du fichier JSON.</param>
        /// <returns>Un objet TestParameters.</returns>
        ///  Converters
        ///  
        
        public static TestParameters DeserializeTestParameters(string filePath)
        {
           
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Le fichier spécifié est introuvable : {filePath}");

            string jsonContent = File.ReadAllText(filePath);
            return ParameterInfo.JsonUtils.JsonIO.FromJson(jsonContent);
        }
        //J'ai besoin d'une autre fonction ici pour Serialiser List<OutData> Objet  en format json un objet je vais te donner 
        //je vais t'envoyer la composition de la classe ParameterInfo.JsonUtils.JsonIO peut tu me dise si ça peut aider


        public static void SerializeOutputDataList(List<OutputData> outputDataList, string filePath)
        {
            if (outputDataList == null)
                throw new ArgumentNullException(nameof(outputDataList), "La liste OutputData ne peut pas être nulle.");
            string jsonContent = ParameterInfo.JsonUtils.JsonIO.ToJson(outputDataList);
            File.WriteAllText(filePath, jsonContent);
        }
    }
}