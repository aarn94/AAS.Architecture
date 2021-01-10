using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AAS.Architecture.Communication.Contexts;
using AAS.Architecture.Extensions;
using Convey.CQRS.Events;
using Convey.MessageBrokers;
using Convey.MessageBrokers.Outbox;
using Convey.MessageBrokers.RabbitMQ;
using Microsoft.Extensions.Logging;
using OpenTracing;

namespace AAS.Architecture.Services
{
    internal sealed class MessageBroker : IMessageBroker
    {
        private const string DefaultSpanContextHeader = "span_context";
        private readonly IBusPublisher busPublisher;
        private readonly IMessageOutbox outbox;
        private readonly IRequestContextAccessor contextAccessor;
        private readonly IMessagePropertiesAccessor messagePropertiesAccessor;
        private readonly ITracer tracer;
        private readonly ILogger<IMessageBroker> logger;
        private readonly string spanContextHeader;

        public MessageBroker(IBusPublisher busPublisher, IMessageOutbox outbox,
            IRequestContextAccessor contextAccessor,
            IMessagePropertiesAccessor messagePropertiesAccessor, RabbitMqOptions options, ITracer tracer,
            ILogger<IMessageBroker> logger)
        {
            this.busPublisher = busPublisher;
            this.outbox = outbox;
            this.contextAccessor = contextAccessor;
            this.messagePropertiesAccessor = messagePropertiesAccessor;
            this.tracer = tracer;
            this.logger = logger;
            spanContextHeader = string.IsNullOrWhiteSpace(options.SpanContextHeader)
                ? DefaultSpanContextHeader
                : options.SpanContextHeader;
        }

        public Task PublishAsync(params IEvent[] events) => PublishAsync(events?.AsEnumerable());

        public async Task PublishAsync(IEnumerable<IEvent> events)
        {
            if (events is null)
            {
                return;
            }

            var messageProperties = messagePropertiesAccessor.MessageProperties;
            var originatedMessageId = messageProperties?.MessageId;
            var correlationId = messageProperties?.CorrelationId;
            var spanContext = messageProperties?.GetSpanContext(spanContextHeader);
            if (string.IsNullOrWhiteSpace(spanContext))
            {
                spanContext = tracer.ActiveSpan is null ? string.Empty : tracer.ActiveSpan.Context.ToString();
            }

            var headers = messageProperties.GetHeadersToForward();
            var correlationContext = contextAccessor.ContextForSend;

            if (correlationContext is {})
                correlationContext.Language = CultureInfo.CurrentCulture.Name;

            foreach (var @event in events)
            {
                if (@event is null)
                {
                    continue;
                }

                var messageId = Guid.NewGuid().ToString("N");
                logger.LogTrace($"Publishing integration event: {@event.GetType().Name} [id: '{messageId}'].");
                if (outbox.Enabled)
                {
                    await outbox.SendAsync(@event, originatedMessageId, messageId, correlationId, spanContext,
                        correlationContext, headers).WithoutCapturingContext();
                    continue;
                }

                await busPublisher.PublishAsync(@event, messageId, correlationId, spanContext, correlationContext,
                    headers).WithoutCapturingContext();
            }
        }
    }
}