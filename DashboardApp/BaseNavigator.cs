using Blazored.LocalStorage;
using CountryApp.Abstractions.Constants;
using CountryApp.Abstractions.Enums;
using CountryApp.Abstractions.Helpers;
using CountryApp.Abstractions.Models;
using CountryApp.Abstractions.Services.Data;
using CountryApp.Abstractions.Services.Util;
using CountryApp.Pages.About;
using CountryApp.Pages.Dashboard;
using CountryApp.Pages.Profile;
using CountryApp.Pages.Profile.Pages;
using CountryApp.Pages.Search;
using CountryApp.Pages.Support;
using CountryApp.Pages.Support.Pages;
using Mendi.Blazor.DynamicNavigation;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;

namespace CountryApp;
public class BaseNavigator : BlazorDynamicNavigatorBase
{
    [Inject]
    public NavigatorRegistry NavigatorRegistry { get; set; } = default!;

    [Inject]
    public ISnackbar SnackBar { get; set; } = null!;

    [Inject]
    public ILocalStorageService LocalStorage { get; set; } = default!;

    [Inject]
    public ISyncLocalStorageService SyncLocalStorage { get; set; } = default!;

    [Inject]
    public NavigationManager NavigationManager { get; set; } = null!;

    [Inject]
    public IJSRuntime AppJSRuntime { get; set; } = null!;

    [Inject]
    public IDocumentDataService DocumentDataService { get; set; } = null!;
    [Inject] public DataModule DataModule { get; set; } = null!;

    protected void ShowNotification(string message, Severity severity)
    {
        SnackBar.Clear();
        SnackBar.Add(message, severity);
    }

    public async Task SignOut()
    {
        var user = CurrentUser;
        user.IsLoggedIn = false;
        var activities = UserActivities;
        activities.Add(new Activity { ActivityType = EnumActivity.SignOut, CreatedOn = DateTime.UtcNow, Description = $"You signed out of your account.", CreatedBy = CurrentUser.Email });

        await LocalStorage.SetItemAsync(ConfigType.UserActivitiesStore, activities);
        await LocalStorage.SetItemAsync(ConfigType.IdentityUserStore, user);
        NavigationManager.NavigateTo("/", true);
    }

    public List<LocationSearchItem> UserFavorites
    {
        get => SyncLocalStorage.GetItem<List<LocationSearchItem>>(ConfigType.UserFavoritesStore) ?? new List<LocationSearchItem>();
        set { InvokeAsync(StateHasChanged); }
    }

    public List<Activity> UserActivities
    {
        get => SyncLocalStorage.GetItem<List<Activity>>(ConfigType.UserActivitiesStore) ?? new List<Activity>();
        set { InvokeAsync(StateHasChanged); }
    }

    public AuthUserData CurrentUser
    {
        get => SyncLocalStorage.GetItem<AuthUserData>(ConfigType.IdentityUserStore)!;
        set { InvokeAsync(StateHasChanged); }
    }

    public async Task UpdateUserData(string key, object value)
    {
        if (string.IsNullOrEmpty(key) || value == null)
            return;
        var updatedUserData = ObjectPropertyHelper.UpdateProperty(CurrentUser ?? new(), key, value);
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