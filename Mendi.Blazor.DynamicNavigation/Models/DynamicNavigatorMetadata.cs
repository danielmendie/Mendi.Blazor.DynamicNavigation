namespace Mendi.Blazor.DynamicNavigation
{
    //
    // Summary:
    //     A single page component metadata.
    //
    // Parameters:
    //   AppId:
    //     The current appId to switched to, defaults to 0. This is used for switching
    //     between multiple apps in the same project.
    //
    //   AppName:
    //     Application name to display on sites'.
    //
    //   ComponentName:
    //     Name of the Component page.
    //
    //   ComponentPath:
    //     Fully qualified name of the page component. This is used to create a type of
    //     the specified page component
    //
    //   ComponentParameters:
    //     Parameters the specified page component requires
    //
    public class DynamicNavigatorMetadata
    {
        public int AppId { get; set; }
        public string AppName { get; set; } = null!;
        public string ComponentName { get; set; } = null!;
        public string ComponentPath { get; set; } = null!;
        public Dictionary<string, object> ComponentParameters { get; set; } = [];
    }
}
