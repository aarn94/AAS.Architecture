using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using AAS.Architecture.Extensions;
using App.Metrics;
using App.Metrics.Gauge;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MetricsOptions = Convey.Metrics.AppMetrics.MetricsOptions;

namespace AAS.Architecture.Metrics
{
    public class MetricsJob : BackgroundService
    {
        private readonly GaugeOptions threads = new GaugeOptions
        {
            Name = "threads"
        };

        private readonly GaugeOptions workingSet = new GaugeOptions
        {
            Name = "working_set"
        };

        private readonly ILogger<MetricsJob> logger;
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly MetricsOptions options;

        public MetricsJob(IServiceScopeFactory serviceScopeFactory, MetricsOptions options, ILogger<MetricsJob> logger)
        {
            this.logger = logger;
            this.serviceScopeFactory = serviceScopeFactory;
            this.options = options;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!options.Enabled)
            {
                logger.LogInformation("Metrics are disabled.");
                return;
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    logger.LogTrace("Processing metrics...");
                    var metricsRoot = scope.ServiceProvider.GetRequiredService<IMetricsRoot>();
                    var process = Process.GetCurrentProcess();
                    metricsRoot.Measure.Gauge.SetValue(threads, process.Threads.Count);
                    metricsRoot.Measure.Gauge.SetValue(workingSet, process.WorkingSet64);
                }

                await Task.Delay(5000, stoppingToken).WithoutCapturingContext();
            }
        }
    }
}