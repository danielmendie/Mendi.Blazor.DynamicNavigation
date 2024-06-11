using System.Reflection;

namespace Mendi.Blazor.DynamicNavigation
{
    //
    // Summary:
    //     Denotes the target class as the base component class.
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    [Obfuscation(Exclude = true)]
    public class NavigatorBaseComponentAttribute : Attribute
    {
    }
}
