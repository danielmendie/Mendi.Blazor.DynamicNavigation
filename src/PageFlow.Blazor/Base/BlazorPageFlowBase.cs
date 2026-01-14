using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace PageFlow.Blazor
{
    public abstract class BlazorPageFlowBase : ComponentBase
    {
        [Inject] protected IRouteResolver RouteResolver { get; set; } = default!;
        [Inject] protected IRouteHistory History { get; set; } = default!;
        [Inject] protected IRouteStorage Storage { get; set; } = default!;
        [Inject] protected ILogger<BlazorPageFlowBase> Logger { get; set; } = default!;
        [Inject] protected NavigationState NavigationState { get; set; } = default!;
        [Inject] protected PageFlowSettings Setting { get; set; } = default!;
        /// <summary>
        /// Gets a value indicating whether navigation to a previous item in the history is possible.
        /// </summary>
        protected bool CanGoBack => History.CanGoBack;
        /// <summary>
        /// Asynchronously initializes the navigator and restores the state or navigates to the default view.
        /// </summary>
        /// <remarks>This method is invoked automatically by the Blazor framework during the component's
        /// initialization lifecycle. It ensures that the component is properly set up before rendering.</remarks>
        /// <returns></returns>
        protected virtual async Task OnAppFlowSetup()
        {
            await RestoreOrNavigateToDefaultAsync();
        }
        /// <summary>
        /// Restores the previously saved route or navigates to the default route if no saved route is available.
        /// </summary>
        /// <remarks>This method attempts to restore the last saved route using the application's storage.
        /// If no route is found, it resolves and navigates to the default route. In case of an error during the
        /// process, the failure is logged, and the <see cref="OnNavigationFailedAsync(Exception)"/> method is
        /// invoked.</remarks>
        /// <returns></returns>
        protected virtual async Task RestoreOrNavigateToDefaultAsync()
        {
            try
            {
                var restored = await Storage.LoadCurrentRouteAsync();
                if (!Setting.IgnoreRouteHistory && restored is not null)
                {
                    await NavigateInternalAsync(restored.Value.Route, restored.Value.Parameters);
                    return;
                }

                var defaultRoute = await RouteResolver.GetDefaultRouteAsync(appId: 0);
                await NavigateInternalAsync(defaultRoute, parameters: null);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to restore or navigate to default route.");
                await OnNavigationFailedAsync(ex);
            }
        }
        /// <summary>
        /// Navigates to the specified component asynchronously, resolving its route and passing optional parameters.
        /// </summary>
        /// <remarks>This method resolves the route for the specified component name using the route
        /// resolver and navigates to it. If the navigation fails, the error is logged, and a custom failure handling
        /// method is invoked.</remarks>
        /// <param name="componentName">The name of the component to navigate to. This value cannot be <see langword="null"/> or empty.</param>
        /// <param name="parameters">An optional dictionary of parameters to pass to the component. The keys represent parameter names, and the
        /// values represent their corresponding values.</param>
        /// <param name="ignoreHistory">Ignores saving the specified route in navigation history.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Thrown if the route for the specified <paramref name="componentName"/> cannot be resolved.</exception>
        protected async Task NavigateToAsync(string componentName, Dictionary<string, string>? parameters = null, bool ignoreHistory = false)
        {
            try
            {
                var route = await RouteResolver.GetRouteAsync(componentName)
                            ?? throw new InvalidOperationException($"Route not found for component '{componentName}'");
                await NavigateInternalAsync(route, parameters, ignoreHistory);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Navigation to {Component} failed.", componentName);
                await OnNavigationFailedAsync(ex);
            }
        }
        /// <summary>
        /// Navigates to the default route of the specified application asynchronously.
        /// </summary>
        /// <remarks>This method resolves the default route for the specified application and performs the
        /// navigation. If an error occurs during navigation, the exception is logged, and the <see
        /// cref="OnNavigationFailedAsync(Exception)"/> method is invoked.</remarks>
        /// <param name="appId">The unique identifier of the application to navigate to.</param>
        /// <returns></returns>
        protected async Task NavigateToAppAsync(int appId)
        {
            try
            {
                var defaultRoute = await RouteResolver.GetDefaultRouteAsync(appId);
                await NavigateInternalAsync(defaultRoute, parameters: null);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Navigation to app with Id: {app} failed.", appId);
                await OnNavigationFailedAsync(ex);
            }
        }
        /// <summary>
        /// Navigates to the previous route in the navigation history, if available.
        /// </summary>
        /// <remarks>If the navigation history does not contain a previous route, the method invokes  <see
        /// cref="OnCannotGoBackAsync"/> to handle the scenario. This method does not perform any action  if the history
        /// is empty or the previous route is null.</remarks>
        /// <returns></returns>
        protected async Task GoBackAsync()
        {
            if (!History.CanGoBack)
            {
                await OnCannotGoBackAsync();
                return;
            }

            var previous = History.GetPrevious();
            if (previous is null)
            {
                await OnCannotGoBackAsync();
                return;
            }

            await NavigateInternalAsync(previous.Value.Route, previous.Value.Parameters, true);
        }
        /// <summary>
        /// Invoked before navigation to a new route occurs.
        /// </summary>
        /// <param name="route">The target route information for the navigation.</param>
        /// <param name="parameters">An optional dictionary of parameters associated with the navigation. Can be <see langword="null"/>.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation. The default implementation returns a
        /// completed task.</returns>
        protected virtual Task OnBeforeNavigateAsync(PageFlowInfo route, Dictionary<string, string>? parameters) => Task.CompletedTask;
        /// <summary>
        /// Executes custom logic after navigation to a new route is completed.
        /// </summary>
        /// <param name="route">The information about the route that was navigated to.</param>
        /// <param name="parameters">A dictionary containing the parameters associated with the navigation, or <see langword="null"/> if no
        /// parameters are provided.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation. The default implementation returns a
        /// completed task.</returns>
        protected virtual Task OnAfterNavigateAsync(PageFlowInfo route, Dictionary<string, string>? parameters) => Task.CompletedTask;
        /// <summary>
        /// Invoked when a navigation operation fails.
        /// </summary>
        /// <param name="exception">The exception that caused the navigation failure.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        protected virtual Task OnNavigationFailedAsync(Exception exception) => Task.CompletedTask;
        /// <summary>
        /// Invoked when a request to navigate back cannot be fulfilled.
        /// </summary>
        /// <remarks>This method is called in scenarios where navigation back is not possible, such as
        /// when the navigation stack is empty. Override this method to provide custom handling or user feedback in such
        /// cases. The default implementation completes immediately with no action.</remarks>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation. The default implementation returns a
        /// completed task.</returns>
        protected virtual Task OnCannotGoBackAsync() => Task.CompletedTask;
        /// <summary>
        /// Navigates to the specified route and updates the navigation state asynchronously.
        /// </summary>
        /// <remarks>This method performs several actions during navigation, including invoking
        /// pre-navigation and post-navigation hooks, updating the route history (if enabled), saving the current route
        /// to storage, and notifying the UI navigator of the updated navigation state.</remarks>
        /// <param name="route">The route information representing the target page.</param>
        /// <param name="parameters">An optional dictionary of parameters to pass to the route. Can be <see langword="null"/>.</param>
        /// <param name="ignoreHistory">An optional parameter that skips route persistency in history</param>
        /// <returns></returns>
        private async Task NavigateInternalAsync(PageFlowInfo route, Dictionary<string, string>? parameters = null, bool ignoreHistory = false)
        {
            await OnBeforeNavigateAsync(route, parameters);
            if (!ignoreHistory && !Setting.IgnoreRouteHistory)
            {
                History.Record(route, parameters);
            }
            await Storage.SaveCurrentRouteAsync(route, parameters);
            NavigationState.Set(route, parameters);
            await OnAfterNavigateAsync(route, parameters);
        }
    }
}
