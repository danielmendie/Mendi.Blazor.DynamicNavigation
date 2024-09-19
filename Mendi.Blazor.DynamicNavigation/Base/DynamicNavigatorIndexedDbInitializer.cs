using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Mendi.Blazor.DynamicNavigation.Base
{
    public class DynamicNavigatorIndexedDbInitializer : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public DynamicNavigatorIndexedDbInitializer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var navigatorAccessor = scope.ServiceProvider.GetService<DynamicNavigatorIndexedDbAccessor>();
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
