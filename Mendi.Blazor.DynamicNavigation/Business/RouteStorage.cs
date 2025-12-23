using Blazored.LocalStorage;

namespace Mendi.Blazor.DynamicNavigation.Business
{

    public sealed class RouteStorage : IRouteStorage
    {
        private const string StorageKey = "Mendi.DynamicNavigator.CurrentRoute";
        private readonly ILocalStorageService _localStorage;
        private readonly IRouteResolver _routeResolver;

        public RouteStorage(ILocalStorageService localStorage,
            IRouteResolver routeResolver)
        {
            _localStorage = localStorage;
            _routeResolver = routeResolver;
        }

        public async Task SaveCurrentRouteAsync(
            RoutePageInfo route,
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

            await _localStorage.SetItemAsync(StorageKey, payload, cancellationToken);
        }

        public async Task<(RoutePageInfo Route, Dictionary<string, string>? Parameters)?> LoadCurrentRouteAsync(CancellationToken cancellationToken = default)
        {
            var payload = await _localStorage.GetItemAsync<StoredRoute?>(StorageKey, cancellationToken);
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

        private sealed class StoredRoute
        {
            public int AppId { get; set; }
            public string Component { get; set; } = default!;
            public Dictionary<string, string>? Parameters { get; set; }
        }
    }

}
