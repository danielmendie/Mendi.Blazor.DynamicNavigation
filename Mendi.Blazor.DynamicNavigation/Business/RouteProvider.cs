namespace Mendi.Blazor.DynamicNavigation.Business
{
    /// <summary>
    /// Provides access to a collection of route information for navigation purposes.
    /// </summary>
    /// <remarks>This class retrieves route data from an underlying <see cref="NavigatorRegistry"/> instance.
    /// It is designed to be used as a read-only provider of route information.</remarks>
    public sealed class RouteProvider : IRoutesProvider
    {
        private readonly NavigatorRegistry _registry;

        public RouteProvider(NavigatorRegistry registry)
        {
            _registry = registry;
        }
        /// <summary>
        /// Retrieves a read-only list of all registered routes.
        /// </summary>
        /// <returns>A read-only list of <see cref="RoutePageInfo"/> objects representing the registered routes. The list will be
        /// empty if no routes are registered.</returns>
        public IReadOnlyList<RoutePageInfo> GetRoutes()
            => _registry.Routes;
    }
}
