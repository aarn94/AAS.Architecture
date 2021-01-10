using System;
using System.Linq;
using System.Threading.Tasks;
using AAS.Architecture.Extensions;
using Convey.CQRS.Commands;
using OpenTracing;
using OpenTracing.Tag;

namespace AAS.Architecture.Decorators.Jaeger
{
    internal sealed class JaegerCommandHandlerDecorator<TCommand> : ICommandHandler<TCommand>
        where TCommand : class, ICommand
    {
        private readonly ICommandHandler<TCommand> handler;
        private readonly ITracer tracer;

        public JaegerCommandHandlerDecorator(ICommandHandler<TCommand> handler, ITracer tracer)
        {
            this.handler = handler;
            this.tracer = tracer;
        }

        public async Task HandleAsync(TCommand command)
        {
            var commandName = ToUnderscoreCase(command.GetType().Name);
            using var scope = BuildScope(commandName);
            var span = scope.Span;

            try
            {
                span.Log($"Handling a message: {commandName}");
                await handler.HandleAsync(command).WithoutCapturingContext();
                span.Log($"Handled a message: {commandName}");
            }
            catch (Exception ex)
            {
                span.Log(ex.Message);
                span.SetTag(Tags.Error, true);
                throw;
            }
        }

        private IScope BuildScope(string commandName)
        {
            var scope = tracer
                .BuildSpan($"handling-{commandName}")
                .WithTag("message-type", commandName);

            if (tracer.ActiveSpan is {})
            {
                scope.AddReference(References.ChildOf, tracer.ActiveSpan.Context);
            }

            return scope.StartActive(true);
        }

        private static string ToUnderscoreCase(string str)
            => string.Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x : x.ToString()))
                .ToLowerInvariant();
    }
}