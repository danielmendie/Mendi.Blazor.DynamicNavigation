using PageFlow.Blazor.Tests.Builders;

namespace PageFlow.Blazor.Tests.Tests.Business
{
    public class NavigationStateTest
    {
        [Test]
        public void NavigationState_Set_ShouldSetRouteAndParameters_Successfully()
        {
            //arrange
            var service = BuildNavigationState();
            var route = new PageFlowInfo
            {
                AppId = 0,
                PageName = "Sample",
                Component = "Sample",
                IsDefault = true,
                ComponentType = typeof(RouteResolverTest),
                Params = new Dictionary<string, object>
                {
                    { "Id", "" },
                    { "Name", "" }
                }
            };
            var parameterValues = new Dictionary<string, string>
            {
                { "Id", "1" },
                { "Name", "Test" }
            };

            //act
            var currentRouteBeforeSet = service.CurrentRoute;
            service.Set(route, parameterValues);
            var setRoute = service.CurrentRoute;

            //assert
            Assert.Multiple(() =>
            {
                Assert.That(currentRouteBeforeSet, Is.Null);
                Assert.That(setRoute, Is.Not.Null);
                Assert.That(setRoute?.AppId, Is.EqualTo(0));
                Assert.That(setRoute?.PageName, Is.EqualTo("Sample"));
                Assert.That(setRoute?.Params["Id"], Is.EqualTo("1"));
                Assert.That(setRoute?.Params["Name"], Is.EqualTo("Test"));
            });
        }


        private NavigationState BuildNavigationState()
        {
            return DefaultServiceBuilder.Build<NavigationState>();
        }
    }
}
