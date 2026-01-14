namespace PageFlow.Blazor
{
    public interface IRoutesProvider
    {
        IReadOnlyList<PageFlowInfo> GetRoutes();
    }
}
