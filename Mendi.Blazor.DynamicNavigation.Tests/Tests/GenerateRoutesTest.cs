using Mendi.Blazor.DynamicNavigation.CLI;
using Mendi.Blazor.DynamicNavigation.CLI.Commands;

namespace Mendi.Blazor.DynamicNavigation.Tests.Tests
{
    [TestFixture]
    public class GenerateRoutesTest
    {

        [Test]
        public void GenerateAppRoutesFromFile_WithValidFile_ShouldProcessSuccessfully()
        {
            var option = new CommandOptions
            {
                Command = "routes",
                Subcommand = "generate",
                Path = @"C:\Users\Danie\OneDrive\Desktop\TestApp\TestApp",
                Force = false,
                Verbose = true,
                DryRun = false
            };
            // Arrange
            var command = new GenerateRoutesCommand();

            Assert.DoesNotThrowAsync(async () =>
            {
                await command.RunAsync(option);
            });
        }

    }
}
