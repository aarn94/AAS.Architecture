using System.Collections.Generic;

namespace AAS.Architecture.Gateway.Infrastructure
{
    internal sealed class AsyncRoutesOptions
    {
        public bool? Authenticate { get; set; }
        public IDictionary<string, AsyncRouteOptions> Routes { get; set; }
    }
}