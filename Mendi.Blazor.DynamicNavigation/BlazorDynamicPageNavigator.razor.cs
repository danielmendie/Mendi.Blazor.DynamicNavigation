using Microsoft.AspNetCore.Components;

namespace Mendi.Blazor.DynamicNavigation
{
    public partial class BlazorDynamicPageNavigator
    {
        [Parameter] public DynamicNavigatorContainer NavigatorContainer { get; set; } = null!;
        [Parameter] public DynamicNavigatorRegistry NavigatorRegistry { get; set; } = null!;

        public Type? CurrentPageRoute;
        public Dictionary<string, DynamicNavigatorMetadata> ApplicationRoutes { get; set; } = [];

        protected override void OnParametersSet()
        {
            CurrentPageRoute = NavigatorContainer.CurrentPageRoute;
            ApplicationRoutes = NavigatorRegistry.ApplicationRoutes;
        }
    }
}
