using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using AAS.Architecture.Extensions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AAS.Architecture.Gateway.Infrastructure
{
    internal sealed class ResourceIdGeneratorMiddleware : IMiddleware
    {
        private readonly IPayloadBuilder payloadBuilder;

        public ResourceIdGeneratorMiddleware(IPayloadBuilder payloadBuilder) => this.payloadBuilder = payloadBuilder;

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (!string.Equals(context.Request.Method, "POST", StringComparison.OrdinalIgnoreCase))
            {
                await next(context).WithoutCapturingContext();
                return;
            }
            
            var resourceId = Guid.NewGuid().ToString("N");
            context.SetResourceIdFoRequest(resourceId);
            await next(context).WithoutCapturingContext();
        }
    }
}