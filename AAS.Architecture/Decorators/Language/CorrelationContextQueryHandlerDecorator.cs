using System.Globalization;
using System.Threading.Tasks;
using AAS.Architecture.Communication.Contexts;
using AAS.Architecture.Services;
using Convey.CQRS.Queries;

namespace AAS.Architecture.Decorators.Language
{
    internal sealed class CorrelationContextQueryHandlerDecorator<TQuery, TDto> : IQueryHandler<TQuery, TDto> where TQuery: class, IQuery<TDto>
    {
        private readonly IQueryHandler<TQuery, TDto> handler;
        private readonly IRequestContextAccessor contextAccessor;

        public CorrelationContextQueryHandlerDecorator(IQueryHandler<TQuery, TDto> handler, IRequestContextAccessor contextAccessor)
        {
            this.handler = handler;
            this.contextAccessor = contextAccessor;
        }


        public Task<TDto> HandleAsync(TQuery query)
        {
            var context = contextAccessor.ReceivedContext;
            if (context is {})
                HandleReceivedContext(context);

            return handler.HandleAsync(query);
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