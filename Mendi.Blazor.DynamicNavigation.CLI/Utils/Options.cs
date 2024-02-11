using CommandLine;

namespace Mendi.Blazor.DynamicNavigation.CLI
{
    public class Options
    {
        [Value(0, MetaName = "command", Required = true, HelpText = "Specify the command.")]
        public string Command { get; set; }

        [Value(1, MetaName = "subcommand", Required = false, HelpText = "Specify the subcommand.")]
        public string Subcommand { get; set; }

        [Option('p', "path", Required = false, HelpText = "Specify the path.")]
        public string Path { get; set; }

        [Option('f', "force", Required = false, HelpText = "Specify the force.")]
        public bool Force { get; set; }
    }
}
