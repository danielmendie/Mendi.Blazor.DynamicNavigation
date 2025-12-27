using Microsoft.JSInterop;

namespace Mendi.Blazor.DynamicNavigation.Business
{
    public sealed class LocalStorageProvider : ILocalStorageProvider, IAsyncDisposable
    {
        private readonly Lazy<Task<IJSObjectReference>> _module;

        public LocalStorageProvider(IJSRuntime jsRuntime)
        {
            _module = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
                "import",
                "./_content/Mendi.Blazor.DynamicNavigation/LocalStorageAccessor.js"
            ).AsTask());
        }

        public async Task SetItemAsync<T>(string key, T value)
        {
            var m = await _module.Value;
            await m.InvokeVoidAsync("setItem", key, value);
        }

        public async Task<T?> GetItemAsync<T>(string key)
        {
            var m = await _module.Value;
            return await m.InvokeAsync<T?>("getItem", key);
        }

        public async Task RemoveItemAsync(string key)
        {
            var m = await _module.Value;
            await m.InvokeVoidAsync("removeItem", key);
        }

        public async Task ClearAsync()
        {
            var m = await _module.Value;
            await m.InvokeVoidAsync("clear");
        }

        public async ValueTask DisposeAsync()
        {
            if (_module.IsValueCreated)
            {
                var m = await _module.Value;
                await m.DisposeAsync();
            }
        }
    }
}
