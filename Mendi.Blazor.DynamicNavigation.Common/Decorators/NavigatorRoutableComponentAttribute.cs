using System.Reflection;

namespace Mendi.Blazor.DynamicNavigation
{
    //
    // Summary:
    //     Denotes the target component as a routeable component.
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    [Obfuscation(Exclude = true)]
    public class NavigatorRoutableComponentAttribute : Attribute
    {
        //
        // Summary:
        //     Gets or sets a value that determines whether the component is a default
        //     page for the application with the specified app Id.
        //
        // Remarks:
        //     IsDefaultPage can be used on at most one component per application type.
        private readonly bool isDefaultPage;
        //
        // Summary:
        //     Gets or sets a value that determines what application the routeable
        //     component is for.
        private readonly int appId;
        //
        // Summary:
        //     Gets or sets a application name base on the appId.
        private readonly string appName;

        public NavigatorRoutableComponentAttribute(string appName, bool isDefault, int appId)
        {
            this.appName = appName;
            this.appId = appId;
            this.isDefaultPage = isDefault;
        }

        public string AppName => appName;

        public bool IsDefaultPage => isDefaultPage;

        public int AppId => appId;
    }
}
