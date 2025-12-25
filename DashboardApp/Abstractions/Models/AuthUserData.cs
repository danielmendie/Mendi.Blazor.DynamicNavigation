namespace DashboardApp.Abstractions.Models
{
    public class AuthUserData
    {
        public bool IsLoggedIn { get; set; }
        public string Email { get; set; } = null!;
        public string? Name { get; set; }
        public string? DisplayImage { get; set; }
        public bool RequireApproval { get; set; }
    }
}
