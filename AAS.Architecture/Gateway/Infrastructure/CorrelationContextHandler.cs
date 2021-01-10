using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using OpenTracing;

namespace AAS.Architecture.Gateway.Infrastructure
{
    [UsedImplicitly]
    internal sealed class CorrelationContextHandler : DelegatingHandler
    {
        private static readonly ISet<string> OperationHeaderRequests = new HashSet<string>
            (StringComparer.OrdinalIgnoreCase)
            {
                "POST", "PUT", "PATCH", "DELETE"
            };

        private readonly ICorrelationContextBuilder correlationContextBuilder;

        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ITracer tracer;

        public CorrelationContextHandler(IHttpContextAccessor httpContextAccessor,
            ICorrelationContextBuilder correlationContextBuilder, ITracer tracer)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.correlationContextBuilder = correlationContextBuilder;
            this.tracer = tracer;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var httpContext = httpContextAccessor.HttpContext;
            var spanContext = tracer.ActiveSpan is null ? string.Empty : tracer.ActiveSpan.Context.ToString();
            var correlationId = Guid.NewGuid().ToString("N");
            var resourceId = httpContext.GetResourceIdFoRequest();
            var language = httpContext.GetLanguage();
            var correlationContext = correlationContextBuilder.Build(httpContext, correlationId, spanContext, language,
                resourceId: resourceId);
            request.Headers.TryAddWithoutValidation("Correlation-Context",
                JsonConvert.SerializeObject(correlationContext));

            if (OperationHeaderRequests.Contains(httpContext.Request.Method))
                httpContext.Response.SetOperationHeader(correlationId);

            return base.SendAsync(request, cancellationToken);
        }
    }
}