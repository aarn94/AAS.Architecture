using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using App.Metrics;
using App.Metrics.Counter;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MetricsOptions = Convey.Metrics.AppMetrics.MetricsOptions;

namespace AAS.Architecture.Metrics
{
    public class CustomMetricsMiddleware : IMiddleware
    {
        private readonly bool enabled;

        private readonly IDictionary<string, CounterOptions> metrics =
            new Dictionary<string, CounterOptions>(StringComparer.OrdinalIgnoreCase);

        private readonly IServiceScopeFactory serviceScopeFactory;

        public CustomMetricsMiddleware(IServiceScopeFactory serviceScopeFactory, MetricsOptions options)
        {
            this.serviceScopeFactory = serviceScopeFactory;
            enabled = options.Enabled;
        }

        public Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (!enabled) return next(context);

            var request = context.Request;
            if (!this.metrics.TryGetValue(GetKey(request.Method, request.Path.ToString()), out var metrics))
                return next(context);

            using var scope = serviceScopeFactory.CreateScope();
            var metricsRoot = scope.ServiceProvider.GetRequiredService<IMetricsRoot>();
            metricsRoot.Measure.Counter.Increment(metrics);

            return next(context);
        }

        private static string GetKey(string method, string path)
            => $"{method}:{path}";

        private static CounterOptions Command(string command)
            => new CounterOptions
            {
                Name = "commands",
                Tags = new MetricTags("command", command),
            };

        private static CounterOptions Query(string query)
            => new CounterOptions
            {
                Name = "queries",
                Tags = new MetricTags("query", query),
            };
    }
}