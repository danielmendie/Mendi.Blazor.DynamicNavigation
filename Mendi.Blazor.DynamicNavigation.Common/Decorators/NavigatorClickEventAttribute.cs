using System.Reflection;

namespace Mendi.Blazor.DynamicNavigation
{
    //
    // Summary:
    //     Denotes the target property as an event callback candidate for OnPageItemClicked.
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    [Obfuscation(Exclude = true)]
    public class NavigatorClickEventAttribute : Attribute
    {
        //
        // Summary:
        //     Sets the next page component to route to from this current page
        private readonly string gotoComponent;

        public NavigatorClickEventAttribute(string gotoComponent)
        {
            this.gotoComponent = gotoComponent;
        }

        public string NextRoutablePage => gotoComponent;
    }
}
