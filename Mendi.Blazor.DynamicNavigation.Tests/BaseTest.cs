using Mendi.Blazor.DynamicNavigation.Tests;
using Mendi.Blazor.DynamicNavigation.Tests.Mocks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

[assembly: Isolated]
namespace Mendi.Blazor.DynamicNavigation.Tests
{
    public class BaseTest
    {
#pragma warning disable NUnit1032 // An IDisposable field/property should be Disposed in a TearDown method
        public static ServiceProvider ServiceProvider;
#pragma warning restore NUnit1032 // An IDisposable field/property should be Disposed in a TearDown method

        static BaseTest()
        {
            ServiceProvider = SetupDependencies();
        }

        public BaseTest()
        {
        }

        [SetUp]
        public void Init()
        {
            //some tests need affect static mock data need to be reset before each test
            ServiceProvider = SetupDependencies();
        }

        private static ServiceProvider SetupDependencies()
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddBlazorDynamicNavigator(options => options.IgnoreRouteHistory = false);
            serviceCollection.AddScoped(typeof(ILogger<>), typeof(MockLogger<>));

            return serviceCollection.BuildServiceProvider();
        }
    }
}
