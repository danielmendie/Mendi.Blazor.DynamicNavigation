namespace Mendi.Blazor.DynamicNavigation
{
    public static class DynamicNavigatorIndexDbTableNameTypes
    {
        public static string Navigator { get; set; } = "data";
    }

    public static class DynamicNavigatorIndexDbKeyTypes
    {
        public static string Page { get; set; } = "navigator.page";
        public static string Routes { get; set; } = "navigator.routes";
    }
}
