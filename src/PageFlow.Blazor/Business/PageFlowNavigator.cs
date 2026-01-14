using Microsoft.AspNetCore.Components;
using System.Reflection;

namespace PageFlow.Blazor.Business
{
    public class PageFlowNavigator : IPageFlowNavigator
    {
        protected readonly IRouteResolver _routeResolver;
        protected readonly NavigationState _navigationState;

        public PageFlowNavigator(IRouteResolver routeResolver,
            NavigationState navigationState)
        {
            _routeResolver = routeResolver;
            _navigationState = navigationState;
        }

        public async Task NavigateToAsync<TComponent>(object? parameters) where TComponent : IComponent
        {
            var componentType = typeof(TComponent);
            var paramDict = ToDictionary(parameters);

            var route = await _routeResolver.GetRouteAsync(componentType.Name);

            if (route is null)
            {
                route = CreateDynamicRoute(componentType, paramDict);
            }

            _navigationState.Set(route, null);
        }

        private static PageFlowInfo CreateDynamicRoute(Type componentType, IDictionary<string, string> raw)
        {
            var parameterNames = componentType.GetProperties(
                    BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.GetCustomAttributes(typeof(ParameterAttribute), true).Any())
                .Select(p => p.Name)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var routeInfo = new PageFlowInfo
            {
                AppId = 0,
                PageName = componentType.Name,
                Component = nameof(componentType),
                IsDefault = false,
                ComponentType = componentType,
                Params = raw
                .Where(kvp => parameterNames.Contains(kvp.Key))
                .ToDictionary(kvp => kvp.Key, kvp => (object)kvp.Value)
            };

            return routeInfo;
        }

        private static Dictionary<string, string> ToDictionary(object? anon)
        {
            return anon is null ? [] : anon.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead)
                .ToDictionary(
                    p => p.Name,
                    p => p.GetValue(anon)?.ToString() ?? string.Empty);
        }
    }
}
