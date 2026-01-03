namespace CountryApp.Abstractions.Models
{
    public class Profile
    {
        public bool IsLoggedIn { get; set; }
        public string Email { get; set; } = null!;
        public string? Name { get; set; }
        public string? DisplayImage { get; set; }
        public bool RequireApproval { get; set; }
        public DateTime LastLoggedIn { get; set; }
    }
}
