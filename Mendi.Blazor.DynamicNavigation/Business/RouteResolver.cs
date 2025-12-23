namespace Mendi.Blazor.DynamicNavigation.Business
{
    /// <summary>
    /// Resolves routes for applications and components based on predefined route information.
    /// </summary>
    /// <remarks>This class provides methods to retrieve the default route for a specific application or a
    /// route associated with a specific component. It relies on route data provided by an <see cref="IRoutesProvider"/>
    /// implementation.</remarks>
    public sealed class RouteResolver : IRouteResolver
    {
        private readonly IReadOnlyList<RoutePageInfo> _routes;

        public RouteResolver(IRoutesProvider routesProvider)
        {
            _routes = routesProvider.GetRoutes();
        }

        public Task<RoutePageInfo> GetDefaultRouteAsync(int appId, CancellationToken cancellationToken = default)
        {
            var route = _routes.FirstOrDefault(r => r.AppId == appId && r.IsDefault)
                        ?? throw new InvalidOperationException($"No default route found for appId {appId}.");
            return Task.FromResult(route);
        }

        public Task<RoutePageInfo?> GetRouteAsync(string componentName, CancellationToken cancellationToken = default)
        {
            var route = _routes.FirstOrDefault(r =>
                string.Equals(r.Component, componentName, StringComparison.Ordinal));

            return Task.FromResult<RoutePageInfo?>(route);
        }

        public Task<RoutePageInfo?> GetRouteWithIdAsync(string componentName, int appId, CancellationToken cancellationToken = default)
        {
            var route = _routes.FirstOrDefault(r => r.AppId == appId &&
                string.Equals(r.Component, componentName, StringComparison.Ordinal));

            return Task.FromResult<RoutePageInfo?>(route);
        }
    }
}
