using System;
using System.Threading.Tasks;
using Convey.CQRS.Events;
using Convey.MessageBrokers;
using Convey.MessageBrokers.Outbox;

namespace AAS.Architecture.Decorators
{
    internal sealed class OutboxEventHandlerDecorator<TEvent> : IEventHandler<TEvent>
        where TEvent : class, IEvent
    {
        private readonly bool enabled;
        private readonly IEventHandler<TEvent> handler;
        private readonly string messageId;
        private readonly IMessageOutbox outbox;

        public OutboxEventHandlerDecorator(IEventHandler<TEvent> handler, IMessageOutbox outbox,
            OutboxOptions outboxOptions, IMessagePropertiesAccessor messagePropertiesAccessor)
        {
            this.handler = handler;
            this.outbox = outbox;
            enabled = outboxOptions.Enabled;

            var messageProperties = messagePropertiesAccessor.MessageProperties;
            messageId = string.IsNullOrWhiteSpace(messageProperties?.MessageId)
                ? Guid.NewGuid().ToString("N")
                : messageProperties.MessageId;
        }

        public Task HandleAsync(TEvent @event) =>
            enabled
                ? outbox.HandleAsync(messageId, () => handler.HandleAsync(@event))
                : handler.HandleAsync(@event);
    }
}