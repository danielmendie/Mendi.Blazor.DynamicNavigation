namespace Mendi.Blazor.DynamicNavigation
{
    public class NavigatorHistory
    {
        public string Page { get; set; } = null!;
        public Dictionary<string, string> Params { get; set; } = [];
    }
}
