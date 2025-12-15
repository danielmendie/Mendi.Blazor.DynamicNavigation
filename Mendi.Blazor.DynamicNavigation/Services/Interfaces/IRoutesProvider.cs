namespace Mendi.Blazor.DynamicNavigation
{
    public interface IRoutesProvider
    {
        IReadOnlyList<RoutePageInfo> GetRoutes();
    }
}
