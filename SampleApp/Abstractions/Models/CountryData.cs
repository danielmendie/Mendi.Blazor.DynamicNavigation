using Newtonsoft.Json;

namespace CountryApp.Abstractions.Models
{
    public class CityDTO
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } = null!;

        [JsonProperty("latitude")]
        public string Latitude { get; set; } = null!;

        [JsonProperty("longitude")]
        public string Longitude { get; set; } = null!;
    }

    public class CountryData
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } = null!;

        [JsonProperty("iso3")]
        public string Iso3 { get; set; } = null!;

        [JsonProperty("iso2")]
        public string Iso2 { get; set; } = null!;

        [JsonProperty("numeric_code")]
        public string NumericCode { get; set; } = null!;

        [JsonProperty("phone_code")]
        public string PhoneCode { get; set; } = null!;

        [JsonProperty("capital")]
        public string Capital { get; set; } = null!;

        [JsonProperty("currency")]
        public string Currency { get; set; } = null!;

        [JsonProperty("currency_name")]
        public string CurrencyName { get; set; } = null!;

        [JsonProperty("currency_symbol")]
        public string CurrencySymbol { get; set; } = null!;

        [JsonProperty("tld")]
        public string Tld { get; set; } = null!;

        [JsonProperty("native")]
        public string Native { get; set; } = null!;

        [JsonProperty("region")]
        public string Region { get; set; } = null!;

        [JsonProperty("region_id")]
        public int RegionId { get; set; }

        [JsonProperty("subregion")]
        public string Subregion { get; set; } = null!;

        [JsonProperty("subregion_id")]
        public int SubregionId { get; set; }

        [JsonProperty("nationality")]
        public string Nationality { get; set; } = null!;

        [JsonProperty("timezones")]
        public List<TimezoneDTO> Timezones { get; set; } = new List<TimezoneDTO>();

        [JsonProperty("latitude")]
        public string Latitude { get; set; } = null!;

        [JsonProperty("longitude")]
        public string Longitude { get; set; } = null!;

        [JsonProperty("emoji")]
        public string Emoji { get; set; } = null!;

        [JsonProperty("emojiU")]
        public string EmojiU { get; set; } = null!;

        [JsonProperty("states")]
        public List<StateDTO> States { get; set; } = new List<StateDTO>();
    }

    public class StateDTO
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } = null!;

        [JsonProperty("state_code")]
        public string StateCode { get; set; } = null!;

        [JsonProperty("latitude")]
        public string Latitude { get; set; } = null!;

        [JsonProperty("longitude")]
        public string Longitude { get; set; } = null!;

        [JsonProperty("cities")]
        public List<CityDTO> Cities { get; set; } = new List<CityDTO>();
    }

    public class TimezoneDTO
    {
        [JsonProperty("zoneName")]
        public string ZoneName { get; set; } = null!;

        [JsonProperty("gmtOffset")]
        public int GmtOffset { get; set; }

        [JsonProperty("gmtOffsetName")]
        public string GmtOffsetName { get; set; } = null!;

        [JsonProperty("abbreviation")]
        public string Abbreviation { get; set; } = null!;

        [JsonProperty("tzName")]
        public string TzName { get; set; } = null!;
    }
}
