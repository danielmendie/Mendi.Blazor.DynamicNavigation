using Microsoft.AspNetCore.Components;

namespace PageFlow.Blazor
{
    public partial class FlowUI : ComponentBase, IDisposable
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
