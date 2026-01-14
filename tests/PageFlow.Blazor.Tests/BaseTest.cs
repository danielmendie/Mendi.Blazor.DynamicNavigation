using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using PageFlow.Blazor.Tests;
using PageFlow.Blazor.Tests.Mocks;

[assembly: Isolated]
namespace PageFlow.Blazor.Tests
{
    public class BaseTest
    {
        public static ServiceProvider ServiceProvider;

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
            serviceCollection.AddBlazorPageFlow();
            serviceCollection.AddScoped(typeof(ILocalStorageProvider), typeof(MockKeyValueStore));
            serviceCollection.AddScoped(typeof(ILogger<>), typeof(MockLogger<>));
            serviceCollection.AddSingleton(typeof(IJSRuntime), typeof(MockJsRuntime));

            return serviceCollection.BuildServiceProvider();
        }
    }
}
