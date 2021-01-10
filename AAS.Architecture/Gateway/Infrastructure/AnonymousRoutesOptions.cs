using System.Collections.Generic;

namespace AAS.Architecture.Gateway.Infrastructure
{
    internal sealed class AnonymousRoutesOptions
    {
        public IEnumerable<string> Routes { get; set; }
    }
}