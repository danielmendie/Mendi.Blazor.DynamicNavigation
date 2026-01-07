namespace PageFlow.Blazor
{
    public interface IRouteHistory
    {
        void Record(PageFlowInfo route, Dictionary<string, string>? parameters);
        bool CanGoBack { get; }
        int EntrySize { get; }
        (PageFlowInfo Route, Dictionary<string, string>? Parameters)? GetPrevious();
        void Clear();
    }
}
