using CommandLine;
using Mendi.Blazor.DynamicNavigation.CLI.Commands;
using Mendi.Blazor.DynamicNavigation.CLI.Helpers;
using System.Reflection;

public class Program
{
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
            UtilsHelper.Log($"Error occurred while running engine tool: {ex.Message}");
        }
    }

    static async Task HandleOptions(CommandOptions options)
    {
        if (options.Command == "routes")
        {
            if (options.Subcommand == "generate")
            {
                await GenerateRoutesCommand.RunAsync(options);
            }
            else
            {
                UtilsHelper.Log($"Invalid command action: {options.Subcommand}");
            }
        }
        else if (options.Command == "version")
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            UtilsHelper.Log($"version: {version}");
        }
        else
        {
            UtilsHelper.Log($"Invalid engine command: {options.Command}");
        }
    }

    static void HandleParsingErrors(IEnumerable<Error> errors)
    {
        // Handle parsing errors (display help, show error messages, etc.)
        UtilsHelper.Log("Error parsing command-line arguments.");
    }
}
