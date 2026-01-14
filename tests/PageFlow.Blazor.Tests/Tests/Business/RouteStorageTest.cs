using PageFlow.Blazor.Tests.Builders;

namespace PageFlow.Blazor.Tests.Tests.Business
{
    public class RouteStorageTest
    {
        [Test]
        public async Task RouteStorage_SaveCurrentRouteAsyncAndLoadCurrentRouteAsync_ShouldStoreAndRetrieveRoute_Successfully()
        {
            //arrange
            var registry = BuildRegistryResolver();
            registry.Routes.Clear();
            registry.Routes.Add(
                    new PageFlowInfo
                    {
                        AppId = 1,
                        PageName = "TestPage",
                        Component = "TestComponent",
                        IsDefault = false,
                        ComponentType = typeof(RouteStorageTest)
                    });

            var service = BuildRouteStorage();
            var route = new PageFlowInfo
            {
                AppId = 1,
                PageName = "TestPage",
                Component = "TestComponent",
                IsDefault = false,
                ComponentType = typeof(RouteStorageTest)
            };
            //act
            await service.SaveCurrentRouteAsync(route, null);
            var retrievedRoute = await service.LoadCurrentRouteAsync();
            //assert
            Assert.Multiple(() =>
            {
                Assert.That(retrievedRoute, Is.Not.Null);
                Assert.That(retrievedRoute?.Route.AppId, Is.EqualTo(1));
                Assert.That(retrievedRoute?.Route.PageName, Is.EqualTo("TestPage"));
                Assert.That(retrievedRoute?.Route.Component, Is.EqualTo("TestComponent"));
            });
        }

        [Test]
        public async Task RouteStorage_LoadCurrentRouteAsync_WhenNoRouteSaved_ShouldReturnNull_Successfully()
        {
            //arrange
            var service = BuildRouteStorage();
            //act
            var retrievedRoute = await service.LoadCurrentRouteAsync();
            //assert
            Assert.That(retrievedRoute, Is.Null);
        }

        [Test]
        public void RouteStorage_SaveCurrentRouteAsync_WithNullRoute_ShouldThrowArgumentNullException_Successfully()
        {
            //arrange
            var service = BuildRouteStorage();
            //act & assert
            Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await service.SaveCurrentRouteAsync(null, null);
            });
        }

        [Test]
        public async Task RouteStorage_LoadCurrentRouteAsync_WithInvalidRoute_ShouldReturnNull_Successfully()
        {
            //arrange
            var registry = BuildRegistryResolver();
            registry.Routes.Clear();
            var service = BuildRouteStorage();
            var invalidRoute = new PageFlowInfo
            {
                AppId = 99,
                PageName = "NonExistentPage",
                Component = "NonExistentComponent",
                IsDefault = false,
                ComponentType = typeof(RouteStorageTest)
            };
            //act & assert
            await service.SaveCurrentRouteAsync(invalidRoute, null);
            var retrievedRoute = await service.LoadCurrentRouteAsync();

            //assert
            Assert.That(retrievedRoute, Is.Null);
        }

        private IRouteStorage BuildRouteStorage()
        {
            return DefaultServiceBuilder.Build<IRouteStorage>();
        }
        private PageFlowRegistry BuildRegistryResolver()
        {
            return DefaultServiceBuilder.Build<PageFlowRegistry>();
        }
    }
}
