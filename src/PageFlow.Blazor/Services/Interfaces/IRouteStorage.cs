namespace PageFlow.Blazor
{
    public interface IRouteStorage
    {
        Task SaveCurrentRouteAsync(PageFlowInfo route, Dictionary<string, string>? parameters, CancellationToken cancellationToken = default);
        Task<(PageFlowInfo Route, Dictionary<string, string>? Parameters)?> LoadCurrentRouteAsync(CancellationToken cancellationToken = default);
    }
}
