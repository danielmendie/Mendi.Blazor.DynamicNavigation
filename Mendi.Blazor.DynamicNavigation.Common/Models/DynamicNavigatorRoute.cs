namespace Mendi.Blazor.DynamicNavigation
{
    //
    // Summary:
    //     Model class for instantiating a page route.
    //
    // Parameters:
    //   AppId:
    //     The current appId to switched to, defaults to 0. This is used for switching
    //     between multiple apps in the same project.
    //
    //   AppName:
    //     Application name to display on sites'.
    //
    //   Component:
    //     Name of the Component page to route to.
    //
    //   Action:
    //     The default action to display for the component route. This is useful for 
    //     controlling/displaying the last actions carried out on the page(e.g editing,
    //     creating, deleting or viewing). To use this, create an enum type and assign
    //     the integer value of that enum then retrieve it and use as needed.
    //
    //   Params:
    //     Parameters for the component page
    public class DynamicNavigatorRoute
    {
        public int AppId { get; set; }
        public string AppName { get; set; } = null!;
        public string Component { get; set; } = null!;
        public int? Action { get; set; }
        public Dictionary<string, string> Params { get; set; } = [];
    }

}
