namespace Mendi.Blazor.DynamicNavigation.CLI
{
    public class RoutableComponentInfo
    {
        public Type ComponentType { get; set; }
        public int AppId { get; set; }
        public string AppName { get; set; }
        public bool IsDefault { get; set; }
        public string Name { get; set; } = null!;
        public string FullName { get; set; } = null!;
    }
}
