using System.Reflection;

namespace Mendi.Blazor.DynamicNavigation
{
    //
    // Summary:
    //     Denotes the target component as a routable component.
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    [Obfuscation(Exclude = true)]
    public class NavigatorRoutableComponentAttribute : Attribute
    {
        //
        // Summary:
        //     Gets or sets a value that determines whether the component is a routable
        //     page for the application with the specified app Id.
        //
        // Remarks:
        //     Makes the page the current default page for the specified app Id
        //     IsDefault can be used on at most one component per application type.
        private readonly bool isDefault;
        //
        // Summary:
        //     Gets or sets a value that determines what application the routable
        //     component is for.
        private readonly int appId;
        //
        // Summary:
        //     Gets or sets a application name base on the appId.
        private readonly string pageName;

        public NavigatorRoutableComponentAttribute(string pageName, bool isDefault, int appId = 0)
        {
            this.pageName = pageName;
            this.appId = appId;
            this.isDefault = isDefault;
        }

        public string PageName => pageName;

        public bool IsDefault => isDefault;

        public int AppId => appId;
    }
}
