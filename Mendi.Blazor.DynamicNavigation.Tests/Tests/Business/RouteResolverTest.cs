using Mendi.Blazor.DynamicNavigation.Tests.Builders;

namespace Mendi.Blazor.DynamicNavigation.Tests.Tests.Business
{
    public class RouteResolverTest
    {
        [Test]
        public void RouteResolver_GetDefaultRouteAsync_NoRoutesSet_ShouldThrowException_Successfully()
        {
            //arrange
            var service = BuildRouteResolver();
            var expectedErrorMessage = "No default route found for appId 0.";

            //act
            var exception = Assert.CatchAsync<InvalidOperationException>(async () =>
            {
                var actual = await service.GetDefaultRouteAsync(0);
            });

            //assert
            Assert.That(exception.Message, Is.EqualTo(expectedErrorMessage));
        }

        [Test]
        public async Task RouteResolver_GetDefaultRouteAsync_RoutesSet_ShouldReturnDefaultRoute_Successfully()
        {
            //arrange
            var registry = BuildRegistryResolver();
            registry.Routes.Clear();
            registry.Routes.Add(
                    new RoutePageInfo
                    {
                        AppId = 0,
                        PageName = "Sample",
                        Component = "Sample",
                        IsDefault = true,
                        ComponentType = typeof(RouteResolverTest)
                    });

            var service = BuildRouteResolver();

            //act
            var actual = await service.GetDefaultRouteAsync(0);

            //assert
            Assert.Multiple(() =>
            {
                Assert.That(actual.AppId, Is.EqualTo(0));
                Assert.That(actual.PageName, Is.EqualTo("Sample"));
            });
        }

        [Test]
        public void RouteResolver_GetDefaultRouteAsync_RoutesSet_AppIdDefaultNotFound_ShouldThrowException_Successfully()
        {
            //arrange
            var expectedErrorMessage = "No default route found for appId 1.";
            var registry = BuildRegistryResolver();
            registry.Routes.Clear();
            registry.Routes.Add(
                    new RoutePageInfo
                    {
                        AppId = 0,
                        PageName = "Dashboard",
                        Component = "Dashboard",
                        IsDefault = true,
                        ComponentType = typeof(RouteResolverTest)
                    });

            var service = BuildRouteResolver();

            //act
            var exception = Assert.CatchAsync<InvalidOperationException>(async () =>
            {
                var actual = await service.GetDefaultRouteAsync(1);
            });

            //assert
            Assert.That(exception.Message, Is.EqualTo(expectedErrorMessage));
        }

        [Test]
        public async Task RouteResolver_GetRouteAsync_RoutesSet_ValidComponentNameShouldReturnValidRoute_Successfully()
        {
            //arrange
            var expectedRouteName = "Dashboard";
            var registry = BuildRegistryResolver();
            registry.Routes.Clear();
            registry.Routes.AddRange([
                    new RoutePageInfo
                    {
                        AppId = 0,
                        PageName = "Sample",
                        Component = "Sample",
                        IsDefault = false,
                        ComponentType = typeof(RouteResolverTest)
                    },
                    new RoutePageInfo
                    {
                        AppId = 0,
                        PageName = "Dashboard",
                        Component = "Dashboard",
                        IsDefault = true,
                        ComponentType = typeof(RouteResolverTest)
                    }
            ]);

            var service = BuildRouteResolver();

            //act
            var actual = await service.GetRouteAsync(expectedRouteName);

            //assert
            Assert.Multiple(() =>
            {
                Assert.That(actual, Is.Not.EqualTo(null));
                Assert.That(actual.PageName, Is.EqualTo(expectedRouteName));
                Assert.That(actual.Component, Is.EqualTo(expectedRouteName));
                Assert.That(actual.IsDefault, Is.EqualTo(true));
                Assert.That(actual.ComponentType, Is.EqualTo(typeof(RouteResolverTest)));
            });
        }

