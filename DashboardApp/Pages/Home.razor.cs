
using CountryApp.Abstractions.Constants;
using CountryApp.Abstractions.Models;

namespace CountryApp.Pages
{
    public partial class Home
    {
        private int _loadProgress = 70;
        private bool _isBuildingIndex;

        protected override async Task OnInitializedAsync()
        {
            await EnsureIndexBuiltAsync();
            await OnAppNavigationSetup();
        }

        public async Task EnsureIndexBuiltAsync()
        {
            if (DataModule._locationIndex.Count > 0) return;

            _isBuildingIndex = true;

            var countries = await DocumentDataService
                .GetConfiguration<List<CountryData>>(ConfigType.CountryData);
            DataModule._countries.AddRange(countries!);

            var list = new List<LocationSearchItem>(countries!.Count * 4);

            int total = countries.Count;
            int processed = 0;

            foreach (var country in countries)
            {
                list.Add(new LocationSearchItem
                {
                    DisplayName = country.Name,
                    Type = "Country",
                    CountryId = country.Id,
                    CountryIso2 = country.Iso2
                });

                foreach (var state in country.States)
                {
                    list.Add(new LocationSearchItem
                    {
                        DisplayName = $"{state.Name}, {country.Name}",
                        Type = "State",
                        CountryId = country.Id,
                        StateId = state.Id,
                        CountryIso2 = country.Iso2
                    });

                    foreach (var city in state.Cities)
                    {
                        list.Add(new LocationSearchItem
                        {
                            DisplayName = $"{city.Name}, {state.Name}, {country.Name}",
                            Type = "City",
                            CountryId = country.Id,
                            StateId = state.Id,
                            CityId = city.Id,
                            CountryIso2 = country.Iso2
                        });
                    }
                }

                processed++;

                _loadProgress += (int)((processed / (double)total) * 100);
                if (processed % 10 == 0)
                {
                    await InvokeAsync(StateHasChanged);
                    await Task.Yield();
                }
            }

            DataModule._locationIndex.AddRange(list
                .OrderBy(x => x.DisplayName, StringComparer.OrdinalIgnoreCase));

            _isBuildingIndex = false;
            await InvokeAsync(StateHasChanged);

        }

    }
}
