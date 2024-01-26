﻿using Microsoft.AspNetCore.Components;

namespace Mendi.Blazor.DynamicNavigation
{
    public abstract class DynamicNavigatorComponentBase : ComponentBase
    {
        [Inject] protected private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] public IndexedDbAccessor IndexedDbAccessor { get; set; } = null!;


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
                    await IndexDbAddValue(IndexDbKeyTypes.Page, data);
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
        public virtual void OnBackToPreviousPageClicked()
        {
            throw new NotImplementedException();
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
        public virtual async Task<DynamicNavigatorContainer> OnPageItemClicked(Dictionary<string, string> parameters, string page, DynamicNavigatorRegistry registry, Type callingComponent)
        {
            DynamicNavigatorContainer Container = new();
            try
            {
                if (parameters.Any())
                {
                    foreach (var item in parameters)
                    {
                        registry.ApplicationRoutes[$"{page}"].ComponentParameters[item.Key] = item.Value;
                    }
                }

                var comInfo = registry.ApplicationRoutes[$"{page}"];
                Container = new DynamicNavigatorContainer { CurrentPageRoute = callingComponent.Assembly.GetType(comInfo.ComponentPath) };
                DynamicNavigatorRoute SinglePageRoute = new() { AppId = comInfo.AppId, AppName = comInfo.AppName, Component = page, Params = parameters };
                await IndexDbAddValue(IndexDbKeyTypes.Page, SinglePageRoute);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return Container;
        }

        /// <summary>
        /// Method invoked when a nav menu item is clicked and a callback event
        /// is fired for the action
        /// </summary>
        /// <param name="pageComponentName"></param>
        /// <param name="callingComponent"></param>
        public virtual async Task<DynamicNavigatorContainer> OnNavMenuItemCliked(string pageComponentName, DynamicNavigatorRegistry registry, Type callingComponent)
        {
            DynamicNavigatorContainer Container = new();
            if (!string.IsNullOrWhiteSpace(pageComponentName) && callingComponent is not null)
            {
                try
                {
                    var comInfo = registry.ApplicationRoutes[$"{pageComponentName}"];
                    Container = new DynamicNavigatorContainer { CurrentPageRoute = callingComponent.Assembly.GetType(comInfo.ComponentPath) };
                    DynamicNavigatorRoute SinglePageRoute = new() { AppId = comInfo.AppId, AppName = comInfo.AppName, Component = pageComponentName };
                    await IndexDbAddValue(IndexDbKeyTypes.Page, SinglePageRoute);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            return Container;
        }

        /// <summary>
        /// Get data value persisted in index db
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<T> IndexDbGetValue<T>(string key) => await IndexedDbAccessor.GetValueAsync<T>(IndexDbTableNameTypes.Navigator, key);
        /// <summary>
        /// Persist data in browswer using index db
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task IndexDbAddValue(string key, object value) => await IndexedDbAccessor.SetValueAsync(IndexDbTableNameTypes.Navigator, new { Id = key, value });
        /// <summary>
        /// Delete data persisted in index db
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task IndexDbDeleteValue(string key) => await IndexedDbAccessor.RemoveValueAsync(IndexDbTableNameTypes.Navigator, key);
        /// <summary>
        /// Clear out all data persisted in index db
        /// </summary>
        /// <returns></returns>
        public async Task IndexDbClearValue() => await IndexedDbAccessor.ClearAllValueAsync(IndexDbTableNameTypes.Navigator);
    }
}
