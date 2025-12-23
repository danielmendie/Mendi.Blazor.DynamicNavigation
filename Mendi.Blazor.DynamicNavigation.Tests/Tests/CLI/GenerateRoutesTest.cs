using Mendi.Blazor.DynamicNavigation.CLI.Commands;

namespace Mendi.Blazor.DynamicNavigation.Tests.Tests.CLI
{
    [TestFixture]
    public class GenerateRoutesTest
    {

        [Test]
        public void GenerateRoutesCommand_WithValidFile_ShouldNotUpdateFileOnDryRun_Successfully()
        {
            var option = new CommandOptions
            {
                Command = "routes",
                Subcommand = "generate",
                Path = @"C:\Users\Danie\OneDrive\Desktop\TestApp\TestApp",
                Force = false,
                Verbose = true,
                DryRun = true
            };

            // Arrange
            Assert.DoesNotThrowAsync(async () =>
            {
                await GenerateRoutesCommand.RunAsync(option);
            });
        }

        [Test]
        public void GenerateRoutesCommand_WithValidFile_ShouldProcess_Successfully()
        {
            var option = new CommandOptions
            {
                Command = "routes",
                Subcommand = "generate",
                Path = @"C:\Users\Danie\OneDrive\Desktop\TestApp\TestApp",
                Force = true,
                Verbose = true,
                DryRun = false
            };

            // Arrange
            Assert.DoesNotThrowAsync(async () =>
            {
                await GenerateRoutesCommand.RunAsync(option);
            });
        }

    }
}
