using System.Reflection;

namespace Mendi.Blazor.DynamicNavigation
{
    //
    // Summary:
    //     Denotes the target component as the index page component.
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    [Obfuscation(Exclude = true)]
    public class NavigatorIndexComponentAttribute : Attribute
    {
    }
}
