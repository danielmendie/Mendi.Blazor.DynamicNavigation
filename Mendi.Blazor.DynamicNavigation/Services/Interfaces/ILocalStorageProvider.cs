namespace Mendi.Blazor.DynamicNavigation
{
    public interface ILocalStorageProvider
    {
        Task SetItemAsync<T>(string key, T value);
        Task<T?> GetItemAsync<T>(string key);
    }
}
