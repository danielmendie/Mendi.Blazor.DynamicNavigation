using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Mendi.Blazor.DynamicNavigation
{
    public static class DynamicNavigatorAccessorExtension
    {
        public static async Task<WebAssemblyHost> UseDynamicNavigator(this WebAssemblyHostBuilder builder)
        {
            builder.Services.AddSingleton<DynamicNavigatorContainer>();
            builder.Services.AddSingleton<DynamicNavigatorRegistry>();
            builder.Services.AddScoped<IndexedDbAccessor>();

            var host = builder.Build();

            using var scope = host.Services.CreateScope();
            var navigatorAccessor = scope.ServiceProvider.GetService<IndexedDbAccessor>();

            if (navigatorAccessor is not null)
                await navigatorAccessor.InitializeAsync();

            return host;
        }
    }
}
