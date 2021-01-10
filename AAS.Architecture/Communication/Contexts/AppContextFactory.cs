using System.Globalization;
using Convey.MessageBrokers;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace AAS.Architecture.Communication.Contexts
{
    internal sealed class AppContextFactory : IAppContextFactory
    {
        private readonly ICorrelationContextAccessor contextAccessor;
        private readonly IHttpContextAccessor httpContextAccessor;

        public AppContextFactory(ICorrelationContextAccessor contextAccessor, IHttpContextAccessor httpContextAccessor)
        {
            this.contextAccessor = contextAccessor;
            this.httpContextAccessor = httpContextAccessor;
        }

        public IAppContext Create()
        {
            if (contextAccessor.CorrelationContext is {})
            {
                var payload = JsonConvert.SerializeObject(contextAccessor.CorrelationContext);
                
                var handledContext = string.IsNullOrWhiteSpace(payload)
                    ? AppContext.Empty
                    : new AppContext(JsonConvert.DeserializeObject<CorrelationContext>(payload));

                return handledContext;
            }

            var context = httpContextAccessor.GetCorrelationContext();
            
            return context is null ? AppContext.Empty : new AppContext(context);
        }
    }
}