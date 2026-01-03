using CommandLine;

namespace Mendi.Blazor.DynamicNavigation.CLI.Commands
{
    public class CommandOptions
    {
        [Value(0, MetaName = "command", Required = true, HelpText = "Specify the command.")]
        public string Command { get; set; }

        [Value(1, MetaName = "subcommand", Required = false, HelpText = "Specify the subcommand.")]
        public string Subcommand { get; set; }

        [Option('p', "path", Required = false, HelpText = "Specify the path.")]
        public string Path { get; set; }

        [Option('f', "force", Required = false, HelpText = "Specify the force.")]
        public bool Force { get; set; }

        [Option('v', "verbose", Required = false, HelpText = "Specify the verbose.")]
        public bool Verbose { get; set; }

        [Option('d', "dry-run", Required = false, HelpText = "Specify the verbose.")]
        public bool DryRun { get; set; }

    }
}
