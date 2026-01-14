using System.Reflection;

namespace PageFlow.Blazor
{
    //
    // Summary:
    //     Denotes the target class as the base component class.
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    [Obfuscation(Exclude = true)]
    public class PageFlowBaseComponentAttribute : Attribute
    {
    }
}
