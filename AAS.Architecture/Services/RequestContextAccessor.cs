using System.Globalization;
using AAS.Architecture.Communication.Contexts;
using Convey.MessageBrokers;
using Microsoft.AspNetCore.Http;

namespace AAS.Architecture.Services
{
    internal sealed class RequestContextAccessor : IRequestContextAccessor
    {
        private readonly ICorrelationContextAccessor contextAccessor;
        private readonly IHttpContextAccessor httpContextAccessor;

        public RequestContextAccessor(ICorrelationContextAccessor contextAccessor, IHttpContextAccessor httpContextAccessor)
        {
            this.contextAccessor = contextAccessor;
            this.httpContextAccessor = httpContextAccessor;
        }

        public CorrelationContext ReceivedContext =>  contextAccessor.GetCorrelationContext() ?? httpContextAccessor.GetCorrelationContext();

        public CorrelationContext ContextForSend
        {
            get
            {
                var context = ReceivedContext;
                if (context is {})
                    context.Language ??= CultureInfo.CurrentCulture.Name;

                return context;
            }
        }
    }
}