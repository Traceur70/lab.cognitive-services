using Spectre.Console;






Console.WriteLine();
AnsiConsole.Write(new FigletText("UPPER-LINK").Color(Color.Green3));
AnsiConsole.Write(new FigletText("   azure ai ").Color(Color.Blue));
Console.WriteLine();

for(var testNumber = 1; testNumber < int.MaxValue; testNumber++)
{
    // Dictionary of all methods in LabSpeechService class
    var methods = typeof(LabSpeechService).GetMethods()
        .OrderBy(m => m.Name)
        .ExceptBy(typeof(object).GetMethods().Select(m => m.Name), m => m.Name)
        .ToDictionary(m => m.Name);

    // Prompt to select a method.
    var selectedMethod = AnsiConsole.Prompt(new SelectionPrompt<string>()
        .Title("[bold green]Select an action[/]")
        .AddChoices(methods.Keys.Append("Exit")));

    // Invoke the selected method.
    if(!methods.TryGetValue(selectedMethod, out var method))
    {
        break;
    }

    AnsiConsole.Write(new Rule($"Test {testNumber} - {method.Name}") { Border = BoxBorder.Double });
    Console.WriteLine();
    method.Invoke(new LabSpeechService(), null);
    Console.WriteLine();
    Console.WriteLine();
}

AnsiConsole.MarkupLine($"Bye !");