using System.Collections.Concurrent;

namespace DashboardApp.Abstractions.Services.Providers
{
    public class JsLoaderService
    {
        public JsLoaderService()
        {
            ResourcesReadyState = new ConcurrentDictionary<string, TaskCompletionSource<bool>>();
        }

        public ConcurrentDictionary<string, TaskCompletionSource<bool>> ResourcesReadyState { get; }
    }
}
