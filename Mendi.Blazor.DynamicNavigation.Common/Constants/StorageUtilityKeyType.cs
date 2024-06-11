namespace Mendi.Blazor.DynamicNavigation
{
    public static class DynamicNavigatorStorageKeyNameType
    {
        public static string Navigator { get; set; } = "data";
        public static string Page { get; set; } = "navigator.page";
        public static string Routes { get; set; } = "navigator.routes";
    }
}
