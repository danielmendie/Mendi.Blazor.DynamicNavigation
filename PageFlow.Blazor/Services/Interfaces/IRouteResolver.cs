namespace PageFlow.Blazor
{
    public interface IRouteResolver
    {
        Task<PageFlowInfo> GetDefaultRouteAsync(int appId, CancellationToken cancellationToken = default);
        Task<PageFlowInfo?> GetRouteAsync(string componentName, CancellationToken cancellationToken = default);
        Task<PageFlowInfo?> GetRouteWithIdAsync(string componentName, int appId, CancellationToken cancellationToken = default);
    }
}
