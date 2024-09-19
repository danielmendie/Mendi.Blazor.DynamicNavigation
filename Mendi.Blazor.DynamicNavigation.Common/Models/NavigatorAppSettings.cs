namespace Mendi.Blazor.DynamicNavigation
{
    //
    // Summary:
    //      Config option for navigator
    public class NavigatorAppSettings
    {
        //
        // Summary:
        //     Set storage type to beused for persisting data - localstorage or indexDb
        public StorageUtilityType StorageType { get; set; }
        //
        // Summary:
        //     Ignores persisting data to storage
        public bool IgnoreDataPesistOption { get; set; }
    }
}
