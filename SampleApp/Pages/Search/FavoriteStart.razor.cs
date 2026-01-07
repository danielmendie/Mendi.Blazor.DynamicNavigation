using CountryApp.Abstractions.Constants;
using CountryApp.Abstractions.Models;
using Newtonsoft.Json;
using PageFlow.Blazor;

namespace CountryApp.Pages.Search
{
    [PageFlowRoutableComponent("Your Favorites", false)]
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
