using System.Reflection;

namespace PageFlow.Blazor
{
    //
    // Summary:
    //     Denotes the target component as the index page component.
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    [Obfuscation(Exclude = true)]
    public class PageFlowIndexComponentAttribute : Attribute
    {
    }
}
