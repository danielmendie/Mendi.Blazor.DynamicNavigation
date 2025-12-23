using Mendi.Blazor.DynamicNavigation.Tests.Builders;

namespace Mendi.Blazor.DynamicNavigation.Tests.Tests
{
    public class RouteResolverTest
    {
        [Test]
        public void RouteResolver_GetDefaultRouteAsync_NoRoutesSet_IsExceptionThrownSuccessfully()
        {
            //arrange
            var service = BuildRouteResolver();

            //act
            var exception = Assert.CatchAsync<InvalidOperationException>(async () =>
            {
                var actual = await service.GetDefaultRouteAsync(0);
            });

            //assert
            Assert.That(exception.Message, Is.EqualTo("No default route found for appId 0."));
        }

        [Test]
        public async Task RouteResolver_GetDefaultRouteAsync_RoutesSet_IsDefaultReturnSuccessfully()
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
                        ComponentType = typeof(RouteResolverTest),
                        Params = new()
                        {
                            {
                                "Username",
                                "Id"
                            }
                        }
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
