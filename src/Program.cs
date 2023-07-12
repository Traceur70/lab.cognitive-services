using System.Net.Http.Headers;
using Azure;
using Azure.AI.TextAnalytics;

Console.WriteLine("Starting...");

var credential = new AzureKeyCredential(Configuration.Instance.CognitiveServicesApiKey);
var client = new TextAnalyticsClient(new Uri(Configuration.Instance.CognitiveServicesEndPoint), credential);
var result = await client.DetectLanguageAsync("Obrigado por me ajudar com este projeto.");
Console.WriteLine(result.Value.Name);