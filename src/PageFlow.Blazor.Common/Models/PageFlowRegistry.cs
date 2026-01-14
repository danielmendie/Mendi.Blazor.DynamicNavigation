namespace PageFlow.Blazor
{
    //
    // Summary:
    //     Page flow route registry for storing/registering routes in the application.
    //
    // Parameters:
    //   Routes:
    //     A list of <see cref="PageFlowInfo"/> objects representing a registered routes.
    //
    public class PageFlowRegistry
    {
        public List<PageFlowInfo> Routes { get; set; } = [];
    }
}
