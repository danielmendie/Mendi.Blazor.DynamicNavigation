namespace PageFlow.Blazor
{
    //
    // Summary:
    //     Model class for instantiating a page flow route.
    //
    // Parameters:
    //   AppId:
    //     The current appId to switched to, defaults to 0. This is used for switching
    //     between multiple apps in the same project.
    //
    //   PageName:
    //     Page name to display on sites.
    //
    //   ComponentType:
    //     The component type using typeof()
    //
    //   Component:
    //     Name of the Component page to route to.
    //
    //   IsDefault:
    //     Makes the page a default page for the specified AppId.
    //
    //   Action:
    //     The default action to display for the component route. This is useful for 
    //     controlling/displaying the last actions carried out on the page(e.g editing,
    //     creating, deleting or viewing). To use this, create an enum type and assign
    //     the integer value of that enum then retrieve it and use as needed.
    //
    //   Params:
    //     Parameters for the component page
    public class PageFlowInfo
    {
        public int AppId { get; set; }
        public string PageName { get; set; } = null!;
        public required Type ComponentType { get; init; }
        public string Component { get; set; } = null!;
        public int? Action { get; set; }
        public bool IsDefault { get; set; }
        public Dictionary<string, object> Params { get; set; } = [];
    }

}
