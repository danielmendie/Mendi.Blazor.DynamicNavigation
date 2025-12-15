using Blazored.LocalStorage;

namespace Mendi.Blazor.DynamicNavigation.Business
{

    public sealed class RouteStorage : IRouteStorage
    {
        private const string StorageKey = "Mendi.DynamicNavigator.CurrentRoute";
        private readonly ILocalStorageService _localStorage;

        public RouteStorage(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public async Task SaveCurrentRouteAsync(
            RoutePageInfo route,
            Dictionary<string, string>? parameters,
            CancellationToken cancellationToken = default)
        {
            var payload = new StoredRoute
            {
                Route = route,
                Parameters = parameters?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
            };

            await _localStorage.SetItemAsync(StorageKey, payload, cancellationToken);
        }

        public async Task<(RoutePageInfo Route, Dictionary<string, string>? Parameters)?> LoadCurrentRouteAsync(
            CancellationToken cancellationToken = default)
        {
            var payload = await _localStorage.GetItemAsync<StoredRoute?>(StorageKey, cancellationToken);
            if (payload is null)
            {
                return null;
            }

            Dictionary<string, string>? parameters = payload.Parameters;
            return (payload.Route, parameters);
        }

        private sealed class StoredRoute
        {
            public RoutePageInfo Route { get; set; } = default!;
            public Dictionary<string, string>? Parameters { get; set; }
        }
    }

}
