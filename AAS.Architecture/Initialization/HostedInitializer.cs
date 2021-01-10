using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AAS.Architecture.Extensions;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AAS.Architecture.Initialization
{
    [UsedImplicitly]
    internal sealed class HostedInitializer: IHostedService
    {
        // We need to inject the IServiceProvider so we can create 
        // the scoped service, MyDbContext
        private readonly IServiceProvider serviceProvider;
        public HostedInitializer(IServiceProvider serviceProvider) => this.serviceProvider = serviceProvider;

        public async Task StartAsync(CancellationToken cancellationToken)
            {
                // Create a new scope to retrieve scoped services
                using(var scope = serviceProvider.CreateScope())
                {
                    // Get the DbContext instance
                    var initializers = scope.ServiceProvider.GetServices<ISingletonInitializer>().ToList();
                    initializers.ForEach(e => e.Initialize());
                }
                
                // Create a new scope to retrieve scoped services
                using(var scope = serviceProvider.CreateScope())
                {
                    // Get the DbContext instance
                    var initializers = scope.ServiceProvider.GetServices<ISingletonInitializerAsync>().ToList();

                    foreach (var initializer in initializers)
                    {
                        await initializer.InitializeAsync().WithoutCapturingContext();
                    }
                }
            }
        
            // noop
            public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}