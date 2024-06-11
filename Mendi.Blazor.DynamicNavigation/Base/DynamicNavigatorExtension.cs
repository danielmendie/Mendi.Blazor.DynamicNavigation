using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Mendi.Blazor.DynamicNavigation
{
    public static class DynamicNavigatorAccessorExtension
    {
        public static async Task<WebAssemblyHost> UseDynamicNavigator(this WebAssemblyHostBuilder builder, StorageUtilityType storageType = StorageUtilityType.LocalStorage)
        {
            builder.Services.AddSingleton<DynamicNavigatorContainer>();
            builder.Services.AddSingleton<DynamicNavigatorRegistry>();

            //initialize settings
            var settings = new NavigatorAppSettings
            {
                StorageType = storageType
            };
            builder.Services.AddSingleton(settings);

            if (storageType == StorageUtilityType.LocalStorage)
            {
                builder.Services.AddBlazoredLocalStorage();
            }
            else
            {
                builder.Services.AddScoped<DynamicNavigatorIndexedDbAccessor>();
            }

            var host = builder.Build();

            if (storageType == StorageUtilityType.IndexDb)
            {
                using var scope = host.Services.CreateScope();
                var navigatorAccessor = scope.ServiceProvider.GetService<DynamicNavigatorIndexedDbAccessor>();

                if (navigatorAccessor is not null)
                    await navigatorAccessor.InitializeAsync();
            }

            return host;
        }
    }
}
