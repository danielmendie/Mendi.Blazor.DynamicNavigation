namespace DashboardApp.Abstractions.Models
{
    public class SettingGeneral
    {
        public string AppName { get; set; } = null!;
        public string AppDescription { get; set; } = null!;
        public string Address { get; set; } = null!;
        public List<string> SocialMediaHandles { get; set; } = new List<string>();
    }

}