        [Test]
        public async Task RouteResolver_GetRouteAsync_RoutesSet_InvalidComponentNameShouldReturnNoRoute_Successfully()
        {
            //arrange
            var expectedRouteName = "Dashboad";
            var registry = BuildRegistryResolver();
            registry.Routes.Clear();
            registry.Routes.AddRange([
                    new RoutePageInfo
                    {
                        AppId = 0,
                        PageName = "Sample",
                        Component = "Sample",
                        IsDefault = false,
                        ComponentType = typeof(RouteResolverTest)
                    },
                    new RoutePageInfo
                    {
                        AppId = 0,
                        PageName = "Dashboard",
                        Component = "Dashboard",
                        IsDefault = true,
                        ComponentType = typeof(RouteResolverTest)
                    }
            ]);

            var service = BuildRouteResolver();

            //act
            var actual = await service.GetRouteAsync(expectedRouteName);

            //assert
            Assert.That(actual, Is.EqualTo(null));
        }

        [Test]
        public async Task RouteResolver_GetRouteWithIdAsync_RoutesSet_ValidComponentWithValidIdShouldReturnValidRoute_Successfully()
        {
            //arrange
            var expectedRouteName = "Dashboard";
            var registry = BuildRegistryResolver();
            registry.Routes.Clear();
            registry.Routes.AddRange([
                    new RoutePageInfo
                    {
                        AppId = 0,
                        PageName = "Sample",
                        Component = "Sample",
                        IsDefault = false,
                        ComponentType = typeof(RouteResolverTest)
                    },
                    new RoutePageInfo
                    {
                        AppId = 1,
                        PageName = "Dashboard",
                        Component = "Dashboard",
                        IsDefault = true,
                        ComponentType = typeof(RouteResolverTest)
                    }
            ]);

            var service = BuildRouteResolver();

            //act
            var actual = await service.GetRouteWithIdAsync(expectedRouteName, 1);

            //assert
            Assert.Multiple(() =>
            {
                Assert.That(actual, Is.Not.EqualTo(null));
                Assert.That(actual.AppId, Is.EqualTo(1));
                Assert.That(actual.PageName, Is.EqualTo(expectedRouteName));
                Assert.That(actual.Component, Is.EqualTo(expectedRouteName));
                Assert.That(actual.IsDefault, Is.EqualTo(true));
                Assert.That(actual.ComponentType, Is.EqualTo(typeof(RouteResolverTest)));
            });
        }

        [Test]
        public async Task RouteResolver_GetRouteWithIdAsync_RoutesSet_ValidComponentWithInvalidIdShouldReturnNoRoute_Successfully()
        {
            //arrange
            var expectedRouteName = "Dashboard";
            var registry = BuildRegistryResolver();
            registry.Routes.Clear();
            registry.Routes.AddRange([
                    new RoutePageInfo
                    {
                        AppId = 0,
                        PageName = "Sample",
                        Component = "Sample",
                        IsDefault = false,
                        ComponentType = typeof(RouteResolverTest)
                    },
                    new RoutePageInfo
                    {
                        AppId = 1,
                        PageName = "Dashboard",
                        Component = "Dashboard",
                        IsDefault = true,
                        ComponentType = typeof(RouteResolverTest)
                    }
            ]);

            var service = BuildRouteResolver();

            //act
            var actual = await service.GetRouteWithIdAsync(expectedRouteName, 0);

            //assert
            Assert.That(actual, Is.EqualTo(null));
        }

        [Test]
        public async Task RouteResolver_GetRouteWithIdAsync_RoutesSet_InvalidComponentWithValidIdShouldReturnNoRoute_Successfully()
        {
            //arrange
            var expectedRouteName = "Dashboad";
            var registry = BuildRegistryResolver();
            registry.Routes.Clear();
            registry.Routes.AddRange([
                    new RoutePageInfo
                    {
                        AppId = 0,
                        PageName = "Sample",
                        Component = "Sample",
                        IsDefault = false,
                        ComponentType = typeof(RouteResolverTest)
                    },
                    new RoutePageInfo
                    {
                        AppId = 1,
                        PageName = "Dashboard",
                        Component = "Dashboard",
                        IsDefault = true,
                        ComponentType = typeof(RouteResolverTest)
                    }
            ]);
            var service = BuildRouteResolver();
            //act
            var actual = await service.GetRouteWithIdAsync(expectedRouteName, 1);
            //assert
            Assert.That(actual, Is.EqualTo(null));
        }


        private IRouteResolver BuildRouteResolver()
        {
            return DefaultServiceBuilder.Build<IRouteResolver>();
        }

        private NavigatorRegistry BuildRegistryResolver()
        {
            return DefaultServiceBuilder.Build<NavigatorRegistry>();
        }
    }
}
