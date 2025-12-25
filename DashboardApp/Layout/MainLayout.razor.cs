using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace DashboardApp.Layout
{
    public partial class MainLayout
    {
        private bool _isDarkMode;
        [Inject] private IJSRuntime JSRuntime { get; set; } = null!;

        protected override async Task OnInitializedAsync()
        {
            var theme = await JSRuntime.InvokeAsync<string>("themeInterop.getSystemTheme");
            _isDarkMode = theme == "dark";
        }
    }
}
