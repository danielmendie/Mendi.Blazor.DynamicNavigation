using CommandLine;
using Mendi.Blazor.DynamicNavigation.CLI.Commands;
using Microsoft.Extensions.Logging;
using System.Reflection;

public class Program
{
    ILogger<Program> Logger { get; }

    static void Main(string[] args)
    {
        try
        {
            Parser.Default.ParseArguments<CommandOptions>(args)
            .WithParsed(async options =>
            {
                await HandleOptions(options);
            })
            .WithNotParsed(errors =>
            {
                HandleParsingErrors(errors);
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error occurred while running engine tool: {ex.Message}");
        }
    }

    static async Task HandleOptions(CommandOptions options)
    {
        if (options.Command == "routes")
        {
            if (options.Subcommand == "generate")
            {
                var engine = new GenerateRoutesCommand();
                await engine.RunAsync(options);
            }
            else if (options.Subcommand == "build")
            {
                var engine = new BuildRoutesCommand();
                await engine.RunAsync(options);
            }
            else
            {
                Console.WriteLine($"Invalid route action: {options.Subcommand}");
            }
        }
        else if (options.Command == "version")
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            Console.WriteLine($"version: {version}");
        }
        else
        {
            Console.WriteLine($"Invalid engine command: {options.Command}");
        }
    }

    static void HandleParsingErrors(IEnumerable<Error> errors)
    {
        // Handle parsing errors (display help, show error messages, etc.)
        Console.WriteLine("Error parsing command-line arguments.");
    }
}
