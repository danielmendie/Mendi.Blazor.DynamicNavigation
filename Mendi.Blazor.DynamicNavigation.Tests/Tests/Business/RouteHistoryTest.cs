using Mendi.Blazor.DynamicNavigation.Tests.Builders;

namespace Mendi.Blazor.DynamicNavigation.Tests.Tests.Business
{
    public class RouteHistoryTest
    {
        // Add your tests for RouteHistory here
        [Test, Order(1)]
        public void RouteHistory_InitialState_ShouldBeEmpty_Successfully()
        {
            //arrange
            var service = BuildRouteHistory();
            //act
            var hasHistory = service.CanGoBack;
            //assert
            Assert.That(hasHistory, Is.False);
        }

        [Test]
        public void RouteHistory_Record_ShouldUpdateHistory_Successfully()
        {
            //arrange
            var service = BuildRouteHistory();
            var route = new RoutePageInfo
            {
                AppId = 0,
                PageName = "Sample",
                Component = "Sample",
                IsDefault = true,
                ComponentType = typeof(RouteHistoryTest)
            };
            //act
            service.Record(route, null);
            var hasHistory = service.CanGoBack;
            //assert
            Assert.That(hasHistory, Is.EqualTo(true));
        }

        [Test]
        public void RouteHistory_GetPrevious_ShouldReturnPreviousRoute_Successfully()
        {
            //arrange
            var service = BuildRouteHistory();
            var route1 = new RoutePageInfo
            {
                AppId = 0,
                PageName = "Sample1",
                Component = "Sample1",
                IsDefault = true,
                ComponentType = typeof(RouteHistoryTest)
            };
            var route2 = new RoutePageInfo
            {
                AppId = 0,
                PageName = "Sample2",
                Component = "Sample2",
                IsDefault = true,
                ComponentType = typeof(RouteHistoryTest)
            };
            service.Record(route1, null);
            service.Record(route2, null);
            //act
            var previousRoute = service.GetPrevious();
            //assert
            Assert.Multiple(() =>
            {
                Assert.That(previousRoute, Is.Not.Null);
                Assert.That(previousRoute?.Route.PageName, Is.EqualTo("Sample1"));
            });
        }

        [Test]
        public void RouteHistory_GetPrevious_NoHistory_ShouldReturnNull_Successfully()
        {
            //arrange
            var service = BuildRouteHistory();
            //act
            var previousRoute = service.GetPrevious();
            //assert
            Assert.That(previousRoute, Is.Null);
        }

        [Test]
        public void RouteHistory_GetPrevious_AfterOneRecord_ShouldReturnNull_Successfully()
        {
            //arrange
            var service = BuildRouteHistory();
            var route = new RoutePageInfo
            {
                AppId = 0,
                PageName = "Sample",
                Component = "Sample",
                IsDefault = true,
                ComponentType = typeof(RouteHistoryTest)
            };
            service.Record(route, null);
            //act
            var previousRoute = service.GetPrevious();
            //assert
            Assert.Multiple(() =>
            {
                Assert.That(previousRoute, Is.Null);
            });
        }

        [Test]
        public void RouteHistory_Clear_ShouldEmptyHistory_Successfully()
        {
            //arrange
            var service = BuildRouteHistory();
            var route = new RoutePageInfo
            {
                AppId = 0,
                PageName = "Sample",
                Component = "Sample",
                IsDefault = true,
                ComponentType = typeof(RouteHistoryTest)
            };
            service.Record(route, null);
            //act
            service.Clear();
            var hasHistory = service.CanGoBack;
            //assert
            Assert.That(hasHistory, Is.EqualTo(false));
        }

        [Test]
        public void RouteHistory_Record_NullRoute_ShouldThrowException_Successfully()
        {
            //arrange
            var service = BuildRouteHistory();
            //act
            var exception = Assert.Catch<ArgumentNullException>(() =>
            {
                service.Record(null!, null);
            });
            //assert
            Assert.That(exception.ParamName, Is.EqualTo("route"));
        }

        private IRouteHistory BuildRouteHistory()
        {
            return DefaultServiceBuilder.Build<IRouteHistory>();
        }
    }
}
