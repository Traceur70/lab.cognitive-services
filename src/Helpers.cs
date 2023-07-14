using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Spectre.Console;
using Spectre.Console.Rendering;

public class Helpers
{

    public static void WriteInPanel(string header, IRenderable content) =>
        AnsiConsole.Write(new Panel(content)
        { 
            Header = new($"[bold gray]| {header.ToUpper()} |[/] "),
            Border = BoxBorder.Double
        });

    public static void WriteInPanel(string header, params string[] contents)
    {
        if(contents.Length == 0)
            contents = new[] { "[i]No result[/]" };
        var firstNumberOfCharacters = contents[0].Length;
        var missingNbOfChars = header.Length + 5 - contents[0].Length;
        if(missingNbOfChars > 0)
        {
            contents[0] += new string(' ', missingNbOfChars);
        }
        AnsiConsole.Write(new Panel(string.Join(Environment.NewLine, contents))
        {
            Header = new($"[bold gray]| {header.ToUpper()} |[/]"),
            Border = BoxBorder.Double
        });
    }

    public static string PromptText(params ConsoleInput[] inputs)
    {
        if (inputs.Length == 0)
            inputs = Enum.GetValues<ConsoleInput>();

        // Prompt to select a method.
        var selectedInput = inputs.Length == 1
            ? inputs[0]
            : Enum.Parse<ConsoleInput>(AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("[bold]Select an action[/]")
                .AddChoices(inputs.Select(e => e.ToString()))));

        var result = selectedInput switch
        {
            ConsoleInput.Inline => AnsiConsole.Prompt(new TextPrompt<string>("Input: ").PromptStyle("blue3 italic")),
            ConsoleInput.Document => File.ReadAllText(AnsiConsole.Prompt(new TextPrompt<string>("Enter path to document to analyze").PromptStyle("green"))),
            ConsoleInput.Microphone => Recognize(),
            ConsoleInput.Random => WaffleGenerator.WaffleEngine.Text(1, false).TrimEnd(Environment.NewLine.ToCharArray()),
            _ => throw new NotImplementedException()
        };

        if (selectedInput != ConsoleInput.Inline)
        {
            AnsiConsole.Write(new Markup($"Input: [blue3 italic]{result}[/]{Environment.NewLine}"));
        }
        Console.WriteLine("");
        return result;
    }

    private static string Recognize()
    {
        using AudioConfig audioConfig = AudioConfig.FromDefaultSpeakerOutput();
        var speechConfig = SpeechConfig.FromSubscription(Configuration.Instance.CognitiveServicesApiKey, Configuration.Instance.CognitiveServicesRegion);
        var speechRecognizer = new SpeechRecognizer(speechConfig, audioConfig);

        return AnsiConsole.Status()
            .Spinner(Spinner.Known.Balloon)
            .Start<string>("Speak in english now...", ctx =>
            {
                var speech = speechRecognizer.RecognizeOnceAsync().Result;
                return speech.Reason == ResultReason.RecognizedSpeech
                    ? speech.Text
                    : throw new NotImplementedException($"Failed to recognize speech: {speech.Reason}");
            });
    }
}