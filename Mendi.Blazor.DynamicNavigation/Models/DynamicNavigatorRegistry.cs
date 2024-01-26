namespace Mendi.Blazor.DynamicNavigation
{
    //
    // Summary:
    //     Route registry for storing/registering routes in the application.
    //
    // Parameters:
    //   ApplicationRoutes:
    //     A dictionary list of available routes the app can route to with its metadata.
    //
    //   DefaultsRoutes:
    //     A dictionary list of default routes for each app.
    public class DynamicNavigatorRegistry
    {
        public Dictionary<string, DynamicNavigatorMetadata> ApplicationRoutes { get; set; } = [];
        public Dictionary<int, string> DefaultsRoutes { get; set; } = [];
    }
}
