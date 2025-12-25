namespace DashboardApp.Abstractions.Models
{
    public class NavMenuItem
    {
        public string Name { get; set; } = null!;
        public string? Icon { get; set; }
        public string Path { get; set; } = null!;
        public int Sort { get; set; }
        public List<NavMenuItem> Children { get; set; } = new List<NavMenuItem>();
    }
}
