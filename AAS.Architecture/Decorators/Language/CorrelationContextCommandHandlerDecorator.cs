using System.Globalization;
using System.Threading.Tasks;
using AAS.Architecture.Communication.Contexts;
using AAS.Architecture.Services;
using Convey.CQRS.Commands;

namespace AAS.Architecture.Decorators.Language
{
    internal sealed class CorrelationContextCommandHandlerDecorator<TCommand> : ICommandHandler<TCommand>
        where TCommand : class, ICommand
    {
        private readonly ICommandHandler<TCommand> handler;
        private readonly IRequestContextAccessor contextAccessor;

        public CorrelationContextCommandHandlerDecorator(ICommandHandler<TCommand> handler, IRequestContextAccessor contextAccessor)
        {
            this.handler = handler;
            this.contextAccessor = contextAccessor;
        }

        public Task HandleAsync(TCommand command)
        {
            var context = contextAccessor.ReceivedContext;
            if (context is {})
                HandleReceivedContext(context);

            return handler.HandleAsync(command);
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