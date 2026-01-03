namespace Mendi.Blazor.DynamicNavigation
{
    /// <summary>
    /// Represents the current navigation state, including the active route and its parameters.
    /// </summary>
    /// <remarks>This class provides functionality to manage and update the current navigation state in an
    /// application. It tracks the active route and its associated parameters, and notifies subscribers when the state
    /// changes.</remarks>
    public sealed class NavigationState
    {
        public RoutePageInfo? CurrentRoute { get; private set; }

        public event Action? Changed;
        /// <summary>
        /// Updates the current route and its associated parameters.
        /// </summary>
        /// <remarks>If <paramref name="parameters"/> is <see langword="null"/> or empty, no parameters
        /// will be updated. After the route and parameters are updated, the <see cref="Changed"/> event is
        /// invoked.</remarks>
        /// <param name="route">The new route to set as the current route.</param>
        /// <param name="parameters">An optional dictionary of parameters to associate with the route. If provided,  the parameters will be added
        /// to or updated in the current route's parameter collection.</param>
        public void Set(RoutePageInfo route, Dictionary<string, string>? parameters)
        {
            CurrentRoute = route;
            if (parameters != null && parameters.Any())
            {
                foreach (KeyValuePair<string, string> parameter in parameters)
                {
                    CurrentRoute.Params[parameter.Key] = parameter.Value;
                }
            }
            Changed?.Invoke();
        }
    }
}
