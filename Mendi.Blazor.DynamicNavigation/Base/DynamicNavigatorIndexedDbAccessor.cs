using Microsoft.JSInterop;

namespace Mendi.Blazor.DynamicNavigation
{
    public class DynamicNavigatorIndexedDbAccessor : IDisposable, IAsyncDisposable
    {
        private Lazy<IJSObjectReference>? _accessorJsRef = new();
        private readonly IJSRuntime _jsRuntime;

        public DynamicNavigatorIndexedDbAccessor(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

#pragma warning disable CS8602 // Dereference of a possibly null reference.
        public async Task InitializeAsync()
        {
            await WaitForReference();
            await _accessorJsRef.Value.InvokeVoidAsync("initialize");
        }

        private async Task WaitForReference(CancellationToken token = default)
        {
            if (_accessorJsRef.IsValueCreated is false)
            {
                _accessorJsRef = new(await _jsRuntime.InvokeAsync<IJSObjectReference>("import", cancellationToken: token, "./_content/Mendi.Blazor.DynamicNavigation/NavigatorAccessor.js"));
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_accessorJsRef.IsValueCreated)
            {
                await _accessorJsRef.Value.DisposeAsync();
            }
        }

        public async Task<T> GetValueAsync<T>(string collectionName, string id, CancellationToken token = default)
        {
            await WaitForReference(token);
            var result = await _accessorJsRef.Value.InvokeAsync<T>("get", cancellationToken: token, collectionName, id);
            return result;
        }

        public async Task SetValueAsync<T>(string collectionName, T value, CancellationToken token = default)
        {
            await WaitForReference(token);
            await _accessorJsRef.Value.InvokeVoidAsync("set", cancellationToken: token, collectionName, value);
        }

        public async Task RemoveValueAsync(string collectionName, string id, CancellationToken token = default)
        {
            await WaitForReference(token);
            await _accessorJsRef.Value.InvokeVoidAsync("remove", cancellationToken: token, collectionName, id);
        }

        public async Task ClearAllValueAsync(string collectionName, CancellationToken token = default)
        {
            await WaitForReference(token);
            await _accessorJsRef.Value.InvokeVoidAsync("clear", cancellationToken: token, collectionName);
        }
#pragma warning restore CS8602 // Dereference of a possibly null reference.

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                (_accessorJsRef as IDisposable)?.Dispose();
                _accessorJsRef = null;
            }
        }

    }
}
