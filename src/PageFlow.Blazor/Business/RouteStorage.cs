namespace PageFlow.Blazor.Business
{
    /// <summary>
    /// Provides functionality to store and retrieve the current route information using local storage.
    /// </summary>
    /// <remarks>This class provides methods to resolve the current route for a specific application or a
    /// route associated with a specific component. It relies on route resolver provided by an <see cref="IRouteResolver"/> implementation and also local storage provider by an <see cref="ILocalStorageProvider"/> implementation for communication with browser storage</remarks>
    public sealed class RouteStorage : IRouteStorage
    {
        private const string StorageKey = "BlazorPageFlow.CurrentRoute";
        private readonly ILocalStorageProvider _localStorage;
        private readonly IRouteResolver _routeResolver;

        public RouteStorage(ILocalStorageProvider localStorage,
            IRouteResolver routeResolver)
        {
            _localStorage = localStorage;
            _routeResolver = routeResolver;
        }
        /// <summary>
        /// Saves the current route information along with its parameters to local storage.
        /// </summary>
        /// <param name="route">The current navigated route</param>
        /// <param name="parameters">Optional parameters for the current route</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">On null route</exception>
        public async Task SaveCurrentRouteAsync(
            PageFlowInfo route,
            Dictionary<string, string>? parameters,
            CancellationToken cancellationToken = default)
        {
            if (route is null)
            {
                throw new ArgumentNullException(nameof(route));
            }
            var payload = new StoredRoute
            {
                AppId = route.AppId,
                Component = route.Component,
                Parameters = parameters
            };
            await _localStorage.SetItemAsync(StorageKey, payload);
        }
        /// <summary>
        /// Retrieves the current route information and its parameters from local storage.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>A tuple containing the route and its associated parameters, or <see langword="null"/> if there is no stored entry in the local storage.</returns>
        public async Task<(PageFlowInfo Route, Dictionary<string, string>? Parameters)?> LoadCurrentRouteAsync(CancellationToken cancellationToken = default)
        {
            var payload = await _localStorage.GetItemAsync<StoredRoute?>(StorageKey);
            if (payload is null)
            {
                return null;
            }
            var route = await _routeResolver.GetRouteWithIdAsync(payload.Component, payload.AppId, cancellationToken);
            if (route is null)
            {
                return null;
            }
            var parameters = payload.Parameters ?? new Dictionary<string, string>();
            return (route, parameters);
        }
        /// <summary>
        /// Class representing the stored route information in local storage.
        /// </summary>
        private sealed class StoredRoute
        {
            public int AppId { get; set; }
            public string Component { get; set; } = default!;
            public Dictionary<string, string>? Parameters { get; set; }
        }
    }

}
