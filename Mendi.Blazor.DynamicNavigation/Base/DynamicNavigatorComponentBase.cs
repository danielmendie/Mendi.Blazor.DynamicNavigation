using Microsoft.AspNetCore.Components;

namespace Mendi.Blazor.DynamicNavigation
{
    public abstract class DynamicNavigatorComponentBase : ComponentBase
    {
        [Inject] protected private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private IndexedDbAccessor IndexedDbAccessor { get; set; } = null!;

        private List<NavigatorHistory> NavigationHistory = [];


        /// <summary>
        /// Constructs an instance of <see cref="DynamicNavigatorComponentBase"/>.
        /// </summary>
        public DynamicNavigatorComponentBase() { }

        /// <summary>
        /// Method invoked when scaffolding the page routes of applications
        /// base on page component configurations done
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public virtual Task GetPageRoutes()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Method invoked when scaffolding the build routes for application.
        /// This looks at the app route tree and builds a navigation point for the 
        /// app
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public virtual Task BuildPageRoutes()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Method invoked to switch between multiple app routes base on appId type
        /// </summary>
        /// <param name="page"></param>
        /// <exception cref="NotImplementedException"></exception>
        public virtual async Task OnSwitchPageCliked(int page, DynamicNavigatorRegistry registry)
        {
            try
            {
                if (registry.DefaultsRoutes is not null)
                {
                    var component = registry.DefaultsRoutes[page];
                    var pageRoute = registry.ApplicationRoutes.FirstOrDefault(v => v.Value.ComponentPath.Equals(component));

                    var data = new DynamicNavigatorRoute
                    {
                        AppId = pageRoute.Value.AppId,
                        AppName = pageRoute.Value.AppName,
                        Component = nameof(pageRoute.Value.ComponentName)
                    };
                    await DynamicNavigatorIndexDbAddValue(DynamicNavigatorIndexDbKeyTypes.Page, data);
                    NavigationManager.NavigateTo("/", forceLoad: true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// Method invoked to return the previous page to the current screen.
        /// This is intended to behave like a browser's back button feature
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public virtual async Task<DynamicNavigatorContainer> OnBackToPreviousPageClicked(DynamicNavigatorContainer container, DynamicNavigatorRegistry registry, Type callingComponent)
        {
            if (NavigationHistory.Count > 1)
            {
                try
                {
                    NavigationHistory.RemoveAt(NavigationHistory.Count - 1);
                    var previousPage = NavigationHistory[NavigationHistory.Count - 1];

                    if (previousPage.Params != null && previousPage.Params.Any())
                    {
                        foreach (var item in previousPage.Params)
                        {
                            registry.ApplicationRoutes[$"{previousPage.Page}"].ComponentParameters[item.Key] = item.Value;
                        }
                    }

                    var comInfo = registry.ApplicationRoutes[$"{previousPage.Page}"];
                    container = new DynamicNavigatorContainer { CurrentPageRoute = callingComponent.Assembly.GetType(comInfo.ComponentPath) };
                    DynamicNavigatorRoute SinglePageRoute = new() { AppId = comInfo.AppId, AppName = comInfo.AppName, Component = previousPage.Page, Params = previousPage.Params ?? [] };
                    await DynamicNavigatorIndexDbAddValue(DynamicNavigatorIndexDbKeyTypes.Page, SinglePageRoute);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            return container;
        }

        /// <summary>
        /// Method invoked when a page components item is clicked and a callback
        /// event is fired for action passing back the required params
        /// for navigation and data consumption
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="page"></param>
        /// <param name="callingComponent"></param>
        /// <returns></returns>
        public virtual async Task<DynamicNavigatorContainer> OnPageItemClicked(Dictionary<string, string> parameters, string page, DynamicNavigatorContainer container, DynamicNavigatorRegistry registry, Type callingComponent)
        {
            try
            {
                if (parameters != null && parameters.Any())
                {
                    foreach (var item in parameters)
                    {
                        registry.ApplicationRoutes[$"{page}"].ComponentParameters[item.Key] = item.Value;
                    }
                }

                var comInfo = registry.ApplicationRoutes[$"{page}"];
                container = new DynamicNavigatorContainer { CurrentPageRoute = callingComponent.Assembly.GetType(comInfo.ComponentPath) };
                DynamicNavigatorRoute SinglePageRoute = new() { AppId = comInfo.AppId, AppName = comInfo.AppName, Component = page, Params = parameters ?? [] };
                await DynamicNavigatorIndexDbAddValue(DynamicNavigatorIndexDbKeyTypes.Page, SinglePageRoute);

                var history = new NavigatorHistory { Page = page, Params = parameters ?? [] };
                if (NavigationHistory.Count == 0 || NavigationHistory[^1] != history)
                    NavigationHistory.Add(history);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return container;
        }

        /// <summary>
        /// Method invoked when a nav menu item is clicked and a callback event
        /// is fired for the action
        /// </summary>
        /// <param name="pageComponentName"></param>
        /// <param name="callingComponent"></param>
        public virtual async Task<DynamicNavigatorContainer> OnNavMenuItemCliked(string pageComponentName, DynamicNavigatorContainer container, DynamicNavigatorRegistry registry, Type callingComponent)
        {
            if (!string.IsNullOrWhiteSpace(pageComponentName) && callingComponent is not null)
            {
                try
                {
                    var comInfo = registry.ApplicationRoutes[$"{pageComponentName}"];
                    container = new DynamicNavigatorContainer { CurrentPageRoute = callingComponent.Assembly.GetType(comInfo.ComponentPath) };
                    DynamicNavigatorRoute SinglePageRoute = new() { AppId = comInfo.AppId, AppName = comInfo.AppName, Component = pageComponentName };
                    await DynamicNavigatorIndexDbAddValue(DynamicNavigatorIndexDbKeyTypes.Page, SinglePageRoute);

                    var history = new NavigatorHistory { Page = pageComponentName, Params = [] };
                    if (NavigationHistory.Count == 0 || NavigationHistory[^1] != history)
                        NavigationHistory.Add(history);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            return container;
        }

        /// <summary>
        /// Get data value persisted in index db
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<T> DynamicNavigatorIndexDbGetValue<T>(string key) => await IndexedDbAccessor.GetValueAsync<T>(DynamicNavigatorIndexDbTableNameTypes.Navigator, key);
        /// <summary>
        /// Persist data in browswer using index db
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task DynamicNavigatorIndexDbAddValue(string key, object value) => await IndexedDbAccessor.SetValueAsync(DynamicNavigatorIndexDbTableNameTypes.Navigator, new { Id = key, value });
        /// <summary>
        /// Delete data persisted in index db
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task DynamicNavigatorIndexDbDeleteValue(string key) => await IndexedDbAccessor.RemoveValueAsync(DynamicNavigatorIndexDbTableNameTypes.Navigator, key);
        /// <summary>
        /// Clear out all data persisted in index db
        /// </summary>
        /// <returns></returns>
        public async Task DynamicNavigatorIndexDbClearValue() => await IndexedDbAccessor.ClearAllValueAsync(DynamicNavigatorIndexDbTableNameTypes.Navigator);
    }
}
