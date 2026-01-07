using CountryApp.Abstractions.Constants;
using CountryApp.Abstractions.Enums;
using CountryApp.Abstractions.Models;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using PageFlow.Blazor;

namespace CountryApp.Pages.Search
{
    [PageFlowRoutableComponent("Country Search", false)]
    public partial class CountryStart
    {
        [Parameter] public string? Favorite { get; set; }

        protected override void OnInitialized()
        {
            if (!string.IsNullOrWhiteSpace(Favorite))
            {
                LocationIndex = JsonConvert.DeserializeObject<LocationSearchItem>(Favorite);
                Country = LocationIndex!.DisplayName;
            }
        }

        private async Task<IEnumerable<string>> Search(string value, CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(value))
                return Enumerable.Empty<string>();

            if (DataModule._locationIndex.Count == 0)
                return Enumerable.Empty<string>();

            if (token.IsCancellationRequested)
                return Enumerable.Empty<string>();

            await Task.Delay(10, token);

            var results = DataModule._locationIndex
                .Where(x => x.DisplayName
                    .StartsWith(value, StringComparison.OrdinalIgnoreCase))
                .Take(20)
                .Select(x => x.DisplayName)
                .ToList();

            return results;
        }

        LocationSearchItem? LocationIndex;
        CountryData? InfoCountry;
        string _country = string.Empty;
        string Country
        {
            get => _country;
            set
            {
                _country = value;
                LocationIndex = value == LocationIndex?.DisplayName ? LocationIndex : DataModule._locationIndex.First(g => g.DisplayName == value);
                InfoCountry = DataModule._countries.First(m => m.Id == LocationIndex.CountryId);
                if (!string.IsNullOrWhiteSpace(value))
                {
                    RecordQuota();
                    RecordActivity(value);
                }
                InvokeAsync(StateHasChanged);
            }
        }

        void RecordQuota()
        {
            var quota = UserQuota;
            quota.Add(new SearchQuota { Owner = CurrentUser.Email, CreatedOn = DateTime.UtcNow });
            SyncLocalStorage.SetItem(ConfigType.UserQuotaStore, quota);
        }

        void RecordActivity(string search)
        {
            var activities = UserActivities;
            activities.Add(new Activity { ActivityType = EnumActivity.Search, CreatedOn = DateTime.UtcNow, Description = $"You search for {search}", CreatedBy = CurrentUser.Email });
            SyncLocalStorage.SetItem(ConfigType.UserActivitiesStore, activities);
        }

        async Task SaveFavorite()
        {
            var favList = UserFavorites;
            if (LocationIndex == null)
                return;
            if (favList.Any(f => f.CountryId == LocationIndex.CountryId && f.StateId == LocationIndex.StateId && f.CityId == LocationIndex.CityId))
            {
                ShowNotification("Already in favorite", MudBlazor.Severity.Success);
                return;
            }

            favList.Add(LocationIndex);
            var activities = UserActivities;
            activities.Add(new Activity { ActivityType = EnumActivity.Favorite, CreatedOn = DateTime.UtcNow, Description = $"You bookmarked a search for {LocationIndex.DisplayName}", CreatedBy = CurrentUser.Email });
            await LocalStorage.SetItemAsync(ConfigType.UserActivitiesStore, activities);
            await LocalStorage.SetItemAsync(ConfigType.UserFavoritesStore, favList);
            ShowNotification("Search added to favorite", MudBlazor.Severity.Success);
        }

    }
}
