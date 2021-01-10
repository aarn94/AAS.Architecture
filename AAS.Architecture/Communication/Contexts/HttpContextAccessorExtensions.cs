using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Convey.MessageBrokers;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace AAS.Architecture.Communication.Contexts
{
    public static class HttpContextAccessorExtensions
    {
        public static CorrelationContext GetCorrelationContext(this ICorrelationContextAccessor accessor)
        {
            if (accessor.CorrelationContext is null)
                return null;

            var payload = JsonConvert.SerializeObject(accessor.CorrelationContext);

            return string.IsNullOrWhiteSpace(payload)
                ? null
                : JsonConvert.DeserializeObject<CorrelationContext>(payload);
        }

        public static CorrelationContext GetCorrelationContext(this IHttpContextAccessor accessor)
            => accessor.HttpContext?.Request.Headers.TryGetValue("Correlation-Context", out var json) is true
                ? JsonConvert.DeserializeObject<CorrelationContext>(json.FirstOrDefault())
                : null;

        internal static IDictionary<string, object> GetHeadersToForward(this IMessageProperties messageProperties)
        {
            const string sagaHeader = "Saga";
            if (messageProperties?.Headers is null ||
                !messageProperties.Headers.TryGetValue(sagaHeader, out var saga)) return null;

            return saga is null
                ? null
                : new Dictionary<string, object>
                    (StringComparer.OrdinalIgnoreCase)
                    {
                        [sagaHeader] = saga
                    };
        }

        internal static string GetSpanContext(this IMessageProperties messageProperties, string header)
        {
            if (messageProperties is null) return string.Empty;

            if (messageProperties.Headers.TryGetValue(header, out var span) && span is byte[] spanBytes)
                return Encoding.UTF8.GetString(spanBytes);

            return string.Empty;
        }
    }
}