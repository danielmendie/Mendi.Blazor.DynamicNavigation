using Blazored.LocalStorage;

namespace Mendi.Blazor.DynamicNavigation.Services.Helpers
{
    public sealed class LocalStorageHelper
    {
        public static void SetValue<T>(ISyncLocalStorageService localStorageService, string sessionName, T Value)
        {
            localStorageService.SetItem(sessionName, Value);
        }

        public static T? GetValue<T>(ISyncLocalStorageService localStorageService, string sessionName)
        {
            return localStorageService.GetItem<T>(sessionName);
        }

        public static void DeleteValue(ISyncLocalStorageService localStorageService, string sessionName)
        {
            localStorageService.RemoveItem(sessionName);
        }

        public static void ClearStorage(ISyncLocalStorageService localStorageService)
        {
            localStorageService.Clear();
        }
    }
}
