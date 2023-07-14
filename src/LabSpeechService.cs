using Azure;
using Azure.AI.TextAnalytics;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Spectre.Console;

public class LabSpeechService
{
    private static readonly AzureKeyCredential credential = new(Configuration.Instance.CognitiveServicesApiKey);

    private static readonly TextAnalyticsClient client = new(new Uri(Configuration.Instance.CognitiveServicesEndPoint), credential);


    public void RecognizeLinkedEntities()
    {
        var result = client.RecognizeLinkedEntities(Helpers.PromptText());
        Console.WriteLine(string.Join(", ", result.Value.Select(v => v.Url)));
    }

    public void RecognizeEntities()
    {
        var result = client.RecognizeEntities(Helpers.PromptText());
        Console.WriteLine(string.Join(", ", result.Value.Select(v => v.Text)));
    }

    public void AnalyzeSentiment()
    {
        var result = client.AnalyzeSentiment(Helpers.PromptText());
        AnsiConsole.Write(new BarChart { Width = 80 }
            // .ShowPercentage()
            .AddItem("Positive", Math.Ceiling(result.Value.ConfidenceScores.Positive * 100), Color.Green)
            .AddItem("Neutral", Math.Ceiling(result.Value.ConfidenceScores.Neutral * 100), Color.Blue)
            .AddItem("Negative", Math.Ceiling(result.Value.ConfidenceScores.Negative * 100), Color.Red));
    }


    public void ExtractKeyPhrases()
    {
        var input = Helpers.PromptText();
        var result = client.ExtractKeyPhrases(input);
        var keyphrasesNodes = new Tree("[bold][/]");
        Helpers.WriteInPanel("Key phrases", result.Value.OrderBy(v => v).ToArray());
        Console.WriteLine("");

        var sentiments = client.AnalyzeSentiment(input);
        Helpers.WriteInPanel("Sentiments", new Panel(new BreakdownChart()
                .ShowPercentage()
                .Width(60)
                .AddItem("Positive", Math.Ceiling(sentiments.Value.ConfidenceScores.Positive * 100), Color.Green)
                .AddItem("Neutral", Math.Ceiling(sentiments.Value.ConfidenceScores.Neutral * 100), Color.Blue)
                .AddItem("Negative", Math.Ceiling(sentiments.Value.ConfidenceScores.Negative * 100), Color.Red)));
    }

    public void DetectLanguage()
    {
        var result = client.DetectLanguage(Helpers.PromptText(ConsoleInput.Inline, ConsoleInput.Document, ConsoleInput.Random));
        Console.WriteLine(result.Value.Name);
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