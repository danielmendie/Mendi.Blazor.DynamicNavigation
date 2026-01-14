using CountryApp.Abstractions.Constants;
using CountryApp.Abstractions.Models;
using Microsoft.JSInterop;
using PageFlow.Blazor;

namespace CountryApp.Layout
{
    public partial class HeaderMenu
    {
        private DotNetObjectReference<HeaderMenu>? objRef;
        ScriptLoader scriptLoader = null!;

        bool IsHeader;
        bool IsLayoutComplete;
        private bool RequiresRebinding = true;
        IEnumerable<NavMenuItem> MenuItems = [];
        private CancellationTokenSource? debounceCts;
        PageFlowInfo? Route;

        protected override async Task OnInitializedAsync()
        {
            MenuItems = await DocumentDataService.GetConfiguration<List<NavMenuItem>>(ConfigType.Menu) ?? [];
            NavigationState.Changed += OnNavigationStateChanged;
            UpdateRoute();
        }

        private void OnNavigationStateChanged()
        {
            UpdateRoute();
            InvokeAsync(StateHasChanged);
        }

        private void UpdateRoute()
        {
            Route = NavigationState.CurrentRoute;
            IsHeader = Route != null && Route.Params.TryGetValue("CustomHeader", out var header);
            RequiresRebinding = true;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (RequiresRebinding)
            {
                debounceCts?.Cancel();
                debounceCts = new CancellationTokenSource();

                try
                {
                    await Task.Delay(300, debounceCts.Token); // Debounce delay
                    if (!debounceCts.Token.IsCancellationRequested && IsLayoutComplete)
                    {
                        objRef ??= DotNetObjectReference.Create(this);
                        await AppJSRuntime.InvokeVoidAsync("binder_menu", objRef);
                        RequiresRebinding = false;
                    }
                }
                catch (TaskCanceledException)
                {
                }
            }
        }

        [JSInvokable]
        public async Task OnMenuVisibilityChanged(bool isMenuVisible)
        {
            if (!isMenuVisible)
            {
                await AppJSRuntime.InvokeVoidAsync("binder_menu", objRef);
                await scriptLoader.InvokeVoidAsync("OpenMenu");
            }
        }

        private async Task OnHamburgerMenuClicked()
        {
            if (objRef == null)
                objRef = DotNetObjectReference.Create(this);

            await AppJSRuntime.InvokeVoidAsync("check_menu_visibility", objRef);
        }

        async Task OnMenuItemClicked(string page, bool ignore = false)
        {
            await NavigateToAsync(page, ignoreHistory: ignore);
            RequiresRebinding = true;
        }

        bool IsMenuPage(string page) => Route != null && Route.Component == page;

        private async Task Logout() => await SignOut();

        public void Dispose()
        {
            objRef?.Dispose();
            debounceCts?.Dispose();
            NavigationState.Changed -= OnNavigationStateChanged;
        }
    }
}
