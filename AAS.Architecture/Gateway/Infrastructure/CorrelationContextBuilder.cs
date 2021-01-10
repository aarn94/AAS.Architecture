using System;
using System.Linq;
using System.Security.Claims;
using AAS.Architecture.Communication.Contexts;
using Microsoft.AspNetCore.Http;

namespace AAS.Architecture.Gateway.Infrastructure
{
    internal sealed class CorrelationContextBuilder : ICorrelationContextBuilder
    {
        public CorrelationContext Build(HttpContext context, string correlationId, string spanContext, string language,
            string name = null, string resourceId = null)
            => new CorrelationContext
            {
                CorrelationId = correlationId,
                Name = name ?? string.Empty,
                ResourceId = resourceId ?? string.Empty,
                SpanContext = spanContext ?? string.Empty,
                TraceId = context.TraceIdentifier,
                ConnectionId = context.Connection.Id,
                Language = language,
                CreatedAt = DateTime.UtcNow,
                User = new CorrelationContext.UserContext
                {
                    Id = context.User.Identity.Name,
                    IsAuthenticated = context.User.Identity.IsAuthenticated,
                    Role = context.User.Claims.FirstOrDefault(c => string.Equals(c.Type, ClaimTypes.Role, StringComparison.OrdinalIgnoreCase))?.Value,
                    Claims = context.User.Claims.ToDictionary(c => c.Type, c => c.Value, StringComparer.OrdinalIgnoreCase)
                }
            };
    }
}