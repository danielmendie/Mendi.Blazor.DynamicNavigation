namespace DashboardApp.Abstractions.Models
{
    public class Article
    {
        public int ParentId { get; set; }
        public int Sort { get; set; }
        public string Title { get; set; } = null!;
        public string Data { get; set; } = null!;
        public string Category { get; set; } = null!;
    }
}
