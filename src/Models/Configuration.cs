public class Configuration
{
    public string CognitiveServicesApiKey { get; set; } = string.Empty;
    public string CognitiveServicesEndPoint { get; set; } = string.Empty;

    private static Configuration? _instance;    
    public static Configuration Instance =>
        _instance ??= System.Text.Json.JsonSerializer.Deserialize<Configuration>(File.ReadAllText("appsettings.json"))!;
}