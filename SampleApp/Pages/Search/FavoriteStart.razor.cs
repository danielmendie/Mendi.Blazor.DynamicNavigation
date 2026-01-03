using CountryApp.Abstractions.Constants;
using CountryApp.Abstractions.Models;
using Mendi.Blazor.DynamicNavigation;
using Newtonsoft.Json;

namespace CountryApp.Pages.Search
{
    [NavigatorRoutableComponent("Your Favorites", false)]
    public partial class FavoriteStart
    {
        List<LocationSearchItem> Bookmarks = [];

        protected override void OnInitialized()
        {
            Bookmarks = UserFavorites;
        }

        async Task ShowFavorite(LocationSearchItem searchItem)
        {
            var parameter = new Dictionary<string, string>()
            {
                {"Favorite", JsonConvert.SerializeObject(searchItem) }
            };
            await NavigateToAsync(nameof(CountryStart), parameter);
        }

        async Task Delete(LocationSearchItem searchItem)
        {
            Bookmarks.Remove(searchItem);
            await LocalStorage.SetItemAsync(ConfigType.UserFavoritesStore, Bookmarks);
            StateHasChanged();
        }
    }
}
