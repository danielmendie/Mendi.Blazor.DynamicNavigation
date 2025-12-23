namespace Mendi.Blazor.DynamicNavigation
{
    public interface IRouteResolver
    {
        Task<RoutePageInfo> GetDefaultRouteAsync(int appId, CancellationToken cancellationToken = default);
        Task<RoutePageInfo?> GetRouteAsync(string componentName, CancellationToken cancellationToken = default);
        Task<RoutePageInfo?> GetRouteWithIdAsync(string componentName, int appId, CancellationToken cancellationToken = default);
    }
}
