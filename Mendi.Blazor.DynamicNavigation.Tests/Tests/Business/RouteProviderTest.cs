using Mendi.Blazor.DynamicNavigation.Tests.Builders;

namespace Mendi.Blazor.DynamicNavigation.Tests.Tests.Business
{
    public class RouteProviderTest
    {
        [Test]
        public void RouteProvider_GetAllRoutes_ShouldReturnRoutes_Successfully()
        {
            //arrange
            var service = BuildRoutesProvider();
            //act
            var actual = service.GetRoutes();
            //assert
            Assert.That(actual, Is.Not.Null);
        }


        private IRoutesProvider BuildRoutesProvider()
        {
            return DefaultServiceBuilder.Build<IRoutesProvider>();
        }
    }
}
