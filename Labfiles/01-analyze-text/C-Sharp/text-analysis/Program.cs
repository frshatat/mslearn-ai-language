using System;
using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;

// Import namespaces
using Azure;
using Azure.AI.TextAnalytics;

namespace text_analysis
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Get config settings from AppSettings
                IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
                IConfigurationRoot configuration = builder.Build();
                string aiSvcEndpoint = configuration["AIServicesEndpoint"];
                string aiSvcKey = configuration["AIServicesKey"];

                // Create client using endpoint and key
                AzureKeyCredential credentials = new AzureKeyCredential(aiSvcKey);
                Uri endpoint = new Uri(aiSvcEndpoint);
                TextAnalyticsClient aiClient = new TextAnalyticsClient(endpoint, credentials);

                // Analyze each text file in the reviews folder
                var folderPath = Path.GetFullPath("./reviews");  
                DirectoryInfo folder = new DirectoryInfo(folderPath);
                foreach (var file in folder.GetFiles("*.txt"))
                {
                    // Read the file contents
                    Console.WriteLine("\n-------------\n" + file.Name);
                    StreamReader sr = file.OpenText();
                    var text = sr.ReadToEnd();
                    sr.Close();
                    Console.WriteLine("\n" + text);
                    Console.WriteLine("\n-------------\n");

                    // Get language
                    DetectedLanguage language = aiClient.DetectLanguage(text);
                    Console.WriteLine($"Language: {language.Name}");

                    // Get sentiment
                    DocumentSentiment sentiment = aiClient.AnalyzeSentiment(text);
                    Console.WriteLine($"Sentiment: {sentiment.Sentiment}");
                    Console.WriteLine("\n-------------\n");

                    // Get key phrases
                    KeyPhraseCollection keyPhrases = aiClient.ExtractKeyPhrases(text);
                    Console.WriteLine("Key Phrases:");
                    foreach(string keyPhrase in keyPhrases)
                    {
                        Console.WriteLine($"Key Phrase: {keyPhrase}");
                    }
                    Console.WriteLine("\n-------------");

                    // Get entities
                    CategorizedEntityCollection entities = aiClient.RecognizeEntities(text);
                    Console.WriteLine("Entities:");
                    foreach (CategorizedEntity entity in entities)
                    {
                        Console.WriteLine($"Entity: {entity.Text} ({entity.Category})");
                    }
                    Console.WriteLine("\n-------------\n");

                    // Get linked entities
                    LinkedEntityCollection linkedEntities = aiClient.RecognizeLinkedEntities(text);
                    Console.WriteLine("Linked Entities:");
                    foreach (LinkedEntity linkedEntity in linkedEntities)
                    {
                        Console.WriteLine($"Linked Entity: {linkedEntity.Name} ({linkedEntity.DataSource})");
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }



    }
}
