namespace Mendi.Blazor.DynamicNavigation
{
    public interface IRouteStorage
    {
        Task SaveCurrentRouteAsync(RoutePageInfo route, Dictionary<string, string>? parameters, CancellationToken cancellationToken = default);
        Task<(RoutePageInfo Route, Dictionary<string, string>? Parameters)?> LoadCurrentRouteAsync(CancellationToken cancellationToken = default);
    }
}
