using System.Globalization;
using System.Threading.Tasks;
using AAS.Architecture.Communication.Contexts;
using AAS.Architecture.Services;
using Convey.CQRS.Events;

namespace AAS.Architecture.Decorators.Language
{
    internal sealed class CorrelationContextEventHandlerDecorator<TEvent> : IEventHandler<TEvent>
        where TEvent : class, IEvent
    {
        private readonly IEventHandler<TEvent> handler;
        private readonly IRequestContextAccessor contextAccessor;

        public CorrelationContextEventHandlerDecorator(IEventHandler<TEvent> handler, IRequestContextAccessor contextAccessor)
        {
            this.handler = handler;
            this.contextAccessor = contextAccessor;
        }

        public Task HandleAsync(TEvent @event)
        {
            var context = contextAccessor.ReceivedContext;
            if (context is {})
                HandleReceivedContext(context);

            return handler.HandleAsync(@event);
        }
        
        private static void HandleReceivedContext(CorrelationContext context)
        {
            if(context.Language is {})
                SetCurrentThreadCulture(context.Language);
        }
        
        private static void SetCurrentThreadCulture(string culture)
        {
            CultureInfo.CurrentCulture = new CultureInfo(culture);
            CultureInfo.CurrentUICulture = new CultureInfo(culture);
        }
    }
}