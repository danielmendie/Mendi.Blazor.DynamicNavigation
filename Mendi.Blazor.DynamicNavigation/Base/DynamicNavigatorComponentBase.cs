using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;

namespace Mendi.Blazor.DynamicNavigation
{
    public abstract class DynamicNavigatorComponentBase : ComponentBase
    {
        [Inject] protected private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] protected private NavigatorAppSettings AppSettings { get; set; } = null!;
        [Inject] protected private DynamicNavigatorIndexedDbAccessor? IndexedDbAccessor { get; set; }
        [Inject] protected private ISyncLocalStorageService LocalStorage { get; set; } = null!;

        readonly List<DynamicNavigatorHistory> NavigationHistory = [];


        /// <summary>
        /// Constructs an instance of <see cref="DynamicNavigatorComponentBase"/>.
        /// </summary>
        public DynamicNavigatorComponentBase()
        { }

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
        /// <param name="appId">App Id of the component app to switch to</param>
        /// <param name="registry">Dynamic navigator registry</param>
        /// <param name="container">Dynamic navigation container</param>
        /// <param name="callingComponent">Type of calling component or class</param>
        /// <param name="customUrl">Custom url to navigate to after app is switched - Defaults to / if not set</param>
        /// <returns></returns>
        public virtual async Task<DynamicNavigatorRouteResult> OnSwitchPageCliked(int appId, DynamicNavigatorContainer container, DynamicNavigatorRegistry registry, Type callingComponent, string? customUrl = null)
        {
            DynamicNavigatorRouteResult result = new DynamicNavigatorRouteResult { NavigatorContainer = container };
            try
            {
                if (registry.DefaultsRoutes is not null)
                {
                    var component = registry.DefaultsRoutes[appId];
                    var pageRoute = registry.ApplicationRoutes.FirstOrDefault(v => v.Value.ComponentPath.Equals(component));

                    var SinglePageRoute = new DynamicNavigatorRoute
                    {
                        AppId = pageRoute.Value.AppId,
                        AppName = pageRoute.Value.AppName,
                        Component = nameof(pageRoute.Value.ComponentName)
                    };

                    result.NavigatorContainer = new DynamicNavigatorContainer
                    {
                        CurrentPageRoute = callingComponent.Assembly.GetType(pageRoute.Value.ComponentPath)
                    };
                    result.NavigatorRoute = SinglePageRoute;

                    if (!AppSettings.IgnoreDataPesistOption)
                    {
                        if (AppSettings.StorageType == StorageUtilityType.LocalStorage)
                        {
                            DynamicNavigatorLocalStorageAccessor.SetValue(LocalStorage!, DynamicNavigatorStorageKeyNameType.Page, SinglePageRoute);
                        }
                        else
                        {
                            await DynamicNavigatorAddStorageItem(DynamicNavigatorStorageKeyNameType.Page, SinglePageRoute);
                        }
                    }

                    if (string.IsNullOrWhiteSpace(customUrl))
                    {
                        NavigationManager.Refresh(forceReload: true);
                    }
                    else
                    {
                        NavigationManager.NavigateTo(customUrl, forceLoad: true);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return result;
        }

        /// <summary>
        /// Method invoked to return the previous page to the current screen.
        /// This is intended to behave like a browser's back button feature
        /// </summary>
        /// <param name="container">Dynamic navigation container</param>
        /// <param name="registry">Dynamic navigation registry</param>
        /// <param name="callingComponent">Type of calling component or class</param>
        /// <returns></returns>
        public virtual async Task<DynamicNavigatorRouteResult> OnBackToPreviousPageClicked(DynamicNavigatorContainer container, DynamicNavigatorRegistry registry, Type callingComponent)
        {
            DynamicNavigatorRouteResult result = new DynamicNavigatorRouteResult { NavigatorContainer = container };
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
                    DynamicNavigatorRoute SinglePageRoute = new()
                    {
                        AppId = comInfo.AppId,
                        AppName = comInfo.AppName,
                        Component = previousPage.Page,
                        Params = previousPage.Params ?? []
                    };

                    result.NavigatorContainer = new DynamicNavigatorContainer
                    {
                        CurrentPageRoute = callingComponent.Assembly.GetType(comInfo.ComponentPath)
                    };
                    result.NavigatorRoute = SinglePageRoute;

                    if (!AppSettings.IgnoreDataPesistOption)
                    {
                        if (AppSettings.StorageType == StorageUtilityType.LocalStorage)
                        {
                            DynamicNavigatorLocalStorageAccessor.SetValue(LocalStorage!, DynamicNavigatorStorageKeyNameType.Page, SinglePageRoute);
                        }
                        else
                        {
                            await DynamicNavigatorAddStorageItem(DynamicNavigatorStorageKeyNameType.Page, SinglePageRoute);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            return result;
        }

        /// <summary>
        /// Method invoked to return the previous page to the current screen.
        /// This is intended to behave like a browser's back button feature
        /// </summary>
        /// <param name="container">Dynamic navigation container</param>
        /// <param name="registry">Dynamic navigation registry</param>
        /// <param name="callingComponent">Type of calling component or class</param>
        /// <param name="navigationHistoryList">Custom list of string for storing navigation history</param>
        /// <returns></returns>
        public virtual async Task<DynamicNavigatorRouteResult> OnBackToPreviousPageClicked(DynamicNavigatorContainer container, DynamicNavigatorRegistry registry, Type callingComponent, List<DynamicNavigatorHistory> navigationHistoryList)
        {
            DynamicNavigatorRouteResult result = new DynamicNavigatorRouteResult { NavigatorContainer = container };
            if (navigationHistoryList.Count > 1)
            {
                try
                {
                    navigationHistoryList.RemoveAt(navigationHistoryList.Count - 1);
                    var previousPage = navigationHistoryList[navigationHistoryList.Count - 1];

                    if (previousPage.Params != null && previousPage.Params.Any())
                    {
                        foreach (var item in previousPage.Params)
                        {
                            registry.ApplicationRoutes[$"{previousPage.Page}"].ComponentParameters[item.Key] = item.Value;
                        }
                    }

                    var comInfo = registry.ApplicationRoutes[$"{previousPage.Page}"];
                    DynamicNavigatorRoute SinglePageRoute = new()
                    {
                        AppId = comInfo.AppId,
                        AppName = comInfo.AppName,
                        Component = previousPage.Page,
                        Params = previousPage.Params ?? []
                    };

                    result.NavigatorContainer = new DynamicNavigatorContainer
                    {
                        CurrentPageRoute = callingComponent.Assembly.GetType(comInfo.ComponentPath)
                    };
                    result.NavigatorRoute = SinglePageRoute;

                    if (!AppSettings.IgnoreDataPesistOption)
                    {
                        if (AppSettings.StorageType == StorageUtilityType.LocalStorage)
                        {
                            DynamicNavigatorLocalStorageAccessor.SetValue(LocalStorage!, DynamicNavigatorStorageKeyNameType.Page, SinglePageRoute);
                        }
                        else
                        {
                            await DynamicNavigatorAddStorageItem(DynamicNavigatorStorageKeyNameType.Page, SinglePageRoute);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            return result;
        }

        /// <summary>
        /// Method invoked when a page components item is clicked and a callback
        /// event is fired for action passing back the required params
        /// for navigation and data consumption
        /// </summary>
        /// <param name="parameters">Paramenters to pass to the receiving component</param>
        /// <param name="page">Component page to invoke to the screen</param>
        /// <param name="callingComponent">Type of calling component or class</param>
        /// <param name="container">Dynamic navigation container</param>
        /// <param name="registry">Dynamic navigation registry</param>
        /// <returns></returns>
        public virtual async Task<DynamicNavigatorRouteResult> OnPageItemClicked(Dictionary<string, string> parameters, string page, DynamicNavigatorContainer container, DynamicNavigatorRegistry registry, Type callingComponent)
        {
            DynamicNavigatorRouteResult result = new DynamicNavigatorRouteResult { NavigatorContainer = container };
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
                DynamicNavigatorRoute SinglePageRoute = new()
                {
                    AppId = comInfo.AppId,
                    AppName = comInfo.AppName,
                    Component = page,
                    Params = parameters ?? []
                };

                result.NavigatorContainer = new DynamicNavigatorContainer
                {
                    CurrentPageRoute = callingComponent.Assembly.GetType(comInfo.ComponentPath)
                };
                result.NavigatorRoute = SinglePageRoute;

                if (!AppSettings.IgnoreDataPesistOption)
                {
                    if (AppSettings.StorageType == StorageUtilityType.LocalStorage)
                    {
                        DynamicNavigatorLocalStorageAccessor.SetValue(LocalStorage!, DynamicNavigatorStorageKeyNameType.Page, SinglePageRoute);
                    }
                    else
                    {
                        await DynamicNavigatorAddStorageItem(DynamicNavigatorStorageKeyNameType.Page, SinglePageRoute);
                    }
                }

                var history = new DynamicNavigatorHistory { Page = page, Params = parameters ?? [] };
                if (NavigationHistory.Count == 0 || NavigationHistory[^1] != history)
                {
                    NavigationHistory.Add(history);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return result;
        }

        /// <summary>
        /// Method invoked when a page components item is clicked and a callback
        /// event is fired for action passing back the required params
        /// for navigation and data consumption
        /// </summary>
        /// <param name="parameters">Paramenters to pass to the receiving component</param>
        /// <param name="page">Component page to invoke to the screen</param>
        /// <param name="callingComponent">Type of calling component or class</param>
        /// <param name="navigationHistoryList">Custom list of string for storing navigation history</param>
        /// <param name="container">Dynamic navigation container</param>
        /// <param name="registry">Dynamic navigation registry</param>
        /// <returns></returns>
        public virtual async Task<DynamicNavigatorRouteResult> OnPageItemClicked(Dictionary<string, string> parameters, string page, DynamicNavigatorContainer container, DynamicNavigatorRegistry registry, Type callingComponent, List<DynamicNavigatorHistory> navigationHistoryList)
        {
            DynamicNavigatorRouteResult result = new DynamicNavigatorRouteResult { NavigatorContainer = container };
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
                DynamicNavigatorRoute SinglePageRoute = new()
                {
                    AppId = comInfo.AppId,
                    AppName = comInfo.AppName,
                    Component = page,
                    Params = parameters ?? []
                };

                result.NavigatorContainer = new DynamicNavigatorContainer
                {
                    CurrentPageRoute = callingComponent.Assembly.GetType(comInfo.ComponentPath)
                };
                result.NavigatorRoute = SinglePageRoute;

                if (!AppSettings.IgnoreDataPesistOption)
                {
                    if (AppSettings.StorageType == StorageUtilityType.LocalStorage)
                    {
                        DynamicNavigatorLocalStorageAccessor.SetValue(LocalStorage!, DynamicNavigatorStorageKeyNameType.Page, SinglePageRoute);
                    }
                    else
                    {
                        await DynamicNavigatorAddStorageItem(DynamicNavigatorStorageKeyNameType.Page, SinglePageRoute);
                    }
                }

                var history = new DynamicNavigatorHistory { Page = page, Params = parameters ?? [] };
                if (navigationHistoryList.Count == 0 || navigationHistoryList[^1] != history)
                    navigationHistoryList.Add(history);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return result;
        }

        /// <summary>
        /// Method invoked when a nav menu item is clicked and a callback event
        /// is fired for the action
        /// </summary>
        /// <param name="pageComponentName">Component page to invoke to the screen</param>
        /// <param name="callingComponent">Type of calling component or class</param>
        /// <param name="navigationHistoryList">Custom list of string for storing navigation history</param>
        /// <param name="container">Dynamic navigation container</param>
        /// <param name="registry">Dynamic navigation registry</param>
        /// <returns></returns>
        public virtual async Task<DynamicNavigatorRouteResult> OnNavMenuItemCliked(string pageComponentName, DynamicNavigatorContainer container, DynamicNavigatorRegistry registry, Type callingComponent, List<DynamicNavigatorHistory> navigationHistoryList)
        {
            DynamicNavigatorRouteResult result = new DynamicNavigatorRouteResult { NavigatorContainer = container };
            if (!string.IsNullOrWhiteSpace(pageComponentName) && callingComponent is not null)
            {
                try
                {
                    var comInfo = registry.ApplicationRoutes[$"{pageComponentName}"];
                    DynamicNavigatorRoute SinglePageRoute = new()
                    {
                        AppId = comInfo.AppId,
                        AppName = comInfo.AppName,
                        Component = pageComponentName
                    };

                    result.NavigatorContainer = new DynamicNavigatorContainer
                    {
                        CurrentPageRoute = callingComponent.Assembly.GetType(comInfo.ComponentPath)
                    };
                    result.NavigatorRoute = SinglePageRoute;

                    if (!AppSettings.IgnoreDataPesistOption)
                    {
                        if (AppSettings.StorageType == StorageUtilityType.LocalStorage)
                        {
                            DynamicNavigatorLocalStorageAccessor.SetValue(LocalStorage!, DynamicNavigatorStorageKeyNameType.Page, SinglePageRoute);
                        }
                        else
                        {
                            await DynamicNavigatorAddStorageItem(DynamicNavigatorStorageKeyNameType.Page, SinglePageRoute);
                        }
                    }

                    var history = new DynamicNavigatorHistory { Page = pageComponentName, Params = [] };
                    if (navigationHistoryList.Count == 0 || navigationHistoryList[^1] != history)
                        navigationHistoryList.Add(history);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            return result;
        }

        /// <summary>
        /// Method invoked when a nav menu item is clicked and a callback event
        /// is fired for the action
        /// </summary>
        /// <param name="pageComponentName">Component page to invoke to the screen</param>
        /// <param name="callingComponent">Type of calling component or class</param>
        /// <param name="container">Dynamic navigation container</param>
        /// <param name="registry">Dynamic navigation registry</param>
        /// <returns></returns>
        public virtual async Task<DynamicNavigatorRouteResult> OnNavMenuItemCliked(string pageComponentName, DynamicNavigatorContainer container, DynamicNavigatorRegistry registry, Type callingComponent)
        {
            DynamicNavigatorRouteResult result = new DynamicNavigatorRouteResult { NavigatorContainer = container };
            if (!string.IsNullOrWhiteSpace(pageComponentName) && callingComponent is not null)
            {
                try
                {
                    var comInfo = registry.ApplicationRoutes[$"{pageComponentName}"];
                    DynamicNavigatorRoute SinglePageRoute = new()
                    {
                        AppId = comInfo.AppId,
                        AppName = comInfo.AppName,
                        Component = pageComponentName
                    };

                    result.NavigatorContainer = new DynamicNavigatorContainer
                    {
                        CurrentPageRoute = callingComponent.Assembly.GetType(comInfo.ComponentPath)
                    };
                    result.NavigatorRoute = SinglePageRoute;

                    if (!AppSettings.IgnoreDataPesistOption)
                    {
                        if (AppSettings.StorageType == StorageUtilityType.LocalStorage)
                        {
                            DynamicNavigatorLocalStorageAccessor.SetValue(LocalStorage!, DynamicNavigatorStorageKeyNameType.Page, SinglePageRoute);
                        }
                        else
                        {
                            await DynamicNavigatorAddStorageItem(DynamicNavigatorStorageKeyNameType.Page, SinglePageRoute);
                        }
                    }

                    var history = new DynamicNavigatorHistory { Page = pageComponentName, Params = [] };
                    if (NavigationHistory.Count == 0 || NavigationHistory[^1] != history)
                        NavigationHistory.Add(history);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            return result;
        }

        /// <summary>
        /// Get data value persisted in storage
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<T?> DynamicNavigatorGetStorageItem<T>(string key)
        {
            if (AppSettings.StorageType == StorageUtilityType.LocalStorage)
            {
                return DynamicNavigatorLocalStorageAccessor.GetValue<T>(LocalStorage!, key);
            }
            else
            {
                if (IndexedDbAccessor != null)
                {
                    return await IndexedDbAccessor.GetValueAsync<T>(DynamicNavigatorStorageKeyNameType.Navigator, key);
                }
                else
                {
                    return default(T);
                }
            }
        }
        /// <summary>
        /// Persist data in browswer using storage
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task DynamicNavigatorAddStorageItem(string key, object value)
        {
            if (AppSettings.StorageType == StorageUtilityType.LocalStorage)
            {
                DynamicNavigatorLocalStorageAccessor.SetValue(LocalStorage!, key, value);
            }
            else
            {
                if (IndexedDbAccessor != null)
                {
                    await IndexedDbAccessor!.SetValueAsync(DynamicNavigatorStorageKeyNameType.Navigator, new { Id = key, value });
                }
            }
        }
        /// <summary>
        /// Delete data persisted in storage
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task DynamicNavigatorDeleteStorageItem(string key)
        {
            if (AppSettings.StorageType == StorageUtilityType.LocalStorage)
            {
                DynamicNavigatorLocalStorageAccessor.DeleteValue(LocalStorage!, key);
            }
            else
            {
                if (IndexedDbAccessor != null)
                {
                    await IndexedDbAccessor!.RemoveValueAsync(DynamicNavigatorStorageKeyNameType.Navigator, key);
                }
            }
        }
        /// <summary>
        /// Clear out all data persisted in storage
        /// </summary>
        /// <returns></returns>
        public async Task DynamicNavigatorClearStorage()
        {
            if (AppSettings.StorageType == StorageUtilityType.LocalStorage)
            {
                DynamicNavigatorLocalStorageAccessor.ClearStorage(LocalStorage!);
            }
            else
            {
                if (IndexedDbAccessor != null)
                {
                    await IndexedDbAccessor!.ClearAllValueAsync(DynamicNavigatorStorageKeyNameType.Navigator);
                }
            }
        }
    }
}
