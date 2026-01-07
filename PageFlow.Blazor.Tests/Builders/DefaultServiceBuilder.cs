using Microsoft.Extensions.DependencyInjection;

namespace PageFlow.Blazor.Tests.Builders
{
    public static class DefaultServiceBuilder
    {
        public static T Build<T>()
        {
            return BaseTest.ServiceProvider.GetService<T>();
        }
    }
}
