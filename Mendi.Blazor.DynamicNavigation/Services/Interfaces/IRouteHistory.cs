namespace Mendi.Blazor.DynamicNavigation
{
    public interface IRouteHistory
    {
        void Record(RoutePageInfo route, Dictionary<string, string>? parameters);
        bool CanGoBack { get; }
        int EntrySize { get; }
        (RoutePageInfo Route, Dictionary<string, string>? Parameters)? GetPrevious();
        void Clear();
    }
}
