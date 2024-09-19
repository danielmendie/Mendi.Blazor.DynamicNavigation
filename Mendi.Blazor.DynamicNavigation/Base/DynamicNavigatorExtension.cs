using Blazored.LocalStorage;
using Mendi.Blazor.DynamicNavigation.Base;
using Microsoft.Extensions.DependencyInjection;

namespace Mendi.Blazor.DynamicNavigation
{
    public static class DynamicNavigatorAccessorExtension
    {
        public static IServiceCollection AddBlazorDynamicNavigator(this IServiceCollection services, Action<NavigatorAppSettings> configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration, "configuration");
            NavigatorAppSettings options = new NavigatorAppSettings();
            configuration(options);
            services.AddSingleton(options);
            services.AddSingleton<DynamicNavigatorContainer>();
            services.AddSingleton<DynamicNavigatorRegistry>();
            services.AddScoped<DynamicNavigatorIndexedDbAccessor>();
            services.AddBlazoredLocalStorage();
            services.AddHostedService<DynamicNavigatorIndexedDbInitializer>();
            return services;
        }
    }
}
