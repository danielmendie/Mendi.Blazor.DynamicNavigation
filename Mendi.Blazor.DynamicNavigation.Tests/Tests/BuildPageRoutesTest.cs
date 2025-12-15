using Mendi.Blazor.DynamicNavigation.CLI;
using Mendi.Blazor.DynamicNavigation.CLI.Commands;

namespace Mendi.Blazor.DynamicNavigation.Tests.Tests
{
    public class BuildPageRoutesTest
    {
        [Test]
        public void BuildPageRoute_WithValidPathButNoMethod_ShouldProcessSuccessfully()
        {
            var option = new CommandOptions
            {
                Command = "routes",
                Subcommand = "build",
                Path = @"C:\Remote\Mendi.Blazor.DynamicNavigation\Test",
                Force = false,
                Verbose = false,
                DryRun = false
            };
            // Arrange
            var command = new BuildRoutesCommand();

            Assert.DoesNotThrowAsync(async () =>
            {
                await command.RunAsync(option);
            });
        }

        [Test]
        public void BuildPageRoute_WithValidPathButWithExistingMethod_ShouldProcessSuccessfully()
        {
            var option = new CommandOptions
            {
                Command = "routes",
                Subcommand = "build",
                Path = @"C:\Remote\Mendi.Blazor.DynamicNavigation\Test",
                Force = true,
                Verbose = true,
                DryRun = false
            };
            // Arrange
            var command = new BuildRoutesCommand();

            Assert.DoesNotThrowAsync(async () =>
            {
                await command.RunAsync(option);
            });
        }

    }
}
