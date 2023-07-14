using Azure;
using Azure.AI.TextAnalytics;
using Microsoft.CognitiveServices.Speech;
using Spectre.Console;

public class LabSpeechService
{
    private static readonly AzureKeyCredential credential = new(Configuration.Instance.CognitiveServicesApiKey);

    private static readonly TextAnalyticsClient client = new(new Uri(Configuration.Instance.CognitiveServicesEndPoint), credential);


    public void RecognizeEntities()
    {
        var result = client.RecognizeEntities(Helpers.PromptText());
        Console.WriteLine(string.Join(", ", result.Value.Select(v => v.Text)));
    }



    public void ExtractKeyPhrases()
    {
        var input = Helpers.PromptText();

        var language = client.DetectLanguage(input);
        var confidenceColor = language.Value.ConfidenceScore switch
        {
            var score when score > 0.8 => Color.Green,
            var score when score > 0.5 => Color.Yellow,
            _ => Color.Red
        };
        Helpers.WriteInPanel("Language", $"{language.Value.Name} ({language.Value.Iso6391Name}) [{confidenceColor}]{language.Value.ConfidenceScore.ToString("P0")}[/]");

        var keyphrases = client.ExtractKeyPhrases(input);
        Helpers.WriteInPanel("Key phrases", keyphrases.Value.OrderBy(v => v).ToArray());
        
        var linkedEntities = client.RecognizeLinkedEntities(input)
            .Value
            .OrderBy(v => v.Name)
            .Select(v => $"{v.Name} => [link={v.Url}]{v.DataSource}[/]")
            .ToArray();
        Helpers.WriteInPanel("Linked entities", linkedEntities);

        var sentiments = client.AnalyzeSentiment(input);
        Helpers.WriteInPanel("Sentiments", new BreakdownChart()
            .Width(60)
            .ShowPercentage()
            .AddItem("Positive", Math.Ceiling(sentiments.Value.ConfidenceScores.Positive * 100), Color.Green)
            .AddItem("Neutral", Math.Ceiling(sentiments.Value.ConfidenceScores.Neutral * 100), Color.Blue)
            .AddItem("Negative", Math.Ceiling(sentiments.Value.ConfidenceScores.Negative * 100), Color.Red));
    }

    /// <summary>
    /// https://docs.microsoft.com/en-us/azure/cognitive-services/speech-service/speech-synthesis-markup?tabs=csharp
    /// </summary>
    public async Task SpeechAsync()
    {
        var speechConfig = SpeechConfig.FromSubscription(Configuration.Instance.CognitiveServicesApiKey, Configuration.Instance.CognitiveServicesRegion);
        speechConfig.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Riff24Khz16BitMonoPcm);
        speechConfig.SpeechSynthesisVoiceName = "en-GB-George";

        var speechSynthesizer = new SpeechSynthesizer(speechConfig);
        var result = await speechSynthesizer.SpeakSsmlAsync(Helpers.PromptText(ConsoleInput.Document));
    }
}