namespace CountryApp.Abstractions.Models
{
    public sealed class LocationSearchItem
    {
        public string DisplayName { get; init; } = null!;
        public string Type { get; init; } = null!;
        public int CountryId { get; init; }
        public int? StateId { get; init; }
        public int? CityId { get; init; }
        public string CountryIso2 { get; init; } = null!;
    }
}
