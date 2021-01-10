using AAS.Architecture.Communication.Contexts;
using Microsoft.AspNetCore.Http;

namespace AAS.Architecture.Gateway.Infrastructure
{
    internal interface ICorrelationContextBuilder
    {
        CorrelationContext Build(HttpContext context, string correlationId, string spanContext,  string language, string name = null,
            string resourceId = null);
    }
}