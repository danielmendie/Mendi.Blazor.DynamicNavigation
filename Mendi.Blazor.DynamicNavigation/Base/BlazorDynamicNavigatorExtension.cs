using Blazored.LocalStorage;
using Mendi.Blazor.DynamicNavigation.Business;
using Microsoft.Extensions.DependencyInjection;

namespace Mendi.Blazor.DynamicNavigation
{
    public static class DynamicNavigatorAccessorExtension
    {
        /// <summary>
        /// Adds services required for the Blazor Dynamic Navigator to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <remarks>This method registers the necessary services for the Blazor Dynamic Navigator,
        /// including navigation state management, route resolution, and route history storage. It also configures the
        /// provided <see cref="NavigatorSettings"/> instance using the specified <paramref name="configuration"/>
        /// delegate.</remarks>
        /// <param name="services">The <see cref="IServiceCollection"/> to which the services will be added.</param>
        /// <param name="configuration">A delegate to configure the <see cref="NavigatorSettings"/> used by the Blazor Dynamic Navigator.</param>
        /// <returns>The updated <see cref="IServiceCollection"/> instance.</returns>
        public static IServiceCollection AddBlazorDynamicNavigator(this IServiceCollection services, Action<NavigatorSettings> configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));
            NavigatorSettings options = new NavigatorSettings();
            configuration(options);
            services.AddBlazoredLocalStorage();
            services.AddSingleton(options);
            services.AddSingleton<NavigatorRegistry>();
            services.AddScoped<IRouteStorage, RouteStorage>();
            services.AddScoped<IRouteHistory, RouteHistory>();
            services.AddScoped<NavigationState>();
            services.AddScoped<IRoutesProvider, RouteProvider>();
            services.AddScoped<IRouteResolver, RouteResolver>();
            return services;
        }
    }
}
