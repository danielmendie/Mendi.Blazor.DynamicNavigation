using Mendi.Blazor.DynamicNavigation.Business;
using Microsoft.AspNetCore.Components;

namespace Mendi.Blazor.DynamicNavigation
{
    public partial class BlazorDynamicNavigator : ComponentBase, IDisposable
    {
        [Inject] private NavigationState NavigationState { get; set; } = default!;

        protected override void OnInitialized()
        {
            NavigationState.Changed += OnStateChanged;
        }

        private void OnStateChanged()
        {
            InvokeAsync(StateHasChanged);
        }

        public void Dispose()
        {
            NavigationState.Changed -= OnStateChanged;
        }
    }
}
