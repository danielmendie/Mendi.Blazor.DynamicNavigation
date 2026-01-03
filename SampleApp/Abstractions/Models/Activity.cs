using CountryApp.Abstractions.Enums;

namespace CountryApp.Abstractions.Models
{
    public class Activity
    {
        public EnumActivity ActivityType { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Description { get; set; } = null!;
        public string CreatedBy { get; set; } = null!;
    }
}
