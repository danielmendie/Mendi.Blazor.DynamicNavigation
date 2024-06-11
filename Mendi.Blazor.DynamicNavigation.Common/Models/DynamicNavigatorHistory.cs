namespace Mendi.Blazor.DynamicNavigation
{
    public class DynamicNavigatorHistory
    {
        public string Page { get; set; } = null!;
        public Dictionary<string, string> Params { get; set; } = [];
    }
}
