using CountryApp.Abstractions.Constants;
using CountryApp.Abstractions.Models;
using Mendi.Blazor.DynamicNavigation;
using Newtonsoft.Json;

namespace CountryApp.Pages.Search
{
    [NavigatorRoutableComponent("Your Favorites", false)]
    public partial class FavoriteStart
    {
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
            UserFavorites.Remove(searchItem);
            await LocalStorage.SetItemAsync(ConfigType.UserFavoritesStore, UserFavorites);
            StateHasChanged();
        }
    }
}
