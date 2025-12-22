using Microsoft.Extensions.DependencyInjection;

namespace Mendi.Blazor.DynamicNavigation.Tests.Builders
{
    public static class DefaultServiceBuilder
    {
        public static T Build<T>()
        {
            return BaseTest.ServiceProvider.GetService<T>();
        }
    }
}
