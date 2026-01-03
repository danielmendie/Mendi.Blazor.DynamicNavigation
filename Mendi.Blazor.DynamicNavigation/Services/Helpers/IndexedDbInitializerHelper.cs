using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Mendi.Blazor.DynamicNavigation.Services.Helpers
{
    public class IndexedDbInitializerHelper : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public IndexedDbInitializerHelper(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var navigatorAccessor = scope.ServiceProvider.GetService<IndexedDbHelper>();
            if (navigatorAccessor is not null)
            {
                await navigatorAccessor.InitializeAsync();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
