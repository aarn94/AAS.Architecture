using System;
using System.Threading.Tasks;
using Convey.CQRS.Commands;
using Convey.MessageBrokers;
using Convey.MessageBrokers.Outbox;

namespace AAS.Architecture.Decorators
{
    internal sealed class OutboxCommandHandlerDecorator<TCommand> : ICommandHandler<TCommand>
        where TCommand : class, ICommand
    {
        private readonly bool enabled;
        private readonly ICommandHandler<TCommand> handler;
        private readonly string messageId;
        private readonly IMessageOutbox outbox;

        public OutboxCommandHandlerDecorator(ICommandHandler<TCommand> handler, IMessageOutbox outbox,
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

        public Task HandleAsync(TCommand command) =>
            enabled
                ? outbox.HandleAsync(messageId, () => handler.HandleAsync(command))
                : handler.HandleAsync(command);
    }
}