using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AAS.Architecture.Events;
using AAS.Architecture.Extensions;
using Convey.CQRS.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AAS.Architecture.Services
{
    internal sealed class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly IEventMapper eventMapper;
        private readonly IMessageBroker messageBroker;
        private readonly ILogger<IEventProcessor> logger;

        public EventProcessor(IServiceScopeFactory serviceScopeFactory, IEventMapper eventMapper,
            IMessageBroker messageBroker, ILogger<IEventProcessor> logger)
        {
            this.serviceScopeFactory = serviceScopeFactory;
            this.eventMapper = eventMapper;
            this.messageBroker = messageBroker;
            this.logger = logger;
        }

        public async Task ProcessAsync(IEnumerable<IDomainEvent> events)
        {
            if (events is null)
            {
                return;
            }

            logger.LogTrace("Processing domain events...");
            var integrationEvents = await HandleDomainEventsAsync(events).WithoutCapturingContext();
            if (!integrationEvents.Any())
            {
                return;
            }

            logger.LogTrace("Processing integration events...");
            await messageBroker.PublishAsync(integrationEvents).WithoutCapturingContext();
        }

        private async Task<List<IEvent>> HandleDomainEventsAsync(IEnumerable<IDomainEvent> events)
        {
            var integrationEvents = new List<IEvent>();
            using var scope = serviceScopeFactory.CreateScope();
            foreach (var @event in events)
            {
                var eventType = @event.GetType();
                logger.LogTrace($"Handling domain event: {eventType.Name}");
                var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(eventType);
                dynamic handlers = scope.ServiceProvider.GetServices(handlerType);
                foreach (var handler in handlers)
                {
                    await handler.HandleAsync((dynamic) @event);
                }

                var integrationEvent = eventMapper.Map(@event);
                if (integrationEvent is null)
                {
                    continue;
                }

                integrationEvents.Add((IEvent) integrationEvent);
            }

            return integrationEvents;
        }
    }
}