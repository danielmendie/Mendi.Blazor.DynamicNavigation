using CommandLine;
using Mendi.Blazor.DynamicNavigation.CLI;
using System.Reflection;

public class Program
{
    static void Main(string[] args)
    {
        // Parse command-line arguments
        try
        {
            Parser.Default.ParseArguments<Options>(args)
            .WithParsed(async options =>
            {
                // Handle parsed options
                await HandleOptions(options);
            })
            .WithNotParsed(errors =>
            {
                // Handle parsing errors
                HandleParsingErrors(errors);
            });
        }
        catch (Exception ex)
        {
            // Close and flush the Serilog logger
            Console.WriteLine($"Error occured while running engine tool: {ex.Message}");
        }
    }

    static async Task HandleOptions(Options options)
    {
        // Implement logic based on parsed options
        if (options.Command == "scaffold")
        {
            // Check for additional subcommands and perform actions
            if (options.Subcommand == "getpageroutes")
            {
                // Execute your method for adding page routes
                var PageGeneratingEngine = new GetPageRoutes();
                await PageGeneratingEngine.Execute(options.Path);
            }
            else if (options.Subcommand == "buildpageroutes")
            {
                // Execute your method for adding page routes
                var PageBuildingEngine = new BuildPageRoutes();
                await PageBuildingEngine.Execute(options.Force, options.Path);
            }
            else
            {
                Console.WriteLine($"Invalid scaffold action: {options.Subcommand}");
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
