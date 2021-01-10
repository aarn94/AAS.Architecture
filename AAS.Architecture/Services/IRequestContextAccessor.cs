using AAS.Architecture.Communication.Contexts;

namespace AAS.Architecture.Services
{
    public interface IRequestContextAccessor
    {
        CorrelationContext ReceivedContext { get; }
        CorrelationContext ContextForSend { get; }
    }
}