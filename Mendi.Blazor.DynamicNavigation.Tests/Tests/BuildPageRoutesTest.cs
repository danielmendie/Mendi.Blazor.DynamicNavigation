using Mendi.Blazor.DynamicNavigation.CLI;

namespace Mendi.Blazor.TrueSPA.CLI.Test.Tests
{
    public class BuildPageRoutesTest
    {
        [Test]
        public void BuildPageRoute_WithValidPathButNoMethod_ShouldProcessSuccessfully()
        {
            // Arrange
            var command = new BuildPageRoutes();
            //specify a valid blazor assembly project path
            var filePath = @"C:\Remote\Mendi.Blazor.DynamicNavigation\Test";

            Assert.DoesNotThrowAsync(async () =>
            {
                await command.Execute(path: filePath);
            });
        }

        [Test]
        public void BuildPageRoute_WithValidPathButWithExistingMethod_ShouldProcessSuccessfully()
        {
            // Arrange
            var command = new BuildPageRoutes();
            //specify a valid blazor assembly project path
            var filePath = @"C:\Remote\Mendi.Blazor.DynamicNavigation\Test";

            Assert.DoesNotThrowAsync(async () =>
            {
                await command.Execute(path: filePath, force: true);
            });
        }

    }
}
