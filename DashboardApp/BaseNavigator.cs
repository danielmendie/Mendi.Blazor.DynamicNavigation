using Blazored.LocalStorage;
using DashboardApp.Abstractions.Constants;
using DashboardApp.Abstractions.Helpers;
using DashboardApp.Abstractions.Models;
using DashboardApp.Abstractions.Services.Data;
using DashboardApp.Pages.About;
using DashboardApp.Pages.Dashboard;
using DashboardApp.Pages.Profile;
using DashboardApp.Pages.Search;
using DashboardApp.Pages.Support;
using DashboardApp.Pages.Support.Pages;
using Mendi.Blazor.DynamicNavigation;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using DashboardApp.Pages.Profile.Pages;

namespace DashboardApp;
public class BaseNavigator : BlazorDynamicNavigatorBase
{
    [Inject]
    public NavigatorRegistry NavigatorRegistry { get; set; } = default !;

    [Inject]
    public ISnackbar Snackbar { get; set; } = null !;

    [Inject]
    public ILocalStorageService LocalStorage { get; set; } = default !;

    [Inject]
    public ISyncLocalStorageService SyncLocalStorage { get; set; } = default !;

    [Inject]
    public NavigationManager NavigationManager { get; set; } = null !;

    [Inject]
    public IJSRuntime AppJSRuntime { get; set; } = null !;

    [Inject]
    public IDocumentDataService DocumentDataService { get; set; } = null !;

    protected void ShowNotification(string message, Severity severity)
    {
        Snackbar.Clear();
        Snackbar.Add(message, severity);
    }

    public async Task SignOut()
    {
        await LocalStorage.ClearAsync();
        NavigationManager.NavigateTo("/", true);
    }

    public AuthUserData CurrentUser
    {
        get => SyncLocalStorage.GetItem<AuthUserData>(ConfigType.IdentityUserStore)!;
        set
        {
            InvokeAsync(StateHasChanged);
        }
    }

    public async Task UpdateUserData(string key, object value)
    {
        if (string.IsNullOrEmpty(key) || value == null)
            return;
        var updatedUserData = ObjectPropertyHelper.UpdateProperty(CurrentUser, key, value);
        await LocalStorage.SetItemAsync(ConfigType.IdentityUserStore, updatedUserData);
        CurrentUser = updatedUserData;
    }

    protected override async Task OnAppNavigationSetup()
    {
        NavigatorRegistry.Routes.Clear();
        NavigatorRegistry.Routes.AddRange(new List<RoutePageInfo>() { new RoutePageInfo { AppId = 0, PageName = "About App", Component = nameof(AboutStart), ComponentType = typeof(AboutStart), IsDefault = false, }, new RoutePageInfo { AppId = 0, PageName = "Dashboard", Component = nameof(StartPage), ComponentType = typeof(StartPage), IsDefault = true, }, new RoutePageInfo { AppId = 0, PageName = "Switch Apps", Component = nameof(SwitchPage), ComponentType = typeof(SwitchPage), IsDefault = false, }, new RoutePageInfo { AppId = 0, PageName = "Account Setting", Component = nameof(AccountStart), ComponentType = typeof(AccountStart), IsDefault = false, }, new RoutePageInfo { AppId = 0, PageName = "Country Search", Component = nameof(CountryStart), ComponentType = typeof(CountryStart), IsDefault = false, }, new RoutePageInfo { AppId = 0, PageName = "Your Favorites", Component = nameof(FavoriteStart), ComponentType = typeof(FavoriteStart), IsDefault = false, }, new RoutePageInfo { AppId = 0, PageName = "Support", Component = nameof(SupportStart), ComponentType = typeof(SupportStart), IsDefault = false, }, new RoutePageInfo { AppId = 0, PageName = "Account Deletion", Component = nameof(AccountDelete), ComponentType = typeof(AccountDelete), IsDefault = false, }, new RoutePageInfo { AppId = 0, PageName = "Profile Update", Component = nameof(AccountProfile), ComponentType = typeof(AccountProfile), IsDefault = false, }, new RoutePageInfo { AppId = 0, PageName = "Help Article", Component = nameof(ArticleView), ComponentType = typeof(ArticleView), IsDefault = false, Params = new() { { "ParentId", "Id" } } } });
        await base.OnAppNavigationSetup();
    }
}